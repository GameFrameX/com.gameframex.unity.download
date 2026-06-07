<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Download Component

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.download)](https://github.com/GameFrameX/com.gameframex.unity.download/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.download)](https://github.com/GameFrameX/com.gameframex.unity.download/releases)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4-black?logo=unity)](https://unity.com/)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

All-in-One Solution for Indie Game Development · Empowering Indie Developers' Dreams

<br />

[Documentation](https://gameframex.doc.alianblank.com) · [Quick Start](#quick-start) · QQ Group: 467608841 / 233840761

<br />

**English** | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

## Overview

Multi-agent download manager for Unity. Handles concurrent file downloads with priority queuing, configurable agent pools, pause/resume, real-time speed reporting, and event-driven callbacks.

## Features

- **Multi-agent concurrency** — Configurable agent pool (default 3 agents) for parallel downloads
- **Priority scheduling** — Higher priority tasks are dispatched first
- **Tag-based grouping** — Add, query, and remove tasks by tag
- **Pause & resume** — Globally pause/resume all downloads via `Paused`
- **Timeout & flush** — Per-component timeout and disk flush threshold (`FlushSize`) for breakpoint resume
- **Real-time metrics** — `CurrentSpeed`, agent counts, and waiting task count
- **Event-driven** — `DownloadStart` / `DownloadUpdate` / `DownloadSuccess` / `DownloadFailure` events via the Event Component
- **Async/await support** — `Download()` returns `Task<bool>` for awaitable downloads
- **Pluggable backends** — Built-in `UnityWebRequestDownloadAgentHelper`; swap via Inspector or implement `IDownloadAgentHelper`

## Installation

Choose one of the following methods:

1. **Scoped Registry (recommended)** — Edit `Packages/manifest.json`:
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

2. **Git URL** — In Unity Package Manager, add `https://github.com/AlianBlank/com.gameframex.unity.download.git`
3. **Local clone** — Clone into your project's `Packages/` directory

## Quick Start

### Event-based usage

```csharp
// Get the DownloadComponent (attached to GameEntry)
var downloadComponent = GameEntry.GetComponent<DownloadComponent>();

// Subscribe to events via EventComponent
var eventComponent = GameEntry.GetComponent<EventComponent>();
eventComponent.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
eventComponent.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);

// Add a download task
int serialId = downloadComponent.AddDownload(
    "/local/save/path/file.zip",    // downloadPath
    "https://example.com/file.zip"   // downloadUri
);

void OnDownloadSuccess(object sender, GameEventArgs e)
{
    var args = (DownloadSuccessEventArgs)e;
    if (args.SerialId == serialId)
    {
        // Download complete
    }
}

void OnDownloadFailure(object sender, GameEventArgs e)
{
    var args = (DownloadFailureEventArgs)e;
    Debug.LogError($"Download failed: {args.ErrorMessage}");
}
```

### Async/await usage

```csharp
var downloadComponent = GameEntry.GetComponent<DownloadComponent>();

bool success = await downloadComponent.Download(
    "/local/save/path/file.zip",
    "https://example.com/file.zip"
);

if (success)
{
    // Download complete
}
```

### Tagged & prioritized downloads

```csharp
// Add with tag and priority
int serialId = downloadComponent.AddDownload(
    downloadPath, downloadUri,
    tag: "assets",       // group label
    priority: 10         // higher = sooner
);

// Query by tag
TaskInfo[] infos = downloadComponent.GetDownloadInfos("assets");

// Remove all tasks in a tag group
downloadComponent.RemoveDownloads("assets");
```

### Pause & resume

```csharp
downloadComponent.Paused = true;   // pause all downloads
downloadComponent.Paused = false;  // resume
```

## API Reference

### Core Properties

| Property | Type | Description |
|----------|------|-------------|
| `Paused` | `bool` | Pause or resume all downloads |
| `Timeout` | `float` | Download timeout in seconds (default 30) |
| `FlushSize` | `int` | Disk write threshold in bytes (default 1 MB) |
| `CurrentSpeed` | `float` | Current aggregate download speed |
| `TotalAgentCount` | `int` | Total number of download agents |
| `FreeAgentCount` | `int` | Available (idle) agents |
| `WorkingAgentCount` | `int` | Busy agents |
| `WaitingTaskCount` | `int` | Tasks waiting for a free agent |

### Key Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `AddDownload(path, uri, ...)` | `int` | Add a task; returns serial ID |
| `Download(path, uri)` | `Task<bool>` | Add a task; awaitable |
| `RemoveDownload(serialId)` | `bool` | Remove a single task |
| `RemoveDownloads(tag)` | `int` | Remove all tasks with the given tag |
| `RemoveAllDownloads()` | `int` | Remove all tasks |
| `GetDownloadInfo(serialId)` | `TaskInfo` | Query a single task |
| `GetDownloadInfos(tag)` | `TaskInfo[]` | Query tasks by tag |
| `GetAllDownloadInfos()` | `TaskInfo[]` | Query all tasks |

## Dependencies

- [com.gameframex.unity.event](https://github.com/AlianBlank/com.gameframex.unity.event) >= 1.1.0

## Links

- [Documentation](https://gameframex.doc.alianblank.com)
- [Changelog](https://github.com/gameframex/com.gameframex.unity.download/releases)
- QQ Group: 467608841 / 233840761

## License

This project is dual-licensed under [MIT](LICENSE.md) and [Apache-2.0](LICENSE.md).
