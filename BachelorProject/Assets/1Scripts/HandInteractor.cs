using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteractor : MonoBehaviour
{
    private manager manager;
    private ObjectGrabHandler grabHandler;
    private GameObject rHand;
    public GameObject currentObject;
    private Transform objectPosition; 
    private bool isHolding;
    int frameCountHandPos;
    int grabCouter;

    // Start is called before the first frame update
    void Start()
    {
        rHand = GameObject.FindWithTag("RHand");
        manager = GameObject.Find("BluetoothAPI").GetComponent<manager>();
    }

    // Update is called once per frame
    void Update()
    {
        frameCountHandPos++;
        if (frameCountHandPos % 10 == 0)
        {
            Debug.Log("Visal Hand Position: " + GameObject.FindGameObjectWithTag("RHand").transform.position);
            Debug.Log("Visual Hand Roatation: " + GameObject.FindGameObjectWithTag("RHand").transform.eulerAngles);
        }
        if (currentObject != null)
        {
            if (manager.btnPressed || OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            {
                grabHandler.currentlyGrabbed = true;
                currentObject.GetComponent<Rigidbody>().isKinematic = true;
                currentObject.transform.position = objectPosition.position;
                currentObject.transform.parent = rHand.transform;
                if (!isHolding)
                {
                    Debug.Log(currentObject.name + " is grabbed");
                    isHolding = true;
                }
            }else if (grabHandler.currentlyGrabbed)
            {
                Debug.Log(currentObject.name + " is released");
                currentObject.transform.parent = null;
                currentObject.GetComponent<Rigidbody>().isKinematic = false;
                isHolding = false;
                grabHandler.currentlyGrabbed = false;
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "grabbable")
        {
            currentObject = other.gameObject;
            grabHandler = currentObject.GetComponent<ObjectGrabHandler>();
            objectPosition = currentObject.transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "grabbable")
        {
            currentObject = null;
        }
    }
}
