using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;  // ī�޶� ���� ���(Transform ������Ʈ)
    public Vector3 offset;    // ������κ����� ������� ������

    void Update()
    {
        // ī�޶��� ��ġ�� ����� ��ġ�� �������� ���� ��ġ�� ����
        transform.position = target.position + offset;
    }
}