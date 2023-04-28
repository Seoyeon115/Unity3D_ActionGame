using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageChanger : MonoBehaviour
{
    public GameObject obj;

    private void Start()
    {
        obj.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            obj.gameObject.SetActive(true);
        }
    }
}

