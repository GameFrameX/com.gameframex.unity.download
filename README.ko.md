<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="Game Frame X Logo" width="160" />

# Game Frame X Download 다운로드 작업 컴포넌트

[![License](https://img.shields.io/github/license/GameFrameX/com.gameframex.unity.download)](https://github.com/GameFrameX/com.gameframex.unity.download/blob/main/LICENSE.md)
[![Version](https://img.shields.io/github/v/release/GameFrameX/com.gameframex.unity.download)](https://github.com/GameFrameX/com.gameframex.unity.download/releases)
[![Documentation](https://img.shields.io/badge/Documentation-docs-blue)](https://gameframex.doc.alianblank.com)

인디 게임 개발자를 위한 올인원 솔루션 · 인디 개발자의 꿈을 실현

<br />

[문서](https://gameframex.doc.alianblank.com) · [빠른 시작](#빠른-시작) · QQ 그룹: 467608841 / 233840761

<br />

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | **한국어**

</div>
## 개요

Unity 멀티 에이전트 다운로드 매니저. 동시 파일 다운로드, 우선순위 큐, 구성 가능한 에이전트 풀, 일시정지/재개, 실시간 속도 보고, 이벤트 기반 콜백을 지원합니다.

## 기능

- **멀티 에이전트 동시성** — 구성 가능한 에이전트 풀(기본 3개)로 병렬 다운로드
- **우선순위 스케줄링** — 높은 우선순위 작업을 우선 디스패치
- **태그 그룹화** — 태그별로 작업 추가, 조회 및 제거
- **일시정지 및 재개** — `Paused`로 전체 다운로드 일시정지/재개
- **타임아웃 및 플러시** — 컴포넌트 수준 타임아웃 설정 및 디스크 쓰기 임계값(`FlushSize`), 중단점 이어서 받기 지원
- **실시간 지표** — `CurrentSpeed`, 에이전트 수, 대기 작업 수
- **이벤트 기반** — Event Component를 통해 `DownloadStart` / `DownloadUpdate` / `DownloadSuccess` / `DownloadFailure` 발생
- **비동기 지원** — `Download()`이 `Task<bool>`을 반환하여 await 가능
- **플러그인 가능한 백엔드** — 기본 제공 `UnityWebRequestDownloadAgentHelper`; Inspector로 전환 또는 `IDownloadAgentHelper` 구현 가능

## 설치 방법

다음 중 하나를 선택하세요:

1. **Scoped Registry (권장)** — `Packages/manifest.json` 편집:
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

   `scopes`는 이 레지스트리를 통해 어떤 패키지를 해석할지 제어합니다. `com.gameframex`로 시작하는 패키지만 이 레지스트리에서 가져옵니다.
2. **Git URL** — Unity Package Manager에서 `https://github.com/AlianBlank/com.gameframex.unity.download.git` 추가
3. **로컬 클론** — 프로젝트의 `Packages/` 디렉토리에 클론

## 빠른 시작

### 이벤트 기반 사용법

```csharp
// DownloadComponent 가져오기 (GameEntry에 연결됨)
var downloadComponent = GameEntry.GetComponent<DownloadComponent>();

// EventComponent를 통해 이벤트 구독
var eventComponent = GameEntry.GetComponent<EventComponent>();
eventComponent.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
eventComponent.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);

// 다운로드 작업 추가
int serialId = downloadComponent.AddDownload(
    "/로컬/저장/경로/file.zip",      // downloadPath
    "https://example.com/file.zip"   // downloadUri
);

void OnDownloadSuccess(object sender, GameEventArgs e)
{
    var args = (DownloadSuccessEventArgs)e;
    if (args.SerialId == serialId)
    {
        // 다운로드 완료
    }
}

void OnDownloadFailure(object sender, GameEventArgs e)
{
    var args = (DownloadFailureEventArgs)e;
    Debug.LogError($"다운로드 실패: {args.ErrorMessage}");
}
```

### 비동기 사용법

```csharp
var downloadComponent = GameEntry.GetComponent<DownloadComponent>();

bool success = await downloadComponent.Download(
    "/로컬/저장/경로/file.zip",
    "https://example.com/file.zip"
);

if (success)
{
    // 다운로드 완료
}
```

### 태그 및 우선순위

```csharp
// 태그와 우선순위를 지정하여 추가
int serialId = downloadComponent.AddDownload(
    downloadPath, downloadUri,
    tag: "assets",       // 그룹 라벨
    priority: 10         // 높을수록 우선
);

// 태그로 조회
TaskInfo[] infos = downloadComponent.GetDownloadInfos("assets");

// 태그 그룹의 모든 작업 제거
downloadComponent.RemoveDownloads("assets");
```

### 일시정지 및 재개

```csharp
downloadComponent.Paused = true;   // 모든 다운로드 일시정지
downloadComponent.Paused = false;  // 재개
```

## API 참조

### 핵심 속성

| 속성 | 타입 | 설명 |
|------|------|------|
| `Paused` | `bool` | 모든 다운로드 일시정지 또는 재개 |
| `Timeout` | `float` | 다운로드 타임아웃 (초), 기본 30 |
| `FlushSize` | `int` | 디스크 쓰기 임계값 (바이트), 기본 1 MB |
| `CurrentSpeed` | `float` | 현재 총 다운로드 속도 |
| `TotalAgentCount` | `int` | 다운로드 에이전트 총 수 |
| `FreeAgentCount` | `int` | 유휴 에이전트 수 |
| `WorkingAgentCount` | `int` | 작업 중 에이전트 수 |
| `WaitingTaskCount` | `int` | 에이전트 대기 중인 작업 수 |

### 주요 메서드

| 메서드 | 반환값 | 설명 |
|--------|--------|------|
| `AddDownload(path, uri, ...)` | `int` | 작업 추가, 시리얼 ID 반환 |
| `Download(path, uri)` | `Task<bool>` | 작업 추가, await 가능 |
| `RemoveDownload(serialId)` | `bool` | 단일 작업 제거 |
| `RemoveDownloads(tag)` | `int` | 지정 태그의 모든 작업 제거 |
| `RemoveAllDownloads()` | `int` | 모든 작업 제거 |
| `GetDownloadInfo(serialId)` | `TaskInfo` | 단일 작업 조회 |
| `GetDownloadInfos(tag)` | `TaskInfo[]` | 태그로 작업 조회 |
| `GetAllDownloadInfos()` | `TaskInfo[]` | 모든 작업 조회 |

## 의존성

- [com.gameframex.unity.event](https://github.com/AlianBlank/com.gameframex.unity.event) >= 1.1.0

## 링크

- [문서](https://gameframex.doc.alianblank.com)
- [변경 로그](https://github.com/gameframex/com.gameframex.unity.download/releases)
- QQ 그룹: 467608841 / 233840761

## 라이선스

이 프로젝트는 듀얼 라이선스로 제공됩니다. 자세한 내용은 [LICENSE](LICENSE.md)를 참조하세요.
