﻿using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour
{
	private char step;
	private int alter;
	private int octave;
	private int duration;
	private bool chord;
	private string key;
	private int noteNumber;
	private AudioPlayer player;
	private Rigidbody rigid;
	public static float startDist = 15f;
	public bool isPlaying = false;
	private float speed;

	void Start() {
		rigid = GetComponent<Rigidbody>();
	}

	public void init(Song.NoteStruct noteStruct)	{
		step = noteStruct.step;
		alter = noteStruct.alter;
		octave = noteStruct.octave;
		duration = noteStruct.duration;
		chord = noteStruct.chord;
		player = noteStruct.player;
		key = noteStruct.key;
		noteNumber = noteStruct.noteNumber;
	}

	public int getNoteNumber()
	{
		return noteNumber;
	}

	public char getStep()
	{
		return step;
	}

	public int getAlter()
	{
		return alter;
	}

	public int getOctave()
	{
		return octave;
	}

	public int getDuration()
	{
		return duration;
	}

	public bool isChord()
	{
		return chord;
	}

	public string getKey()
	{
		return key;
	}

	public void play()
	{
		isPlaying = true;
		player.startNoteAudio(this);
	}

	public void stop()
	{
		isPlaying = false;
		player.stopNoteAudio(this);
	}

	public void startMovement(float _speed, float hitDist, float playTime)
	{
		if (step != 'R')
		{
			speed = _speed;

			float yScale = (speed * playTime - hitDist);
			transform.localScale = new Vector3(0.8f, yScale, 0.1f);
			transform.position = new Vector3(0, startDist + yScale / 2, 0);
			transform.rotation = Quaternion.identity;
			transform.RotateAround(Vector3.zero, Vector3.forward, 30 * noteNumber);
			Vector3 norm = (Vector3.zero - transform.position).normalized;
			GetComponent<Rigidbody>().velocity = norm * speed;
		}
	}

	

	
	void OnTriggerStay(Collider col)
	{
		if (col.gameObject.tag == "Center")
		{
			rigid.velocity = Vector3.zero;
			transform.localScale -= new Vector3(0, speed * Time.deltaTime, 0);

			transform.RotateAround(Vector3.zero, Vector3.forward, -30 * noteNumber);
			transform.position -= new Vector3(0, speed * Time.deltaTime / 2, 0);
			transform.RotateAround(Vector3.zero, Vector3.forward, 30 * noteNumber);

			if (transform.localScale.y < 0.1)
			{
				transform.position = new Vector3(-100, -100, 0);
				rigid.velocity = Vector3.zero;
			}
		}
	}

}