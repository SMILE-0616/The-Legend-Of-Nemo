using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 요소를 조작하기 위해 추가
using UnityEngine.AI; // AI 요소를 조작하기 위해 추가

/* Enemy script */
public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    public Transform target;
    public bool isChase;


    Rigidbody rigid;
    BoxCollider boxCollider;

    Material mat;
    NavMeshAgent nav;

    Animator anim;

    // Enemy의 이동 속도
    public float moveSpeed = 1.0f;

    // 적 처리된 수를 세는 변수
    private static int enemiesDestroyed = 0;

    // 승리 메시지를 표시할 UI Text 요소
    public Text winText;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material; 
        // 자식의 전부를 가져와야 하므로 InChildren 추가
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {
        if(isChase)
            nav.SetDestination(target.position);
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;  // 속도
            rigid.angularVelocity = Vector3.zero;   // 회전력
        }
    }

    void FixedUpdate()
    {
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec, false));

            Debug.Log("Collision detected with: " + other.tag);
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec, false));

            Debug.Log("Collision detected with: " + other.tag);
        }

        // 적을 처치한 경우
        if (curHealth <= 0)
        {
            Destroy(gameObject);
            enemiesDestroyed++;

            // 모든 적이 처리되었는지 확인
            if (enemiesDestroyed >= 2)
            {
                // 모든 적이 처리되었다면 승리 메시지 표시
                print("Game Win");
            }
        }
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 12;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");


            // 죽었을 시, 넉백

            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rigid.freezeRotation = false; // freezeRotation
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }
            Destroy(gameObject, 4);
        }
    }
}