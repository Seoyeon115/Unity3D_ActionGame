using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //�ν����� â���� ������ �� �ֵ��� public ���� �߰�
    public float speed;

    //�÷��̾��� ������� �迭 �Լ�2�� ����
    public GameObject[] weapons;
    public bool[] hasWeapons;
    //����ź. �����ϴ� ��ü�� ��Ʈ���ϱ� ���� �迭���� ����
    public GameObject[] grenades;
    public GameObject[] bigGrenades;
    //����ź �׷�
    public GameObject GrenadeGroup;
    public GameObject BigGrenadeGroup;


    //���콺�� ȸ���ϱ�
    public Camera followCamera;

    //�÷��̾� ��ġ ���� ���� (ź��, ����, ü��, ����ź(�ʻ��))
    public int ammo;
    public int coin;
    public int health;

    public int hasGrenades;
    public int hasBigGrenades;
    public GameObject grenadeObject;   //����ź �������� ������ ����
    public GameObject bigGrenadeObject;

    //�� ������ �ִ밪�� ������ ����
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;
    public int maxHasBigGrenades;

    //Input Axis ���� ���� �������� ����
    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool iDown;
    bool fDown; //Ű�Է�
    bool gDown;  //����ź
    bool rDown;  //������

    //���ⱳü
    bool sDown1;
    bool sDown2;
    bool sDown3;

    //����ź ��ü
    bool tDown;

    bool isJump;  //������ �ϰ� �ִ°�?
    bool isDodge;
    bool isSwap;  //��ü �ð���
    bool isFireReady = true;  //���� �غ�
    bool isReload;
    bool isBorder; //�� �浹 �÷���
    bool isDamage;      //����Ÿ��
    bool isTab = true; //�ǹ�ư �����°�?



    //������ ���ļ� �����
    Vector3 moveVec;
    Vector3 dodgeVec;

    //���� ȿ���� ���� Rigidbody ���� ���� ��, �ʱ�ȭ
    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] meshs;

    //Ʈ���� �� �������� �����ϱ� ���� ���� ����
    GameObject nearObject;
    //����ִ� ���� ����
    Weapon equipWeapon;
    //����ִ� ���Ⱑ ���� �������� Ȯ��
    int equipWeaponIdex = -1;
    //���� ������
    float fireDelay;

    //����ִ� ����ź
    Grenade equipGrenade;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //Animator ������ GetConponentInChildren()���� �ʱ�ȭ
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
        //GetAxisRaw() : Axis ���� ������ ��ȯ�ϴ� �Լ�
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        //Shift�� �������� �۵��ǵ��� GetButton()���
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        //fDown = Input.GetButtonDown("Fire1");    //Ŭ�� �ѹ��� �Ѿ� �ϳ�(�� ������ �ѹ�)
        fDown = Input.GetButton("Fire1");   //�� ������ ������ ��� ����
        gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reload");
        tDown = Input.GetButtonDown("Tab");    //tab������ ����ź ��ü
    }

    void Move()
    {
        //moveVec: ������ ����
        //normalized: ���� ���� 1�� ������ ����
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        //ȸ�� �߿��� ������ ���� -> ȸ�ǹ��� ���ͷ� �ٲٵ��� ����
        if (isDodge)
        {
            moveVec = dodgeVec;
        }
        //���ⱳü�Ҷ�, ���� �� ������ �Ұ�,
        if (isSwap || !isFireReady || isReload)
        {
            moveVec = Vector3.zero;
        }

        //
        if (!isBorder)

            //�������� �ӵ��� �پ�ߵȴ�! ���� ����
            /*if (wDown)
                transform.position += moveVec * speed * 0.3f * Time.deltaTime;
            else
                transform.position += moveVec * speed * Time.deltaTime;*/
            //���׿����ڸ� �̿��� �� ����. (bool���� ���� ? true�϶� �� : false�� �� ��)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;


        //SetBool()�Լ��� �Ķ���� ���� �����ϱ�
        //Animator�� �Ķ���ʹ� Bool���̰�, �⺻������ �޸���: isRun, ������ ��: moveVec
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }


    void Turn()
    {
        //ȸ�� / LookAt(): ������ ���͸� ���ؼ� ȸ�������ִ� �Լ�
        //���� �츮 ��ġ: transform.position , ���������� ��: moveVec => ���ư��� �������� �ٶ󺻴�!

        //Ű���忡 ���� ȸ��
        transform.LookAt(transform.position + moveVec);

        //���콺�� ���� ȸ��
        if (fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                //RayCastHit�� ���̴� �����ϵ��� y�� ���� 0���� �ʱ�ȭ
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }

        }
    }


    void Jump()
    {
        //���� ���� ����. ������ false�϶��� �ȿ����� ����
        //bool ���� �ݴ�� ����ϰ� ������ �տ� !(����ǥ) �߰�. (bool������ ���밡��)
        //moveVec==Vector3.zero : �������� ���� ��
        //if (jDown && moveVec==Vector3.zero && !isJump) //���� ����Ű�� ������, ������ false�϶�
        //�����ϸ鼭 ȸ�� �Ұ���
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            
            //AddForce() �Լ��� �������� ���� ���ϱ�
            //Vertor3.up: ��
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }


    //����ź
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
                //RayCastHit�� ���̴� �����ϵ��� y�� ���� ����
                nextVec.y = 10;

                if(isTab && hasGrenades>0)
                {
                    //����ź ����
                    GameObject instantGrenade = Instantiate(grenadeObject, transform.position, transform.rotation);

                   // Debug.Log("������ ����");
                    Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();

                    //������ �������� ������
                    rigidGrenade.AddForce(nextVec, ForceMode.Impulse);

                    //������ �� ȸ��
                    rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                    //����ź�� ������ ������, ����ź ������ �Ѱ� ����� ��                   
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



    //����
    void Attack()
    {
        if (equipWeapon == null)
            return;

        //���ݵ����̿� �ð��� �����ְ� ���ݰ��� ���θ� Ȯ��
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            //���� �����ϰ� �ִ� ���Ⱑ �ٰŸ�(Melee)�̸� doSwing, �ƴϸ� doShot //���׿�����
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            //���ݵ����̸� 0���� ������ ���� ���ݱ��� ��ٸ���
            fireDelay = 0;
        }

    }



    //����
    //OnCollisionEnter() �̺�Ʈ �Լ��� ���� ����
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    //ȸ��
    void Dodge()
    {
        //���� ���� ����. ������ false�϶��� �ȿ����� ����
        //bool ���� �ݴ�� ����ϰ� ������ �տ� !(����ǥ) �߰�. (bool������ ���밡��)
        //if (jDown && moveVec != Vector3.zero && !isJump) //���� ����Ű�� ������, ������ false�϶�
        //�����ϸ鼭 ȸ�� �Ұ���
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
        {

            dodgeVec = moveVec;

            //ȸ���Ҷ� ���� �ӵ��� 2��
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            //�ð��� �Լ� ȣ��. Invoke("�Լ��̸�", ����(�ð���));
            Invoke("DodgeOut", 0.4f);

        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }


    //������Ʈ ����. �÷��̾ ���⿡ �����ߴ�.
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = other.gameObject;   //������ ����

            //Debug.Log(nearObject.name);

        }
    }

    void OnTriggerExit(Collider other)
    {
        nearObject = null;
    }


    //��ȣ�ۿ� �Լ��� �۵��� �� �ִ� ���� �ۼ� //e������ ������ ����
    void Interation()
    {
        if (iDown && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon")
            {
                //Debug.Log("Eat");
                //�������� ���� ��������
                Item item = nearObject.GetComponent<Item>();
                int weaponIdex = item.value;
                hasWeapons[weaponIdex] = true;

                Destroy(nearObject);
            }

        }
    }

    //���� ��ü �Լ�
    void Swap()
    {
        //���� �ߺ� ��ü, ���� ���� Ȯ���� ���� ����
        //���� 1��Ű�� �����µ�(sDown1), hasWeapons[0]�� ���� ��(false), ����� ���Ⱑ ���� �����϶�
        if (sDown1 && (!hasWeapons[0] || equipWeaponIdex == 0))
            //�����ϸ� �ȵȴ�.
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
            //����϶��� equipWeapon.SetActive(false);�� �������� ������ ����x
            if (equipWeapon != null)
                //��Ȱ��ȭ
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIdex = weaponIndex;
            //������ ������ ���⸦ �����ϴ� ������ �����ϰ� Ȯ��
            //����Ű�� �´� ���⸦ �迭���� Ȱ��ȭ�ϱ�
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            //�ִϸ��̼�
            anim.SetTrigger("doSwap");
            //��ü �ð����� ���� �÷��� ����
            isSwap = true;
            //�ð��� �Լ� ȣ��. Invoke("�Լ��̸�", ����(�ð���));
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


    //������ �Լ� �� ���� ������
    //OnTriggerEnter()���� Ʈ���� �̺�Ʈ �ۼ�
    void OnTriggerEnter(Collider other)
    {
        //�΋H�� �±װ� �������̸�
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
                    //����ź ������� ����ü�� Ȱ��ȭ �ǵ��� ����
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
                //Bullet ��ũ��Ʈ�� ��Ȱ���Ͽ� ������ ����
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                //������ �������� ������Ʈ�� �̸����� ���� ������ ����
                bool isBossAtk = other.name == "Boss Melee Area";

                StartCoroutine(OnDamage(isBossAtk));
            }
            //�̻��ϸ� ã��. �̻��ϸ� Rigidbody�� ���°� �̿�
            //�÷��̾��� �������� ������� ����ü�� Destroy
            if (other.GetComponent<Rigidbody>() != null)
                //�̻��� ���ֱ�
                Destroy(other.gameObject);
        }

    }

    IEnumerator OnDamage(bool isBossAtk)
    {

        //�����̸� ���� ����Ÿ��
        isDamage = true;
        //�ݺ����� ����Ͽ� ��� ������ ������ ����(�ǰݽ� ���)
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }

        //�ǰ� �ڷ�ƾ���� �˹��� AddForce()�� ����
        if (isBossAtk)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);  //1�� ����Ÿ��

        //�������
        if (isBossAtk)
            rigid.velocity = Vector3.zero;


        isDamage = false;
        //�ݺ����� ����Ͽ� ��� ������ ������ ����(���󺹱�)
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

    }


    //������
    void Reload()
    {
        //���� ���Ⱑ ���� ��
        if (equipWeapon == null)
            return;
        //���� ���Ⱑ ���� �����϶�
        if (equipWeapon.type == Weapon.Type.Melee)
            return;
        //�Ѿ��� �Ѱ��� ���� ��
        if (ammo == 0)
            return;
        if (rDown && !isJump && !isSwap && isFireReady)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2f);  //������ �ð�
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }

    //������ ���ư��� ���� ����
    void FreezeRotation()
    {
        //angularVelocity: ���� ȸ�� �ӵ�
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

