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

	public Light lt;
	public float duration = 6000.0F; // duration of light change back to initial
	private float initialIntensity;
	public float dimLight;

	private bool dimTriggered;

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

	// Update is called once per frame
	void Update () {
		float audio = audioSource.GetSpectrumData(128,0,FFTWindow.BlackmanHarris)[64]*1000000;
		if (audio > minThreshold && !dimTriggered) 
		{
			Debug.Log ("***** Audio Volume: " + audio + "*******");
			lt.intensity = dimLight;
			dimTriggered = true; // prevent dimming light while light is dim
		}
		// gradually return to initial light intensity
		if (lt.intensity < initialIntensity) 
		{
			float phi = Time.time / duration * 2 * Mathf.PI;
			float amplitude = Mathf.Cos (phi) * (initialIntensity - dimLight) + dimLight;
			lt.intensity = amplitude;
			if (lt.intensity > initialIntensity - 0.05F) 
			{
				lt.intensity = initialIntensity;
				dimTriggered = false;
			}
		}
	}
}
