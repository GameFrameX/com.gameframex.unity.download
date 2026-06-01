<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="GameFrameX Logo" width="160"/>

# Game Frame X Download Component

[![License](https://img.shields.io/github/license/gameframex/com.gameframex.unity.download)](https://github.com/gameframex/com.gameframex.unity.download/blob/main/LICENSE)
[![Version](https://img.shields.io/github/v/release/gameframex/com.gameframex.unity.download)](https://github.com/gameframex/com.gameframex.unity.download/releases)
[![Documentation](https://img.shields.io/badge/Documentation-Documentation-blue)](https://gameframex.doc.alianblank.com)

All-in-One Solution for Indie Game Development · Empowering Indie Developers' Dreams

[Documentation](https://gameframex.doc.alianblank.com) · [Quick Start](#quick-start) · [QQ Group](https://qm.qq.com/q/5kbDVBdUeS) · **Language**

**English** | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

---

## Project Overview

The **Download Component** provides interfaces for managing download queues, handling download tasks, and providing real-time updates on download status.

## Quick Start

### Installation

Choose one of the following methods:

1. Edit your Unity project's `Packages/manifest.json` and add the `scopedRegistries` section:
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

   `scopes` controls which packages are resolved through this registry. Only packages whose names start with `com.gameframex` will be fetched from it.
2. Use **Packages Manager** in Unity with **Git URL**: `https://github.com/AlianBlank/com.gameframex.unity.download.git`
3. Clone the repository into your Unity project's `Packages` directory. It will be loaded automatically.

## Usage Examples

`DownloadComponent` is a game framework component for handling download tasks. It manages download queues, processes download tasks, and provides real-time status updates.

### Features

- Manage multiple download tasks
- Support for resuming interrupted downloads
- Priority-based task scheduling
- Real-time download progress and speed updates
- Event-driven notifications for download stages

### Core Properties

- `Paused`: Gets or sets whether downloads are paused.
- `TotalAgentCount`: Gets the total number of download agents.
- `FreeAgentCount`: Gets the number of available download agents.
- `WorkingAgentCount`: Gets the number of working download agents.
- `WaitingTaskCount`: Gets the number of waiting download tasks.
- `Timeout`: Gets or sets the download timeout duration.
- `FlushSize`: Gets or sets the threshold size for writing to disk.
- `CurrentSpeed`: Gets the current download speed.

### Adding Download Tasks

```csharp
// Add a download task with the specified path and URI
public int AddDownload(string downloadPath, string downloadUri);

// Add a download task with the specified path, URI, and tag
public int AddDownload(string downloadPath, string downloadUri, string tag);

// Add a download task with the specified path, URI, and priority
public int AddDownload(string downloadPath, string downloadUri, int priority);

// Add a download task with the specified path, URI, and user data
public int AddDownload(string downloadPath, string downloadUri, object userData);
```

### Removing Download Tasks

```csharp
// Remove a download task by serial ID
public bool RemoveDownload(int serialId);

// Remove download tasks by tag
public int RemoveDownloads(string tag);

// Remove all download tasks
public int RemoveAllDownloads();
```

### Event Notifications

`DownloadComponent` provides events for receiving notifications when download tasks start, update, succeed, or fail.

- `DownloadStart`: Triggered when a download starts.
- `DownloadUpdate`: Triggered when a download updates.
- `DownloadSuccess`: Triggered when a download succeeds.
- `DownloadFailure`: Triggered when a download fails.

### Example

```csharp
// Assuming a downloadComponent instance exists
int serialId = downloadComponent.AddDownload("local/storage/path", "https://example.com/file.zip");

// Subscribe to download success event via event component
eventComponent.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);

// Download success callback
void OnDownloadSuccess(object sender, GameEventArgs e)
{
    DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs)e;
    if (ne.SerialId == serialId)
    {
        // Handle download success
    }
}
```

> **Note:** This component depends on the [Event Component](https://github.com/AlianBlank/com.gameframex.unity.event).

## Documentation & Resources

- [Documentation](https://gameframex.doc.alianblank.com)

## Community & Support

- [QQ Group](https://qm.qq.com/q/5kbDVBdUeS)

## Changelog

See [Releases](https://github.com/gameframex/com.gameframex.unity.download/releases) for changelog.

## License

This project is open source. See [LICENSE](LICENSE.md) for details.
