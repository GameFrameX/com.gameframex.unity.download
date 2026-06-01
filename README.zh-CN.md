<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="GameFrameX Logo" width="160"/>

# Game Frame X Download 下载任务组件

[![License](https://img.shields.io/github/license/gameframex/com.gameframex.unity.download)](https://github.com/gameframex/com.gameframex.unity.download/blob/main/LICENSE)
[![Version](https://img.shields.io/github/v/release/gameframex/com.gameframex.unity.download)](https://github.com/gameframex/com.gameframex.unity.download/releases)
[![Documentation](https://img.shields.io/badge/Documentation-文档-blue)](https://gameframex.doc.alianblank.com)

独立游戏前后端一体化解决方案 · 独立游戏开发者的圆梦大使

[文档](https://gameframex.doc.alianblank.com) · [快速开始](#快速开始) · [QQ群](https://qm.qq.com/q/5kbDVBdUeS) · **语言**

[English](README.md) | **简体中文** | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

---

## 项目简介

**Download 下载任务组件 (Download Component)** - 提供管理下载队列、处理下载任务，并提供下载状态的实时更新相关的接口。

## 快速开始

### 安装方式（任选其一）

1. 编辑 Unity 项目的 `Packages/manifest.json`，添加 `scopedRegistries` 部分：
   ```json
   {
     "scopedRegistries": [
       {
         "name": "GameFrameX",
         "url": "https://gameframex.upm.alianblank.uk",
         "scopes": [
           "com.gameframex"
         ]
       }
     ],
     "dependencies": {
       "com.gameframex.unity.download": "1.1.1"
     }
   }
   ```

   `scopes` 控制哪些包通过此注册表解析。只有以 `com.gameframex` 开头的包才会从这个注册表获取。
2. 在 Unity 的 `Packages Manager` 中使用 `Git URL` 的方式添加库，地址为：`https://github.com/AlianBlank/com.gameframex.unity.download.git`
3. 直接下载仓库放置到 Unity 项目的 `Packages` 目录下，会自动加载识别。

## 使用示例

`DownloadComponent` 是一个用于处理下载任务的游戏框架组件。它负责管理下载队列、处理下载任务，并提供下载状态的实时更新。

### 功能概述

- 管理多个下载任务
- 支持断点续传功能
- 提供下载任务的优先级设置
- 实时更新下载进度和下载速度
- 可以通过事件接收下载的各个阶段状态

### 核心属性

- `Paused`：获取或设置下载是否被暂停。
- `TotalAgentCount`：获取下载代理总数量。
- `FreeAgentCount`：获取可用下载代理数量。
- `WorkingAgentCount`：获取工作中下载代理数量。
- `WaitingTaskCount`：获取等待下载任务数量。
- `Timeout`：获取或设置下载超时时长。
- `FlushSize`：获取或设置写入磁盘的临界大小。
- `CurrentSpeed`：获取当前下载速度。

### 如何增加下载任务

```csharp
// 根据指定的下载路径和URI增加下载任务
public int AddDownload(string downloadPath, string downloadUri);

// 根据指定的下载路径、URI和任务标签增加下载任务
public int AddDownload(string downloadPath, string downloadUri, string tag);

// 根据指定的下载路径、URI和优先级增加下载任务
public int AddDownload(string downloadPath, string downloadUri, int priority);

// 根据指定的下载路径、URI和用户自定义数据增加下载任务
public int AddDownload(string downloadPath, string downloadUri, object userData);
```

### 如何移除下载任务

```csharp
// 根据序列编号移除下载任务
public bool RemoveDownload(int serialId);

// 根据标签移除下载任务
public int RemoveDownloads(string tag);

// 移除所有下载任务
public int RemoveAllDownloads();
```

### 事件通知

`DownloadComponent` 提供事件，以便于当下载任务开始、更新、成功、失败时接收通知。

- `DownloadStart`：当下载开始时触发。
- `DownloadUpdate`：当下载更新时触发。
- `DownloadSuccess`：当下载成功时触发。
- `DownloadFailure`：当下载失败时触发。

### 示例

```csharp
// 假设已经存在 downloadComponent 实例
int serialId = downloadComponent.AddDownload("本地存储路径", "下载链接");

// 通过事件组件订阅下载成功事件
eventComponent.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);

// 下载成功回调
void OnDownloadSuccess(object sender, GameEventArgs e)
{
    DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs)e;
    if (ne.SerialId == serialId)
    {
        // 处理下载成功
    }
}
```

> **注意：** 此组件依赖于 [Event 事件组件](https://github.com/AlianBlank/com.gameframex.unity.event)。

## 文档与资源

- [文档](https://gameframex.doc.alianblank.com)

## 社区与支持

- [QQ群](https://qm.qq.com/q/5kbDVBdUeS)

## 更新日志

查看 [Releases](https://github.com/gameframex/com.gameframex.unity.download/releases) 了解更新日志。

## 开源协议

本项目开源，详情请参阅 [LICENSE](LICENSE.md)。
