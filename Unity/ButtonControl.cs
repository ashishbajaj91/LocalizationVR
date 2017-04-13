//Link to get data: http://www.contrib.andrew.cmu.edu/~ashishb/cgi-bin/buttonread.php?user=test
//Link to simulate: http://www.contrib.andrew.cmu.edu/~ashishb/cgi-bin/buttonpress.cgi?user=test&buttonState=True

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonControl : MonoBehaviour {

	string username = "test";

	WWW command;
	string commandURL = "http://www.contrib.andrew.cmu.edu/~ashishb/cgi-bin/buttonread.php?user=test";

	private string status; // button action: true or false;
	private bool switchOn = false;
	public Queue<string> commandQue = new Queue<string> (); // collects all the commands

	public Light lt;
	private float initialIntensity;
	public float dimLight;

	// Use this for initialization
	void Start () 
	{
		initialIntensity = lt.intensity; // set initial intensity
		StartCoroutine(ConnectWeb());
	}

	// Update is called once per frame
	void Update () 
	{
		if (commandQue.Count != 0) 
		{
			string currStatus = commandQue.Dequeue();
			Debug.Log ("Current command" + currStatus);

			if (currStatus == "On") 
			{
				lt.intensity = dimLight;
			}

			else
			{
				lt.intensity = initialIntensity;
			}
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
			// this part prevents error WWW not ready and saves FPS!
			if (command.text == "")
			{
				//Debug.Log ("Null command");
				yield return commandQue; 
			}
			else
			{
				JSONObject tempData = new JSONObject (command.text);

				for (int i = 0; i < tempData.list.Count; i++) 
				{
					switchOn = !switchOn;
					if (switchOn) {
						commandQue.Enqueue ("On");
					} else {
						commandQue.Enqueue ("Off");
					}
				}
			}
			yield return commandQue;
		}
	}
}
