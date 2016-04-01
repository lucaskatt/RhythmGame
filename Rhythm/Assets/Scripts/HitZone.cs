using UnityEngine;
using System.Collections;
using InControl;
using System.Collections.Generic;

public class HitZone : MonoBehaviour
{
	public Material inactiveMat;
	public Material activeMat;
	private bool active = false;
	private InputControl control1 = null;
	private InputControl control2 = null;
	private List<Note> playerNotes = new List<Note>();
	public Player player;



	// Use this for initialization
	void Start()
	{
		GetComponent<Renderer>().material = inactiveMat;
	}

	// Update is called once per frame
	void Update()
	{

	}

	 void OnTriggerStay(Collider col) {
		if (active && col.gameObject.tag == "Note") {
			/*
			if (control1 != null)
			{
				if (control1.State != control1.LastState)
				{
					
					if (control1.State)
					{
						playerNote = (Note)col.gameObject.GetComponent(typeof(Note));
						playerNote.play();
						StartCoroutine("scoring");
					}
					else
					{
						if (playerNote != null)
						{
							playerNote.stop();
							StopCoroutine("scoring");
						}
					}
				}
			}
			if (control2 != null)
			{
				if (control2.State != control2.LastState)
				{
					if (control2.State)
					{
						playerNote = (Note)col.gameObject.GetComponent(typeof(Note));
						playerNote.play();
						StartCoroutine("scoring");
					}
					else
					{
						if (playerNote != null)
						{
							playerNote.stop();
							StopCoroutine("scoring");
						}
					}
				}
			}
			*/
			Note playerNote = (Note)col.gameObject.GetComponent(typeof(Note));
			if (!playerNote.isPlaying)
			{
				if (playerNotes.Count == 0 || playerNote != playerNotes[playerNotes.Count - 1])
				{
					playerNotes.Add(playerNote);
				}
				if (playerNote == playerNotes[0]) {
					playerNote.play();
					StartCoroutine("scoring");
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
				note.stop();
				StopCoroutine("scoring");
				playerNotes.RemoveAt(0);
				if (playerNotes.Count > 0) {
					playerNotes[0].play();
					StartCoroutine("scoring");
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
			GetComponent<Renderer>().material = mat;

		}
		else if (control == control2) {
			control2 = null;
			GetComponent<Renderer>().material = mat;

		}

		if (control1 == null && control2 == null)
		{
			GetComponent<Renderer>().material = inactiveMat;
			active = false;
			foreach (Note playerNote in playerNotes) {
				if (playerNote != null)
				{
					playerNote.stop();
					StopCoroutine("scoring");
				}
			}
			playerNotes.Clear();
		}
	}

	IEnumerator scoring() {
		while (true) {
			player.score += 1;
			yield return new WaitForFixedUpdate();
		}
	}
}