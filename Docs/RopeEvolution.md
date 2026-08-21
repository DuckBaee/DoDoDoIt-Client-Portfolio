# Rope Action Evolution

## 요약

Rope 구현은 다음 네 단계로 확인됩니다.

```text
SpringJoint Prototype
→ 단일 State의 고정 반경 Swing
→ Enter / Swing / Exit 책임 분리
→ Targeting / Rotation / Camera / Sound / Level 안정화
```

이 과정은 기억에 의존해 재현한 코드가 아니라 각 Git Commit 당시 파일을 그대로 보존한 것입니다.

## 1. Prototype 01 — SpringJoint

- Commit: `1ff69ecb`, 2024-09-14
- 최초 추가 근거: `28e86a2b`, 2024-09-11
- 파일: `RopeActionWithRunning.cs`

### Target 선택

`Camera.main.ScreenPointToRay(Input.mousePosition)`로 Ray를 생성하고, 지정 LayerMask에 대해 최대 25 거리의 Raycast를 수행했습니다. Target Transform이 아니라 `RaycastHit.point`를 연결 지점으로 사용했습니다.

### Rope 연결

Player GameObject에 `SpringJoint`를 런타임으로 추가했습니다. 최초 거리를 기준으로 다음 값을 설정했습니다.

- `minDistance = distance * 0.25f`
- `maxDistance = distance * 0.6f`
- `spring = 4.5f`
- `damper = 7f`
- `massScale = 4.5f`

LineRenderer는 Lantern 위치와 Raycast hit point를 연결했습니다.

### Player 이동 영향

Player의 이동 경로를 직접 계산하지 않고 SpringJoint의 장력과 Rigidbody 물리 반응에 맡겼습니다. 따라서 연결 시점의 위치, 속도, Joint 거리, 다른 Force와 FixedUpdate 상태가 결과에 함께 영향을 줍니다.

### Prototype 가치

- Runtime Joint와 LineRenderer만으로 Rope 감각을 빠르게 확인 가능
- 연결·해제 입력과 기본 시각 피드백을 짧은 코드로 검증
- 최종 구현 방식을 결정하기 전에 플레이 가능성을 확인하는 용도에 적합

## 2. Prototype 02 — 단일 Grappling State

- Commit: `51c040b8`, 2024-09-26
- 파일: 당시 `Assets/@Scripts/Controllers/State/PlayerGrapplingState.cs`

### 변경 내용

- `OverlapSphere` 반경 25 안의 첫 `Linkable` Target 선택
- Rigidbody 전진 속도 적용
- Target과 Player 사이의 반경을 25로 보정
- `Vector3.left` 축으로 초당 90도 `RotateAround`
- 목표 높이 도달 후 Jump 상태로 전환

SpringJoint를 제거하고 Rope 구간의 반경과 각속도를 직접 계산하기 시작했습니다. 다만 다음 책임이 하나의 State에 함께 있었습니다.

- Target 탐색
- 입력 판정
- Rope 그리기
- 위치 정렬
- Swing 이동
- 종료 및 회전 복구

이 단계는 최종 구조가 아니라 **이동 제어 방식의 전환을 확인할 수 있는 중간 결과**로 가치가 있습니다.

## 3. Lifecycle Split

- Commit: `a59070ce`, 2024-10-20
- Commit 제목: `[Update] 로프 액션 구조 개선`

### 구조 변화

이 Commit에서 다음 파일이 추가됐습니다.

- `PlayerEnterGrapllingState.cs`
- `PlayerExitGrapllingState.cs`
- `RopeSystem.cs`

기존 `PlayerGrapplingState.cs`는 약 200줄이 감소했습니다. Rope 단계가 다음 책임으로 분리됐습니다.

| 상태 | 책임 |
|---|---|
| Enter | Rope 연결 표현과 진입 준비 |
| Grappling | Swing 궤적과 종료 판정 |
| Exit | 이탈 속도와 회전 복구 |

StateMachine 자체는 팀 공용 코드지만 Rope lifecycle의 구체적인 상태 구현과 전환은 본인 기여입니다.

## 4. Release

- Snapshot: `c0247122`, 2024-12-05
- 마지막 Commit 작성자: `StarPilot01`

Release snapshot을 팀 전체 최종 커밋으로 사용하되, 각 파일의 작성자는 blame으로 별도 구분했습니다.

### Target 탐색

`GrapplePointDetector`는 다음 순서로 후보를 판정합니다.

```text
OverlapSphereNonAlloc
→ Linkable LayerMask
→ Player forward 기준 Cone angle
→ Dot product > 0
→ 첫 유효 Collider 반환
```

### Rope 진입

- Animator의 Swing 시작 Trigger
- Target 방향으로 Player GFX 정렬
- 0.4초 동안 50개 LineRenderer segment 연장
- Sin 진폭을 1에서 0으로 줄여 Rope 연결 표현
- FMOD `Type=0`, 연결 완료 후 `Type=1`
- Ground 진입 시 전방 이동 유지

### Swing

- Player forward 방향으로 속도 56 적용
- `-transform.right`를 회전축으로 사용
- 초당 90도 `RotateAround`
- Target 기준 반경 20으로 위치 보정
- 높이와 최소 Swing 조건 충족 시 Exit 전환
- FMOD `Type=2`

### 종료

- Player forward 방향으로 속도 100 적용
- Curve 누적 방향을 기준으로 Player와 GFX 회전 복구
- Running 상태로 복귀
- FMOD `Type=3`

### 판단

이 변화는 SpringJoint API를 다른 API로 교체한 정도가 아닙니다. Player의 Rope 이동 결과를 물리 반응에 맡기던 방식에서 속도·반경·회전축·각속도·종료 조건을 게임 코드가 직접 결정하는 구조로 이동했습니다.
