using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace DoDoDoIt
{
    public class PlayerGrapplingState : IPlayerState , IDamageableState
    {
        private PlayerController _player;

        private readonly float _targetHeight = -4f;
        private readonly float _fixedRadius = 20f;
        private float _swingTime;


        public void EnterState(PlayerController player)
        {
            _swingTime = 0f;
            _player = player;
            _player.LineRenderer.enabled = true;
            _player.LineRenderer.positionCount = 2;
            SoundManager.Instance.PlaySFX("event:/SFX/Dodo/Lantern", "Type", 2f);
        }

        public void Update()
        {
            DrawRope();
        }

        public void FixedUpdate()
        {
            ApplyGravity();
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
            _player.Rigidbody.velocity = _player.gameObject.transform.forward.normalized * 56f;
            float currentPlayerHeight = _player.transform.position.y;
            _player.transform.RotateAround(_player.GrapPoint.transform.position, -_player.gameObject.transform.right, 90f * Time.deltaTime);
            
            if (currentPlayerHeight < _player.GrapPoint.transform.position.y + _targetHeight)
            {
                Debug.Log(_swingTime);
                _swingTime += 0.1f;
                Vector3 directionToGrapPoint = _player.gameObject.transform.position - _player.GrapPoint.transform.position;
                float currentDistance = directionToGrapPoint.magnitude;

                if (Mathf.Abs(currentDistance - _fixedRadius) > 0.01f)
                {
                    Vector3 targetPosition = _player.GrapPoint.transform.position + directionToGrapPoint.normalized * _fixedRadius;
                    _player.gameObject.transform.position = Vector3.Lerp(_player.transform.position, targetPosition, 0.1f);
                }
            }
            else if((currentPlayerHeight >= _player.GrapPoint.transform.position.y + _targetHeight) && _swingTime >= 1f)
            {
                Debug.Log("end swing : " + _swingTime);
                _player.Animator.SetTrigger("EndSwing");
                _player.TransitionTo(Define.EPlayerState.ExitGrappling);
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
        void ApplyGravity()
        {
            _player.Rigidbody.AddForce(Vector3.up * _player.Stats.Attributes.GravityScale.GetValue(), ForceMode.Acceleration);
        }
        public bool IsGrounded()
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