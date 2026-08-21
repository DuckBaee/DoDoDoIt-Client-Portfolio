using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace DoDoDoIt
{
    public class PlayerEnterRotateGrapplingState : IPlayerState
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
            _player.Rigidbody.velocity = (Vector3.up) * 30f;
            TurnDirection();
            _player.StartCoroutine(ExtendRope());
        }

        

        public void Update()
        {
            
        }

        public void FixedUpdate()
        {
            
        }

        public void ExitState()
        {
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
            
            for (int i = 0; i < segmentCount; i++)
            {
                float segmentT = (float)i / (segmentCount - 1);
                Vector3 finalPosition = Vector3.Lerp(start, end, segmentT);
            }
            
            DisableLineRenderer();
            _player.TransitionTo(Define.EPlayerState.RotateGrappling);
        }

        private void DisableLineRenderer()
        {
            _lineRenderer.enabled = false;  // 라인 렌더러를 비활성화
            _lineRenderer.positionCount = 0;  // 점 개수를 0으로 초기화
        }
        private void ApplyMovement()
        {
            _player.Rigidbody.velocity = new Vector3 (
                0f,
                _player.Rigidbody.velocity.y,
                _player.Stats.Attributes.ForwardSpeed.GetValue() / 3f
            );
        }

        private void ApplyGravity()
        {
            _player.Rigidbody.AddForce ( 
                Vector3.up * _player.Stats.Attributes.GravityScale.GetValue() / 3f,
                ForceMode.Acceleration
            );
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
    }
}