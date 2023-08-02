using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;

public class ObjectGrabHandler : MonoBehaviour
{
    public Rigidbody rigidBody;
    public GameObject doneCanvas;
    public GameObject ray;
    private GameObject curser;
    private GameObject OVRInteraction;
    private int grabCounter;
    public bool currentlyGrabbed = false;
    public bool selected = false;
    public bool isReleased = false;
    manager manager;
    



    void Start()
    {
        OVRInteraction = GameObject.Find("OVRInteraction");
        ray = GameObject.FindGameObjectWithTag("Ray");
        doneCanvas = GameObject.Find("DoneCanvas");
        rigidBody = gameObject.GetComponent<Rigidbody>();
        curser = GameObject.Find("Cursor");
        manager = GameObject.Find("BluetoothAPI").GetComponent<manager>();
        StartCoroutine(LateStart());
    }
    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            Debug.Log("LoadScene");
            SceneManager.LoadSceneAsync(1);
        }
    }
    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(1);
        doneCanvas.SetActive(false);
        OVRInteraction.SetActive(false);
    }
    //public void toggleGrabbed()
    //{
        
    //    currentlyGrabbed = !currentlyGrabbed;
    //    if (currentlyGrabbed)
    //    {
    //        Debug.Log(gameObject.name + " is grabbed");
    //        gameObject.transform.parent = rHand.transform;
    //        rigidBody.isKinematic = false;
    //    }
    //    else
    //    {
    //        Debug.Log(gameObject.name + " is released");
    //        gameObject.transform.parent = null;
    //    }
    //}
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "TargetTable" || other.gameObject.name == "Floor")
        {
            curser.SetActive(true);
            ray.SetActive(true);
            if(!isReleased)
            {
                doneCanvas.GetComponent<SaveSystem>().grabCounter++;
                isReleased = true;
            }
            if (doneCanvas.GetComponent<SaveSystem>().grabCounter >= 8)
            {
                //canvas.SetActive(true);
                doneCanvas.SetActive(true);
                //StartCoroutine(LoadScene(1));
            }
            if (other.gameObject.name == "TargetTable")
            {
                Debug.Log(gameObject.name + " was placed on the Table ");
            }
            if (other.gameObject.name == "Floor")
            {
                Debug.Log(gameObject.name + " was dropped ");
            }
            manager.ResetServo();
            StartCoroutine(Destoy());
        }
    }
    private IEnumerator Destoy()
    {
        yield return new WaitForSeconds(10);
        gameObject.SetActive(false);
    }
    //private IEnumerator LoadScene(int scene)
    //{
    //    yield return new WaitForSeconds(60);
    //    Debug.Log("LoadScene");
    //    SceneManager.LoadScene(scene);
    //}
    public void RayInteraction()
    {
        curser.SetActive(false);
        selected = true;
        Debug.Log("Interacting with " + gameObject.name);
        ray.SetActive(false);
    }
}
