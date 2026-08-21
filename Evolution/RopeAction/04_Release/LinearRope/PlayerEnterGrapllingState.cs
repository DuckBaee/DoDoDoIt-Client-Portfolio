using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace DoDoDoIt
{
    public class PlayerEnterGrapplingState : IPlayerState
    {

        private PlayerController _player;
        private LineRenderer _lineRenderer;

        public void EnterState(PlayerController player)
        {
            _player = player;
            _player.Animator.SetTrigger("StartSwing");
            _player.Animator.SetBool("IsFullyExtended", false);
            _lineRenderer = _player.LineRenderer;
            _lineRenderer.enabled = true;
            TurnDirection();
            _player.StartCoroutine(ExtendRope());
            SoundManager.Instance.PlaySFX("event:/SFX/Dodo/Lantern", "Type", 0f);
        }

        

        public void Update()
        {
            
        }

        public void FixedUpdate()
        {
            if (IsGrounded())
            {
                ApplyMovement();
            }
            ApplyGravity();
        }

        public void ExitState()
        {
            _player.Rigidbody.velocity = Vector3.zero;
            _player.Animator.SetBool("IsFullyExtended", true);
        }

        private IEnumerator ExtendRope()
        {
            Vector3 start = _player.LanternTrs.position;
            Vector3 end = _player.GrapPoint.transform.position;

            int segmentCount = 50;
            _lineRenderer.positionCount = segmentCount;

            float totalDuration = 0.4f;
            float elapsedTime = 0f;

            float initialAmplitude = 1f;
            float finalAmplitude = 0f;
            float frequency = 5f;

            while (elapsedTime < totalDuration)
            {
                start = _player.LanternTrs.position;
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / totalDuration);
                float amplitude = Mathf.Lerp(initialAmplitude, finalAmplitude, t);
                for (int i = 0; i < segmentCount; i++)
                {
                    float segmentT = (float)i / (segmentCount - 1);
                    Vector3 segmentPosition = Vector3.Lerp(start, end, segmentT * t);
                    
                    float phase = segmentT * Mathf.PI * frequency + Time.time * Mathf.PI;
                    float sinOffsetX = Mathf.Sin(phase) * amplitude;

                    Vector3 finalPosition = segmentPosition + new Vector3(sinOffsetX, 0, 0);

                    _lineRenderer.SetPosition(i, finalPosition);
                }

                yield return null;
            }
            SoundManager.Instance.PlaySFX("event:/SFX/Dodo/Lantern", "Type", 1f);
            
            for (int i = 0; i < segmentCount; i++)
            {
                float segmentT = (float)i / (segmentCount - 1);
                Vector3 finalPosition = Vector3.Lerp(start, end, segmentT);
            }
            
            _player.StartCoroutine(RopeTransformMatch());
            DisableLineRenderer();
            _player.TransitionTo(Define.EPlayerState.Grappling);
        }
        
        IEnumerator RopeTransformMatch()
        {
            // 부모(그랩 포인트)의 위치 가져오기
            Vector3 parentPosition = _player.GrapPoint.gameObject.transform.position;

            // 자식(플레이어)의 초기 위치 가져오기
            Vector3 childPosition = _player.transform.position;

            // 기준 거리 설정
            float distanceThreshold = 0.05f;

            // 그랩 포인트와 특정 축을 점진적으로 위치를 맞춤
            while (true)
            {
                // 현재 위치 업데이트
                childPosition = _player.transform.position;

                // 플레이어가 왼쪽 또는 오른쪽을 바라볼 경우 (Z축만 맞춤)
                if (_player.gameObject.transform.forward == Vector3.left ||
                    _player.gameObject.transform.forward == Vector3.right)
                {
                    float newZ = Mathf.Lerp(childPosition.z, parentPosition.z, 2f * Time.deltaTime);

                    // 종료 조건: Z축 차이가 기준 거리 이하일 경우 루프 종료
                    if (Mathf.Abs(newZ - childPosition.z) < distanceThreshold)
                        break;

                    _player.transform.position = new Vector3(childPosition.x, childPosition.y, newZ);
                }
                // 그 외 경우 (X축만 맞춤)
                else
                {
                    float newX = Mathf.Lerp(childPosition.x, parentPosition.x, 2f * Time.deltaTime);

                    // 종료 조건: X축 차이가 기준 거리 이하일 경우 루프 종료
                    if (Mathf.Abs(newX - childPosition.x) < distanceThreshold)
                        break;

                    _player.transform.position = new Vector3(newX, childPosition.y, childPosition.z);
                }
                // 다음 프레임까지 대기
                yield return null;
            }

            // 최종 위치를 정확히 맞춤
            _player.transform.position = new Vector3(
                (_player.gameObject.transform.forward == Vector3.left ||
                 _player.gameObject.transform.forward == Vector3.right)
                    ? childPosition.x // X축 유지
                    : parentPosition.x, // X축을 부모 위치에 맞춤
                childPosition.y, // Y축은 유지
                (_player.gameObject.transform.forward == Vector3.left ||
                 _player.gameObject.transform.forward == Vector3.right)
                    ? parentPosition.z // Z축을 부모 위치에 맞춤
                    : childPosition.z // Z축 유지
            );

            yield break;
        }

        private void DisableLineRenderer()
        {
            _lineRenderer.enabled = false;  // 라인 렌더러를 비활성화
            _lineRenderer.positionCount = 0;  // 점 개수를 0으로 초기화
        }

        void ApplyGravity()
        {
            _player.Rigidbody.AddForce(Vector3.up * _player.Stats.Attributes.GravityScale.GetValue() / 2f , ForceMode.Acceleration);
        }
        
        private void ApplyMovement()
        {
            Vector3 movement = (_player.transform.forward * _player.Stats.Attributes.ForwardSpeed.GetValue());
            _player.Rigidbody.velocity = new Vector3(movement.x, _player.Rigidbody.velocity.y, movement.z);
        }
        
        private void TurnDirection()
        {
            Vector3 direction = _player.GrapPoint.transform.position - _player.PlayerGFX.transform.position;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                _player.PlayerGFX.transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        
        private bool IsGrounded()
        {
            return Physics.Raycast (
                _player.GroundCheckRayOriginTrs.transform.position,
                Vector3.down,
                out RaycastHit hit,
                1.1f,
                _player.GroundLayer
            );
        }
    }
}