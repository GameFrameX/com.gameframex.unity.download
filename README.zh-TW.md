<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="GameFrameX Logo" width="160"/>

# Game Frame X Download 下載任務組件

[![License](https://img.shields.io/github/license/gameframex/com.gameframex.unity.download)](https://github.com/gameframex/com.gameframex.unity.download/blob/main/LICENSE)
[![Version](https://img.shields.io/github/v/release/gameframex/com.gameframex.unity.download)](https://github.com/gameframex/com.gameframex.unity.download/releases)
[![Documentation](https://img.shields.io/badge/Documentation-文檔-blue)](https://gameframex.doc.alianblank.com)

獨立遊戲前後端一體化解決方案 · 獨立遊戲開發者的圓夢大使

[文檔](https://gameframex.doc.alianblank.com) · [快速開始](#快速開始) · [QQ群](https://qm.qq.com/q/5kbDVBdUeS) · **語言**

[English](README.md) | [简体中文](README.zh-CN.md) | **繁體中文** | [日本語](README.ja.md) | [한국어](README.ko.md)

</div>

---

## 項目簡介

**Download 下載任務組件 (Download Component)** - 提供管理下載佇列、處理下載任務，並提供下載狀態的即時更新相關的介面。

## 快速開始

### 安裝方式（任選其一）

1. 直接在 `manifest.json` 的 `dependencies` 節點下新增以下內容：
   ```json
   {
      "com.gameframex.unity.event": "https://github.com/AlianBlank/com.gameframex.unity.event.git",
      "com.gameframex.unity.download": "https://github.com/AlianBlank/com.gameframex.unity.download.git"
   }
   ```
2. 在 Unity 的 `Packages Manager` 中使用 `Git URL` 的方式新增庫，地址為：`https://github.com/AlianBlank/com.gameframex.unity.download.git`
3. 直接下載倉庫放置到 Unity 專案的 `Packages` 目錄下，會自動載入識別。

## 使用範例

`DownloadComponent` 是一個用於處理下載任務的遊戲框架組件。它負責管理下載佇列、處理下載任務，並提供下載狀態的即時更新。

### 功能概述

- 管理多個下載任務
- 支援斷點續傳功能
- 提供下載任務的優先級設定
- 即時更新下載進度和下載速度
- 可以透過事件接收下載的各個階段狀態

### 核心屬性

- `Paused`：取得或設定下載是否被暫停。
- `TotalAgentCount`：取得下載代理總數量。
- `FreeAgentCount`：取得可用下載代理數量。
- `WorkingAgentCount`：取得工作中下載代理數量。
- `WaitingTaskCount`：取得等待下載任務數量。
- `Timeout`：取得或設定下載逾時時長。
- `FlushSize`：取得或設定寫入磁碟的臨界大小。
- `CurrentSpeed`：取得目前下載速度。

### 如何增加下載任務

```csharp
// 根據指定的下載路徑和URI增加下載任務
public int AddDownload(string downloadPath, string downloadUri);

// 根據指定的下載路徑、URI和任務標籤增加下載任務
public int AddDownload(string downloadPath, string downloadUri, string tag);

// 根據指定的下載路徑、URI和優先級增加下載任務
public int AddDownload(string downloadPath, string downloadUri, int priority);

// 根據指定的下載路徑、URI和使用者自訂資料增加下載任務
public int AddDownload(string downloadPath, string downloadUri, object userData);
```

### 如何移除下載任務

```csharp
// 根據序列編號移除下載任務
public bool RemoveDownload(int serialId);

// 根據標籤移除下載任務
public int RemoveDownloads(string tag);

// 移除所有下載任務
public int RemoveAllDownloads();
```

### 事件通知

`DownloadComponent` 提供事件，以便於當下載任務開始、更新、成功、失敗時接收通知。

- `DownloadStart`：當下載開始時觸發。
- `DownloadUpdate`：當下載更新時觸發。
- `DownloadSuccess`：當下載成功時觸發。
- `DownloadFailure`：當下載失敗時觸發。

### 範例

```csharp
// 假設已經存在 downloadComponent 實例
int serialId = downloadComponent.AddDownload("本地儲存路徑", "下載連結");

// 透過事件組件訂閱下載成功事件
eventComponent.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);

// 下載成功回呼
void OnDownloadSuccess(object sender, GameEventArgs e)
{
    DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs)e;
    if (ne.SerialId == serialId)
    {
        // 處理下載成功
    }
}
```

> **注意：** 此組件依賴於 [Event 事件組件](https://github.com/AlianBlank/com.gameframex.unity.event)。

## 文檔與資源

- [文檔](https://gameframex.doc.alianblank.com)

## 社區與支援

- [QQ群](https://qm.qq.com/q/5kbDVBdUeS)

## 更新日誌

查看 [Releases](https://github.com/gameframex/com.gameframex.unity.download/releases) 了解更新日誌。

## 開源協議

本專案基於 [MIT 協議](https://github.com/gameframex/com.gameframex.unity.download/blob/main/LICENSE) 開源。
