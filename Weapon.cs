using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //근접공격, 원거리 공격 타입 결정. 열거형
    public enum Type { Melee, Range };
    //위 저장 변수
    public Type type;
    //데미지
    public int damage;
    //공속
    public float rate;
    //공격 범위 - 근접
    public BoxCollider meleeArea;
    //효과
    public TrailRenderer trailEffect;
    //총알. 프리팹을 생성해야할 위치
    public Transform bulletPos;
    //총알. 프리팹을 저장할 변수
    public GameObject bullet;
    //탄피. 프리팹을 생성해야할 위치
    public Transform bulletCasePos;
    //탄피. 프리팹을 저장할 변수
    public GameObject bulletCase;

    //최대 탄창
    public int maxAmmo;
    //현재 탄약
    public int curAmmo;






    //공격 로직(코루틴)
    public void Use()
    {
        if (type == Type.Melee)
        {
            //코루틴 함수는 호출 방법이 다르다.
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Range && curAmmo > 0)
        {
            //탄약 한개씩 소모
            curAmmo--;
            
            StartCoroutine("Shot");
        }
    }


    IEnumerator Swing()
    {
        //1
        //결과를 전달하는 키워드 yield
        yield return new WaitForSeconds(0.45f);   //0.1초 대기 대기
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
        //#1. 총알 발사. 총알이 만들어지면서 속도가 붙는다.
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        yield return null;

        //#2. 탄피 배출
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(-2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);

    }



    //일반적으로 : Use() 메인루틴 -> Swing() 서브루틴 -> 메인 루틴 (교차 실행)
    //코루틴 함수: 메인루틴 + 코루틴(Co-Op)  (동시 실행)

}       
