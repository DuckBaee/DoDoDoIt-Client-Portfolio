# Project Overview

## 프로젝트 소개

**DoDoDoIt!**은 12명이 함께 제작한 3D Rope Action Running Game입니다. 저는 클라이언트 프로그래머로 참여해 Player와 Rope를 중심으로 기능을 개발하고, Camera·Sound·UI를 연결해 최종 빌드를 안정화했습니다.

| 항목 | 내용 |
|---|---|
| 인원 | 12명 |
| 엔진 | Unity 2022.3 |
| 장르 | 3D Rope Action Running Game |
| 주요 기술 | C#, Rigidbody, Cinemachine, FMOD, Addressables |

## 담당 업무

- Player 이동·Jump 상태 연동과 오류 수정
- SpringJoint Rope Prototype 제작
- 고정 반경·회전 기반 Swing 구현
- Rope Enter / Swing / Exit 상태 구현
- Target 범위·각도·진행 방향 판정
- Rotation Rope와 Curve Level 대응
- Rope 상태별 Cinemachine Camera 전환
- Rope 단계별 FMOD Sound 연동
- 실제 레벨에서 발생한 Rope 문제 수정

## 가장 크게 바꾼 부분

처음에는 SpringJoint의 물리 반응으로 Rope 움직임을 만들었습니다. Prototype을 플레이하며 Runner 게임에서는 진입 조건에 따라 궤적이 크게 달라지는 것보다 속도와 종료 시점을 직접 제어하는 편이 적합하다고 판단했습니다.

이후 고정 반경과 회전 기반 이동으로 바꾸고, 하나의 State에 모여 있던 로직을 Enter·Swing·Exit로 나눴습니다. 최종 단계에서는 Player 방향을 기준으로 Target과 회전축을 계산해 Curve Level까지 대응했습니다.

## 협업 범위

PlayerStateMachine과 PlayerController는 팀 공용 시스템입니다. 저는 이 구조 위에 Rope 상태를 구현하고 Camera, Sound, Target UI와 Level Trigger를 연결했습니다.

코드가 공동 수정된 경우에는 [CodeOwnership.md](CodeOwnership.md)에 그대로 표시했습니다.

## 저장소 구성

이 저장소는 Unity 프로젝트를 다시 만든 것이 아니라 실제 개발 코드를 보여주기 위한 저장소입니다.

- 각 단계의 실제 Git Commit 코드 보존
- C# 원본 코드 수정 없음
- Scene, Prefab, Material, Animation과 외부 Asset 제외
- 포함하지 않은 팀 시스템은 문서로 관계만 설명
