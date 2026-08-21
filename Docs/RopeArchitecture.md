# Rope Architecture

## 전체 구조

제가 구현한 Rope는 팀 Player State에서 입력을 받아 Target을 찾고 Enter·Swing·Exit 순서로 진행합니다.

```mermaid
flowchart TD
    Input[Running / Jump / Falling<br/>Team Player States] --> Detector[GrapplePointDetector]
    Detector -->|Valid GrapPoint| Enter[PlayerEnterGrapplingState]
    Enter --> Swing[PlayerGrapplingState]
    Swing --> Exit[PlayerExitGrapplingState]
    Exit --> Running[Running State]

    StateMachine[PlayerStateMachine<br/>Team System] -. transition .-> Enter
    StateMachine -. transition .-> Swing
    StateMachine -. transition .-> Exit

    Enter --> Camera[SwitchCam / Cinemachine]
    Swing --> Camera
    Enter --> Sound[SoundManager / FMOD]
    Swing --> Sound
    Exit --> Sound
```

점선으로 표시한 PlayerStateMachine은 팀 공용 시스템입니다.

## Target Detection

`GrapplePointDetector`는 크기 10의 Collider 배열을 재사용하고 `Physics.OverlapSphereNonAlloc`으로 후보를 찾습니다.

1. Linkable Layer
2. Detection radius 내부
3. Player `transform.forward` 기준 45도 이내
4. Target 방향과 forward의 Dot product가 양수

조건을 통과한 GameObject를 Player의 `GrapPoint`로 전달하고 Enter 상태로 전환합니다.

## Linear Rope

```mermaid
stateDiagram-v2
    Running --> EnterGrappling: Shift + valid target
    Jump --> EnterGrappling: Shift + valid target
    Falling --> EnterGrappling: Shift + valid target
    EnterGrappling --> Grappling: Rope 연결 완료
    Grappling --> ExitGrappling: 종료 조건 충족
    ExitGrappling --> Running: 회전 복구 완료
```

## Rotation Rope

```mermaid
flowchart LR
    LevelCurve -->|GrapPoint + RotationDirection| EnterRotate
    EnterRotate --> Rotate
    Rotate -->|누적 회전각 도달| ExitRotate
    ExitRotate --> Running
```

`LevelCurve.rotationDirection`의 -1 또는 1 값을 Player에 전달하고, Rotate State에서 해당 값에 따라 수평 회전 방향을 바꿨습니다.

## Camera

`SwitchCam`은 현재 Player State에 따라 다음 Cinemachine Virtual Camera를 선택합니다.

- Main
- Rope Start
- Rope Swing
- Rope Rotate
- Curve Left / Right
- Ending

Camera 전환을 수행하는 CameraManager는 팀원 코드이므로 이 저장소에는 포함하지 않았습니다.

## Sound

Rope lifecycle은 하나의 FMOD Event와 `Type` parameter를 공유합니다.

| 단계 | Parameter |
|---|---:|
| Enter 시작 | 0 |
| Rope 연결 | 1 |
| Swing | 2 |
| Exit | 3 |

각 상태가 시작될 때 해당 parameter를 호출해 Rope의 진행 단계와 Sound가 함께 바뀌도록 연결했습니다.
