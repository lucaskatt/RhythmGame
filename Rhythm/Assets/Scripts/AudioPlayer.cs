using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioPlayer : MonoBehaviour {
	private Dictionary<string, AudioSource> keys = new Dictionary<string, AudioSource>();
	public float volume = 0.1f;
	private HashSet<string> playing = new HashSet<string>();

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void pauseAll() {
		foreach (string key in playing) {
			keys[key].Stop();
		}
		playing.Clear();
	}

	public void startNoteAudio(Note note, float playTime) {
		if (note.getStep() != 'R')
		{
			playing.Add(note.getKey());
			keys[note.getKey()].Play();
			if (playTime < 0.25) {
				StartCoroutine(AudioFadeOut.FadeOut(keys[note.getKey()], 1.5f * playTime));
			}
		}
	}

	public void stopNoteAudio(Note note)
	{
		if (note.getStep() != 'R')
		{
			playing.Remove(note.getKey());
			keys[note.getKey()].Stop();
		}
	}

	public void startNoteStructAudio(Song.NoteStruct note, float playTime)
	{
		if (note.step != 'R')
		{
			playing.Add(note.key);
			keys[note.key].Play();
			if (playTime < 0.25)
			{
				StartCoroutine(AudioFadeOut.FadeOut(keys[note.key], playTime));
			}
		}
	}

	public void stopNoteStructAudio(Song.NoteStruct note)
	{
		if (note.step != 'R')
		{
			playing.Remove(note.key);
			keys[note.key].Stop();
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
