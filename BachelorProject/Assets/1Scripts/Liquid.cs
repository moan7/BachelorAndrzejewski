using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Liquid : MonoBehaviour
{
    public GameObject drop;
    private GameObject dropClone;
    public Renderer rend;
    public float fill;
    private float radius = 20;
    private Vector3 offset = new Vector3(0, 0 ,0);
    int frames;
    // Start is called before the first frame update
    void Start()
    {
        rend = gameObject.GetComponent<Renderer>();
        fill = 0.7f;
        drop = GameObject.Find("Drop");
        //StartCoroutine(Inactive());
    }
    private IEnumerator Inactive()
    {
        yield return new WaitForSeconds(1);
        drop.SetActive(false);
    }

    // Update is called once per frame

    void Update()
    {
        
        rend.material.SetFloat("_Fill", fill);
        if (gameObject.transform.rotation.eulerAngles.x > radius && gameObject.transform.rotation.eulerAngles.x < 360 - radius || gameObject.transform.rotation.eulerAngles.z > radius && gameObject.transform.rotation.eulerAngles.z < 360 - radius && fill > 0)
        {

            fill -= 0.001f;
            radius += 0.5f;
            //yOffset -= 0.005f * Time.deltaTime;
            if (gameObject.transform.rotation.eulerAngles.x > radius && gameObject.transform.rotation.eulerAngles.x < 180)
            {
                offset = new Vector3(-0.05f, 0, 0);
            }
            else if (gameObject.transform.rotation.eulerAngles.x < 360 - radius && gameObject.transform.rotation.eulerAngles.x > 180)
            {
                offset = new Vector3(0.05f, 0, 0);
            }
            else if (gameObject.transform.rotation.eulerAngles.z > radius && gameObject.transform.rotation.eulerAngles.z < 180)
            {
                offset = new Vector3(0, 0, -0.05f);
            }
            else if (gameObject.transform.rotation.eulerAngles.z < 360 - radius && gameObject.transform.rotation.eulerAngles.z > 180)
            {
                offset = new Vector3(0, 0, 0.05f);
            }
            frames++;
            if (frames % 8 == 0)
            {

                Vector3 position = gameObject.transform.position + offset;
                dropClone = Instantiate(drop, position, transform.rotation) as GameObject;
                Debug.Log("Liquid Amount: " + fill);

            }
        }
    }
}