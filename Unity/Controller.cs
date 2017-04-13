using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller : MonoBehaviour {


	public float speed;
	public float rotationalSpeed;

	private string status; // user action (up, down, right, left)
	private string factor;
	private Vector3 spawnPoint; // initial user position

	string username = "test";

	WWW command;
	string commandURL = "http://www.contrib.andrew.cmu.edu/~ashishb/cgi-bin/kinectread.php?user=test";

	public Queue<string> commandQue = new Queue<string> (); // collects all the commands 

	public int numCopy; // number of repeated commands for smooth motion

	/** User name input field **/
	Rect usernameField = new Rect (200, 130, 400, 400);
	GUIStyle usernameStyle = new GUIStyle();

	bool worldCoordinateMode = true;

	bool GUIEnabled;
	float lastTouched;
	bool userHasHitReturn = true;

	void OnGUI()
	{
		usernameStyle.fontSize = 30;
		usernameStyle.normal.textColor = Color.black;

		if (GUIEnabled) {
			GUILayout.BeginArea (usernameField, usernameStyle);
			{
				GUILayout.Label ("Username: ", usernameStyle);
				GUI.changed = false;
				username = GUILayout.TextField (username, usernameStyle);
				if (GUI.changed) {
					commandURL = "http://www.contrib.andrew.cmu.edu/~ashishb/cgi-bin/read.php?user=" + username;
				}
			}
			GUILayout.EndArea ();		
		}
	}

	// Use this for initialization
	void Start () 
	{
		GUIEnabled = true;
		status = "stop";
		spawnPoint = transform.position;
		StartCoroutine(ConnectWeb());
	}

	// Update is called once per frame
	void Update () 
	{
		//Debug.Log (commandURL);
		if (username == "") {
			username = "Please Type Username";
		}
		int nbTouches = Input.touchCount;		
		if (nbTouches > 0 && Time.time - lastTouched >= 0.1F) {
			userHasHitReturn = false;
			lastTouched = Time.time;
			GUIEnabled = !GUIEnabled;
			Debug.Log ("Touched");
		}

		if (commandQue.Count != 0) 
		{
			string currStatus = commandQue.Dequeue();
			Debug.Log ("Current command" + currStatus);

			if (currStatus == "up") 
			{
				if(!worldCoordinateMode)
					transform.position = transform.position + Camera.main.transform.forward * speed * Time.deltaTime;
				else 
					transform.position = transform.position + transform.forward * speed * Time.deltaTime; // forward in world coordinate
			}

			if (currStatus == "down") 
			{
				if(!worldCoordinateMode)
					transform.position = transform.position - Camera.main.transform.forward * speed * Time.deltaTime;
				else
					transform.position = transform.position - transform.forward * speed * Time.deltaTime;
			}
			if (currStatus == "left") 
			{
				if(!worldCoordinateMode)
					transform.position = transform.position + Quaternion.Euler(0, -90, 0) * Camera.main.transform.forward * speed * Time.deltaTime;
				else
					transform.position = transform.position + Quaternion.Euler(0, -90, 0) * transform.forward * speed * Time.deltaTime;
			}

			if (currStatus == "right") 
			{
				if(!worldCoordinateMode)
					transform.position = transform.position + Quaternion.Euler(0, 90, 0) * Camera.main.transform.forward * speed * Time.deltaTime;
				else
					transform.position = transform.position + Quaternion.Euler(0, 90, 0) * transform.forward * speed * Time.deltaTime;
			}
			
		}

		// if user fall from the ground, return to original position
		if (transform.position.y < -10f) 
		{
			transform.position = spawnPoint;
		}
	}

	public IEnumerator ConnectWeb ()
	{
		while (true) 
		{
			//Debug.Log ("Connect Web running");
			command = new WWW (commandURL);

			StartCoroutine (WaitForRequest (command));
			// do nothing until json is loaded
			// while (!command.isDone) {} // this line slows down FPS
			// deltaTime = Time.deltaTime;
			yield return command;
			StartCoroutine(GetCommandQue());
		}
	}

	public IEnumerator WaitForRequest (WWW www)
	{
		yield return www;
	}

	public IEnumerator GetCommandQue()
	{
		while (true) 
		{
			//Debug.Log ("GetCommandQue Running");

			// this part prevents error WWW not ready and saves FPS!
			if (command.text == "")
			{
				//Debug.Log ("Null command");
				yield return commandQue; 
			}

//			if (command != null && command.text != "" && 
//				!command.text.StartsWith ("<") && (command.error == null || command.error == ""))
			else
			{
				JSONObject tempData = new JSONObject (command.text);

				for (int i = 0; i < tempData.list.Count; i++) 
				{
					JSONObject input = (JSONObject)tempData.list [i];

					if (input == null) 
					{
						factor = "1";
						status = "stop";
					} else 
					{
						factor = input ["Factor"].str;
						status = input ["Action"].str;
					}
					for (int copy = 0; copy < float.Parse(factor) * numCopy; copy++) 
					{
						commandQue.Enqueue (status);
					}
				}
			}
			yield return commandQue;
		}
	}
}
	