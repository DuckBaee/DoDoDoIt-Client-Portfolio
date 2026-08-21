using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace DoDoDoIt
{
    public class PlayerRotateGrapplingState : IPlayerState , IDamageableState
    {
        private PlayerController _player;

        public float rotationSpeed = 60f;  // 회전 속도 (초당 45도)
        public float maxRotationAngle = 135f;  // 회전할 최대 각도 (90도)
        private float currentRotation = 0f;  // 누적된 회전 각도

       
        public void EnterState(PlayerController player)
        {
            _player = player;
            _player.LineRenderer.enabled = true;
            _player.LineRenderer.positionCount = 2;
        }

        public void Update()
        {
            DrawRope();
        }

        public void FixedUpdate()
        {
            Grappling();
        }

        public void ExitState()
        {
            DisableLineRenderer();
            _player.GrapPoint = null;
            _player.IsSwinging = false;
        }

        private void Grappling()
        {
            _player.Rigidbody.velocity = new Vector3(0f, 0f, _player.Stats.Attributes.ForwardSpeed.GetValue());
            // 이번 프레임에서 회전할 각도를 계산합니다.
            float rotationStep = rotationSpeed * Time.deltaTime;

            // 목표 각도에 도달했는지 확인하고, 초과하지 않도록 조정합니다.
            if (currentRotation + rotationStep > maxRotationAngle)
            {
                rotationStep = maxRotationAngle - currentRotation;
            }

            // 실제로 회전합니다.
            _player.transform.RotateAround(_player.GrapPoint.transform.position, Vector3.up, rotationStep * _player.RotationDirection);

            // 회전한 각도를 누적합니다.
            currentRotation += rotationStep;

            // 목표 각도에 도달하면 더 이상 회전하지 않음.
            if (currentRotation >= maxRotationAngle)
            {
                Debug.Log("회전 완료!");
                _player.Animator.SetTrigger("EndSwing");
                _player.TransitionTo(Define.EPlayerState.ExitRotateGrappling);
            }
        }

        private void DrawRope()
        {
            if (_player.LineRenderer != null && _player.GrapPoint != null)
            {
                _player.LineRenderer.SetPosition(0, _player.LanternTrs.gameObject.transform.position);
                _player.LineRenderer.SetPosition(1, _player.GrapPoint.gameObject.transform.position);
            }
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            throw new System.NotImplementedException();
        }
        
        private void DisableLineRenderer()
        {
            _player.LineRenderer.enabled = false;  // 라인 렌더러를 비활성화
            _player.LineRenderer.positionCount = 0;  // 점 개수를 0으로 초기화
        }
    }
}