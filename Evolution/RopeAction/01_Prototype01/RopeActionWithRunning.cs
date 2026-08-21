using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RopeActionWithRunning : MonoBehaviour
{
    public Transform player;
    public LayerMask mask;
    public Transform gunTip;
    //public Image crosshair;
    
    
    private Camera cam;
    private RaycastHit hit;
    private LineRenderer lr;
    private SpringJoint sj;
    private Vector3 grapplePoint;
    private bool isGrappling = false;

    void Start()
    {
        cam = Camera.main;
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        //CanShootTheRope();

        if (Input.GetMouseButtonDown(0))
        {
            ShootRope();
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndGrapple();
        }

        DrawRope();
    }

    // void CanShootTheRope()
    // {
    //     Ray ray = cam.ScreenPointToRay(Input.mousePosition);
    //
    //     // 마우스 커서 위치에서 레이캐스트를 쏩니다.
    //     if (Physics.Raycast(ray, out hit, 40f, mask))
    //     {
    //         // Raycast가 맞은 경우 크로스헤어 색상을 빨간색으로 변경
    //         if (crosshair != null)
    //         {
    //             crosshair.color = Color.red;
    //         }
    //     }
    //     else
    //     {
    //         // Raycast가 맞지 않은 경우 크로스헤어 색상을 원래 색상으로 되돌림
    //         if (crosshair != null)
    //         {
    //             crosshair.color = Color.black;
    //         }
    //     }
    // }

    void ShootRope()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // 마우스 커서 위치에서 레이캐스트를 쏩니다.
        if (Physics.Raycast(ray, out hit, 25f, mask))
        {
            isGrappling = true;

            grapplePoint = hit.point;
            lr.positionCount = 2;
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, grapplePoint);

            sj = player.gameObject.AddComponent<SpringJoint>();
            sj.autoConfigureConnectedAnchor = false;
            sj.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            sj.minDistance = distanceFromPoint * 0.25f;  // 스프링 장력 설정
            sj.maxDistance = distanceFromPoint * 0.6f;   // 거리를 줄여 더 강하게 당기도록 설정
            sj.spring = 4.5f;                            // 스프링 강도 설정
            sj.damper = 7f;                              // 댐퍼 값 조정
            sj.massScale = 4.5f;
        }
    }

    void EndGrapple()
    {
        isGrappling = false;
        lr.positionCount = 0;

        // SpringJoint 제거
        if (sj != null)
        {
            Destroy(sj);
        }
    }

    void DrawRope()
    {
        if (!isGrappling) return;

        // 로프의 시작점을 총구 위치로 갱신
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);
    }
}
