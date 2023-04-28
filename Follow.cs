using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    //카메라가 따라가야할 타겟
    public Transform target;

    //우리가 설정한 위치를 그대로 하기 위해서
    //따라갈 목표와 오프셋을 public 변수로 선언. offset: 보정 값
    public Vector3 offset;

    
    void Update()
    {
        //타겟의 위치에서 보정값을 더 한 값이다
        transform.position = target.position + offset;
    }
}
