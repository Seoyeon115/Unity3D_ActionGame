using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    
    //enemyType 결정 배열 변수
    public enum Type { A, B, C, C1, D };
    public Type enemyType;
    
    //체력
    public int maxHealth;
    public int curHealth;
    //목표물
    public Transform target;
    //콜라이더를 담을 변수
    public BoxCollider meleeArea;
    //미사일 프리펩을 담아둘 변수
    public GameObject bullet;
    public GameObject bullet2;
    public GameObject bullet3;
    //추적 결정
    public bool isChase;
    //공격 중인지
    public bool isAttack;
    //죽었을 때를 알기 위한 플래그 bool변수
    public bool isDead; 


    public Rigidbody rigid;
    public BoxCollider boxCollider;
    //Material mat;
    public  MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;


    //보상 아이템
    public Transform dropItemPos;
    public GameObject dropItem;


    //초기화
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        
        if(enemyType != Type.D)
            Invoke("ChaseStart", 2);
    }

    
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {
        if (nav.enabled && enemyType != Type.D )  //네비게이션이 활성화되어있을 때만
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }

    //1. 일반형 몬스터: 공격을 하긴 하는데 가만히 정지해서 약하게 공격
    //2. 돌격형 몬스터: 빠른 속도로 돌격하여 공격
    //3. 원거리형 몬스터: 원거리 공격
    void Targeting()
    {
        if(!isDead && enemyType != Type.D)
        {
            float targetRadius = 0;   //반지름
            float targetRange = 0;    //거리

            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;   //반지름
                    targetRange = 3f;    //거리
                    break;
                case Type.B:
                    targetRadius = 1f;   //반지름
                    targetRange = 12f;    //거리
                    break;
                case Type.C:
                    targetRadius = 0.5f;   //반지름
                    targetRange = 25f;    //거리
                    break;
                case Type.C1:
                    targetRadius = 0.5f;   //반지름
                    targetRange = 25f;    //거리
                    break;
            }

            //피격
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

            //rayHit 변수에 데이터가 들어오면 공격 코루틴 실행
            //공격 범위 안으로 플레이어갇 들어오면 공격
            if (rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
        
        
    }
    IEnumerator Attack()
    {
        
        //먼저 정지를 한 다음, 애니메이션과 함께 공격범위 활성화
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);


        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;
                
                yield return new WaitForSeconds(1f);     //멈춰 세운다
                meleeArea.enabled = false;
                
                yield return new WaitForSeconds(1f);
               
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);  //선 딜레이
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);   //돌격 구현
                meleeArea.enabled = true;   //공격범위 활성화

                yield return new WaitForSeconds(0.5f);     //멈춰 세운다
                rigid.velocity = Vector3.zero;    //속도 제어
                meleeArea.enabled = false;   //공격범위 비활성화

                yield return new WaitForSeconds(2f); //쉬기
                break;
            case Type.C:
                //미사일을 만들어서 쏜다
                yield return new WaitForSeconds(0.5f);  //쏘는데 준비시간
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                
                //총알 쏘기
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
               
                rigidBullet.velocity = transform.forward * 20;
               
                //2초 쉬기
                yield return new WaitForSeconds(2f);

                break;
            case Type.C1:
                //미사일을 만들어서 쏜다
                yield return new WaitForSeconds(0.5f);  //쏘는데 준비시간
                GameObject instantBullet1 = Instantiate(bullet, transform.position, transform.rotation);
                GameObject instantBullet2 = Instantiate(bullet2, transform.position, transform.rotation);
                GameObject instantBullet3 = Instantiate(bullet3, transform.position, transform.rotation);

                //총알 쏘기
                Rigidbody rigidBullet1 = instantBullet1.GetComponent<Rigidbody>();
                Rigidbody rigidBullet2 = instantBullet2.GetComponent<Rigidbody>();
                Rigidbody rigidBullet3 = instantBullet3.GetComponent<Rigidbody>();

                rigidBullet1.velocity = transform.forward * 20;
                rigidBullet2.velocity = transform.right * 20;
                rigidBullet3.velocity = -(transform.right * 20);

                //2초 쉬기
                yield return new WaitForSeconds(2f);

                break;
        }

       //다시 움직이기(공격)
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);

    }



    //스스로 돌아가는 현상 제어
    void FreezeVelocity()
    {
        if (isChase)
        {
        //물리력이 NavAgent 이동을 방해햐지 않도록
        rigid.velocity = Vector3.zero;
        //angularVelocity: 물리 회전 속도
        rigid.angularVelocity = Vector3.zero;

        }
    }

    void FixedUpdate()
    {
        //타게팅을 위한 함수
        Targeting();
        
        FreezeVelocity();
    }


    //해당 몬스터가 맞아야되는 것. 망치, 총알
    void OnTriggerEnter(Collider other)
    {
        
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            
            StartCoroutine(OnDamage(reactVec, false));

            Debug.Log("Melee : " + curHealth);
        }
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;

            //총알의 경우, 적과 닿았을 때 삭제되도록
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec, false));

            Debug.Log("Range : " + curHealth);
        }
        
    }


    public void HitByGrenade(Vector3 explosionPos, bool IsBig)
    {
        if (!IsBig)
            curHealth -= 100;
        else
            curHealth -= 200;
        Vector3 reactVec = transform.position - explosionPos;

        StartCoroutine(OnDamage(reactVec, true));
    }

    

    //로직을 담을 코루틴 생성
    //수류탄만의 리액션을 위해 구분자 추가 bool isGrenade
    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {

        //피격 당했을 시 빨간색으로 표시
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;                
        yield return new WaitForSeconds(0.1f);

        //남아있는 체력을 조건으로 피격 결과
        if(curHealth > 0)
        {
            //색 원위치
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;           
        }
        else   //죽었을 때
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 14;   //더이상 물리 충돌x

            if (enemyType != Type.D && isDead == false)
            {
                Destroy(gameObject, 4);
                //드랍 아이템 생성
                GameObject dropCoin = Instantiate(dropItem, dropItemPos.position, dropItemPos.rotation);
             }
            isDead = true;
            isChase = false;
            //사망 리액션을 유지하기 위해 NavAgent 비활성화
            nav.enabled = false;
            anim.SetTrigger("doDie");

            if (isGrenade)     //수류탄인가? 맞다면
            {
                //죽을때, 뒤로 밀쳐지는걸 표현
                //리액션을 설정하기 위해 백터를 반대방향으로 설정
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;
                                
                //죽을때 공중회전하면서
                rigid.freezeRotation = false; //x,y 체크 된거 풀기
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);  //힘이 가해진다
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                //죽을때, 뒤로 밀쳐지는걸 표현
                //리액션을 설정하기 위해 백터를 반대방향으로 설정
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;

                rigid.AddForce(reactVec * 5, ForceMode.Impulse);  //힘이 가해진다
            }            
        }

    }

}
