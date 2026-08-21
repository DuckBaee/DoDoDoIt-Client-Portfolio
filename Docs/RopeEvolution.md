# Rope Action Evolution

## 전체 흐름

제가 구현한 Rope Action은 다음 순서로 발전했습니다.

```text
SpringJoint Prototype
→ 단일 State의 고정 반경 Swing
→ Enter / Swing / Exit 분리
→ Targeting / Rotation / Camera / Sound 통합
```

각 폴더에는 현재 코드를 과거 형태로 다시 만든 것이 아니라 해당 Commit 당시의 실제 파일을 보관했습니다.

## 1. Prototype 01 — SpringJoint

- Commit: `1ff69ecb`, 2024-09-14
- 최초 추가: `28e86a2b`, 2024-09-11
- 코드: [`RopeActionWithRunning.ShootRope`](../Evolution/RopeAction/01_Prototype01/RopeActionWithRunning.cs#L67-L93)

마우스 위치에서 Raycast를 쏘고, 충돌한 `RaycastHit.point`에 Rope를 연결했습니다. Player에는 SpringJoint를 런타임으로 추가했습니다.

최초 연결 거리를 기준으로 `minDistance`와 `maxDistance`를 정하고 Spring, Damper, MassScale을 적용했습니다. LineRenderer는 Lantern 위치와 연결 지점을 이어 주었습니다.

이 방식은 짧은 시간 안에 Rope 연결과 해제, 기본적인 Swing 느낌을 확인하기 좋았습니다. 다만 Player 이동 경로를 직접 계산하지 않았기 때문에 진입 위치와 속도, Rigidbody에 적용된 다른 힘이 결과에 함께 영향을 줬습니다.

## 2. Prototype 02 — 고정 반경 Swing

- Commit: `51c040b8`, 2024-09-26
- 코드: [Target 탐색](../Evolution/RopeAction/02_Prototype02/PlayerGrapplingState.cs#L85-L102) · [고정 반경 Swing](../Evolution/RopeAction/02_Prototype02/PlayerGrapplingState.cs#L104-L147)

SpringJoint 대신 제가 Rope 구간의 이동 규칙을 직접 계산했습니다.

- `OverlapSphere`로 반경 25 안의 Linkable Target 탐색
- Player 전진 속도 적용
- Target과 Player 사이의 반경을 25로 보정
- `RotateAround`로 초당 90도 회전
- 목표 높이에 도달하면 다음 상태로 전환

이 단계에서 Rope 결과를 Spring의 물리 반응이 아니라 속도·반경·각속도로 제어할 수 있게 됐습니다. 하지만 Target 탐색, Rope 표시, Swing, 종료와 회전 복구가 한 State에 함께 있었습니다.

## 3. Enter / Swing / Exit 분리

- Commit: `a59070ce`, 2024-10-20
- Commit 제목: `[Update] 로프 액션 구조 개선`

기능을 다듬기 쉽도록 Rope 진행 단계를 세 상태로 나눴습니다.

| 상태 | 제가 분리한 책임 |
|---|---|
| Enter | Rope 연결 표현, Target 방향 정렬, 진입 준비 |
| Grappling | Swing 이동, 반경 보정, 종료 판정 |
| Exit | 이탈 속도, Player와 GFX 회전 복구 |

이 Commit에서 Enter와 Exit State, RopeSystem을 추가했고 기존 Grappling State의 책임을 줄였습니다.

- [Enter 코드](../Evolution/RopeAction/03_LifecycleSplit/PlayerEnterGrapllingState.cs#L18-L91)
- [Swing 코드](../Evolution/RopeAction/03_LifecycleSplit/PlayerGrapplingState.cs#L43-L80)
- [Exit 코드](../Evolution/RopeAction/03_LifecycleSplit/PlayerExitGrapllingState.cs#L13-L55)

## 4. Release

- Snapshot: `c0247122`, 2024-12-05

### Target 탐색

`GrapplePointDetector`에서 미리 만든 Collider 배열과 `OverlapSphereNonAlloc`을 사용했습니다. 후보는 Linkable Layer, Player 전방 각도와 Dot product 조건을 통과해야 합니다.

→ [Release Target Detection 코드](../Evolution/RopeAction/04_Release/Targeting/GrapplePointDetector.cs#L84-L133)

```text
OverlapSphereNonAlloc
→ Linkable Layer
→ Player forward 기준 Cone
→ Dot product
→ Grapple Point 선택
```

### Rope 진입

- 0.4초 동안 50개 LineRenderer segment 연장
- Sin 진폭을 줄여 Rope가 펴지는 모양 표현
- Target 방향으로 Player GFX 정렬
- Ground 진입 중에도 전방 이동 유지
- FMOD 연결 단계 parameter 적용

→ [Release Enter 코드](../Evolution/RopeAction/04_Release/LinearRope/PlayerEnterGrapllingState.cs#L16-L96)

### Swing

- Player forward 방향으로 전진 속도 적용
- `-transform.right`를 회전축으로 사용
- Target 기준 고정 반경 보정
- 높이와 최소 Swing 조건으로 Exit 전환

→ [Release Swing 코드](../Evolution/RopeAction/04_Release/LinearRope/PlayerGrapplingState.cs#L45-L69)

### 종료

- Player forward 방향으로 이탈 속도 적용
- Curve 방향을 반영해 Player와 GFX 회전 복구
- Running 상태로 복귀

→ [Release Exit 코드](../Evolution/RopeAction/04_Release/LinearRope/PlayerExitGrapllingState.cs#L14-L72)

Release에서는 일반 Rope에 Rotation Rope와 Curve 방향 처리를 추가하고, 같은 상태 흐름을 Cinemachine과 FMOD에도 연결했습니다.
