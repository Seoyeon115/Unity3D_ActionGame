using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //인스펙터 창에서 설정할 수 있도록 public 변수 추가
    public float speed;

    //플레이어의 무기관련 배열 함수2개 선언
    public GameObject[] weapons;
    public bool[] hasWeapons;
    //수류탄. 공전하는 물체를 컨트롤하기 위해 배열변수 생성
    public GameObject[] grenades;
    public GameObject[] bigGrenades;
    //수류탄 그룹
    public GameObject GrenadeGroup;
    public GameObject BigGrenadeGroup;


    //마우스로 회전하기
    public Camera followCamera;

    //플레이어 수치 변수 생성 (탄약, 동전, 체력, 수류탄(필살기))
    public int ammo;
    public int coin;
    public int health;

    public int hasGrenades;
    public int hasBigGrenades;
    public GameObject grenadeObject;   //수류탄 프리펩을 저장할 변수
    public GameObject bigGrenadeObject;

    //위 변수의 최대값을 저장할 변수
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;
    public int maxHasBigGrenades;

    //Input Axis 값을 받을 전역변수 선언
    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool iDown;
    bool fDown; //키입력
    bool gDown;  //수류탄
    bool rDown;  //재장전

    //무기교체
    bool sDown1;
    bool sDown2;
    bool sDown3;

    //수류탄 교체
    bool tDown;

    bool isJump;  //점프를 하고 있는가?
    bool isDodge;
    bool isSwap;  //교체 시간차
    bool isFireReady = true;  //공격 준비
    bool isReload;
    bool isBorder; //벽 충돌 플래그
    bool isDamage;      //무적타임
    bool isTab = true; //탭버튼 눌렀는가?



    //위에꺼 합쳐서 만들거
    Vector3 moveVec;
    Vector3 dodgeVec;

    //물리 효과를 위해 Rigidbody 변수 선언 후, 초기화
    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] meshs;

    //트리거 된 아이템을 저장하기 위한 변수 선언
    GameObject nearObject;
    //들고있는 무기 변수
    Weapon equipWeapon;
    //들고있는 무기가 같은 무기인지 확인
    int equipWeaponIdex = -1;
    //공격 딜레이
    float fireDelay;

    //들고있는 수류탄
    Grenade equipGrenade;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //Animator 변수를 GetConponentInChildren()으로 초기화
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
    }
    private void Start()
    {
        BigGrenadeGroup.SetActive(false);
    }


    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();
        Attack();
        Reload();
        Dodge();
        Interation();
        Swap();

    }


    void GetInput()
    {
        //GetAxisRaw() : Axis 값을 정수로 반환하는 함수
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        //Shift는 누를때만 작동되도록 GetButton()사용
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        //fDown = Input.GetButtonDown("Fire1");    //클릭 한번에 총알 하나(꾹 눌러도 한번)
        fDown = Input.GetButton("Fire1");   //꾹 누르고 있으면 계속 공격
        gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reload");
        tDown = Input.GetButtonDown("Tab");    //tab눌러서 수류탄 교체
    }

    void Move()
    {
        //moveVec: 움직임 벡터
        //normalized: 방향 값이 1로 보정된 벡터
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //회피 중에는 움직임 벡터 -> 회피방향 벡터로 바꾸도록 구현
        if (isDodge)
        {
            moveVec = dodgeVec;
        }
        //무기교체할때, 장전 중 움직임 불가,
        if (isSwap || !isFireReady || isReload)
        {
            moveVec = Vector3.zero;
        }

        //
        if (!isBorder)

            //걸을때는 속도가 줄어야된다! 조건 적용
            /*if (wDown)
                transform.position += moveVec * speed * 0.3f * Time.deltaTime;
            else
                transform.position += moveVec * speed * Time.deltaTime;*/
            //삼항연산자를 이용할 수 있음. (bool형태 조건 ? true일때 값 : false일 때 값)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;


        //SetBool()함수로 파라메터 값을 설정하기
        //Animator의 파라미터는 Bool값이고, 기본적으로 달리기: isRun, 움직입 값: moveVec
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }


    void Turn()
    {
        //회전 / LookAt(): 지정된 벡터를 향해서 회전시켜주는 함수
        //지금 우리 위치: transform.position , 움직여야할 값: moveVec => 나아가는 방향으로 바라본다!

        //키보드에 의한 회전
        transform.LookAt(transform.position + moveVec);

        //마우스에 의한 회전
        if (fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                //RayCastHit의 높이는 무시하도록 y축 값을 0으로 초기화
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }

        }
    }


    void Jump()
    {
        //무한 점프 방지. 점프가 false일때만 안에꺼를 실행
        //bool 값을 반대로 사용하고 싶으면 앞에 !(느낌표) 추가. (bool값에만 적용가능)
        //moveVec==Vector3.zero : 움직임이 없을 떄
        //if (jDown && moveVec==Vector3.zero && !isJump) //내가 점프키를 눌렀고, 점프가 false일때
        //점프하면서 회피 불가능
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            
            //AddForce() 함수로 물리적인 힘을 가하기
            //Vertor3.up: 위
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }


    //수류탄
    void Grenade()
    {
        if (hasGrenades == 0 && hasBigGrenades == 0)
            return;

        if (gDown && !isReload && !isSwap)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                //RayCastHit의 높이는 무시하도록 y축 값을 지정
                nextVec.y = 10;

                if(isTab && hasGrenades>0)
                {
                    //수류탄 생성
                    GameObject instantGrenade = Instantiate(grenadeObject, transform.position, transform.rotation);

                   // Debug.Log("던질때 생성");
                    Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();

                    //던지는 방향으로 던지기
                    rigidGrenade.AddForce(nextVec, ForceMode.Impulse);

                    //던지기 중 회전
                    rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                    //수류탄을 던졌기 때문에, 수류탄 개수를 한개 빼줘야 함                   
                    hasGrenades--;
                    grenades[hasGrenades].SetActive(false);
                }
                if(!isTab && hasBigGrenades>0)
                {
                    
                    GameObject instantBigGrenade = Instantiate(bigGrenadeObject, transform.position, transform.rotation);

                    Rigidbody rigidBigGrenade = instantBigGrenade.GetComponent<Rigidbody>();

                    rigidBigGrenade.AddForce(nextVec, ForceMode.Impulse);

                    rigidBigGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                    hasBigGrenades--;
                    bigGrenades[hasBigGrenades].SetActive(false);
                }

            }
        }
    }



    //공격
    void Attack()
    {
        if (equipWeapon == null)
            return;

        //공격딜레이에 시간을 더해주고 공격가능 여부를 확인
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            //현재 착용하고 있는 무기가 근거리(Melee)이면 doSwing, 아니면 doShot //삼항연산자
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            //공격딜레이를 0으로 돌려서 다음 공격까지 기다린다
            fireDelay = 0;
        }

    }



    //착지
    //OnCollisionEnter() 이벤트 함수로 착지 구현
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    //회피
    void Dodge()
    {
        //무한 점프 방지. 점프가 false일때만 안에꺼를 실행
        //bool 값을 반대로 사용하고 싶으면 앞에 !(느낌표) 추가. (bool값에만 적용가능)
        //if (jDown && moveVec != Vector3.zero && !isJump) //내가 점프키를 눌렀고, 점프가 false일때
        //점프하면서 회피 불가능
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
        {

            dodgeVec = moveVec;

            //회피할때 지금 속도의 2배
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            //시간차 함수 호출. Invoke("함수이름", 숫자(시간차));
            Invoke("DodgeOut", 0.4f);

        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }


    //오브젝트 감지. 플레이어가 무기에 접근했다.
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = other.gameObject;   //변수에 저장

            //Debug.Log(nearObject.name);

        }
    }

    void OnTriggerExit(Collider other)
    {
        nearObject = null;
    }


    //상호작용 함수가 작동될 수 있는 조건 작성 //e누르면 아이템 섭취
    void Interation()
    {
        if (iDown && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon")
            {
                //Debug.Log("Eat");
                //아이템의 종류 가져오기
                Item item = nearObject.GetComponent<Item>();
                int weaponIdex = item.value;
                hasWeapons[weaponIdex] = true;

                Destroy(nearObject);
            }

        }
    }

    //무기 교체 함수
    void Swap()
    {
        //무기 중복 교체, 없는 무기 확인을 위한 조건
        //만약 1번키를 눌렀는데(sDown1), hasWeapons[0]가 없을 때(false), 들려진 무기가 같은 무기일때
        if (sDown1 && (!hasWeapons[0] || equipWeaponIdex == 0))
            //실행하면 안된다.
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIdex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIdex == 2))
            return;


        //index
        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;


        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            //빈손일때는 equipWeapon.SetActive(false);가 오류나기 때문에 실행x
            if (equipWeapon != null)
                //비활성화
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIdex = weaponIndex;
            //기존에 장착된 무기를 저장하는 변수를 선언하고 확용
            //단축키에 맞는 무기를 배열에서 활성화하기
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            //애니매이션
            anim.SetTrigger("doSwap");
            //교체 시간차를 위한 플래그 로직
            isSwap = true;
            //시간차 함수 호출. Invoke("함수이름", 숫자(시간차));
            Invoke("SwapOut", 0.4f);

        }


        if (tDown)
        {
            isTab = !isTab;
            GrenadeGroup.SetActive(isTab);
            BigGrenadeGroup.SetActive(!isTab);
        }

    }

    void SwapOut()
    {
        isSwap = false;
    }


    //아이템 입수 및 몬스터 데미지
    //OnTriggerEnter()에서 트리거 이벤트 작성
    void OnTriggerEnter(Collider other)
    {
        //부딫힌 태그가 아이템이면
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    //수류탄 개수대로 공전체가 활성화 되도록 구현
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
                case Item.Type.BigGrenade:
                    bigGrenades[hasBigGrenades].SetActive(true);
                    hasBigGrenades += item.value;
                    if (hasBigGrenades > maxHasBigGrenades)
                        hasBigGrenades = maxHasBigGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }
        else if (other.tag == "EnemyBullet")
        {

            if (!isDamage)
            {
                //Bullet 스크립트를 재활용하여 데미지 적용
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                //보스의 근접공격 오브젝트의 이름으로 보스 공격을 인지
                bool isBossAtk = other.name == "Boss Melee Area";

                StartCoroutine(OnDamage(isBossAtk));
            }
            //미사일만 찾기. 미사일만 Rigidbody가 없는걸 이용
            //플레이어의 무적과는 관계없이 투사체는 Destroy
            if (other.GetComponent<Rigidbody>() != null)
                //미사일 없애기
                Destroy(other.gameObject);
        }

    }

    IEnumerator OnDamage(bool isBossAtk)
    {

        //딜레이를 위한 무적타임
        isDamage = true;
        //반복문을 사용하여 모든 재질의 색상을 변경(피격시 노랑)
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }

        //피격 코루틴에서 넉백을 AddForce()로 구현
        if (isBossAtk)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);  //1초 무적타임

        //원래대로
        if (isBossAtk)
            rigid.velocity = Vector3.zero;


        isDamage = false;
        //반복문을 사용하여 모든 재질의 색상을 변경(원상복구)
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

    }


    //재장전
    void Reload()
    {
        //착용 무기가 없을 때
        if (equipWeapon == null)
            return;
        //착용 무기가 근접 무기일때
        if (equipWeapon.type == Weapon.Type.Melee)
            return;
        //총알이 한개도 없을 때
        if (ammo == 0)
            return;
        if (rDown && !isJump && !isSwap && isFireReady)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2f);  //재장전 시간
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }

    //스스로 돌아가는 현상 제어
    void FreezeRotation()
    {
        //angularVelocity: 물리 회전 속도
        rigid.angularVelocity = Vector3.zero;

    }

    //
    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }


    //
    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }


}

