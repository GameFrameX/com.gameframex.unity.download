<div align="center">

<img src="https://download.alianblank.com/gameframex/gameframex_logo_320.png" alt="GameFrameX Logo" width="160"/>

# Game Frame X Download 다운로드 작업 컴포넌트

[![License](https://img.shields.io/github/license/gameframex/com.gameframex.unity.download)](https://github.com/gameframex/com.gameframex.unity.download/blob/main/LICENSE)
[![Version](https://img.shields.io/github/v/release/gameframex/com.gameframex.unity.download)](https://github.com/gameframex/com.gameframex.unity.download/releases)
[![Documentation](https://img.shields.io/badge/Documentation-문서-blue)](https://gameframex.doc.alianblank.com)

인디 게임 개발자를 위한 올인원 솔루션 · 인디 개발자의 꿈을 실현

[문서](https://gameframex.doc.alianblank.com) · [빠른 시작](#빠른-시작) · [QQ 그룹](https://qm.qq.com/q/5kbDVBdUeS) · **언어**

[English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | **한국어**

</div>

---

## 프로젝트 개요

**Download 다운로드 작업 컴포넌트 (Download Component)** - 다운로드 큐 관리, 다운로드 작업 처리, 다운로드 상태의 실시간 업데이트를 제공하는 인터페이스입니다.

## 빠른 시작

### 설치 방법 (선택)

1. `manifest.json`의 `dependencies`에 다음 내용을 추가:
   ```json
   {
      "com.gameframex.unity.event": "https://github.com/AlianBlank/com.gameframex.unity.event.git",
      "com.gameframex.unity.download": "https://github.com/AlianBlank/com.gameframex.unity.download.git"
   }
   ```
2. Unity의 `Packages Manager`에서 `Git URL`을 사용하여 추가: `https://github.com/AlianBlank/com.gameframex.unity.download.git`
3. 저장소를 직접 다운로드하여 Unity 프로젝트의 `Packages` 디렉토리에 배치하면 자동으로 로드됩니다.

## 사용 예시

`DownloadComponent`는 다운로드 작업을 처리하기 위한 게임 프레임워크 컴포넌트입니다. 다운로드 큐를 관리하고, 다운로드 작업을 처리하며, 실시간 상태 업데이트를 제공합니다.

### 기능 개요

- 여러 다운로드 작업 관리
- 이어서 다운로드(재개) 기능 지원
- 다운로드 작업 우선순위 설정
- 실시간 다운로드 진행률 및 속도 업데이트
- 이벤트를 통한 다운로드 각 단계 상태 수신

### 핵심 속성

- `Paused`: 다운로드가 일시 중지되었는지 여부를 가져오거나 설정합니다.
- `TotalAgentCount`: 다운로드 에이전트 총 수를 가져옵니다.
- `FreeAgentCount`: 사용 가능한 다운로드 에이전트 수를 가져옵니다.
- `WorkingAgentCount`: 작업 중인 다운로드 에이전트 수를 가져옵니다.
- `WaitingTaskCount`: 대기 중인 다운로드 작업 수를 가져옵니다.
- `Timeout`: 다운로드 시간 초과 시간을 가져오거나 설정합니다.
- `FlushSize`: 디스크 쓰기 임계값 크기를 가져오거나 설정합니다.
- `CurrentSpeed`: 현재 다운로드 속도를 가져옵니다.

### 다운로드 작업 추가

```csharp
// 지정된 다운로드 경로와 URI로 다운로드 작업 추가
public int AddDownload(string downloadPath, string downloadUri);

// 지정된 다운로드 경로, URI, 태그로 다운로드 작업 추가
public int AddDownload(string downloadPath, string downloadUri, string tag);

// 지정된 다운로드 경로, URI, 우선순위로 다운로드 작업 추가
public int AddDownload(string downloadPath, string downloadUri, int priority);

// 지정된 다운로드 경로, URI, 사용자 데이터로 다운로드 작업 추가
public int AddDownload(string downloadPath, string downloadUri, object userData);
```

### 다운로드 작업 제거

```csharp
// 일련 ID로 다운로드 작업 제거
public bool RemoveDownload(int serialId);

// 태그로 다운로드 작업 제거
public int RemoveDownloads(string tag);

// 모든 다운로드 작업 제거
public int RemoveAllDownloads();
```

### 이벤트 알림

`DownloadComponent`는 다운로드 작업이 시작, 업데이트, 성공, 실패할 때 알림을 받기 위한 이벤트를 제공합니다.

- `DownloadStart`: 다운로드가 시작될 때 트리거됩니다.
- `DownloadUpdate`: 다운로드가 업데이트될 때 트리거됩니다.
- `DownloadSuccess`: 다운로드가 성공할 때 트리거됩니다.
- `DownloadFailure`: 다운로드가 실패할 때 트리거됩니다.

### 예시

```csharp
// downloadComponent 인스턴스가 존재한다고 가정
int serialId = downloadComponent.AddDownload("로컬/저장/경로", "https://example.com/file.zip");

// 이벤트 컴포넌트를 통해 다운로드 성공 이벤트 구독
eventComponent.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);

// 다운로드 성공 콜백
void OnDownloadSuccess(object sender, GameEventArgs e)
{
    DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs)e;
    if (ne.SerialId == serialId)
    {
        // 다운로드 성공 처리
    }
}
```

> **참고:** 이 컴포넌트는 [Event 이벤트 컴포넌트](https://github.com/AlianBlank/com.gameframex.unity.event)에 의존합니다.

## 문서 및 자료

- [문서](https://gameframex.doc.alianblank.com)

## 커뮤니티 및 지원

- [QQ 그룹](https://qm.qq.com/q/5kbDVBdUeS)

## 변경 로그

변경 로그는 [Releases](https://github.com/gameframex/com.gameframex.unity.download/releases)에서 확인하세요.

## 라이선스

이 프로젝트는 [MIT 라이선스](https://github.com/gameframex/com.gameframex.unity.download/blob/main/LICENSE)에 따라 배포됩니다.
