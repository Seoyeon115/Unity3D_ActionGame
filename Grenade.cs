using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;
    public bool IsBig;


    void Start()
    {
        //코루틴 함수 호출
        StartCoroutine(Explosion()); 
    }
    
    //3초뒤에 터진다. 코루틴
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);

        //굴러가는 속도 없애기
        rigid.velocity = Vector3.zero;
        //회전 속도 없애기
        rigid.angularVelocity = Vector3.zero;

        meshObj.SetActive(false);
        effectObj.SetActive(true);


        //수류탄 피격
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));

        //걸리는 애들이 있으면..
        foreach(RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position, IsBig);
        }

        //수류탄 없애기
        Destroy(gameObject, 5);
    }

}
