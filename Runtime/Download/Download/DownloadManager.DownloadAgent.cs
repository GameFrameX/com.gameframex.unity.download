// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System;
using System.IO;
using GameFrameX.Runtime;

namespace GameFrameX.Download.Runtime
{
    public sealed partial class DownloadManager : GameFrameworkModule, IDownloadManager
    {
        /// <summary>
        /// 下载代理。
        /// </summary>
        private sealed class DownloadAgent : ITaskAgent<DownloadTask>, IDisposable
        {
            private readonly IDownloadAgentHelper m_Helper;
            private DownloadTask m_Task;
            private FileStream m_FileStream;
            private int m_WaitFlushSize;
            private float m_WaitTime;
            private long m_StartLength;
            private long m_DownloadedLength;
            private long m_SavedLength;
            private bool m_Disposed;

            public Action<DownloadAgent> DownloadAgentStart;
            public Action<DownloadAgent, int> DownloadAgentUpdate;
            public Action<DownloadAgent, long> DownloadAgentSuccess;
            public Action<DownloadAgent, string> DownloadAgentFailure;

            /// <summary>
            /// 初始化下载代理的新实例。
            /// </summary>
            /// <param name="downloadAgentHelper">下载代理辅助器。</param>
            public DownloadAgent(IDownloadAgentHelper downloadAgentHelper)
            {
                if (downloadAgentHelper == null)
                {
                    throw new GameFrameworkException("Download agent helper is invalid.");
                }

                m_Helper = downloadAgentHelper;
                m_Task = null;
                m_FileStream = null;
                m_WaitFlushSize = 0;
                m_WaitTime = 0f;
                m_StartLength = 0L;
                m_DownloadedLength = 0L;
                m_SavedLength = 0L;
                m_Disposed = false;

                DownloadAgentStart = null;
                DownloadAgentUpdate = null;
                DownloadAgentSuccess = null;
                DownloadAgentFailure = null;
            }

            /// <summary>
            /// 获取下载任务。
            /// </summary>
            public DownloadTask Task
            {
                get { return m_Task; }
            }

            /// <summary>
            /// 获取已经等待时间。
            /// </summary>
            public float WaitTime
            {
                get { return m_WaitTime; }
            }

            /// <summary>
            /// 获取开始下载时已经存在的大小。
            /// </summary>
            public long StartLength
            {
                get { return m_StartLength; }
            }

            /// <summary>
            /// 获取本次已经下载的大小。
            /// </summary>
            public long DownloadedLength
            {
                get { return m_DownloadedLength; }
            }

            /// <summary>
            /// 获取当前的大小。
            /// </summary>
            public long CurrentLength
            {
                get { return m_StartLength + m_DownloadedLength; }
            }

            /// <summary>
            /// 获取已经存盘的大小。
            /// </summary>
            public long SavedLength
            {
                get { return m_SavedLength; }
            }

            /// <summary>
            /// 初始化下载代理。
            /// </summary>
            public void Initialize()
            {
                m_Helper.DownloadAgentHelperUpdateBytes += OnDownloadAgentHelperUpdateBytes;
                m_Helper.DownloadAgentHelperUpdateLength += OnDownloadAgentHelperUpdateLength;
                m_Helper.DownloadAgentHelperComplete += OnDownloadAgentHelperComplete;
                m_Helper.DownloadAgentHelperError += OnDownloadAgentHelperError;
            }

            /// <summary>
            /// 下载代理轮询。
            /// </summary>
            /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
            /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
            public void Update(float elapseSeconds, float realElapseSeconds)
            {
                if (m_Task.Status == DownloadTaskStatus.Doing)
                {
                    m_WaitTime += realElapseSeconds;
                    if (m_WaitTime >= m_Task.Timeout)
                    {
                        DownloadAgentHelperErrorEventArgs downloadAgentHelperErrorEventArgs = DownloadAgentHelperErrorEventArgs.Create(false, "Timeout");
                        OnDownloadAgentHelperError(this, downloadAgentHelperErrorEventArgs);
                        ReferencePool.Release(downloadAgentHelperErrorEventArgs);
                    }
                }
            }

            /// <summary>
            /// 关闭并清理下载代理。
            /// </summary>
            public void Shutdown()
            {
                Dispose();

                m_Helper.DownloadAgentHelperUpdateBytes -= OnDownloadAgentHelperUpdateBytes;
                m_Helper.DownloadAgentHelperUpdateLength -= OnDownloadAgentHelperUpdateLength;
                m_Helper.DownloadAgentHelperComplete -= OnDownloadAgentHelperComplete;
                m_Helper.DownloadAgentHelperError -= OnDownloadAgentHelperError;
            }

            /// <summary>
            /// 开始处理下载任务。
            /// </summary>
            /// <param name="task">要处理的下载任务。</param>
            /// <returns>开始处理任务的状态。</returns>
            public StartTaskStatus Start(DownloadTask task)
            {
                if (task == null)
                {
                    throw new GameFrameworkException("Task is invalid.");
                }

                m_Task = task;

                m_Task.Status = DownloadTaskStatus.Doing;
                string downloadFile = Utility.Text.Format("{0}.download", m_Task.DownloadPath);

                try
                {
                    if (File.Exists(downloadFile))
                    {
                        m_FileStream = File.OpenWrite(downloadFile);
                        m_FileStream.Seek(0L, SeekOrigin.End);
                        m_StartLength = m_SavedLength = m_FileStream.Length;
                        m_DownloadedLength = 0L;
                    }
                    else
                    {
                        string directory = Path.GetDirectoryName(m_Task.DownloadPath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        m_FileStream = new FileStream(downloadFile, FileMode.Create, FileAccess.Write);
                        m_StartLength = m_SavedLength = m_DownloadedLength = 0L;
                    }

                    if (DownloadAgentStart != null)
                    {
                        DownloadAgentStart(this);
                    }

                    if (m_StartLength > 0L)
                    {
                        m_Helper.Download(m_Task.DownloadUri, m_StartLength, m_Task.UserData);
                    }
                    else
                    {
                        m_Helper.Download(m_Task.DownloadUri, m_Task.UserData);
                    }

                    return StartTaskStatus.CanResume;
                }
                catch (Exception exception)
                {
                    DownloadAgentHelperErrorEventArgs downloadAgentHelperErrorEventArgs = DownloadAgentHelperErrorEventArgs.Create(false, exception.ToString());
                    OnDownloadAgentHelperError(this, downloadAgentHelperErrorEventArgs);
                    ReferencePool.Release(downloadAgentHelperErrorEventArgs);
                    return StartTaskStatus.UnknownError;
                }
            }

            /// <summary>
            /// 重置下载代理。
            /// </summary>
            public void Reset()
            {
                m_Helper.Reset();

                if (m_FileStream != null)
                {
                    m_FileStream.Close();
                    m_FileStream = null;
                }

                m_Task = null;
                m_WaitFlushSize = 0;
                m_WaitTime = 0f;
                m_StartLength = 0L;
                m_DownloadedLength = 0L;
                m_SavedLength = 0L;
            }

            /// <summary>
            /// 释放资源。
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// 释放资源。
            /// </summary>
            /// <param name="disposing">释放资源标记。</param>
            private void Dispose(bool disposing)
            {
                if (m_Disposed)
                {
                    return;
                }

                if (disposing)
                {
                    if (m_FileStream != null)
                    {
                        m_FileStream.Dispose();
                        m_FileStream = null;
                    }
                }

                m_Disposed = true;
            }

            private void OnDownloadAgentHelperUpdateBytes(object sender, DownloadAgentHelperUpdateBytesEventArgs e)
            {
                m_WaitTime = 0f;
                try
                {
                    m_FileStream.Write(e.GetBytes(), e.Offset, e.Length);
                    m_WaitFlushSize += e.Length;
                    m_SavedLength += e.Length;

                    if (m_WaitFlushSize >= m_Task.FlushSize)
                    {
                        m_FileStream.Flush();
                        m_WaitFlushSize = 0;
                    }
                }
                catch (Exception exception)
                {
                    DownloadAgentHelperErrorEventArgs downloadAgentHelperErrorEventArgs = DownloadAgentHelperErrorEventArgs.Create(false, exception.ToString());
                    OnDownloadAgentHelperError(this, downloadAgentHelperErrorEventArgs);
                    ReferencePool.Release(downloadAgentHelperErrorEventArgs);
                }
            }

            private void OnDownloadAgentHelperUpdateLength(object sender, DownloadAgentHelperUpdateLengthEventArgs e)
            {
                m_WaitTime = 0f;
                m_DownloadedLength += e.DeltaLength;
                if (DownloadAgentUpdate != null)
                {
                    DownloadAgentUpdate(this, e.DeltaLength);
                }
            }

            private void OnDownloadAgentHelperComplete(object sender, DownloadAgentHelperCompleteEventArgs e)
            {
                m_WaitTime = 0f;
                m_DownloadedLength = e.Length;
                if (m_SavedLength != CurrentLength)
                {
                    throw new GameFrameworkException("Internal download error.");
                }

                m_Helper.Reset();
                m_FileStream.Close();
                m_FileStream = null;

                if (File.Exists(m_Task.DownloadPath))
                {
                    File.Delete(m_Task.DownloadPath);
                }

                File.Move(Utility.Text.Format("{0}.download", m_Task.DownloadPath), m_Task.DownloadPath);

                m_Task.Status = DownloadTaskStatus.Done;

                if (DownloadAgentSuccess != null)
                {
                    DownloadAgentSuccess(this, e.Length);
                }

                m_Task.Done = true;
            }

            private void OnDownloadAgentHelperError(object sender, DownloadAgentHelperErrorEventArgs e)
            {
                m_Helper.Reset();
                if (m_FileStream != null)
                {
                    m_FileStream.Close();
                    m_FileStream = null;
                }

                if (e.DeleteDownloading)
                {
                    File.Delete(Utility.Text.Format("{0}.download", m_Task.DownloadPath));
                }

                m_Task.Status = DownloadTaskStatus.Error;

                if (DownloadAgentFailure != null)
                {
                    DownloadAgentFailure(this, e.ErrorMessage);
                }

                m_Task.Done = true;
            }
        }
    }
}