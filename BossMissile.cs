using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet  //���
{
    public Transform target;
    NavMeshAgent nav;


    void Awake()
    {
        //�ʱ�ȭ
        nav = GetComponent<NavMeshAgent>();
    }

    
    void Update()
    {
        //����
        nav.SetDestination(target.position);
        //�̻��� �ð� ����
        StartCoroutine(MissileTimer());
    }

    IEnumerator MissileTimer()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject);
    }
}
