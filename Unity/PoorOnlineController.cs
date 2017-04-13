using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoorOnlineController : MonoBehaviour {

	public float speed;
	public float rotationalSpeed;
	private string status;

	private Vector3 spawnPoint;

	// Use this for initialization
	void Start () {
		status = "stop";
		spawnPoint = transform.position;
	}

	// Without web controller. control by keyboard
	// Update is called once per frame
	// void Update () {
	// if (transform.position.y < -10f) {
	// transform.position = spawnPoint;
	// }
	// float moveHorizontal = Input.GetAxis ("Horizontal");
	// float moveVertical = Input.GetAxis ("Vertical");
	// Vector3 rotation = new Vector3 (0, moveHorizontal, 0);
	// transform.position += Camera.main.transform.forward * moveVertical * speed * Time.deltaTime;
	// transform.Rotate (rotation * rotationalSpeed, Space.World);
	// }
	//
	string commandURL = "http://www.contrib.andrew.cmu.edu/~ashishb/cgi-bin/read.php?user=test";

	// Update is called once per frame
	void Update () {

		status = "stop";

		// Test use json to fetch status of the user
		WWW command = new WWW (commandURL);
		StartCoroutine(WaitForRequest(command));

		// do nothing until json is loaded
		while (!command.isDone) { }

		if (!command.text.StartsWith("<") && (command.error == null || command.error == ""))
		{

			JSONObject tempData = new JSONObject (command.text);
			Debug.Log (tempData);

			// 1. Get number of keys of the tempData (total data)
			// 2. execute action from oldest to newest

			for (int i = 0; i < tempData.list.Count; i++)
			{

				JSONObject input = (JSONObject)tempData.list [i];
				Debug.Log (input ["Action"]);

				if (input == null) {
					status = "stop";
				} else {
					status = input ["Action"].str;
				}

				if (status == "up") {
					transform.position = transform.position + Camera.main.transform.forward * speed * Time.deltaTime;
				}

				if (status == "down") {
					transform.position = transform.position - Camera.main.transform.forward * speed * Time.deltaTime;
				}

				if (status == "left") {
					transform.Rotate (0, -rotationalSpeed * Time.deltaTime, 0, Space.World);
				}

				if (status == "right") {
					transform.Rotate (0, rotationalSpeed * Time.deltaTime, 0, Space.World);
				}

				if (transform.position.y < -10f) {
					transform.position = spawnPoint;
				}
			}
		}
	}

	private IEnumerator WaitForRequest (WWW www)
	{
		yield return www;
	}
}
