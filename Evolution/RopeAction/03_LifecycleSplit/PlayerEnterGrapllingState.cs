using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DoDoDoIt
{
    public class PlayerEnterGrapplingState : IPlayerState
    {

        private PlayerController _player;
        private LineRenderer _lineRenderer;
        private bool _isRopeExtending;
        private bool _isRopeExtended;
        private float _ropeExtendSpeed = 50f;

        public void EnterState(PlayerController player)
        {
            _player = player;
            _player.Animator.SetTrigger("StartSwing");
            _player.Animator.SetBool("IsFullyExtended", false);
            _isRopeExtending = false;
            _isRopeExtended = false;
            _lineRenderer = _player.LineRenderer;
            _player.Rigidbody.velocity = Vector3.up * 3;
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
            _isRopeExtending = false;
            _isRopeExtended = true;
            _player.Animator.SetBool("IsFullyExtended", true);
        }

        private IEnumerator ExtendRope()
        {
            _isRopeExtending = true;

            Vector3 start = _player.LanternTrs.position;
            Vector3 end = _player.GrapPoint.transform.position;

            int segmentCount = 50;
            _lineRenderer.positionCount = segmentCount;

            float totalDuration = 0.7f;
            float elapsedTime = 0f;

            float initialAmplitude = 1f;
            float finalAmplitude = 0f;
            float frequency = 10f;

            while (elapsedTime < totalDuration)
            {
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
            _player.TransitionTo(Define.EPlayerState.Grappling);
        }

        private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 point = (uu * p0) + (2 * u * t * p1) + (tt * p2);
            return point;
        }
    }
}