using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    //수류탄들이 공전할 중심. 공전 목표
    public Transform target;
    //공전 속도
    public float orbitSpeed;
    //목표와의 거리
    Vector3 offSet;


    void Start()
    {
        //플레이어와 수류탄 사이의 거리 = 현재 수류탄 위치 - 타겟 위치
        offSet = transform.position - target.position;
    }

    
    void Update()
    {
        transform.position = target.position + offSet;
        //RotateAround(): 타겟 주위를 회전하는 함수
        transform.RotateAround(target.position,
                                Vector3.up,
                                orbitSpeed * Time.deltaTime);

        //위치가 바뀌기 때문에
        offSet = transform.position - target.position;;

    }
}
