using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KinectControllers : MonoBehaviour {


	public float speed;
	public float rotationalSpeed;

	private int[] newCommand;
	private string status; // user action (up, down, right, left)
	private Vector3 spawnPoint; // initial user position

	string username = "test";

	WWW command;
	string commandURL = "http://www.contrib.andrew.cmu.edu/~ashishb/cgi-bin/kinectread.php?user=test";

	public Queue<string> commandQue = new Queue<string> (); // collects all the commands 

	public int numCopyX; // number of repeated commands for smooth motion for X
	public int numCopyZ; // number of repeated commands for smooth motion for Z

	/** User name input field **/
	Rect usernameField = new Rect (200, 130, 400, 400);
	GUIStyle usernameStyle = new GUIStyle();

	bool worldCoordinateMode = false;

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
					commandURL = "http://www.contrib.andrew.cmu.edu/~ashishb/cgi-bin/kinectread.php?user=" + username;
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
		Debug.Log (commandURL);
		if (username == "") {
			username = "Please Type Username";
		}
		int nbTouches = Input.touchCount;		
		if (nbTouches > 0 && Time.time - lastTouched >= 0.1F) {
			userHasHitReturn = false;
			lastTouched = Time.time;
			GUIEnabled = !GUIEnabled;
			//Debug.Log ("Touched");
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

					int xstep;
					int zstep;
					bool up;
					bool right;

					if (input == null) 
					{
						xstep = 0;
						zstep = 0;
						commandQue.Enqueue ("stop");
					} else
					{
						string Xstring = input ["X"].str;
						string Zstring = input ["Z"].str;
						xstep = (int)float.Parse (Xstring);
						zstep = (int)float.Parse (Zstring);
					}

					// determine number of repeats based on size of steps
					numCopyX = Mathf.Abs (xstep) * 2/10;
					numCopyZ = Mathf.Abs (zstep) * 2/10;

					// check direction of steps
					right 	= xstep > 0 ? true : false;
					up = zstep > 0 ? true : false;

				
					for (int copy = 0; copy < numCopyX; copy++) 
					{
						if (right) {
							commandQue.Enqueue ("right");
						} else {
							commandQue.Enqueue ("left");
						}
					}

					for (int copy=0; copy < numCopyZ; copy++)
					{
						if (up) {
							commandQue.Enqueue ("up");
						} else {
							commandQue.Enqueue ("down");
						}
					}
				}
			}
			yield return commandQue;
		}
	}
}
