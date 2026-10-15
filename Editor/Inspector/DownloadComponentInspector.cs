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
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameFrameX.Download.Runtime;
using GameFrameX.Editor;
using GameFrameX.Runtime;
using UnityEditor;
using UnityEngine;

namespace GameFrameX.Download.Editor
{
    [CustomEditor(typeof(DownloadComponent))]
    internal sealed class DownloadComponentInspector : ComponentTypeComponentInspector
    {
        private SerializedProperty m_InstanceRoot = null;
        private SerializedProperty m_CustomDownloadAgentHelper = null;

        /// <summary>
        /// Options 成员名 → 组件持有字段名别名（DownloadOptions.AgentHelperCount 由 m_DownloadAgentHelperCount 持有）。
        /// </summary>
        private static readonly Dictionary<string, string> MemberFieldAliases = new Dictionary<string, string>
        {
            { "AgentHelperCount", "m_DownloadAgentHelperCount" },
        };

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            DownloadComponent t = (DownloadComponent)target;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EditorGUILayout.PropertyField(m_InstanceRoot);
                EditorGUILayout.PropertyField(m_CustomDownloadAgentHelper);
            }
            EditorGUI.EndDisabledGroup();

            // C9 活视图：编辑态提示 DownloadOptions 入口；Play 态 Timeout 实时可改（经 DownloadComponent.Timeout 属性），
            // AgentHelperCount/FlushSize/DownloadAgentHelperFactory 为启动期型只读（AgentHelperCount 持有字段名经别名映射）。
            GameFrameXOptionsLiveViewUtility.DrawLiveView(t, typeof(DownloadOptions), MemberFieldAliases);
            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Paused", t.Paused.ToString());
                EditorGUILayout.LabelField("Total Agent Count", t.TotalAgentCount.ToString());
                EditorGUILayout.LabelField("Free Agent Count", t.FreeAgentCount.ToString());
                EditorGUILayout.LabelField("Working Agent Count", t.WorkingAgentCount.ToString());
                EditorGUILayout.LabelField("Waiting Agent Count", t.WaitingTaskCount.ToString());
                EditorGUILayout.LabelField("Current Speed", t.CurrentSpeed.ToString());
                EditorGUILayout.BeginVertical("box");
                {
                    TaskInfo[] downloadInfos = t.GetAllDownloadInfos();
                    if (downloadInfos.Length > 0)
                    {
                        foreach (TaskInfo downloadInfo in downloadInfos)
                        {
                            DrawDownloadInfo(downloadInfo);
                        }

                        if (GUILayout.Button("Export CSV Data"))
                        {
                            string exportFileName = EditorUtility.SaveFilePanel("Export CSV Data", string.Empty, "Download Task Data.csv", string.Empty);
                            if (!string.IsNullOrEmpty(exportFileName))
                            {
                                try
                                {
                                    int index = 0;
                                    string[] data = new string[downloadInfos.Length + 1];
                                    data[index++] = "Download Path,Serial Id,Tag,Priority,Status";
                                    foreach (TaskInfo downloadInfo in downloadInfos)
                                    {
                                        data[index++] = Utility.Text.Format("{0},{1},{2},{3},{4}", downloadInfo.Description, downloadInfo.SerialId, downloadInfo.Tag ?? string.Empty, downloadInfo.Priority, downloadInfo.Status);
                                    }

                                    File.WriteAllLines(exportFileName, data, Encoding.UTF8);
                                    Debug.Log(Utility.Text.Format("Export download task CSV data to '{0}' success.", exportFileName));
                                }
                                catch (Exception exception)
                                {
                                    Debug.LogError(Utility.Text.Format("Export download task CSV data to '{0}' failure, exception is '{1}'.", exportFileName, exception));
                                }
                            }
                        }
                    }
                    else
                    {
                        GUILayout.Label("Download Task is Empty ...");
                    }
                }
                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();

            RefreshTypeNames();
        }

        protected override void Enable()
        {
            base.Enable();
            m_InstanceRoot = serializedObject.FindProperty("m_InstanceRoot");

            m_CustomDownloadAgentHelper = serializedObject.FindProperty("m_CustomDownloadAgentHelper");
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDownloadInfo(TaskInfo downloadInfo)
        {
            EditorGUILayout.LabelField(downloadInfo.Description, Utility.Text.Format("[SerialId]{0} [Tag]{1} [Priority]{2} [Status]{3}", downloadInfo.SerialId, downloadInfo.Tag ?? "<None>", downloadInfo.Priority, downloadInfo.Status));
        }

        protected override void RefreshTypeNames()
        {
            RefreshComponentTypeNames(typeof(IDownloadManager));
        }
    }
}