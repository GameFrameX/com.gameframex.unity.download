// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and other related rights
//   are protected by the laws and regulations of the People's Republic of China and other international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   complete license text please refer to the LICENSE file in the source code root directory.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes or liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer and the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB 仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System;
using GameFrameX.Runtime;
using UnityEngine;

namespace GameFrameX.Download.Runtime
{
    /// <summary>
    /// 下载模块启动配置（模块级 Options）。
    /// </summary>
    /// <remarks>
    /// 默认值即开箱值（与原 DownloadComponent 序列化字段默认值同源）；游戏在首场景加载前（BeforeSceneLoad）经 <c>GameFrameXOptions.Configure(new DownloadOptions{...})</c> 注入差异值，
    /// 由 <see cref="DownloadComponent"/> 在 Awake 中一次性消费。
    /// </remarks>
    public sealed class DownloadOptions
    {
        /// <summary>
        /// 下载代理辅助器数量（默认 3）。
        /// </summary>
        /// <remarks>启动期型：仅在组件 Awake 创建代理时消费，运行期不可修改。</remarks>
        [GameFrameXStartupOnly]
        public int AgentHelperCount = 3;

        /// <summary>
        /// 下载超时时长，以秒为单位（默认 30 秒）。
        /// </summary>
        /// <remarks>实时型：运行期可经 <see cref="DownloadComponent.Timeout"/> 修改，立即生效。</remarks>
        public float Timeout = 30f;

        /// <summary>
        /// 将缓冲区写入磁盘的临界大小，仅当开启断点续传时有效（默认 1MB = 1048576 字节）。
        /// </summary>
        /// <remarks>启动期型：仅模块首次实例化前经 <c>GameFrameXOptions.Configure</c> 生效；组件运行期修改该值会被拒绝。</remarks>
        [GameFrameXStartupOnly]
        public int FlushSize = OneMegaBytes;

        /// <summary>
        /// 下载代理辅助器工厂（默认 UnityWebRequest 实现）。
        /// </summary>
        /// <remarks>启动期型：仅在组件 Awake 创建代理时消费，运行期不可修改。</remarks>
        [GameFrameXStartupOnly]
        public Func<DownloadAgentHelperBase> DownloadAgentHelperFactory = () => new GameObject().AddComponent<UnityWebRequestDownloadAgentHelper>();

        /// <summary>
        /// 一兆字节对应的字节数（1048576），与原组件常量同值。
        /// </summary>
        public const int OneMegaBytes = 1024 * 1024;
    }
}
