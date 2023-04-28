using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //��������, ���Ÿ� ���� Ÿ�� ����. ������
    public enum Type { Melee, Range };
    //�� ���� ����
    public Type type;
    //������
    public int damage;
    //����
    public float rate;
    //���� ���� - ����
    public BoxCollider meleeArea;
    //ȿ��
    public TrailRenderer trailEffect;
    //�Ѿ�. �������� �����ؾ��� ��ġ
    public Transform bulletPos;
    //�Ѿ�. �������� ������ ����
    public GameObject bullet;
    //ź��. �������� �����ؾ��� ��ġ
    public Transform bulletCasePos;
    //ź��. �������� ������ ����
    public GameObject bulletCase;

    //�ִ� źâ
    public int maxAmmo;
    //���� ź��
    public int curAmmo;






    //���� ����(�ڷ�ƾ)
    public void Use()
    {
        if (type == Type.Melee)
        {
            //�ڷ�ƾ �Լ��� ȣ�� ����� �ٸ���.
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Range && curAmmo > 0)
        {
            //ź�� �Ѱ��� �Ҹ�
            curAmmo--;
            
            StartCoroutine("Shot");
        }
    }


    IEnumerator Swing()
    {
        //1
        //����� �����ϴ� Ű���� yield
        yield return new WaitForSeconds(0.45f);   //0.1�� ��� ���
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        
        //2
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = false;
        
        //3
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        //#1. �Ѿ� �߻�. �Ѿ��� ��������鼭 �ӵ��� �ٴ´�.
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        yield return null;

        //#2. ź�� ����
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(-2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);

    }



    //�Ϲ������� : Use() ���η�ƾ -> Swing() �����ƾ -> ���� ��ƾ (���� ����)
    //�ڷ�ƾ �Լ�: ���η�ƾ + �ڷ�ƾ(Co-Op)  (���� ����)

}       
