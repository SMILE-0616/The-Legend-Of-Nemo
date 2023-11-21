using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;          // 플레이어 이동 속도
    public GameObject[] weapons;
    public bool[] hasWeapons;

    float hAxis;                 // 수평 입력값
    float vAxis;                 // 수직 입력값

    bool wDown;                  // 걷기 버튼 입력 여부
    bool jDown;                  // 점프 버튼 입력 여부
    bool fDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;                 // 점프 중인지 여부
    bool isDodge;                // 회피 중인지 여부
    bool isSwap;
    bool isFireReady = true;

    Vector3 moveVec;             // 이동 벡터
    Vector3 dodgeVec;            // 회피 벡터

    Rigidbody rigid;             // Rigidbody 컴포넌트
    Animator anim;               // Animator 컴포넌트

    GameObject nearObject;
    Weapon equipWeapon;
    int equipWeaponIndex = -1; // 처음에 0이면 실행이 안됨
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
        hAxis = Input.GetAxisRaw("Horizontal");     // 수평 입력 값 받아오기
        vAxis = Input.GetAxisRaw("Vertical");       // 수직 입력 값 받아오기
        wDown = Input.GetButton("Walk");            // 걷기 버튼 입력 여부 확인
        jDown = Input.GetButtonDown("Jump");        // 점프 버튼 입력 여부 확인
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
            // 점프 처리
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
            // 회피 처리
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetBool("isJump", true);
            anim.SetTrigger("doDodge");
            isDodge = true;

            // 일정 시간 후 회피 상태를 해제하기 위해 Invoke를 사용
            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        // 회피 상태를 해제하고 이동 속도를 원래대로 복구
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        // 인벤토리에 무기가 없다면 실행 안되고, 똑같은 무기는 꺼낼 수 없습니다.
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

            // 스왑 중
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
            // 바닥에 닿으면 점프 상태 해제
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