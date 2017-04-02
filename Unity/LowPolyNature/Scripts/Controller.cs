using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller : MonoBehaviour {

	public float speed;
	public float rotationalSpeed;

	private string status;
	private Vector3 spawnPoint;
	public WWW command;
	public string commandURL = "http://www.contrib.andrew.cmu.edu/~ashishb/cgi-bin/read.php?user=test";

	public Queue<string> commandQue = new Queue<string> (); // collects all the commands 

	public int numCopy; // number of repeated commands

	// Use this for initialization
	void Start () 
	{
		status = "stop";
		spawnPoint = transform.position;
		StartCoroutine(ConnectWeb());
	}

	// Update is called once per frame
	void Update () 
	{
		if (commandQue.Count != 0) 
		{
			string currStatus = commandQue.Dequeue();
			Debug.Log ("Current command" + currStatus);

			if (currStatus == "up") 
			{
				transform.position = transform.position + Camera.main.transform.forward * speed * Time.deltaTime;
			}

			if (currStatus == "down") 
			{
				transform.position = transform.position - Camera.main.transform.forward * speed * Time.deltaTime;
			}
			if (currStatus == "left") 
			{
				//transform.Rotate (0, -rotationalSpeed * Time.deltaTime, 0, Space.World);
				transform.position = transform.position + Quaternion.Euler(0, -90, 0) * Camera.main.transform.forward * speed * Time.deltaTime;
			}

			if (currStatus == "right") 
			{
				//transform.Rotate (0, rotationalSpeed * Time.deltaTime, 0, Space.World);
				transform.position = transform.position + Quaternion.Euler(0, 90, 0) * Camera.main.transform.forward * speed * Time.deltaTime;

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

			if (command.text == "")
			{
				//Debug.Log ("Null command");
				yield return commandQue; // this part prevents error WWW not ready and saves FPS!
			}

			if (command != null && command.text != "" && 
				!command.text.StartsWith ("<") && (command.error == null || command.error == "")) 
			{
				JSONObject tempData = new JSONObject (command.text);

				for (int i = 0; i < tempData.list.Count; i++) 
				{
					JSONObject input = (JSONObject)tempData.list [i];

					if (input == null) 
					{
						status = "stop";
					} else 
					{
						status = input ["Action"].str;
					}
					for (int copy = 0; copy < numCopy; copy++) 
					{
						commandQue.Enqueue (status);
					}
				}
			}
			yield return commandQue;
		}
	}
}
	