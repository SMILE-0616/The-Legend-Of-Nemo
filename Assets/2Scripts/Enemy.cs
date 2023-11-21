using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI ��Ҹ� �����ϱ� ���� �߰�
using UnityEngine.AI; // AI ��Ҹ� �����ϱ� ���� �߰�

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

    // Enemy�� �̵� �ӵ�
    public float moveSpeed = 1.0f;

    // �� ó���� ���� ���� ����
    private static int enemiesDestroyed = 0;

    // �¸� �޽����� ǥ���� UI Text ���
    public Text winText;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material; 
        // �ڽ��� ���θ� �����;� �ϹǷ� InChildren �߰�
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
            rigid.velocity = Vector3.zero;  // �ӵ�
            rigid.angularVelocity = Vector3.zero;   // ȸ����
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

        // ���� óġ�� ���
        if (curHealth <= 0)
        {
            Destroy(gameObject);
            enemiesDestroyed++;

            // ��� ���� ó���Ǿ����� Ȯ��
            if (enemiesDestroyed >= 2)
            {
                // ��� ���� ó���Ǿ��ٸ� �¸� �޽��� ǥ��
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


            // �׾��� ��, �˹�

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