using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoudnessCounter : MonoBehaviour {

	Rect fpsRect;
	GUIStyle style;

	// Use this for initialization
	void Start () {
		fpsRect = new Rect (100, 600, 400, 100);
		style = new GUIStyle ();
		style.fontSize = 30;
		StartCoroutine (RecalculateFPS ());
	}

	private IEnumerator RecalculateFPS()
	{
		while (true) 
		{
			yield return new WaitForSeconds (1);
		}
	}

	void OnGUI()
	{
		float fps = 1 / Time.deltaTime;
		GUI.Label (fpsRect, "FPS: " + string.Format("{0:0.0}",fps), style);
	}
}
