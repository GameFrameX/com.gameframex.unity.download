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

## プロジェクト概要

**Download ダウンロードタスクコンポーネント (Download Component)** - ダウンロードキューの管理、ダウンロードタスクの処理、ダウンロード状況のリアルタイム更新を提供するインターフェース。

## クイックスタート

### インストール方法（いずれかを選択）

1. Unity プロジェクトの `Packages/manifest.json` を編集し、`scopedRegistries` セクションを追加してください：
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

   `scopes` は、どのパッケージをこのレジストリから解決するかを制御します。`com.gameframex` で始まるパッケージのみがこのレジストリから取得されます。
2. Unity の `Packages Manager` で `Git URL` を使用して追加：`https://github.com/AlianBlank/com.gameframex.unity.download.git`
3. リポジトリを直接ダウンロードして Unity プロジェクトの `Packages` ディレクトリに配置すると、自動的に読み込まれます。

## 使用例

`DownloadComponent` はダウンロードタスクを処理するためのゲームフレームワークコンポーネントです。ダウンロードキューの管理、ダウンロードタスクの処理、リアルタイムのステータス更新を行います。

### 機能概要

- 複数のダウンロードタスクを管理
- レジューム機能（中断からの再開）をサポート
- ダウンロードタスクの優先度設定
- リアルタイムのダウンロード進捗と速度の更新
- イベントによるダウンロードの各段階の状態通知

### コアプロパティ

- `Paused`：ダウンロードが一時停止されているかどうかを取得または設定します。
- `TotalAgentCount`：ダウンロードエージェントの総数を取得します。
- `FreeAgentCount`：利用可能なダウンロードエージェントの数を取得します。
- `WorkingAgentCount`：動作中のダウンロードエージェントの数を取得します。
- `WaitingTaskCount`：待機中のダウンロードタスクの数を取得します。
- `Timeout`：ダウンロードタイムアウト時間を取得または設定します。
- `FlushSize`：ディスク書き込みのしきい値サイズを取得または設定します。
- `CurrentSpeed`：現在のダウンロード速度を取得します。

### ダウンロードタスクの追加

```csharp
// 指定されたダウンロードパスとURIでダウンロードタスクを追加
public int AddDownload(string downloadPath, string downloadUri);

// 指定されたダウンロードパス、URI、タグでダウンロードタスクを追加
public int AddDownload(string downloadPath, string downloadUri, string tag);

// 指定されたダウンロードパス、URI、優先度でダウンロードタスクを追加
public int AddDownload(string downloadPath, string downloadUri, int priority);

// 指定されたダウンロードパス、URI、ユーザーデータでダウンロードタスクを追加
public int AddDownload(string downloadPath, string downloadUri, object userData);
```

### ダウンロードタスクの削除

```csharp
// シリアルIDでダウンロードタスクを削除
public bool RemoveDownload(int serialId);

// タグでダウンロードタスクを削除
public int RemoveDownloads(string tag);

// すべてのダウンロードタスクを削除
public int RemoveAllDownloads();
```

### イベント通知

`DownloadComponent` は、ダウンロードタスクの開始、更新、成功、失敗時に通知を受け取るためのイベントを提供します。

- `DownloadStart`：ダウンロード開始時にトリガーされます。
- `DownloadUpdate`：ダウンロード更新時にトリガーされます。
- `DownloadSuccess`：ダウンロード成功時にトリガーされます。
- `DownloadFailure`：ダウンロード失敗時にトリガーされます。

### 例

```csharp
// downloadComponent インスタンスが存在すると仮定
int serialId = downloadComponent.AddDownload("ローカル/保存/パス", "https://example.com/file.zip");

// イベントコンポーネントを通じてダウンロード成功イベントをサブスクライブ
eventComponent.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);

// ダウンロード成功コールバック
void OnDownloadSuccess(object sender, GameEventArgs e)
{
    DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs)e;
    if (ne.SerialId == serialId)
    {
        // ダウンロード成功の処理
    }
}
```

> **注意：** このコンポーネントは [Event イベントコンポーネント](https://github.com/AlianBlank/com.gameframex.unity.event) に依存しています。

## ドキュメントとリソース

- [ドキュメント](https://gameframex.doc.alianblank.com)

## コミュニティとサポート

- [QQグループ](https://qm.qq.com/q/5kbDVBdUeS)

## 変更履歴

変更履歴は [Releases](https://github.com/gameframex/com.gameframex.unity.download/releases) をご覧ください。

## ライセンス

このプロジェクトはオープンソースです。詳細は [LICENSE](LICENSE.md) をご覧ください。
