using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet    //���
{
    //������ٵ� ����
    Rigidbody rigid;
    //ȸ�� �Ŀ�
    float angularPower = 2;
    //ũ�� ���ڰ� ����
    float scaleValue = 0.1f;
    
    
    //�⸦ ������ ��� Ÿ�̹��� ������ bool����
    bool isShoot;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }


   IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        isShoot = true;
    }

    //�������
    IEnumerator GainPower()
    {
        while (!isShoot)
        {
            angularPower += 0.02f;
            scaleValue += 0.005f;
            //While������ ������ ���� Ʈ������, ������ٵ� ����
            transform.localScale = Vector3.one * scaleValue;
            //ȸ����
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            
            yield return null;     //While������ yield return null ����
        }
    }


}
