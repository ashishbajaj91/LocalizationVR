using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffLineController : MonoBehaviour {

	public float speed;
	public float rotationalSpeed;

	private Vector3 spawnPoint;

	// Use this for initialization
	void Start () 
	{
		spawnPoint = transform.position;
	}

	// offline controller. control by keyboard
	// Update is called once per frame
	void Update () {
		if (transform.position.y < -10f) {
			transform.position = spawnPoint;
		}
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		Vector3 rotation = new Vector3 (0, moveHorizontal, 0);
		transform.position += Camera.main.transform.forward * moveVertical * speed * Time.deltaTime;
		transform.Rotate (rotation * rotationalSpeed, Space.World);
	}
}
