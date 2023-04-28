using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
   //enum: 열거형 타입 (타입 이름 지정 필요)
    public enum Type { Ammo, Coin, Grenade, BigGrenade, Heart, Weapon };
    public Type type;

    //아이템 종류와 값을 저장할 변수 선언
    public int value;

  
    Rigidbody rigid;
    SphereCollider sphereCollider;

    void Awake()
    {
        //GetComponent() : 첫번째 컴포넌트를 가져옴
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }


    private void Update()
    {
        //Rotate(): 계속 회전하는 효과
        transform.Rotate(Vector3.up * 10 * Time.deltaTime);
    }

    //OnCollisionEnter(): 변수를 호출하여 물리효과 변경
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }

}
