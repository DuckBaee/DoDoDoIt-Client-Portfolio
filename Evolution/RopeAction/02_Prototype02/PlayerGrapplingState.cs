using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class PlayerGrapplingState : IPlayerState
{
    //유틸리티
    private DrawDetectionRadius _playerDetectionRadius;
    private DrawDetectionRadius _ropePosDetectionRadius;
    //끝
    
    private PlayerController _player;
    private bool _bShouldGrappling;
    private bool _bExistGrapplingPoint;

    private Vector3 _grapPointPos;
    private float _detectionRadius = 25f;
    private float _targetHeight = 3f;
    private LayerMask _detectionLayer = LayerMask.GetMask("Linkable");
    private float _fixedRadius = 25f;
    private float _rotationDuration = 1f;
    private float _flySpeed = 10f;

    public void EnterState(PlayerController player)
    {
        //유틸리티 설정-
        _ropePosDetectionRadius = new DrawDetectionRadius();
        _ropePosDetectionRadius.detectionRadius = _detectionRadius;
        _ropePosDetectionRadius.drawColor = Color.green;
        _playerDetectionRadius = new DrawDetectionRadius();
        _playerDetectionRadius.detectionRadius = _detectionRadius;
        _playerDetectionRadius.drawColor = Color.red;
        //끝-
        
        _player = player;
        _bShouldGrappling = false;
        _bExistGrapplingPoint = false;
        handleInput();
    }

    public void Update()
    {
        //그래플링 조건 충족 시 로프 그리기
        if (_bShouldGrappling && _bExistGrapplingPoint)
        {
            DrawRope();
        }
        
        RopeTransformMatch();
    }

    public void FixedUpdate()
    {
        Grappling();
    }

    public void ExitState()
    {
        //그래플링 상태 종료 시 라인렌더러 끄기. 
        _player.LineRenderer.enabled = false;
    }

    void handleInput()
    {
        _grapPointPos = checkGrapplingPoint(); //로프를 걸 위치가 있는지 확인용 데이터
        if (_grapPointPos != Vector3.zero)
        {
            Debug.Log("Find object");
            _bShouldGrappling = true;
        }
        else if (_grapPointPos == Vector3.zero)
        {
            Debug.Log("there is no Linkable object");
            _bShouldGrappling = false;
            _player.TransitionTo(EPlayerState.Running);
        }
        else
        {
            Debug.Log("exception detected");
        }
    }

    Vector3 checkGrapplingPoint()
    {
        Vector3 noGrapplingPoint = Vector3.zero;
        Collider[] hitcolliders =
            Physics.OverlapSphere(_player.gameObject.transform.position, _detectionRadius, _detectionLayer);
        foreach (var collider in hitcolliders)
        {
            if (collider.CompareTag("Linkable"))
            {
                _grapPointPos = collider.transform.position;
                _bExistGrapplingPoint = true;
                Debug.Log("find Linkable");
                return _grapPointPos;
            }
        }

        return noGrapplingPoint;
    }

    private void Grappling()
    {
        if (_bShouldGrappling && _bExistGrapplingPoint)
        {
            _ropePosDetectionRadius.DrawCircle(_grapPointPos);
            _playerDetectionRadius.DrawCircle(_player.transform.position);
            _player.Rigidbody.velocity = new Vector3(0f, 0f, _player.ForwardSpeed);
            Debug.Log("Start Grappling");
            // 현재 플레이어의 Y 위치
            float currentPlayerHeight = _player.gameObject.transform.position.y;
            // 목표 높이에 도달하지 않았다면 계속 회전
            if (currentPlayerHeight < _grapPointPos.y + _targetHeight)
            {
                Debug.Log(currentPlayerHeight);
                // 현재 플레이어와 grapPointPos 사이의 거리를 계산
                Vector3 directionToGrapPoint = _player.transform.position - _grapPointPos;
                float currentDistance = directionToGrapPoint.magnitude;
                
                // 현재 거리가 고정 반경과 다르면 고정 반경에 맞춰 위치 조정
                if (Mathf.Abs(currentDistance - _fixedRadius) > 0.01f)
                {
                    // 고정 반경에 맞춰 서서히 플레이어 위치를 조정 (Lerp 사용)
                    Vector3 targetPosition = _grapPointPos + directionToGrapPoint.normalized * _fixedRadius;
                    _player.transform.position = Vector3.Lerp(_player.transform.position, targetPosition, 0.1f);
                }

                // 고정된 반경을 유지하며 회전
                _player.gameObject.transform.RotateAround(_grapPointPos, Vector3.left, 90f * Time.deltaTime);
            }
            else
            {
                // 목표 높이에 도달하면 회전을 멈추고 필요한 다른 동작 수행
                _bShouldGrappling = false;
                _bExistGrapplingPoint = false;
                _grapPointPos = Vector3.zero;
                _player.Rigidbody.velocity = Camera.main.transform.forward.normalized * _flySpeed;
                // 자식의 회전을 부드럽게 변경 (여기서 자식은 플레이어)
                _player.StartCoroutine(SmoothRotate(_player.transform));
                Debug.Log("Target height reached, grappling stopped.");
                _player.TransitionTo(EPlayerState.Jump);
            }
        }
    }
    
    private void RopeTransformMatch()
    {
        // 부모 오브젝트의 위치 (현재 스크립트가 붙어 있는 오브젝트)
        Vector3 parentPosition = _grapPointPos;

        // 자식 오브젝트의 현재 위치
        Vector3 childPosition = _player.transform.position;

        // 자식 오브젝트의 X 좌표를 부모의 X 좌표로 천천히 보간
        float newX = Mathf.Lerp(childPosition.x, parentPosition.x, 1f * Time.deltaTime);

        // 자식 오브젝트의 새로운 위치 설정 (X 좌표만 변경)
        _player.transform.position = new Vector3(newX, childPosition.y, childPosition.z); 
    }
    
    private void DrawRope()
    {
        // LineRenderer가 있는지 확인
        if (_player.LineRenderer != null)
        {
            _player.LineRenderer.enabled = true;
            _player.LineRenderer.SetPosition(0, _player.transform.position); // 시작점은 플레이어 위치
            _player.LineRenderer.SetPosition(1, _grapPointPos); // 끝점은 그랩 포인트 위치
        }
    }

    private IEnumerator SmoothRotate(Transform child)
    {
        Debug.Log("start coroutine");
        Quaternion initialRotation = child.rotation; // 현재 자식의 회전 값
        Quaternion targetRotation = Quaternion.Euler(Vector3.zero); // 목표 회전 값 (0,0,0)

        float elapsedTime = 0f;

        while (elapsedTime < _rotationDuration)
        {
            // 시간에 따라 회전을 부드럽게 변경
            child.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / _rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종적으로 목표 회전에 정확히 도달
        child.rotation = targetRotation;
    }
}
