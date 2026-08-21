# Release Stabilization

Release 단계의 가치는 최종 구조뿐 아니라 실제 Level에서 발견된 문제를 지속적으로 수정한 이력에 있습니다.

| 문제 | Commit | 실제 변경 | 판단 |
|---|---|---|---|
| 꺾인 Level에서 Rope가 월드 축 방향으로 Swing | `85caf2b8` | `Vector3.left`와 고정 Z 속도를 Player `forward/right` 기준으로 변경 | 좌표 기준 문제 수정 |
| Curve에서 뒤쪽 Target 선택 가능 | `e26ed65a` | 전방 Cone과 Dot product를 추가 | Target 조건 강화 |
| Swing이 너무 일찍 종료 | `277cd7ad` | `_swingTime` guard와 종료 조건 추가 | 최소 진행 구간 확보 |
| 특정 높이 이후 회전이 진행되지 않음 | `0a82c9c3` | `RotateAround`를 높이 조건 밖으로 이동 | 회전과 반경 보정 책임 분리 |
| Rope 입력이 전역과 상태에서 중복 처리 | `0a82c9c3` | Shift 입력을 Running/Jump/Falling 상태로 이동 | 상태별 진입 제어 |
| Ground에서 Rope Enter 시 진행이 끊김 | `d8f6662a` | Ground Raycast 시 forward movement 유지 | Runner 흐름 유지 |
| Tutorial Swing 중 추락 | `a85b8242` | Attribute 속도 대신 명시적 Swing 속도 56 적용 | 해당 구간 이동량 안정화 |
| Rotation Rope 필요 | `f009a55a` | `LevelCurve`, EnterRotate와 Rotate State 추가 | 수평 Curve 기능 확장 |
| Rotation 종료 방향 필요 | `8c771de6` | ExitRotate State 추가 | 진행 방향 복구 |
| 좌우 Rotation 설정 | `ddacb9cc` | `rotationDirection`을 통한 좌우 설정 | Level 재사용성 확장 |
| Rope 없는 Curve 구간 | `dcb55691` | Left/Right Curve State와 Trigger 추가 | Rope 외 Curve 진행 대응 |
| Camera가 장애물과 충돌 | `2a4a88e3`, `ad2dc2c3` | Cinemachine Collider와 Layer/Camera Prefab 설정 조정 | Scene/Prefab 변경이라 Source에는 미포함 |
| Tutorial Curve 타이밍·Rope Point | `00a66f88`, `362d036f` | Prefab Trigger, Collider, Rope Point 수정 | Asset 변경이라 Source에는 미포함 |

## 해석 시 주의

Commit 제목과 코드 차이는 문제와 변경 사실을 증명하지만, 정량적인 플레이 테스트 결과까지 기록하지는 않습니다. 따라서 포트폴리오에서는 “완전히 예측 가능한 물리” 같은 절대 표현보다 다음처럼 설명하는 것이 정확합니다.

> 속도·반경·회전축·종료 조건을 명시해 SpringJoint보다 결과를 직접 통제할 수 있는 구조로 변경했다.
