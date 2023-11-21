using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;  // 카메라가 따라갈 대상(Transform 컴포넌트)
    public Vector3 offset;    // 대상으로부터의 상대적인 오프셋

    void Update()
    {
        // 카메라의 위치를 대상의 위치에 오프셋을 더한 위치로 설정
        transform.position = target.position + offset;
    }
}