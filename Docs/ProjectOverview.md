# Project Overview

## 프로젝트

- 이름: **DoDoDoIt!**
- 형태: 12인 팀 프로젝트
- 장르: 3D Rope Action Running Game
- 엔진: Unity 2022.3 계열
- 주요 연동: Cinemachine 2.9.7, FMOD, Addressables, DOTween, URP

## 검증된 담당 범위

Git Commit과 blame으로 다음 기여를 확인했습니다.

- SpringJoint Rope Prototype 제작
- 고정 반경·회전 기반 Swing 구현
- Rope Enter / Swing / Exit lifecycle 구성
- Rotation Rope와 Curve 방향 처리
- Target 범위·각도·진행 방향 판정
- Rope 상태별 Cinemachine Camera 전환
- Rope 단계별 FMOD parameter 호출
- Ground 진입, 조기 종료, 추락, 방향 오류 등 Release 안정화

## 협업 코드와의 경계

Player 공용 StateMachine은 `StarPilot` 계열 작성자가 먼저 도입했습니다. Release의 `PlayerController`, Running, Jump, Falling도 팀원 코드 비중이 높습니다.

따라서 이 Repository에서 주장하는 범위는 다음과 같습니다.

> 팀의 Player State 기반 위에 Rope lifecycle과 이동 규칙을 구현하고, Target·Camera·Sound·Curve 시스템과 통합했다.

다음 내용은 주장하지 않습니다.

- Player State Pattern 전체 설계
- PlayerController 전체 구현
- Running/Jump/Falling 전체 구현
- CameraManager 전체 구현
- 팀 공용 Resource/UI/Scene Manager 구현

## Repository 성격

본 저장소는 코드 검토용 자료이며 Unity 프로젝트가 아닙니다.

- `.cs` 원본만 보존
- 실행 의존성은 문서로 설명
- 코드 수정 없이 Commit snapshot 사용
- Scene과 Asset을 통한 기능 재현은 하지 않음
