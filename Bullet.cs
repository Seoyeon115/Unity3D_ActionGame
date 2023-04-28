using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public int damage;
    public bool isMelee;   //이게 근접입니까?
    public bool isRock;


    void OnCollisionEnter(Collision collision)
    {
        if(!isRock && collision.gameObject.tag == "Floor")
        {
            //총알이 땅에 떨어지면 3초뒤에 사라진다
            Destroy(gameObject, 3f);
            
        }
        
    }

    //안 없어지는 총알 제거
    void OnTriggerEnter(Collider other)
    {
            
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
    
    //근접공격 범위가 파괴되지 않도록 조건 추가

}
