using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
{
    public Transform target;
    NavMeshAgent nav;

    // 원하는 속도 값으로 조절하세요.
    public float missileSpeed = 10f;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();

        // 초기 속도 설정
        nav.speed = missileSpeed;
    }

    void Update()
    {
        nav.SetDestination(target.position);
    }
}
