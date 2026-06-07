<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Download 下載任務組件

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.download)](https://github.com/GameFrameX/com.gameframex.unity.download/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.download)](https://github.com/GameFrameX/com.gameframex.unity.download/releases)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

獨立遊戲前後端一體化解決方案 · 獨立遊戲開發者的圓夢大使

<br />

[文檔](https://gameframex.doc.alianblank.com) · [快速開始](#快速開始) · QQ群: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | **繁體中文** | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>
## 概述

Unity 多代理下載管理器。支援並行檔案下載、優先級佇列、可配置代理池、暫停/恢復、即時速度回報，以及事件驅動的回調通知。

## 功能特性

- **多代理並行** — 可配置代理池（預設 3 個代理）平行下載
- **優先級排程** — 高優先級任務優先派發
- **標籤分組** — 按標籤新增、查詢和移除任務
- **暫停與恢復** — 透過 `Paused` 全域暫停/恢復所有下載
- **逾時與刷盤** — 組件級逾時設定和磁碟寫入閾值（`FlushSize`），支援斷點續傳
- **即時指標** — `CurrentSpeed`、代理計數和等待任務數
- **事件驅動** — 透過事件組件觸發 `DownloadStart` / `DownloadUpdate` / `DownloadSuccess` / `DownloadFailure`
- **非同步支援** — `Download()` 回傳 `Task<bool>`，支援 await
- **可插拔後端** — 內建 `UnityWebRequestDownloadAgentHelper`；可透過 Inspector 切換或實作 `IDownloadAgentHelper`

## 安裝方式（任選其一）

1. **Scoped Registry（推薦）** — 編輯 `Packages/manifest.json`：
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

   `scopes` 控制哪些套件透過此註冊表解析。只有以 `com.gameframex` 開頭的套件才會從這個註冊表取得。
2. **Git URL** — 在 Unity Package Manager 中新增 `https://github.com/AlianBlank/com.gameframex.unity.download.git`
3. **本機克隆** — 克隆到專案的 `Packages/` 目錄

## 快速開始

### 事件驅動用法

```csharp
// 取得 DownloadComponent（掛載在 GameEntry 上）
var downloadComponent = GameEntry.GetComponent<DownloadComponent>();

// 透過 EventComponent 訂閱事件
var eventComponent = GameEntry.GetComponent<EventComponent>();
eventComponent.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
eventComponent.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);

// 新增下載任務
int serialId = downloadComponent.AddDownload(
    "/本機儲存路徑/file.zip",      // downloadPath
    "https://example.com/file.zip"  // downloadUri
);

void OnDownloadSuccess(object sender, GameEventArgs e)
{
    var args = (DownloadSuccessEventArgs)e;
    if (args.SerialId == serialId)
    {
        // 下載完成
    }
}

void OnDownloadFailure(object sender, GameEventArgs e)
{
    var args = (DownloadFailureEventArgs)e;
    Debug.LogError($"下載失敗：{args.ErrorMessage}");
}
```

### 非同步用法

```csharp
var downloadComponent = GameEntry.GetComponent<DownloadComponent>();

bool success = await downloadComponent.Download(
    "/本機儲存路徑/file.zip",
    "https://example.com/file.zip"
);

if (success)
{
    // 下載完成
}
```

### 標籤與優先級

```csharp
// 帶標籤和優先級新增
int serialId = downloadComponent.AddDownload(
    downloadPath, downloadUri,
    tag: "assets",       // 分組標籤
    priority: 10         // 數值越高越優先
);

// 按標籤查詢
TaskInfo[] infos = downloadComponent.GetDownloadInfos("assets");

// 移除標籤組內所有任務
downloadComponent.RemoveDownloads("assets");
```

### 暫停與恢復

```csharp
downloadComponent.Paused = true;   // 暫停所有下載
downloadComponent.Paused = false;  // 恢復下載
```

## API 參考

### 核心屬性

| 屬性 | 類型 | 說明 |
|------|------|------|
| `Paused` | `bool` | 暫停或恢復所有下載 |
| `Timeout` | `float` | 下載逾時時間（秒），預設 30 |
| `FlushSize` | `int` | 磁碟寫入閾值（位元組），預設 1 MB |
| `CurrentSpeed` | `float` | 目前總下載速度 |
| `TotalAgentCount` | `int` | 下載代理總數 |
| `FreeAgentCount` | `int` | 閒置代理數 |
| `WorkingAgentCount` | `int` | 工作中代理數 |
| `WaitingTaskCount` | `int` | 等待代理的任務數 |

### 主要方法

| 方法 | 回傳值 | 說明 |
|------|--------|------|
| `AddDownload(path, uri, ...)` | `int` | 新增任務，回傳序列 ID |
| `Download(path, uri)` | `Task<bool>` | 新增任務，可 await |
| `RemoveDownload(serialId)` | `bool` | 移除單一任務 |
| `RemoveDownloads(tag)` | `int` | 移除指定標籤的所有任務 |
| `RemoveAllDownloads()` | `int` | 移除所有任務 |
| `GetDownloadInfo(serialId)` | `TaskInfo` | 查詢單一任務 |
| `GetDownloadInfos(tag)` | `TaskInfo[]` | 按標籤查詢任務 |
| `GetAllDownloadInfos()` | `TaskInfo[]` | 查詢所有任務 |

## 依賴

- [com.gameframex.unity.event](https://github.com/AlianBlank/com.gameframex.unity.event) >= 1.1.0

## 連結

- [文檔](https://gameframex.doc.alianblank.com)
- [更新日誌](https://github.com/gameframex/com.gameframex.unity.download/releases)
- QQ群: 467608841 / 233840761

## 開源協議

本專案採用雙授權發布，詳見 [LICENSE](LICENSE.md)。
