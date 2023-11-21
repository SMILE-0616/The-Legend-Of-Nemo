using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;          // �÷��̾� �̵� �ӵ�
    public GameObject[] weapons;
    public bool[] hasWeapons;

    float hAxis;                 // ���� �Է°�
    float vAxis;                 // ���� �Է°�

    bool wDown;                  // �ȱ� ��ư �Է� ����
    bool jDown;                  // ���� ��ư �Է� ����
    bool fDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;                 // ���� ������ ����
    bool isDodge;                // ȸ�� ������ ����
    bool isSwap;
    bool isFireReady = true;

    Vector3 moveVec;             // �̵� ����
    Vector3 dodgeVec;            // ȸ�� ����

    Rigidbody rigid;             // Rigidbody ������Ʈ
    Animator anim;               // Animator ������Ʈ

    GameObject nearObject;
    Weapon equipWeapon;
    int equipWeaponIndex = -1; // ó���� 0�̸� ������ �ȵ�
    float fireDelay;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Dodge();
        Swap();
        Interaction();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");     // ���� �Է� �� �޾ƿ���
        vAxis = Input.GetAxisRaw("Vertical");       // ���� �Է� �� �޾ƿ���
        wDown = Input.GetButton("Walk");            // �ȱ� ��ư �Է� ���� Ȯ��
        jDown = Input.GetButtonDown("Jump");        // ���� ��ư �Է� ���� Ȯ��
        fDown = Input.GetButtonDown("Fire1");
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap || !isFireReady)
            moveVec = Vector3.zero;

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            // ���� ó��
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if(fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            // ȸ�� ó��
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetBool("isJump", true);
            anim.SetTrigger("doDodge");
            isDodge = true;

            // ���� �ð� �� ȸ�� ���¸� �����ϱ� ���� Invoke�� ���
            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        // ȸ�� ���¸� �����ϰ� �̵� �ӵ��� ������� ����
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        // �κ��丮�� ���Ⱑ ���ٸ� ���� �ȵǰ�, �Ȱ��� ����� ���� �� �����ϴ�.
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;

        if ((sDown1 || sDown2) && !isJump && !isDodge)
        {
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            // ���� ��
            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }
    }
    void SwapOut()
    {
        isSwap = false;
    }

    void Interaction()
    {
        if (iDown && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            // �ٴڿ� ������ ���� ���� ����
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}