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

using GameFrameX.Runtime;

namespace GameFrameX.Download.Runtime
{
    public sealed partial class DownloadManager : GameFrameworkModule, IDownloadManager
    {
        private sealed partial class DownloadCounter
        {
            private readonly GameFrameworkLinkedList<DownloadCounterNode> m_DownloadCounterNodes;
            private float m_UpdateInterval;
            private float m_RecordInterval;
            private float m_CurrentSpeed;
            private float m_Accumulator;
            private float m_TimeLeft;

            public DownloadCounter(float updateInterval, float recordInterval)
            {
                if (updateInterval <= 0f)
                {
                    throw new GameFrameworkException("Update interval is invalid.");
                }

                if (recordInterval <= 0f)
                {
                    throw new GameFrameworkException("Record interval is invalid.");
                }

                m_DownloadCounterNodes = new GameFrameworkLinkedList<DownloadCounterNode>();
                m_UpdateInterval = updateInterval;
                m_RecordInterval = recordInterval;
                Reset();
            }

            public float UpdateInterval
            {
                get
                {
                    return m_UpdateInterval;
                }
                set
                {
                    if (value <= 0f)
                    {
                        throw new GameFrameworkException("Update interval is invalid.");
                    }

                    m_UpdateInterval = value;
                    Reset();
                }
            }

            public float RecordInterval
            {
                get
                {
                    return m_RecordInterval;
                }
                set
                {
                    if (value <= 0f)
                    {
                        throw new GameFrameworkException("Record interval is invalid.");
                    }

                    m_RecordInterval = value;
                    Reset();
                }
            }

            public float CurrentSpeed
            {
                get
                {
                    return m_CurrentSpeed;
                }
            }

            public void Shutdown()
            {
                Reset();
            }

            public void Update(float elapseSeconds, float realElapseSeconds)
            {
                if (m_DownloadCounterNodes.Count <= 0)
                {
                    return;
                }

                m_Accumulator += realElapseSeconds;
                if (m_Accumulator > m_RecordInterval)
                {
                    m_Accumulator = m_RecordInterval;
                }

                m_TimeLeft -= realElapseSeconds;
                foreach (DownloadCounterNode downloadCounterNode in m_DownloadCounterNodes)
                {
                    downloadCounterNode.Update(elapseSeconds, realElapseSeconds);
                }

                while (m_DownloadCounterNodes.Count > 0)
                {
                    DownloadCounterNode downloadCounterNode = m_DownloadCounterNodes.First.Value;
                    if (downloadCounterNode.ElapseSeconds < m_RecordInterval)
                    {
                        break;
                    }

                    ReferencePool.Release(downloadCounterNode);
                    m_DownloadCounterNodes.RemoveFirst();
                }

                if (m_DownloadCounterNodes.Count <= 0)
                {
                    Reset();
                    return;
                }

                if (m_TimeLeft <= 0f)
                {
                    long totalDeltaLength = 0L;
                    foreach (DownloadCounterNode downloadCounterNode in m_DownloadCounterNodes)
                    {
                        totalDeltaLength += downloadCounterNode.DeltaLength;
                    }

                    m_CurrentSpeed = m_Accumulator > 0f ? totalDeltaLength / m_Accumulator : 0f;
                    m_TimeLeft += m_UpdateInterval;
                }
            }

            public void RecordDeltaLength(int deltaLength)
            {
                if (deltaLength <= 0)
                {
                    return;
                }

                DownloadCounterNode downloadCounterNode = null;
                if (m_DownloadCounterNodes.Count > 0)
                {
                    downloadCounterNode = m_DownloadCounterNodes.Last.Value;
                    if (downloadCounterNode.ElapseSeconds < m_UpdateInterval)
                    {
                        downloadCounterNode.AddDeltaLength(deltaLength);
                        return;
                    }
                }

                downloadCounterNode = DownloadCounterNode.Create();
                downloadCounterNode.AddDeltaLength(deltaLength);
                m_DownloadCounterNodes.AddLast(downloadCounterNode);
            }

            private void Reset()
            {
                m_DownloadCounterNodes.Clear();
                m_CurrentSpeed = 0f;
                m_Accumulator = 0f;
                m_TimeLeft = 0f;
            }
        }
    }
}
