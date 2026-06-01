<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="GameFrameX Logo" width="160"/>

# Game Frame X Download ダウンロードタスクコンポーネント

[![License](https://img.shields.io/github/license/gameframex/com.gameframex.unity.download)](https://github.com/gameframex/com.gameframex.unity.download/blob/main/LICENSE)
[![Version](https://img.shields.io/github/v/release/gameframex/com.gameframex.unity.download)](https://github.com/gameframex/com.gameframex.unity.download/releases)
[![Documentation](https://img.shields.io/badge/Documentation-ドキュメント-blue)](https://gameframex.doc.alianblank.com)

インディゲーム開発者向けオールインワンソリューション · インディ開発者の夢を支援

[ドキュメント](https://gameframex.doc.alianblank.com) · [クイックスタート](#クイックスタート) · [QQグループ](https://qm.qq.com/q/5kbDVBdUeS) · **言語**

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | **日本語** | [한국어](README.ko.md)

</div>

---

## 概要

Unity 向けマルチエージェントダウンロードマネージャー。並行ファイルダウンロード、優先度キュー、設定可能なエージェントプール、一時停止/再開、リアルタイム速度報告、イベント駆動コールバックを提供します。

## 機能

- **マルチエージェント並行処理** — 設定可能なエージェントプール（デフォルト 3 エージェント）による並行ダウンロード
- **優先度スケジューリング** — 高優先度タスクを優先的にディスパッチ
- **タググループ化** — タグによるタスクの追加・照会・削除
- **一時停止・再開** — `Paused` で全ダウンロードをグローバルに一時停止/再開
- **タイムアウト・フラッシュ** — コンポーネントレベルのタイムアウト設定とディスク書き込み閾値（`FlushSize`）、ブレークポイント再開をサポート
- **リアルタイムメトリクス** — `CurrentSpeed`、エージェント数、待機タスク数
- **イベント駆動** — Event Component 経由で `DownloadStart` / `DownloadUpdate` / `DownloadSuccess` / `DownloadFailure` を発火
- **非同期対応** — `Download()` が `Task<bool>` を返し、await 可能
- **プラグイン可能バックエンド** — 組み込み `UnityWebRequestDownloadAgentHelper`；Inspector で切り替え、または `IDownloadAgentHelper` を実装可能

## インストール

以下のいずれかの方法を選択してください：

1. **Scoped Registry（推奨）** — `Packages/manifest.json` を編集：
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

   `scopes` は、どのパッケージをこのレジストリから解決するかを制御します。`com.gameframex` で始まるパッケージのみがこのレジストリから取得されます。
2. **Git URL** — Unity Package Manager で `https://github.com/AlianBlank/com.gameframex.unity.download.git` を追加
3. **ローカルクローン** — プロジェクトの `Packages/` ディレクトリにクローン

## クイックスタート

### イベント駆動の使用方法

```csharp
// DownloadComponent を取得（GameEntry にアタッチ済み）
var downloadComponent = GameEntry.GetComponent<DownloadComponent>();

// EventComponent 経由でイベントをサブスクライブ
var eventComponent = GameEntry.GetComponent<EventComponent>();
eventComponent.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
eventComponent.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);

// ダウンロードタスクを追加
int serialId = downloadComponent.AddDownload(
    "/ローカル保存パス/file.zip",    // downloadPath
    "https://example.com/file.zip"   // downloadUri
);

void OnDownloadSuccess(object sender, GameEventArgs e)
{
    var args = (DownloadSuccessEventArgs)e;
    if (args.SerialId == serialId)
    {
        // ダウンロード完了
    }
}

void OnDownloadFailure(object sender, GameEventArgs e)
{
    var args = (DownloadFailureEventArgs)e;
    Debug.LogError($"ダウンロード失敗：{args.ErrorMessage}");
}
```

### 非同期の使用方法

```csharp
var downloadComponent = GameEntry.GetComponent<DownloadComponent>();

bool success = await downloadComponent.Download(
    "/ローカル保存パス/file.zip",
    "https://example.com/file.zip"
);

if (success)
{
    // ダウンロード完了
}
```

### タグと優先度

```csharp
// タグと優先度を指定して追加
int serialId = downloadComponent.AddDownload(
    downloadPath, downloadUri,
    tag: "assets",       // グループラベル
    priority: 10         // 高いほど優先
);

// タグで照会
TaskInfo[] infos = downloadComponent.GetDownloadInfos("assets");

// タググループの全タスクを削除
downloadComponent.RemoveDownloads("assets");
```

### 一時停止・再開

```csharp
downloadComponent.Paused = true;   // 全ダウンロードを一時停止
downloadComponent.Paused = false;  // 再開
```

## API リファレンス

### コアプロパティ

| プロパティ | 型 | 説明 |
|-----------|-----|------|
| `Paused` | `bool` | 全ダウンロードの一時停止・再開 |
| `Timeout` | `float` | ダウンロードタイムアウト（秒）、デフォルト 30 |
| `FlushSize` | `int` | ディスク書き込み閾値（バイト）、デフォルト 1 MB |
| `CurrentSpeed` | `float` | 現在の総ダウンロード速度 |
| `TotalAgentCount` | `int` | ダウンロードエージェント総数 |
| `FreeAgentCount` | `int` | アイドルエージェント数 |
| `WorkingAgentCount` | `int` | 稼働中エージェント数 |
| `WaitingTaskCount` | `int` | エージェント待ちタスク数 |

### 主要メソッド

| メソッド | 戻り値 | 説明 |
|---------|--------|------|
| `AddDownload(path, uri, ...)` | `int` | タスクを追加、シリアル ID を返す |
| `Download(path, uri)` | `Task<bool>` | タスクを追加、await 可能 |
| `RemoveDownload(serialId)` | `bool` | 単一タスクを削除 |
| `RemoveDownloads(tag)` | `int` | 指定タグの全タスクを削除 |
| `RemoveAllDownloads()` | `int` | 全タスクを削除 |
| `GetDownloadInfo(serialId)` | `TaskInfo` | 単一タスクを照会 |
| `GetDownloadInfos(tag)` | `TaskInfo[]` | タグでタスクを照会 |
| `GetAllDownloadInfos()` | `TaskInfo[]` | 全タスクを照会 |

## 依存関係

- [com.gameframex.unity.event](https://github.com/AlianBlank/com.gameframex.unity.event) >= 1.1.0

## リンク

- [ドキュメント](https://gameframex.doc.alianblank.com)
- [変更履歴](https://github.com/gameframex/com.gameframex.unity.download/releases)
- [QQグループ](https://qm.qq.com/q/5kbDVBdUeS)

## ライセンス

本プロジェクトはデュアルライセンスで公開されています。詳細は [LICENSE](LICENSE.md) をご覧ください。
