using System;
using UnityEngine;
using UnityEngine.UI;

namespace DoDoDoIt
{
    public class GrapplePointDetector : MonoBehaviour, IGrapplable
    {
        [Header("감지 설정")]
        [SerializeField] 
        private float _detectionRadius = 10f;

        [SerializeField] 
        private float _forwardDetectionAngle = 45f; // 전방 감지 각도

        // 감지할 최대 콜라이더 수 설정
        [SerializeField]
        private const int MaxColliders = 10;
        private Collider[] _hitColliders = new Collider[MaxColliders];

        [SerializeField]
        private Image _indicatorImage;
        
        private bool _isRequestingGrab;
        private LayerMask _linkableLayerMask;

        private void Start()
        {
            _isRequestingGrab = false;
            _linkableLayerMask = LayerMask.GetMask("Linkable");
        }

        private void Update()
        {
            if (!_isRequestingGrab)
            {
                int colliderCount = Physics.OverlapSphereNonAlloc(
                    transform.position,
                    _detectionRadius,
                    _hitColliders,
                    _linkableLayerMask
                );

                
                bool foundGrapplePoint = false;
                
                
                for (int i = 0; i < colliderCount; i++)
                {
                    if (_hitColliders[i] != null)
                    {
                        if (IsInDetectionCone(_hitColliders[i].transform.position))
                        {
                            IsGrapplable(_hitColliders[i].gameObject);
                            
                            UpdateIndicatorPosition(_hitColliders[i].transform);
                            foundGrapplePoint = true;
                        }
                    }
                }
                
                if (!foundGrapplePoint && _indicatorImage != null)
                {
                    _indicatorImage.gameObject.SetActive(false); // 감지된 지점이 없으면 표시 이미지 비활성화
                }
            }
        }

        private void UpdateIndicatorPosition(Transform grapplePointTransform)
        {
            if (_indicatorImage != null)
            {
                // 감지된 지점 약간 밑과 앞의 위치를 계산
                Vector3 targetPosition = grapplePointTransform.position + grapplePointTransform.forward * -1.5f +
                                         grapplePointTransform.up  * -5.0f;

                _indicatorImage.transform.position = targetPosition;
                _indicatorImage.transform.rotation = grapplePointTransform.rotation;

                _indicatorImage.gameObject.SetActive(true); // 활성화
            }
        }
        
        public GameObject DetectGrapPoint()
        {
            int colliderCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                _detectionRadius,
                _hitColliders,
                _linkableLayerMask
            );

            for (int i = 0; i < colliderCount; i++)
            {
                Collider collider = _hitColliders[i];

                if (collider != null && IsInDetectionCone(collider.transform.position))
                {
                    // 추가 조건: 플레이어의 이동 방향과 로프 연결 지점의 방향 비교
                    Vector3 directionToGrapPoint = (collider.transform.position - transform.position).normalized;
                    float dotProduct = Vector3.Dot(transform.forward, directionToGrapPoint);

                    if (dotProduct > 0.0f) // 전방일 경우만 연결
                    {
                        Debug.Log($"GrapPoint detected: {collider.gameObject.name}");
                        return collider.gameObject;
                    }
                    else
                    {
                        Debug.LogWarning($"GrapPoint is behind the player: {collider.gameObject.name}");
                    }
                }
            }

            Debug.LogWarning("No valid GrapPoint detected.");
            return null;
        }


        private bool IsInDetectionCone(Vector3 targetPosition)
        {
            // 대상 위치를 기준으로 플레이어 방향 계산
            Vector3 directionToTarget = (targetPosition - this.gameObject.transform.position).normalized;

            // 플레이어 정면 방향 (로컬 기준)
            Vector3 forwardDirection = this.gameObject.transform.forward;

            // 플레이어 정면과 대상 방향 사이의 각도 계산
            float angleToTarget = Vector3.Angle(forwardDirection, directionToTarget);

            // 대상이 플레이어의 정면 탐지 각도 내에 있는지 확인
            return angleToTarget <= _forwardDetectionAngle;
        }

        public void IsGrapplable(GameObject GrapplePoint)
        {
            var effect = Util.FindChild<Transform>(GrapplePoint, "RopePointEffect", false);
            if (effect != null && !effect.gameObject.activeSelf)
            {
                SoundManager.Instance.PlaySFX("event:/SFX/Obj/Ropepoint");
                effect.gameObject.SetActive(true); // 활성화
            }
        }
        
        
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            // 탐지 범위 원
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);

            // 전방 탐지 각도
            Vector3 forward = transform.forward * _detectionRadius;
            Quaternion leftRayRotation = Quaternion.Euler(0, -_forwardDetectionAngle, 0);
            Quaternion rightRayRotation = Quaternion.Euler(0, _forwardDetectionAngle, 0);

            Vector3 leftRayDirection = leftRayRotation * forward;
            Vector3 rightRayDirection = rightRayRotation * forward;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, leftRayDirection);
            Gizmos.DrawRay(transform.position, rightRayDirection);

            // 탐지 각도 안의 영역을 연하게 표시
            Gizmos.color = new Color(1, 0, 0, 0.1f);
            Gizmos.DrawMesh(CreateConeMesh(), transform.position, transform.rotation, Vector3.one * _detectionRadius);
        }
        
        private Mesh CreateConeMesh()
        {
            Mesh mesh = new Mesh();

            const int segments = 30;
            Vector3[] vertices = new Vector3[segments + 2];
            int[] triangles = new int[segments * 3];

            vertices[0] = Vector3.zero;
            float angleStep = _forwardDetectionAngle * 2f / segments;

            for (int i = 0; i <= segments; i++)
            {
                float angle = -_forwardDetectionAngle + angleStep * i;
                float rad = Mathf.Deg2Rad * angle;
                vertices[i + 1] = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * _detectionRadius;
            }

            for (int i = 0; i < segments; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
