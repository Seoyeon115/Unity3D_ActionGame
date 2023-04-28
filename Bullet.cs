using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public int damage;
    public bool isMelee;   //�̰� �����Դϱ�?
    public bool isRock;


    void OnCollisionEnter(Collision collision)
    {
        if(!isRock && collision.gameObject.tag == "Floor")
        {
            //�Ѿ��� ���� �������� 3�ʵڿ� �������
            Destroy(gameObject, 3f);
            
        }
        
    }

    //�� �������� �Ѿ� ����
    void OnTriggerEnter(Collider other)
    {
            
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
    
    //�������� ������ �ı����� �ʵ��� ���� �߰�

}
