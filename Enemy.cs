using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    
    //enemyType ���� �迭 ����
    public enum Type { A, B, C, C1, D };
    public Type enemyType;
    
    //ü��
    public int maxHealth;
    public int curHealth;
    //��ǥ��
    public Transform target;
    //�ݶ��̴��� ���� ����
    public BoxCollider meleeArea;
    //�̻��� �������� ��Ƶ� ����
    public GameObject bullet;
    public GameObject bullet2;
    public GameObject bullet3;
    //���� ����
    public bool isChase;
    //���� ������
    public bool isAttack;
    //�׾��� ���� �˱� ���� �÷��� bool����
    public bool isDead; 


    public Rigidbody rigid;
    public BoxCollider boxCollider;
    //Material mat;
    public  MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;


    //���� ������
    public Transform dropItemPos;
    public GameObject dropItem;


    //�ʱ�ȭ
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
        if (nav.enabled && enemyType != Type.D )  //�׺���̼��� Ȱ��ȭ�Ǿ����� ����
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }

    //1. �Ϲ��� ����: ������ �ϱ� �ϴµ� ������ �����ؼ� ���ϰ� ����
    //2. ������ ����: ���� �ӵ��� �����Ͽ� ����
    //3. ���Ÿ��� ����: ���Ÿ� ����
    void Targeting()
    {
        if(!isDead && enemyType != Type.D)
        {
            float targetRadius = 0;   //������
            float targetRange = 0;    //�Ÿ�

            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;   //������
                    targetRange = 3f;    //�Ÿ�
                    break;
                case Type.B:
                    targetRadius = 1f;   //������
                    targetRange = 12f;    //�Ÿ�
                    break;
                case Type.C:
                    targetRadius = 0.5f;   //������
                    targetRange = 25f;    //�Ÿ�
                    break;
                case Type.C1:
                    targetRadius = 0.5f;   //������
                    targetRange = 25f;    //�Ÿ�
                    break;
            }

            //�ǰ�
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

            //rayHit ������ �����Ͱ� ������ ���� �ڷ�ƾ ����
            //���� ���� ������ �÷��̾ ������ ����
            if (rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
        
        
    }
    IEnumerator Attack()
    {
        
        //���� ������ �� ����, �ִϸ��̼ǰ� �Բ� ���ݹ��� Ȱ��ȭ
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);


        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;
                
                yield return new WaitForSeconds(1f);     //���� �����
                meleeArea.enabled = false;
                
                yield return new WaitForSeconds(1f);
               
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);  //�� ������
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);   //���� ����
                meleeArea.enabled = true;   //���ݹ��� Ȱ��ȭ

                yield return new WaitForSeconds(0.5f);     //���� �����
                rigid.velocity = Vector3.zero;    //�ӵ� ����
                meleeArea.enabled = false;   //���ݹ��� ��Ȱ��ȭ

                yield return new WaitForSeconds(2f); //����
                break;
            case Type.C:
                //�̻����� ���� ���
                yield return new WaitForSeconds(0.5f);  //��µ� �غ�ð�
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                
                //�Ѿ� ���
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
               
                rigidBullet.velocity = transform.forward * 20;
               
                //2�� ����
                yield return new WaitForSeconds(2f);

                break;
            case Type.C1:
                //�̻����� ���� ���
                yield return new WaitForSeconds(0.5f);  //��µ� �غ�ð�
                GameObject instantBullet1 = Instantiate(bullet, transform.position, transform.rotation);
                GameObject instantBullet2 = Instantiate(bullet2, transform.position, transform.rotation);
                GameObject instantBullet3 = Instantiate(bullet3, transform.position, transform.rotation);

                //�Ѿ� ���
                Rigidbody rigidBullet1 = instantBullet1.GetComponent<Rigidbody>();
                Rigidbody rigidBullet2 = instantBullet2.GetComponent<Rigidbody>();
                Rigidbody rigidBullet3 = instantBullet3.GetComponent<Rigidbody>();

                rigidBullet1.velocity = transform.forward * 20;
                rigidBullet2.velocity = transform.right * 20;
                rigidBullet3.velocity = -(transform.right * 20);

                //2�� ����
                yield return new WaitForSeconds(2f);

                break;
        }

       //�ٽ� �����̱�(����)
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);

    }



    //������ ���ư��� ���� ����
    void FreezeVelocity()
    {
        if (isChase)
        {
        //�������� NavAgent �̵��� �������� �ʵ���
        rigid.velocity = Vector3.zero;
        //angularVelocity: ���� ȸ�� �ӵ�
        rigid.angularVelocity = Vector3.zero;

        }
    }

    void FixedUpdate()
    {
        //Ÿ������ ���� �Լ�
        Targeting();
        
        FreezeVelocity();
    }


    //�ش� ���Ͱ� �¾ƾߵǴ� ��. ��ġ, �Ѿ�
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

            //�Ѿ��� ���, ���� ����� �� �����ǵ���
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

    

    //������ ���� �ڷ�ƾ ����
    //����ź���� ���׼��� ���� ������ �߰� bool isGrenade
    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {

        //�ǰ� ������ �� ���������� ǥ��
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;                
        yield return new WaitForSeconds(0.1f);

        //�����ִ� ü���� �������� �ǰ� ���
        if(curHealth > 0)
        {
            //�� ����ġ
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;           
        }
        else   //�׾��� ��
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 14;   //���̻� ���� �浹x

            if (enemyType != Type.D && isDead == false)
            {
                Destroy(gameObject, 4);
                //��� ������ ����
                GameObject dropCoin = Instantiate(dropItem, dropItemPos.position, dropItemPos.rotation);
             }
            isDead = true;
            isChase = false;
            //��� ���׼��� �����ϱ� ���� NavAgent ��Ȱ��ȭ
            nav.enabled = false;
            anim.SetTrigger("doDie");

            if (isGrenade)     //����ź�ΰ�? �´ٸ�
            {
                //������, �ڷ� �������°� ǥ��
                //���׼��� �����ϱ� ���� ���͸� �ݴ�������� ����
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;
                                
                //������ ����ȸ���ϸ鼭
                rigid.freezeRotation = false; //x,y üũ �Ȱ� Ǯ��
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);  //���� ��������
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                //������, �ڷ� �������°� ǥ��
                //���׼��� �����ϱ� ���� ���͸� �ݴ�������� ����
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;

                rigid.AddForce(reactVec * 5, ForceMode.Impulse);  //���� ��������
            }            
        }

    }

}
