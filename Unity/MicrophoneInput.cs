using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneInput : MonoBehaviour {

	public float minThreshold = 0;
	public float frequency = 0.0f;
	public int audioSampleRate = 44100;
	public string microphone;

	private List<string> options = new List<string>();
	private AudioSource audioSource;
	public float sensitivity = 1000F;

	public Light lt;
	public float duration = 6000.0F; // duration of light change back to initial
	private float initialIntensity;
	public float dimLight;

	bool dimTriggered;

	// for loudness counter
	public float loudThresh = 3F;
	float loudness;
	Rect loudRect;
	GUIStyle style;
	GUIStyle loudStyle;
	float prevTime;
	float currTime;

	float t0 = 0F;

	// Use this for initialization
	void Start () {
		initialIntensity = lt.intensity; // set initial intensity
		dimTriggered = false;
		//get components you'll need
		audioSource = GetComponent<AudioSource> ();

		// get all available microphones
		foreach (string device in Microphone.devices) {
			if (microphone == null) {
				//set default mic to first mic found.
				microphone = device;
			}
			options.Add(device);
		}

		UpdateMicrophone ();

		// Box for loudness viewer
		loudRect = new Rect (100, 600, 400, 100);
		style = new GUIStyle ();
		style.fontSize = 30;

		loudStyle = new GUIStyle ();
		loudStyle.fontSize = 60;
		loudStyle.normal.textColor = Color.red;
	}

	void UpdateMicrophone(){
		audioSource.Stop(); 
		//Start recording to audioclip from the mic
		audioSource.clip = Microphone.Start(microphone, true, 10, audioSampleRate);
		audioSource.loop = true; 
		// Mute the sound with an Audio Mixer group becuase we don't want the player to hear it
		Debug.Log(Microphone.IsRecording(microphone).ToString());

		if (Microphone.IsRecording (microphone)) { //check that the mic is recording, otherwise you'll get stuck in an infinite loop waiting for it to start
			while (!(Microphone.GetPosition (microphone) > 0)) {
			} // Wait until the recording has started. 

			Debug.Log ("recording started with " + microphone);

			// Start playing the audio source
			audioSource.Play (); 
		} else {
			//microphone doesn't work for some reason

			Debug.Log (microphone + " doesn't work!");
		}
	}

	public float GetAveragedVolume()
	{ 
		float[] data = new float[256];
		float a = 0;
		audioSource.GetOutputData(data,0);
		foreach(float s in data)
		{
			a += Mathf.Abs(s);
		}
		return a/256;
	}

	// Update is called once per frame
	void Update () 
	{
		loudness = GetAveragedVolume () * sensitivity;
		currTime = Time.time;

		if (loudness > loudThresh && !dimTriggered)
		{
			Debug.Log ("***** Audio Volume: " + loudness + "*******");
			lt.intensity = dimLight;
			dimTriggered = true; // prevent dimming light while light is dim
			prevTime = Time.time;
		}

		// gradually return to initial light intensity
		if (lt.intensity < initialIntensity && dimTriggered && currTime > 3.0F + prevTime) 
		{
			lt.intensity = initialIntensity;
			dimTriggered = false;
//			float phi = t0 / duration * 2 * Mathf.PI; // TODO: this is wrong. time should start from 0
//			t0 += Time.deltaTime;
//			float amplitude = Mathf.Cos (phi) * (initialIntensity - dimLight) + dimLight;
//			lt.intensity = amplitude;
//			if (lt.intensity > initialIntensity - 0.01F) 
//			{
//				lt.intensity = initialIntensity;
//				dimTriggered = false;
//				t0 = 0F;
//			}
		}
			
	}

	void OnGUI()
	{
		if (loudness > loudThresh) 
		{
			GUI.Label (loudRect, "Loudness: " + string.Format ("{0:0.0}", loudness), loudStyle);
		} 
		else 
		{
			GUI.Label (loudRect, "Loudness: " + string.Format ("{0:0.0}", loudness), style);
		}
	}
}
