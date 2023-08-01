using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKController : MonoBehaviour
{
    public Animator animator;
    private manager manager;
    private GameObject handTarget;
    private TwoBoneIKConstraint constraint;
    private GameObject neckTarget;
    private MultiAimConstraint neckConstraint;
    private ObjectGrabHandler grabHandler;
    private GameObject cameraRig;
    public GameObject currentObj;
    private Transform standPosition;
    private Vector3 startPosition;
    private Vector3 objectPosition;
    private Vector3 objectDirection;
    private Vector3 userDirection;
    private Vector3 startDirection;
    private bool hold = false;
    private bool reachedObject = false;
    public bool reachedUser = false;
    private bool reachedStart = true;
    private bool robotReleasing = false;
    private bool sendAngle = false;
   
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        animator = GetComponent<Animator>();
        handTarget = gameObject.transform.Find("Rig1").Find("IKRightHand").Find("IKRightHand_target").gameObject;
        constraint = gameObject.transform.Find("Rig1").Find("IKRightHand").GetComponent<TwoBoneIKConstraint>();
        neckConstraint = gameObject.transform.Find("Rig1").Find("IKNeck").GetComponent<MultiAimConstraint>();
        neckTarget = GameObject.Find("NeckTarget");
        cameraRig = GameObject.Find("OVRCameraRigBA");
        manager = GameObject.Find("BluetoothAPI").GetComponent<manager>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (currentObj != null && grabHandler.selected == true)
        {
            neckTarget.transform.position = currentObj.transform.position;
            if (neckConstraint.weight < 1)
            {
                neckConstraint.weight += 0.1f * Time.deltaTime;
            }
        }
        else if (neckConstraint.weight > 0)
        {
            neckConstraint.weight -= 0.1f * Time.deltaTime;
        }

        if (currentObj!= null)
        {
            if (constraint.weight > 0)
            {
                if (grabHandler.currentlyGrabbed)
                {
                    grabHandler.selected = false;
                    reachedObject = false;
                    if (currentObj.name.Contains("2") || currentObj.name.Contains("3") || currentObj.name.Contains("5"))
                    {
                        handTarget.transform.position = currentObj.transform.position;
                        //if (Vector3.Distance(objectPosition, currentObj.transform.position) > 0.08)
                        if (objectPosition.y < currentObj.transform.position.y-0.03)
                        {
                            robotReleasing = true;
                        }
                        if(robotReleasing)
                        {
                            constraint.weight -= 1f * Time.deltaTime;
                            if (constraint.weight >= 0.9)
                            {
                                Debug.Log("Robot released after lifting " + currentObj);
                                hold = false;
                            }
                        }
                    }
                    else
                    {
                        constraint.weight -= 1f * Time.deltaTime;
                        if (constraint.weight >= 0.9)
                        {
                            Debug.Log("Robot released immediately " + currentObj);
                            hold = false;
                        }
                    }
                }else if (grabHandler.isReleased)
                {
                    constraint.weight -= 1f * Time.deltaTime;
                    if (constraint.weight >= 0.9)
                    {
                        Debug.Log("Robot released " + currentObj + " due to user released Object");
                        hold = false;
                    }
                }

            }
            else
            {
                robotReleasing = false;
            }
        }
        float startDistance;
        startDistance = Vector3.Distance(transform.position, startPosition);
        if (currentObj != null && startDistance > 0 && !reachedStart && !grabHandler.selected && constraint.weight <= 0)
        {
            animator.SetBool("walking", true);
            Vector3 targetPosition = new Vector3(startPosition.x, 0, startPosition.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 0.5f * Time.deltaTime);
            startDirection = (targetPosition - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
            Quaternion rotation = Quaternion.LookRotation(startDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1f);
            standPosition = gameObject.transform;
        }
        else if (startDistance <= 0 && standPosition != null)
        {
            Debug.Log("reachedStart");
            reachedStart = true;
            transform.position = standPosition.position;
            animator.SetBool("walking", false);
        }

        foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("grabbable")){
            if(Obj.GetComponent<ObjectGrabHandler>().selected == true)
            {
                currentObj = Obj;
                grabHandler = currentObj.GetComponent<ObjectGrabHandler>();

                reachedStart = false;

                float objectDistance;
                objectDistance = Vector3.Distance(transform.position, currentObj.transform.position);
                if(objectDistance < 1.8)
                {
                    handTarget.transform.position = currentObj.transform.position;
                    if (constraint.weight < 1)
                    {
                        constraint.weight += 0.5f * Time.deltaTime;
                    }
                }
                if (objectDistance > 1.1 && reachedObject == false)
                {
                    reachedUser = false;
                    animator.SetBool("walking", true);
                    Vector3 targetPosition = new Vector3(currentObj.transform.position.x, 0, currentObj.transform.position.z);
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, 0.5f * Time.deltaTime);
                    objectDirection = (targetPosition - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
                    Quaternion rotation = Quaternion.LookRotation(objectDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 0.9f);
                    standPosition = transform;
                }
                else
                {
                    reachedObject = true;
                    gameObject.transform.position = standPosition.position;
                    if (currentObj.transform.parent != transform)
                    {
                        animator.SetBool("walking", false);
                    }                    
                    if(constraint.weight>=1)
                    {
                        grabHandler.rigidBody.isKinematic = true;
                        currentObj.transform.localPosition = Vector3.MoveTowards(currentObj.transform.localPosition, new Vector3(0.115f, 1.354f, 0.6f), 0.8f * Time.deltaTime);
                        float userDistance;
                        userDistance = Vector3.Distance(transform.position, cameraRig.transform.position);
                        if (userDistance > 1.1f && reachedUser == false)
                        {
                            currentObj.transform.parent = transform;
                            animator.SetBool("walking", true);
                            Vector3 targetPosition = new Vector3(cameraRig.transform.position.x, 0, cameraRig.transform.position.z);
                            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 0.5f * Time.deltaTime);
                            userDirection = (targetPosition - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
                            Quaternion rotation = Quaternion.LookRotation(userDirection);
                            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1f);
                            standPosition = transform;
                            if (!sendAngle)
                            {
                                Debug.Log("$$SENDANGLE");
                                manager.SendAngle(1);
                                sendAngle= true;
                            }

                        }
                        else
                        {
                            if(!reachedUser)
                            {
                                Debug.Log("Reached User with " + currentObj + "Object Position" + currentObj.transform.position);
                                reachedUser = true;
                            }
                            
                            gameObject.transform.position = standPosition.position;
                            animator.SetBool("walking", false);
                            if (!hold)
                            {
                                if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || manager.btnPressed)
                                {
                                    sendAngle= false;
                                    objectPosition = currentObj.transform.position;
                                    hold= true;
                                }
                            }
                        }
                    }
                }
            }
        }
        
    }
}