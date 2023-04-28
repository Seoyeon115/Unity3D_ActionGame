using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy  //상속
{
    //미사일 저장 변수
    public GameObject missile;
    //미사일이 나가는 포트 두개
    public Transform missilePortA;
    public Transform missilePortB;

    //플레이어 움직임 예측 벡터 변수
    Vector3 lookVec;
    Vector3 tauntVec;

    //플레이어 바라보는 플래그 bool변수
    public bool isLook;

    

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        nav.isStopped = true;

        StartCoroutine(Think());
    }


    void Update()
    {
        //죽음 플래그 변수를 활용하여 패턴 정지
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }
        
        //플레이어 바라보기
        if (isLook)
        {
            
            //플레이어가 가는 방향을 예측해서 그 방향을 본다
            //보스가 플레이어 방향을 예측한다
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;

            //예측해서 바라보기
            transform.LookAt(target.position + lookVec);
        }
        else    //점프공격 할 때 목표지점으로 이동
        {
            nav.SetDestination(tauntVec);
        }
    }

    IEnumerator Think()
    {
        //생각하는 시간.
        //시간을 늘리면 난이도 낮아짐.
        yield return new WaitForSeconds(0.3f);
        

        //행동 패턴
        int ranAction = Random.Range(0, 5);
        switch (ranAction)
        {
            case 0:
            
            //미사일 발사
            case 1:
                StartCoroutine(MissileShot());
                break;

            case 2:

            //돌 굴러가는 패턴
            case 3:
                StartCoroutine(RockShot());
                break;

            //점프 공격 패턴
            case 4:
                StartCoroutine(Taunt());
                break;
        }

    }

    //3종 패턴을 담당할 코루틴
    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
       
        //액션 하나당 걸리는 시간
        yield return new WaitForSeconds(0.3f);
        //미사일 생성
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        
        //미사일 스크립트까지 접근하여 목표물 설정
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        //액션 하나당 걸리는 시간
        yield return new WaitForSeconds(0.3f);

        //미사일 생성
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        //미사일 스크립트까지 접근하여 목표물 설정
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2.5f);

        //패턴이 끝나면 다음 패턴
        StartCoroutine(Think());
    }

    IEnumerator RockShot()
    {
        //기 모을 때는 바라보기 중지
        isLook = false;        
        anim.SetTrigger("doBigShot");

        //돌 만들기
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);

        //꺼둔 바라보기 플래그 변수 되돌리기
        isLook = true;
        StartCoroutine(Think());
    }

    //플레이어쪽으로 점프해서 내려찍음
    IEnumerator Taunt() 
    {
        //내려찍을 위치
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;   //네비게이션이 정상적으로 따라감
        //플레이어와 충돌을 멈춤
        //콜라이더가 플레이어를 밀지 않도록 비활성
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);
        //발 아래 있는 공격 범위 콜라이더 활성화
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        //내려찍기
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);

        //공격이 끝났으므로 false를 true로 다시 바꾸기
        isLook = true;
        boxCollider.enabled = true; 
        nav.isStopped = true;

        StartCoroutine(Think());
    }
}
