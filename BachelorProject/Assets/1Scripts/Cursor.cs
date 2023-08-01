using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    GameObject curretObejct;
    MeshRenderer meshRenderer;
    manager manager;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer= GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.btnPressed || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            if(curretObejct != null && !curretObejct.GetComponent<ObjectGrabHandler>().isReleased)
            curretObejct.GetComponent<ObjectGrabHandler>().RayInteraction();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="grabbable")
        {
            meshRenderer.enabled= true;
            curretObejct = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "grabbable")
        {
            meshRenderer.enabled = false;
            curretObejct = null;
        } 
    }
}
