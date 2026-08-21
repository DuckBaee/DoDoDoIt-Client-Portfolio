# Release Stabilization

Prototype 코드를 실제 Tutorial과 Game Level에 적용하면서 단독 테스트에서는 보이지 않았던 문제들이 발생했습니다. 아래 Commit은 제가 Release까지 수정한 주요 사례입니다.

| 문제 | Commit | 제가 수정한 내용 | 코드 |
|---|---|---|---|
| 꺾인 레벨에서 Rope가 월드 축 방향으로 Swing | `85caf2b8` | `Vector3.left`와 고정 Z 속도를 Player `forward/right` 기준으로 변경 | [Linear Swing](../Evolution/RopeAction/04_Release/LinearRope/PlayerGrapplingState.cs#L45-L63) |
| Curve에서 진행 방향 밖의 Target이 선택됨 | `e26ed65a` | Player 전방 Cone과 Dot product 조건 추가 | [Target Detection](../Evolution/RopeAction/04_Release/Targeting/GrapplePointDetector.cs#L84-L133) |
| Swing이 너무 일찍 종료됨 | `277cd7ad` | 최소 Swing 유지 조건 추가 | [종료 조건](../Evolution/RopeAction/04_Release/LinearRope/PlayerGrapplingState.cs#L51-L69) |
| 특정 높이 이후 회전이 진행되지 않음 | `0a82c9c3` | `RotateAround`를 높이 조건 밖으로 이동 | [회전 계산](../Evolution/RopeAction/04_Release/LinearRope/PlayerGrapplingState.cs#L45-L64) |
| Rope 입력이 전역과 상태에 중복됨 | `0a82c9c3` | Rope 입력을 Running/Jump/Falling 상태에서 처리 | 팀 Player State 의존 |
| Ground에서 Enter 상태로 진입하면 진행이 끊김 | `d8f6662a` | Ground 상태에서는 전방 이동을 유지 | [Enter 물리 처리](../Evolution/RopeAction/04_Release/LinearRope/PlayerEnterGrapllingState.cs#L33-L47) |
| Tutorial Swing 중 추락 | `a85b8242` | Rope 구간 전진 속도 보정 | [Swing 속도](../Evolution/RopeAction/04_Release/LinearRope/PlayerGrapplingState.cs#L45-L49) |
| 수평 방향으로 도는 Rope가 필요함 | `f009a55a` | LevelCurve, EnterRotate와 Rotate State 추가 | [LevelCurve](../Evolution/RopeAction/04_Release/RotationRope/LevelCurve.cs#L21-L30) · [Rotate](../Evolution/RopeAction/04_Release/RotationRope/PlayerRotateGrapplingState.cs#L42-L66) |
| Rotation 종료 후 진행 방향이 맞지 않음 | `8c771de6` | ExitRotate State 추가 | [ExitRotate](../Evolution/RopeAction/04_Release/RotationRope/PlayerExitRotateGrapplingState.cs#L39-L70) |
| 좌우 Rotation 설정이 필요함 | `ddacb9cc` | `rotationDirection`으로 회전 방향 전달 | [방향 전달](../Evolution/RopeAction/04_Release/RotationRope/LevelCurve.cs#L21-L30) |
| Rope가 없는 Curve 구간이 필요함 | `dcb55691` | Left/Right Curve State와 Trigger 추가 | [CurveTrigger](../Evolution/RopeAction/04_Release/RotationRope/CurveTrigger.cs#L24-L41) |
| Camera가 장애물과 충돌함 | `2a4a88e3`, `ad2dc2c3` | Cinemachine Collider, Camera Layer와 Prefab 설정 조정 |
| Tutorial Curve 타이밍과 Rope Point가 어긋남 | `00a66f88`, `362d036f` | Trigger, Collider와 Rope Point 배치 수정 |

이 과정을 거치며 Rope 자체의 계산뿐 아니라 Player 상태, 레벨 방향, Camera, Sound와 Collider까지 함께 확인하고 수정했습니다.
