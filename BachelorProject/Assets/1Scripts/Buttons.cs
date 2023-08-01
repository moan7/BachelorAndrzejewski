using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{

    manager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("BluetoothAPI").GetComponent<manager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Button1");
            manager.SendAngle(0f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Button2");
            manager.SendAngle(0.5f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Button3");
            manager.SendAngle(1);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Reset");
            manager.ResetServo();
        }
    }

}
