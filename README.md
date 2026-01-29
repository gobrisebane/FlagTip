# FlagTip 🚩  
**Caret-aware text helper for Windows**

FlagTip은 **현재 입력 중인 캐럿(caret) 위치를 정확히 추적**하여  
텍스트 선택, 입력 보조, UI 피드백을 제공하는 Windows 데스크톱 유틸리티입니다.

Chrome, VS Code 등 실제 입력 환경에서  
“지금 내가 어디에 타이핑하고 있는지”를 기준으로 동작하도록 설계되었습니다.

---

## ✨ 주요 기능

- 🖱 **캐럿(Caret) 위치 실시간 추적**
  - `UIAutomation`, `AccessibleObjectFromWindow (OBJID_CARET)` 기반
  - Chrome, 일반 Win32 앱에서 동작

- 🟨 **현재 텍스트 선택 영역 하이라이트**
  - 사용자가 인지하기 쉬운 시각적 피드백 제공

- ⚡ **가볍고 빠른 백그라운드 실행**
  - 트레이 상주
  - 불필요한 리소스 사용 최소화

- 🔁 **Windows 시작 시 자동 실행**
  - MSIX / 작업 스케줄러 기반 안정적인 오토스타트

---

## 🧠 왜 만들었나요?

기존의 텍스트 도구들은  
> “현재 포커스는 어디인지”,  
> “실제 입력 캐럿은 어디인지”  

를 **정확히 알지 못한 채 동작**하는 경우가 많았습니다.

FlagTip은  
👉 *“캐럿 위치를 기준으로 동작하는 도구”*  
라는 명확한 목표를 가지고 만들어졌습니다.

---

## 🛠 기술 스택

- **Language**: C#
- **Framework**: .NET (WinForms)
- **Windows API**
  - UIAutomation
  - MSAA (`AccessibleObjectFromWindow`, `OBJID_CARET`)
- **Packaging**
  - MSIX
  - AppInstaller (자동 업데이트)

---

## 📦 설치

### MSIX (권장)

1. 아래 AppInstaller 파일을 다운로드
2. 더블 클릭 후 설치

> Windows SmartScreen이 경고할 수 있습니다.  
> 정상적인 개인 프로젝트입니다 🙂

---

## 🔄 업데이트

- AppInstaller 기반 자동 업데이트 지원
- 새 버전 배포 시 재부팅 또는 재실행 시 자동 체크

---

## 🚧 현재 상태

- [x] 캐럿 위치 추적
- [x] 텍스트 선택 하이라이트
- [x] 자동 실행
- [ ] 설정 UI 개선
- [ ] 단축키 커스터마이징
- [ ] 다중 앱 예외 처리 강화

---

## 🤝 기여

이 프로젝트는 개인 문제 해결에서 시작된 프로젝트입니다.  
아이디어, 이슈, PR 모두 환영합니다.

---

## 📄 라이선스

MIT License

---

## 👤 만든 사람

**Tori**  
문제 있으면 직접 고쳐버리는 타입의 개발자  
