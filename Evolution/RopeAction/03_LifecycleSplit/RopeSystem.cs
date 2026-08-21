using System;
using UnityEngine;

namespace DoDoDoIt
{
    public class RopeSystem : MonoBehaviour
    {
        [Header("감지 설정")]
        [SerializeField] private float _detectionRadius = 10f;

        private LayerMask _linkableLayerMask;
        private void Start()
        {
            _linkableLayerMask = LayerMask.GetMask("Linkable");
        }

        public GameObject DetectGrapPoint()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectionRadius, LayerMask.GetMask("Linkable"));
            if (hitColliders.Length > 0)
            {
                Debug.Log($"GrapPoint detected: {hitColliders[0].gameObject.name}");
                return hitColliders[0].gameObject;
            }
            else
            {
                Debug.LogWarning("No GrapPoint detected.");
                return null;
            }
        }
    }
}
