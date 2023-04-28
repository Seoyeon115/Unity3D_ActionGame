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
        //�ڷ�ƾ �Լ� ȣ��
        StartCoroutine(Explosion()); 
    }
    
    //3�ʵڿ� ������. �ڷ�ƾ
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);

        //�������� �ӵ� ���ֱ�
        rigid.velocity = Vector3.zero;
        //ȸ�� �ӵ� ���ֱ�
        rigid.angularVelocity = Vector3.zero;

        meshObj.SetActive(false);
        effectObj.SetActive(true);


        //����ź �ǰ�
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));

        //�ɸ��� �ֵ��� ������..
        foreach(RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position, IsBig);
        }

        //����ź ���ֱ�
        Destroy(gameObject, 5);
    }

}
