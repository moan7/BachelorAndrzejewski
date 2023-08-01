using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimator : MonoBehaviour
{
    private Animator handAnimator;
    private GameObject currentObj;
    private ObjectGrabHandler grabHandler;

    // Start is called before the first frame update
    void Start()
    {
        handAnimator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("grabbable"))
        {
            if (Obj.GetComponent<ObjectGrabHandler>().selected == true)
            {
                currentObj = Obj;
                grabHandler = Obj.GetComponent<ObjectGrabHandler>();
            }
        }
        if(currentObj!= null)
        {
            if (Vector3.Distance(gameObject.transform.position, currentObj.transform.position) < 0.3)
            {
                handAnimator.SetBool("grabbing", true);
            }
            else
            {
                handAnimator.SetBool("grabbing", false);
                handAnimator.SetBool("hold", false);
            }
            if (grabHandler.currentlyGrabbed == true)
            {
                handAnimator.SetBool("grabbing", false);
                handAnimator.SetBool("release", false);
                handAnimator.SetBool("hold", true);
            }
            if (grabHandler.currentlyGrabbed == false)
            {
                handAnimator.SetBool("release", true);
            }
            if (grabHandler.selected == true)
            {
                handAnimator.SetBool("release", false);
            }
        }
    }
}
