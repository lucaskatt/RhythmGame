using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioPlayer : MonoBehaviour {
	private Dictionary<string, AudioSource> keys = new Dictionary<string, AudioSource>();
	public float volume = 0.1f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void startNoteAudio(Note note) {
		if (note.getStep() != 'R')
		{
			keys[note.getKey()].Play();
		}
	}

	public void stopNoteAudio(Note note)
	{
		if (note.getStep() != 'R')
		{
			keys[note.getKey()].Stop();
		}
	}


	public void assignKeys() {
		foreach(Transform child in transform) {
			AudioSource audioSource = child.GetComponent<AudioSource>();
			audioSource.volume = volume;
			keys[child.name] = audioSource;
		}
	}
}
