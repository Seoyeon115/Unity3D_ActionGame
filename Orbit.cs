using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    //����ź���� ������ �߽�. ���� ��ǥ
    public Transform target;
    //���� �ӵ�
    public float orbitSpeed;
    //��ǥ���� �Ÿ�
    Vector3 offSet;


    void Start()
    {
        //�÷��̾�� ����ź ������ �Ÿ� = ���� ����ź ��ġ - Ÿ�� ��ġ
        offSet = transform.position - target.position;
    }

    
    void Update()
    {
        transform.position = target.position + offSet;
        //RotateAround(): Ÿ�� ������ ȸ���ϴ� �Լ�
        transform.RotateAround(target.position,
                                Vector3.up,
                                orbitSpeed * Time.deltaTime);

        //��ġ�� �ٲ�� ������
        offSet = transform.position - target.position;;

    }
}
