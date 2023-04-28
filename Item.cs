using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
   //enum: ������ Ÿ�� (Ÿ�� �̸� ���� �ʿ�)
    public enum Type { Ammo, Coin, Grenade, BigGrenade, Heart, Weapon };
    public Type type;

    //������ ������ ���� ������ ���� ����
    public int value;

  
    Rigidbody rigid;
    SphereCollider sphereCollider;

    void Awake()
    {
        //GetComponent() : ù��° ������Ʈ�� ������
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }


    private void Update()
    {
        //Rotate(): ��� ȸ���ϴ� ȿ��
        transform.Rotate(Vector3.up * 10 * Time.deltaTime);
    }

    //OnCollisionEnter(): ������ ȣ���Ͽ� ����ȿ�� ����
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }

}
