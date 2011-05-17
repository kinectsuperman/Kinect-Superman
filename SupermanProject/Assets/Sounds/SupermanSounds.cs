using UnityEngine;
using System.Collections;
using System;

public class SupermanSounds : MonoBehaviour {

	private AudioSource flightSource;
	public AudioClip flightSound;
	private float fspeed;
	private Nite niteScript;
	
	// Use this for initialization
	void Start () {
		niteScript = gameObject.GetComponent(typeof(Nite)) as Nite;
		if (flightSource == null) {
			flightSource = gameObject.AddComponent("AudioSource") as AudioSource;
			flightSource.playOnAwake = false;
			flightSource.loop = true;
			flightSource.clip = flightSound;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (niteScript != null) {
			fspeed = niteScript.GetFlyingSpeed();
			if (fspeed > 0) {
				flightSource.volume = fspeed/3.0F;
				flightSource.pitch = 1 + (fspeed/2.0F);
				if (!flightSource.isPlaying)
					Debug.Log(String.Format("Playing wind at speed: {0}, volume: {1}, pitch: {2}",fspeed, flightSource.volume, flightSource.pitch));
					flightSource.Play();
			} else {
				Debug.Log("Stopped playing");
				flightSource.Stop();
				flightSource.volume = 0;
				flightSource.pitch = 1;
			}
		}
	}
}
