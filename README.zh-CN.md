<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Download 下载任务组件

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.download)](https://github.com/GameFrameX/com.gameframex.unity.download/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.download)](https://github.com/GameFrameX/com.gameframex.unity.download/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

独立游戏前后端一体化解决方案 · 独立游戏开发者的圆梦大使

<br />

[文档](https://gameframex.doc.alianblank.com) · [快速开始](#快速开始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | **简体中文** | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## 项目简介

Unity 多代理下载管理器。支持并发文件下载、优先级队列、可配置代理池、暂停/恢复、实时速度上报，以及事件驱动的回调通知。

## 功能特性

- **多代理并发** — 可配置代理池（默认 3 个代理）并行下载
- **优先级调度** — 高优先级任务优先派发
- **标签分组** — 按标签添加、查询和移除任务
- **暂停与恢复** — 通过 `Paused` 全局暂停/恢复所有下载
- **超时与刷盘** — 组件级超时设置和磁盘写入阈值（`FlushSize`），支持断点续传
- **实时指标** — `CurrentSpeed`、代理计数和等待任务数
- **事件驱动** — 通过事件组件触发 `DownloadStart` / `DownloadUpdate` / `DownloadSuccess` / `DownloadFailure`
- **异步支持** — `Download()` 返回 `Task<bool>`，支持 await
- **可插拔后端** — 内置 `UnityWebRequestDownloadAgentHelper`；可通过 Inspector 切换或实现 `IDownloadAgentHelper`

## 安装方式（任选其一）

1. **Scoped Registry（推荐）** — 编辑 `Packages/manifest.json`：
   ```json
   {
     "scopedRegistries": [
       {
         "name": "GameFrameX",
         "url": "https://gameframex.upm.alianblank.uk",
         "scopes": ["com.gameframex"]
       }
     ],
     "dependencies": {
       "com.gameframex.unity.download": "1.1.1"
     }
   }
   ```

   `scopes` 控制哪些包通过此注册表解析。只有以 `com.gameframex` 开头的包才会从这个注册表获取。
2. **Git URL** — 在 Unity Package Manager 中添加 `https://github.com/AlianBlank/com.gameframex.unity.download.git`
3. **本地克隆** — 克隆到项目的 `Packages/` 目录

## 快速开始

### 事件驱动用法

```csharp
// 获取 DownloadComponent（挂载在 GameEntry 上）
var downloadComponent = GameEntry.GetComponent<DownloadComponent>();

// 通过 EventComponent 订阅事件
var eventComponent = GameEntry.GetComponent<EventComponent>();
eventComponent.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
eventComponent.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);

// 添加下载任务
int serialId = downloadComponent.AddDownload(
    "/本地保存路径/file.zip",      // downloadPath
    "https://example.com/file.zip"  // downloadUri
);

void OnDownloadSuccess(object sender, GameEventArgs e)
{
    var args = (DownloadSuccessEventArgs)e;
    if (args.SerialId == serialId)
    {
        // 下载完成
    }
}

void OnDownloadFailure(object sender, GameEventArgs e)
{
    var args = (DownloadFailureEventArgs)e;
    Debug.LogError($"下载失败：{args.ErrorMessage}");
}
```

### 异步用法

```csharp
var downloadComponent = GameEntry.GetComponent<DownloadComponent>();

bool success = await downloadComponent.Download(
    "/本地保存路径/file.zip",
    "https://example.com/file.zip"
);

if (success)
{
    // 下载完成
}
```

### 标签与优先级

```csharp
// 带标签和优先级添加
int serialId = downloadComponent.AddDownload(
    downloadPath, downloadUri,
    tag: "assets",       // 分组标签
    priority: 10         // 数值越高越优先
);

// 按标签查询
TaskInfo[] infos = downloadComponent.GetDownloadInfos("assets");

// 移除标签组内所有任务
downloadComponent.RemoveDownloads("assets");
```

### 暂停与恢复

```csharp
downloadComponent.Paused = true;   // 暂停所有下载
downloadComponent.Paused = false;  // 恢复下载
```

## API 参考

### 核心属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `Paused` | `bool` | 暂停或恢复所有下载 |
| `Timeout` | `float` | 下载超时时间（秒），默认 30 |
| `FlushSize` | `int` | 磁盘写入阈值（字节），默认 1 MB |
| `CurrentSpeed` | `float` | 当前总下载速度 |
| `TotalAgentCount` | `int` | 下载代理总数 |
| `FreeAgentCount` | `int` | 空闲代理数 |
| `WorkingAgentCount` | `int` | 工作中代理数 |
| `WaitingTaskCount` | `int` | 等待代理的任务数 |

### 主要方法

| 方法 | 返回值 | 说明 |
|------|--------|------|
| `AddDownload(path, uri, ...)` | `int` | 添加任务，返回序列 ID |
| `Download(path, uri)` | `Task<bool>` | 添加任务，可 await |
| `RemoveDownload(serialId)` | `bool` | 移除单个任务 |
| `RemoveDownloads(tag)` | `int` | 移除指定标签的所有任务 |
| `RemoveAllDownloads()` | `int` | 移除所有任务 |
| `GetDownloadInfo(serialId)` | `TaskInfo` | 查询单个任务 |
| `GetDownloadInfos(tag)` | `TaskInfo[]` | 按标签查询任务 |
| `GetAllDownloadInfos()` | `TaskInfo[]` | 查询所有任务 |

## 依赖

- [com.gameframex.unity.event](https://github.com/AlianBlank/com.gameframex.unity.event) >= 1.1.0

## 链接

- [文档](https://gameframex.doc.alianblank.com)
- [更新日志](https://github.com/gameframex/com.gameframex.unity.download/releases)
- QQ群: 467608841 / 233840761


## 文档与资源

- [官方文档](https://gameframex.doc.alianblank.com)

## 社区与支持

- QQ群: 467608841 / 233840761

## 更新日志

查看 [Releases](https://github.com/GameFrameX/gameframex/com.gameframex.unity.download/releases) 了解更新日志。
## 开源协议

本项目采用双许可证发布，详见 [LICENSE](LICENSE.md)。
