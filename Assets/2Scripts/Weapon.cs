using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public int maxAmmo;
    public int curAmmo;
    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }

        else if (type == Type.Range)
        {
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing()
    {
        // 중요한 개념인 코루틴
        // 기존 : Use() 메인루틴 -> Swing() 서브루틴 -> Use() 메인루틴 (교차실행)
        // 코루틴 : Use() 메인루틴 + Swing() 코루틴 Co - (동시실행)

        // yield : 결과를 전달하는 키워드, 여러 개 사용해 시간차 로직 구현가능

        //1
        yield return new WaitForSeconds(0.1f); // 0.1 초 대기
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        //2
        yield return new WaitForSeconds(0.3f); // 0.3 초 대기
        meleeArea.enabled = false;

        //3
        yield return new WaitForSeconds(0.3f); // 0.3 초 대기
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        // 1. 총알 발사 , Instantiate() 함수로 총알 인스턴스화 하기
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;         // velocity 속력주기

        yield return null;
        // 2. 탄피 배출
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);

        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 탄피 회전
    }
}