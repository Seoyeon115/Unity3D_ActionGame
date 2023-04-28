using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy  //���
{
    //�̻��� ���� ����
    public GameObject missile;
    //�̻����� ������ ��Ʈ �ΰ�
    public Transform missilePortA;
    public Transform missilePortB;

    //�÷��̾� ������ ���� ���� ����
    Vector3 lookVec;
    Vector3 tauntVec;

    //�÷��̾� �ٶ󺸴� �÷��� bool����
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
        //���� �÷��� ������ Ȱ���Ͽ� ���� ����
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }
        
        //�÷��̾� �ٶ󺸱�
        if (isLook)
        {
            
            //�÷��̾ ���� ������ �����ؼ� �� ������ ����
            //������ �÷��̾� ������ �����Ѵ�
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;

            //�����ؼ� �ٶ󺸱�
            transform.LookAt(target.position + lookVec);
        }
        else    //�������� �� �� ��ǥ�������� �̵�
        {
            nav.SetDestination(tauntVec);
        }
    }

    IEnumerator Think()
    {
        //�����ϴ� �ð�.
        //�ð��� �ø��� ���̵� ������.
        yield return new WaitForSeconds(0.3f);
        

        //�ൿ ����
        int ranAction = Random.Range(0, 5);
        switch (ranAction)
        {
            case 0:
            
            //�̻��� �߻�
            case 1:
                StartCoroutine(MissileShot());
                break;

            case 2:

            //�� �������� ����
            case 3:
                StartCoroutine(RockShot());
                break;

            //���� ���� ����
            case 4:
                StartCoroutine(Taunt());
                break;
        }

    }

    //3�� ������ ����� �ڷ�ƾ
    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
       
        //�׼� �ϳ��� �ɸ��� �ð�
        yield return new WaitForSeconds(0.3f);
        //�̻��� ����
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        
        //�̻��� ��ũ��Ʈ���� �����Ͽ� ��ǥ�� ����
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        //�׼� �ϳ��� �ɸ��� �ð�
        yield return new WaitForSeconds(0.3f);

        //�̻��� ����
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        //�̻��� ��ũ��Ʈ���� �����Ͽ� ��ǥ�� ����
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2.5f);

        //������ ������ ���� ����
        StartCoroutine(Think());
    }

    IEnumerator RockShot()
    {
        //�� ���� ���� �ٶ󺸱� ����
        isLook = false;        
        anim.SetTrigger("doBigShot");

        //�� �����
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);

        //���� �ٶ󺸱� �÷��� ���� �ǵ�����
        isLook = true;
        StartCoroutine(Think());
    }

    //�÷��̾������� �����ؼ� ��������
    IEnumerator Taunt() 
    {
        //�������� ��ġ
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;   //�׺���̼��� ���������� ����
        //�÷��̾�� �浹�� ����
        //�ݶ��̴��� �÷��̾ ���� �ʵ��� ��Ȱ��
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);
        //�� �Ʒ� �ִ� ���� ���� �ݶ��̴� Ȱ��ȭ
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        //�������
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);

        //������ �������Ƿ� false�� true�� �ٽ� �ٲٱ�
        isLook = true;
        boxCollider.enabled = true; 
        nav.isStopped = true;

        StartCoroutine(Think());
    }
}
