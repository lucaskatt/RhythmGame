using UnityEngine;
using System.Collections;
using InControl;
using System.Collections.Generic;

public class HitZone : MonoBehaviour
{
	public Material inactiveMat;
	public Material prevMat;
	public Material playMat;
	private bool active = false;
	private InputControl control1 = null;
	private InputControl control2 = null;
	private List<Note> playerNotes = new List<Note>();
	public Player player;
	private Light pointLight;
	private bool playing = false;
	private bool paused = false;
	public Color noteColor;
	//private ParticleSystem particle;



	// Use this for initialization
	void Start()
	{
		GetComponent<Renderer>().material = inactiveMat;
		prevMat = inactiveMat;
		pointLight = GetComponent<Light>();

		//particle = GetComponent<ParticleSystem>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "Note") {
			Note note = (Note)col.gameObject.GetComponent(typeof(Note));
			note.traveling = false;
		}
	}

	 void OnTriggerStay(Collider col) {
		if (active && col.gameObject.tag == "Note") {
			Note playerNote = (Note)col.gameObject.GetComponent(typeof(Note));
			if (!playerNote.isPlaying)
			{
				if (playerNotes.Count == 0 || playerNote != playerNotes[playerNotes.Count - 1])
				{
					playerNotes.Add(playerNote);
				}
				if (playerNote == playerNotes[0]) {
					playNote(playerNote, GetComponent<Renderer>().material);
				}
			}
		}
	 }

	 void OnTriggerExit(Collider col) {
		if (col.gameObject.tag == "Note")
		{
			Note note = (Note)col.gameObject.GetComponent(typeof(Note));
			if (note.isPlaying && note == playerNotes[0])
			{
				stopNote(note);
				playerNotes.RemoveAt(0);
				if (playerNotes.Count > 0) {
					playNote(playerNotes[0], GetComponent<Renderer>().material);
				}
			}
		}
	}

	public void activate(InputControl control, Material mat)
	{
		active = true;
		if (control1 == null && control2 == null) {
			GetComponent<Renderer>().material = mat;
		}
		if (control1 == null)
		{
			control1 = control;
		}
		else {
			control2 = control;
		}
	}

	public void deactivate(InputControl control, Material mat)
	{
		if (control == control1) {
			control1 = null;
			if (!playing)
			{
				GetComponent<Renderer>().material = mat;
			}

		}
		else if (control == control2) {
			control2 = null;
			if (!playing)
			{
				GetComponent<Renderer>().material = mat;
			}
		}

		if (control1 == null && control2 == null)
		{
			GetComponent<Renderer>().material = inactiveMat;
			active = false;
			foreach (Note playerNote in playerNotes) {
				if (playerNote != null)
				{
					stopNote(playerNote);

				}
			}
			playerNotes.Clear();
			GetComponent<Renderer>().material = inactiveMat;
		}
	}

	IEnumerator scoring() {
		while (true) {
			player.score += 1;
			yield return new WaitForFixedUpdate();
		}
	}

	void playNote(Note note, Material mat) {
		if (!paused)
		{
			playing = true;
			note.play();
			note.GetComponent<Renderer>().material.SetColor("_TintColor", noteColor);
			StartCoroutine("scoring");
			pointLight.enabled = true;
			GetComponent<Renderer>().material = playMat;
			prevMat = mat;
		}
	}

	void stopNote(Note note) {
		playing = false;
		note.stop();
		StopCoroutine("scoring");
		pointLight.enabled = false;
		GetComponent<Renderer>().material = prevMat;
	}

	public void pause() {
		paused = true;
	}

	public void resume() {
		paused = false;
	}
}