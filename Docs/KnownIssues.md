# Known Issues

코드는 Commit 원본을 보존하기 위해 수정하지 않았습니다. 다음 내용은 공개 전 숨길 항목이 아니라 면접에서 개선 방향을 설명할 수 있는 검토 기록입니다.

## Rope Enter

- `ExtendRope`와 `RopeTransformMatch` Coroutine이 상태 종료와 함께 취소되지 않습니다.
- Player 방향을 `Vector3.left/right`와 정확히 비교하는 구간이 있습니다.
- `RopeTransformMatch`의 종료 조건이 Target까지 남은 거리보다 한 프레임 Lerp 변화량을 비교합니다.
- Rope segment의 마지막 계산 결과가 사용되지 않는 반복문이 있습니다.
- 사용되지 않는 `using`이 남아 있습니다.

## Linear Swing

- `_swingTime += 0.1f`가 `Time.fixedDeltaTime`이 아닌 FixedUpdate 호출 수에 의존합니다.
- Rigidbody velocity와 `Transform.RotateAround`, position Lerp를 함께 사용합니다.
- 속도 56, 반경 20, 각속도 90 등 값이 코드에 직접 포함돼 있습니다.
- `TakeDamage`가 `NotImplementedException`을 발생시킵니다.
- Debug Log가 Release 코드에 남아 있습니다.

## Target Detection

- `OverlapSphereNonAlloc`이 반환한 후보를 별도 정렬하지 않아 첫 Target 우선순위가 명시적이지 않습니다.
- Update의 Indicator는 반복 중 마지막 후보를 사용할 수 있지만 `DetectGrapPoint`는 첫 후보를 반환합니다.
- `_isRequestingGrab`은 false로 초기화되지만 true로 변경되는 코드가 없습니다.
- `CreateConeMesh`가 Gizmo 호출마다 새 Mesh를 생성합니다.

## Rotation Rope

- `PlayerRotateGrapplingState.currentRotation`이 `EnterState`에서 초기화되지 않습니다.
- 동일 State 인스턴스를 재사용하면 두 번째 진입 시 즉시 종료될 가능성이 있습니다.
- public field와 주석에 적힌 각도 설명이 실제 기본값과 일치하지 않습니다.
- `TakeDamage`가 구현되지 않았습니다.

## Camera

- `SwitchCam`이 매 Update마다 State 타입을 검사하고 Camera 전환을 요청합니다.
- Rope Enter/Rotate 분기에서 `GrapPoint` null 여부를 확인하지 않습니다.
- Virtual Camera field가 public으로 노출돼 있습니다.

## 포트폴리오 설명 원칙

다음처럼 설명하는 것이 적절합니다.

> 당시 제출 일정 안에서 플레이 문제를 해결한 Release 코드이며, 현재 다시 설계한다면 시간 누적, Target 우선순위, Rigidbody 이동 정책과 상태 재진입 초기화를 우선 개선하겠습니다.

원본 코드를 수정해 현재 수준의 코드처럼 보이게 만들지 않고, 당시 판단과 남은 한계를 함께 공개합니다.
