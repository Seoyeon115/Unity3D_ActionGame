using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet    //상속
{
    //리지드바디 변수
    Rigidbody rigid;
    //회전 파워
    float angularPower = 2;
    //크기 숫자값 변수
    float scaleValue = 0.1f;
    
    
    //기를 모으고 쏘는 타이밍을 관리할 bool변수
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

    //기모으기
    IEnumerator GainPower()
    {
        while (!isShoot)
        {
            angularPower += 0.02f;
            scaleValue += 0.005f;
            //While문에서 증가된 값을 트랜스폼, 리지드바디에 적용
            transform.localScale = Vector3.one * scaleValue;
            //회전력
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            
            yield return null;     //While문에는 yield return null 포함
        }
    }


}
