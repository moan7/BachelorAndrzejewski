using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArduinoBluetoothAPI;
using System;
using System.Text;
using TMPro;

public class manager : MonoBehaviour
{

	// Use this for initialization
	BluetoothHelper bluetoothHelper;
	string deviceName;

	public TMP_Text text;

	string received_message;

	//

	public int max_angle = 157; // state: turned off
	public static int max_study_angle = 145; // state: min TR = 0       // Triggermuscle 1.0: 147
	public static int min_study_angle = 87; // state: safe max TR = 1  ^// Triggermuscle 1.0: 88
	public static int study_angle_range; // calculates: 58

	int weightPercent = 0;

	// Buttons
	static Button btn_1;
	static Button btn_2;
	static Button btn_3;
	static Button btn_4;
	static Button btn_5;
	Button[] buttons = new Button[5];
	static Button btn_reset_servo;
	Vector4 original_color;

	
	public static TMP_Text graspedWeight;
	public static TMP_Text TR_value;
	private int active_N;

	public static TMP_Text CD_value;
	public static float activeCD;
	public static TMP_Text Gain_value;
	public static float activeGain;


	public static bool btnPressed = false;

	public bool isConnected = false;






	void Start()
	{
		study_angle_range = max_study_angle - min_study_angle;

		ConnectBT();		
	}

	private void ConnectBT()
	{
		deviceName = "ESP32"; //bluetooth should be turned ON;
		try
		{
			bluetoothHelper = BluetoothHelper.GetInstance(deviceName);
			bluetoothHelper.OnConnected += OnConnected;
			bluetoothHelper.OnConnectionFailed += OnConnectionFailed;
			bluetoothHelper.OnDataReceived += OnMessageReceived; //read the data

			//bluetoothHelper.setFixedLengthBasedStream(3); //receiving every 3 characters together
			bluetoothHelper.setTerminatorBasedStream("\n"); //delimits received messages based on \n char
															//if we received "Hi\nHow are you?"
															//then they are 2 messages : "Hi" and "How are you?"

			debugGUI(); // GUI for debug purpose


			LinkedList<BluetoothDevice> ds = bluetoothHelper.getPairedDevicesList();

			foreach (BluetoothDevice d in ds)
			{
				Debug.Log($"{d.DeviceName} {d.DeviceAddress}");
			}

			//Debug.Log(ds);
			// if(bluetoothHelper.isDevicePaired())
			// 	sphere.GetComponent<Renderer>().material.color = Color.blue;
			// else
			// 	sphere.GetComponent<Renderer>().material.color = Color.grey;
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
			//text.text = ex.Message;
			//BlueToothNotEnabledException == bluetooth Not turned ON
			//BlueToothNotSupportedException == device doesn't support bluetooth
			//BlueToothNotReadyException == the device name you chose is not paired with your android or you are not connected to the bluetooth device;
			//								bluetoothHelper.Connect () returned false;
		}
	}


	// Update is called once per frame
	void Update()
	{

		if (bluetoothHelper.isDevicePaired()) // check if device is paired
		{

			// then try to connect, if not yet connected
			if (!bluetoothHelper.isConnected())
			{
				//Debug.Log("Trying to connect...");
				bluetoothHelper.Connect(); // tries to connect
			}
		}
		else
		{
			//Debug.Log("Device is not paired");
		}

		// Connect manually again after timeout
		//if (Input.GetKeyDown(KeyCode.R))
		//{
		//	bluetoothHelper.Disconnect();
		//	Debug.Log("disconnect called");
		//	ConnectBT();
		//	Debug.Log("ConnectBT()");
		//}

	}

	public void SendAngle(float sender_value)
	{
		if (bluetoothHelper.isConnected())
		{
			//Debug.Log("manager.send stimuli TR: " + sender_value);

			float weight_tmp = sender_value * 100.0f;
			weightPercent = (int)weight_tmp;

			// convert from 0-1 range into angle range
			float float_angle = max_study_angle - (sender_value * study_angle_range);
			//Debug.Log("float_angle: " + float_angle);
			int int_angle = (int)float_angle;
			//Debug.Log("int_angle: " + int_angle);

			// make sure to send only valid angles
			if (int_angle <= max_angle && int_angle >= min_study_angle)
			{
				//Debug.Log("manager.SendAngle(): " + int_angle + ", TR value: " + sender_value); // always prints into console when an angle is sent to the controller

				if (sender_value == 1)
				{

					active_N = 11; // ResistanceWeight.maxTR_N;

				}
				else if (sender_value == 0.5)
				{
					active_N = 8; // ResistanceWeight.middleTR_N;

				}
				else if (sender_value == 0)
				{
					active_N = 4; // ResistanceWeight.minTR_N;
				}

				try
				{
					bluetoothHelper.SendData(int_angle.ToString());
				}
				catch (Exception e)
				{
					bluetoothHelper.Disconnect();
					ConnectBT();
					Debug.LogWarning("Reconnected to Triggermuscle due to Timeout Exception.");
				}
			}
			else
			{
				Debug.Log("Angle out of range");
			}
		}

	}


	//Asynchronous method to receive messages
	void OnMessageReceived(BluetoothHelper helper)
	{
		Debug.Log("OnMessageReceived");

		received_message = helper.Read();

		String msg = received_message.Trim(); //trim(): remove any disrupting spaces from sent data
        bool pressed = false;
        if (msg.Equals("0"))
		{
			if(!pressed)
			{
                //Debug.Log("PRESSED");
				pressed= true;
            }
			
			//enableLabel();
			//phwd.SetRightTrigger(true);
			//phwd.SetLeftTrigger(true);

			btnPressed = true; // phw.SetTrigger(true);???
							   //phw.SetTrigger(true);
							   //PseudoHapticWeight.SetTrigger(true);

		}
		else if (msg.Equals("1"))
		{

            //Debug.Log("RELEASED");

			
			//disableLabel();
			btnPressed = false; // phw.SetTrigger(false);???
                                //phw.SetTrigger(false);
                                //PseudoHapticWeight.SetTrigger(false);
                                //phwd.SetRightTrigger(false);
                                //phwd.SetLeftTrigger(false);
            pressed = false;
        }
	}

	void OnConnected(BluetoothHelper helper)
	{

		try
		{

			//enableButtons();

			helper.StartListening();
			isConnected = true;
			//Debug.Log("Connected...");

		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}

	}


	void OnConnectionFailed(BluetoothHelper helper)
	{
		Debug.Log("Connection Failed");
	}

	void debugGUI()
	{
		//debug_resistance.onValueChange.AddListener();

		// buttons
		btn_1 = GameObject.Find("Debug_btn_1").GetComponent<Button>();
		btn_2 = GameObject.Find("Debug_btn_2").GetComponent<Button>();
		btn_3 = GameObject.Find("Debug_btn_3").GetComponent<Button>();
		btn_4 = GameObject.Find("Debug_btn_4").GetComponent<Button>();
		btn_5 = GameObject.Find("Debug_btn_5").GetComponent<Button>();
		btn_reset_servo = GameObject.Find("Debug_btn_reset_servo").GetComponent<Button>();

		original_color = btn_1.image.color; // get original color

		for (int i = 0; i < buttons.Length; i++)
		{
			float intensity = i * 0.25f;

			if (i == 0)
			{
				buttons[i] = btn_1;
				//btn_1.interactable = false;
				btn_1.onClick.AddListener(
					delegate
					{
						Debug.Log("intensity: " + intensity);
						SendAngle(intensity);
						Debug.Log("Button1");
					}
				);

			}
			else if (i == 1)
			{
				buttons[i] = btn_2;
				btn_2.onClick.AddListener(
					 delegate
					 {
						 Debug.Log("intensity: " + intensity);
						 SendAngle(intensity);
					 }
				 );
			}
			else if (i == 2)
			{
				buttons[i] = btn_3;
				btn_3.onClick.AddListener(
					 delegate
					 {
						 Debug.Log("intensity: " + intensity);
						 SendAngle(intensity);
					 }
				 );
			}
			else if (i == 3)
			{
				buttons[i] = btn_4;
				btn_4.onClick.AddListener(
					 delegate
					 {
						 Debug.Log("intensity: " + intensity);
						 SendAngle(intensity);
					 }
				 );
			}
			else if (i == 4)
			{
				buttons[i] = btn_5;
				btn_5.onClick.AddListener(
					 delegate
					 {
						 Debug.Log("intensity: " + intensity);
						 SendAngle(intensity);
					 }
				 );
			}

		}

		// disable buttons as long BT is not connected
		disableButtons();

		// label
		graspedWeight = GameObject.Find("GraspedWeightValue").GetComponent<TMP_Text>();
		TR_value = GameObject.Find("TRLabel_value").GetComponent<TMP_Text>();
		CD_value = GameObject.Find("CDLabel_value").GetComponent<TMP_Text>();
		Gain_value = GameObject.Find("GainLabel_value").GetComponent<TMP_Text>();
		disableLabel();

		// reset servo button
		btn_reset_servo.onClick.AddListener(
			delegate
			{
				ResetServo();
			}
		);

	}

	void disableButtons()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].image.color = Color.gray;
			buttons[i].interactable = false;
		}
	}

	void enableButtons()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].image.color = original_color;
			buttons[i].interactable = true;
		}
	}

	public void enableLabel()
	{
		// weight
		graspedWeight.enabled = true;
		graspedWeight.text = weightPercent.ToString() + " %";

		// resistance
		TR_value.enabled = true;
		TR_value.text = active_N.ToString() + " N";

		// C/D
		CD_value.enabled = true;
		CD_value.text = activeCD.ToString();


		// Gain
		Gain_value.enabled = true;
		Gain_value.text = activeGain.ToString();


	}
	public static void disableLabel()
	{
		//graspedWeight.enabled = false;
		graspedWeight.enabled = false;
		TR_value.enabled = false;
		CD_value.enabled = false;
		Gain_value.enabled = false;
	}



	public void ResetServo() // Reset servo to 1
	{
		if (bluetoothHelper.isConnected())
		{
			int angle = max_angle; // max_prototype_angle;
			//Debug.Log("RESET: " + angle);

			try
			{
				bluetoothHelper.SendData(angle.ToString());
			}
			catch (Exception e)
			{
				bluetoothHelper.Disconnect();
				ConnectBT();
				Debug.LogWarning("Reconnected to Triggermuscle due to Timeout Exception.");
			}
		}
	}

	/*IEnumerator WaitForServoReset()
	{
		ResetServo();

		yield return new WaitForSecondsRealtime(0.1f);
		
		bluetoothHelper.Disconnect();

	}*/

	void OnDestroy()
	{
		if (bluetoothHelper != null)
		{
			//StartCoroutine(WaitForServoReset());
			
			ResetServo();
			bluetoothHelper.Disconnect();

		}
	}
}
