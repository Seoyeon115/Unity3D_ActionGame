using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    //ī�޶� ���󰡾��� Ÿ��
    public Transform target;

    //�츮�� ������ ��ġ�� �״�� �ϱ� ���ؼ�
    //���� ��ǥ�� �������� public ������ ����. offset: ���� ��
    public Vector3 offset;

    
    void Update()
    {
        //Ÿ���� ��ġ���� �������� �� �� ���̴�
        transform.position = target.position + offset;
    }
}
