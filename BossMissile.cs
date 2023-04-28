using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet  //상속
{
    public Transform target;
    NavMeshAgent nav;


    void Awake()
    {
        //초기화
        nav = GetComponent<NavMeshAgent>();
    }

    
    void Update()
    {
        //추적
        nav.SetDestination(target.position);
        //미사일 시간 설정
        StartCoroutine(MissileTimer());
    }

    IEnumerator MissileTimer()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject);
    }
}
