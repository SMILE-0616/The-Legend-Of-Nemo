using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
{
    public Transform target;
    NavMeshAgent nav;

    // ���ϴ� �ӵ� ������ �����ϼ���.
    public float missileSpeed = 10f;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();

        // �ʱ� �ӵ� ����
        nav.speed = missileSpeed;
    }

    void Update()
    {
        nav.SetDestination(target.position);
    }
}
