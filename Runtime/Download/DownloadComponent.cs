﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameFrameX.Runtime;
using GameFrameX.Event.Runtime;
using UnityEngine;

namespace GameFrameX.Download.Runtime
{
    /// <summary>
    /// 下载组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Download")]
    public sealed class DownloadComponent : GameFrameworkComponent
    {
        private const int DefaultPriority = 0;
        private const int OneMegaBytes = 1024 * 1024;

        private IDownloadManager m_DownloadManager = null;
        private EventComponent m_EventComponent = null;

        [SerializeField] private Transform m_InstanceRoot = null;

        [SerializeField] private string m_DownloadAgentHelperTypeName = "UnityGameFramework.Runtime.UnityWebRequestDownloadAgentHelper";

        [SerializeField] private DownloadAgentHelperBase m_CustomDownloadAgentHelper = null;

        [SerializeField] private int m_DownloadAgentHelperCount = 3;

        [SerializeField] private float m_Timeout = 30f;

        [SerializeField] private int m_FlushSize = OneMegaBytes;

        /// <summary>
        /// 获取或设置下载是否被暂停。
        /// </summary>
        public bool Paused
        {
            get { return m_DownloadManager.Paused; }
            set { m_DownloadManager.Paused = value; }
        }

        /// <summary>
        /// 获取下载代理总数量。
        /// </summary>
        public int TotalAgentCount
        {
            get { return m_DownloadManager.TotalAgentCount; }
        }

        /// <summary>
        /// 获取可用下载代理数量。
        /// </summary>
        public int FreeAgentCount
        {
            get { return m_DownloadManager.FreeAgentCount; }
        }

        /// <summary>
        /// 获取工作中下载代理数量。
        /// </summary>
        public int WorkingAgentCount
        {
            get { return m_DownloadManager.WorkingAgentCount; }
        }

        /// <summary>
        /// 获取等待下载任务数量。
        /// </summary>
        public int WaitingTaskCount
        {
            get { return m_DownloadManager.WaitingTaskCount; }
        }

        /// <summary>
        /// 获取或设置下载超时时长，以秒为单位。
        /// </summary>
        public float Timeout
        {
            get { return m_DownloadManager.Timeout; }
            set { m_DownloadManager.Timeout = m_Timeout = value; }
        }

        /// <summary>
        /// 获取或设置将缓冲区写入磁盘的临界大小，仅当开启断点续传时有效。
        /// </summary>
        public int FlushSize
        {
            get { return m_DownloadManager.FlushSize; }
            set { m_DownloadManager.FlushSize = m_FlushSize = value; }
        }

        /// <summary>
        /// 获取当前下载速度。
        /// </summary>
        public float CurrentSpeed
        {
            get { return m_DownloadManager.CurrentSpeed; }
        }

        /// <summary>
        /// 获取下载任务的字典。
        /// </summary>
        private ConcurrentDictionary<int, DownloadData> m_Downloads = new ConcurrentDictionary<int, DownloadData>();

        /// <summary>
        /// 下载数据。
        /// </summary>
        sealed class DownloadData
        {
            public TaskCompletionSource<bool> TCS { get; private set; }
            public object UserData { get; private set; }
            public string Tag { get; private set; }
            public int SerialId { get; private set; }
            public string Url { get; private set; }

            /// <summary>
            /// 初始化下载数据的新实例。
            /// </summary>
            public DownloadData(string url, string tag, int serialId, object userData)
            {
                Url = url;
                Tag = tag;
                SerialId = serialId;
                UserData = userData;
                TCS = new TaskCompletionSource<bool>();
            }
        }

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            ImplementationComponentType = Utility.Assembly.GetType(componentType);
            InterfaceComponentType = typeof(IDownloadManager);
            base.Awake();
            m_DownloadManager = GameFrameworkEntry.GetModule<IDownloadManager>();
            if (m_DownloadManager == null)
            {
                Log.Fatal("Download manager is invalid.");
                return;
            }

            m_DownloadManager.DownloadStart += OnDownloadStart;
            m_DownloadManager.DownloadUpdate += OnDownloadUpdate;
            m_DownloadManager.DownloadSuccess += OnDownloadSuccess;
            m_DownloadManager.DownloadFailure += OnDownloadFailure;
            m_DownloadManager.FlushSize = m_FlushSize;
            m_DownloadManager.Timeout = m_Timeout;
        }

        private void Start()
        {
            m_EventComponent = GameEntry.GetComponent<EventComponent>();
            if (m_EventComponent == null)
            {
                Log.Fatal("Event component is invalid.");
                return;
            }

            if (m_InstanceRoot == null)
            {
                m_InstanceRoot = new GameObject("Download Agent Instances").transform;
                m_InstanceRoot.SetParent(gameObject.transform);
                m_InstanceRoot.localScale = Vector3.one;
            }

            for (int i = 0; i < m_DownloadAgentHelperCount; i++)
            {
                AddDownloadAgentHelper(i);
            }
        }

        /// <summary>
        /// 根据下载任务的序列编号获取下载任务的信息。
        /// </summary>
        /// <param name="serialId">要获取信息的下载任务的序列编号。</param>
        /// <returns>下载任务的信息。</returns>
        public TaskInfo GetDownloadInfo(int serialId)
        {
            return m_DownloadManager.GetDownloadInfo(serialId);
        }

        /// <summary>
        /// 根据下载任务的标签获取下载任务的信息。
        /// </summary>
        /// <param name="tag">要获取信息的下载任务的标签。</param>
        /// <returns>下载任务的信息。</returns>
        public TaskInfo[] GetDownloadInfos(string tag)
        {
            return m_DownloadManager.GetDownloadInfos(tag);
        }

        /// <summary>
        /// 根据下载任务的标签获取下载任务的信息。
        /// </summary>
        /// <param name="tag">要获取信息的下载任务的标签。</param>
        /// <param name="results">下载任务的信息。</param>
        public void GetDownloadInfos(string tag, List<TaskInfo> results)
        {
            m_DownloadManager.GetDownloadInfos(tag, results);
        }

        /// <summary>
        /// 获取所有下载任务的信息。
        /// </summary>
        /// <returns>所有下载任务的信息。</returns>
        public TaskInfo[] GetAllDownloadInfos()
        {
            return m_DownloadManager.GetAllDownloadInfos();
        }

        /// <summary>
        /// 获取所有下载任务的信息。
        /// </summary>
        /// <param name="results">所有下载任务的信息。</param>
        public void GetAllDownloadInfos(List<TaskInfo> results)
        {
            m_DownloadManager.GetAllDownloadInfos(results);
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri)
        {
            return AddDownload(downloadPath, downloadUri, null, DefaultPriority, null);
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="tag">下载任务的标签。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri, string tag)
        {
            return AddDownload(downloadPath, downloadUri, tag, DefaultPriority, null);
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="priority">下载任务的优先级。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri, int priority)
        {
            return AddDownload(downloadPath, downloadUri, null, priority, null);
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">存储路径</param>
        /// <param name="downloadUri">下载地址</param>
        /// <returns>返回是否下载成功</returns>
        public Task<bool> Download(string downloadPath, string downloadUri)
        {
            var serialId = AddDownload(downloadPath, downloadUri, null, DefaultPriority, null);
            if (m_Downloads.TryGetValue(serialId, out var value))
            {
                return value.TCS.Task;
            }

            return null;
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri, object userData)
        {
            return AddDownload(downloadPath, downloadUri, null, DefaultPriority, userData);
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="tag">下载任务的标签。</param>
        /// <param name="priority">下载任务的优先级。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri, string tag, int priority)
        {
            return AddDownload(downloadPath, downloadUri, tag, priority, null);
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="tag">下载任务的标签。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri, string tag, object userData)
        {
            return AddDownload(downloadPath, downloadUri, tag, DefaultPriority, userData);
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="priority">下载任务的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri, int priority, object userData)
        {
            return AddDownload(downloadPath, downloadUri, null, priority, userData);
        }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="tag">下载任务的标签。</param>
        /// <param name="priority">下载任务的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri, string tag, int priority, object userData)
        {
            var serialId = m_DownloadManager.AddDownload(downloadPath, downloadUri, tag, priority, userData);
            DownloadData downloadData = new DownloadData(downloadUri, tag, serialId, userData);
            m_Downloads.TryAdd(serialId, downloadData);
            return serialId;
        }

        /// <summary>
        /// 根据下载任务的序列编号移除下载任务。
        /// </summary>
        /// <param name="serialId">要移除下载任务的序列编号。</param>
        /// <returns>是否移除下载任务成功。</returns>
        public bool RemoveDownload(int serialId)
        {
            m_Downloads.TryRemove(serialId, out _);
            return m_DownloadManager.RemoveDownload(serialId);
        }

        /// <summary>
        /// 根据下载任务的标签移除下载任务。
        /// </summary>
        /// <param name="tag">要移除下载任务的标签。</param>
        /// <returns>移除下载任务的数量。</returns>
        public int RemoveDownloads(string tag)
        {
            int serialId = -1;
            foreach (var downloadData in m_Downloads.Values)
            {
                if (downloadData.Tag == tag)
                {
                    serialId = downloadData.SerialId;
                    break;
                }
            }

            m_Downloads.TryRemove(serialId, out _);
            return m_DownloadManager.RemoveDownloads(tag);
        }

        /// <summary>
        /// 移除所有下载任务。
        /// </summary>
        /// <returns>移除下载任务的数量。</returns>
        public int RemoveAllDownloads()
        {
            m_Downloads.Clear();
            return m_DownloadManager.RemoveAllDownloads();
        }

        /// <summary>
        /// 增加下载代理辅助器。
        /// </summary>
        /// <param name="index">下载代理辅助器索引。</param>
        private void AddDownloadAgentHelper(int index)
        {
            DownloadAgentHelperBase downloadAgentHelper = Helper.CreateHelper(m_DownloadAgentHelperTypeName, m_CustomDownloadAgentHelper, index);
            if (downloadAgentHelper == null)
            {
                Log.Error("Can not create download agent helper.");
                return;
            }

            downloadAgentHelper.name = Utility.Text.Format("Download Agent Helper - {0}", index);
            Transform transform = downloadAgentHelper.transform;
            transform.SetParent(m_InstanceRoot);
            transform.localScale = Vector3.one;

            m_DownloadManager.AddDownloadAgentHelper(downloadAgentHelper);
        }

        private void OnDownloadStart(object sender, DownloadStartEventArgs e)
        {
            m_EventComponent.Fire(this, DownloadStartEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, e.CurrentLength, e.UserData));
        }

        private void OnDownloadUpdate(object sender, DownloadUpdateEventArgs e)
        {
            m_EventComponent.Fire(this, DownloadUpdateEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, e.CurrentLength, e.UserData));
        }

        private void OnDownloadSuccess(object sender, DownloadSuccessEventArgs e)
        {
            m_EventComponent.Fire(this, DownloadSuccessEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, e.CurrentLength, e.UserData));
            if (m_Downloads.TryRemove(e.SerialId, out var value))
            {
                value.TCS.TrySetResult(true);
            }
        }

        private void OnDownloadFailure(object sender, DownloadFailureEventArgs e)
        {
            Log.Warning("Download failure, download serial id '{0}', download path '{1}', download uri '{2}', error message '{3}'.", e.SerialId, e.DownloadPath, e.DownloadUri, e.ErrorMessage);
            m_EventComponent.Fire(this, DownloadFailureEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, e.ErrorMessage, e.UserData));
            if (m_Downloads.TryRemove(e.SerialId, out var value))
            {
                value.TCS.TrySetResult(false);
            }
        }
    }
}