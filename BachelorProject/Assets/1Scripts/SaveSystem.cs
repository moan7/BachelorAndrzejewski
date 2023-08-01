using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    string filename;
    string filenameHand;
    private int counter = 0;
    private Liquid liquid;
    
    public int grabCounter = 0;


    private void Start()
    {
        liquid = FindObjectOfType<Liquid>();
        for (int i = 0; i <= counter; i++)
        {

            if (System.IO.File.Exists(Application.persistentDataPath + "/" + counter + ".txt"))
            {
                counter++;
                //Debug.Log(counter);
            }
            else
            {
                filename = Application.persistentDataPath + "/" + counter + ".txt";
                filenameHand = Application.persistentDataPath + "/Hand" + counter + ".txt";
            }
        }
    }
    
    private void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    private void Log(string logString, string stackTrace, LogType type)
    {
        TextWriter tw = new StreamWriter(filename, true);
        if (!logString.Contains("OVR") && !logString.Contains("Null"))
        {
            tw.WriteLine("[" + System.DateTime.Now + ":" + System.DateTime.Now.Millisecond + "]" + logString);
        }
        
        tw.Close();
    }

    public void Save()
    {

        foreach (GameObject answers in GameObject.FindGameObjectsWithTag("answer"))
        {
            if (answers.name.Contains("Toggle") && answers.name.Contains(".a"))
            {
                if (answers.GetComponent<Toggle>().isOn == true)
                {
                    Debug.Log("Anwer1: A");
                }
                else
                {
                    Debug.Log("Anwer1: B");
                }

            }
            if (answers.name.Contains("Slider"))
            {
                Debug.Log("Answer2: " + answers.GetComponent<Slider>().value);
            }
        }
        Debug.Log("Liquid Amount: " + liquid.fill);
        gameObject.SetActive(false);
    }
}
