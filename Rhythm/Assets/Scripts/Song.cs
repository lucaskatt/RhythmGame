using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Song : MonoBehaviour {
	private List<Note> song = new List<Note>();
	private float bps;
	private int divisions;
	private int beatType;
	private int nextNote = 0;
	public float noteSpeed = 3;
	public BuildPolygon polygonBuilder;
	bool playerPart = true;
	private float beatFactor;
	private float noteDelay;

	public void init(int _bpm, int _divisions, int _beatType, bool _playerPart) {
		bps = _bpm / 60f;
		divisions = _divisions;
		beatType = _beatType;
		nextNote = 0;
		playerPart = _playerPart;
		noteDelay = (Note.startDist - (3.8637f * polygonBuilder.sideLength / 2) - polygonBuilder.hitSizeY / 2) / noteSpeed;
    song = new List<Note>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void addNote(Note note) {
		song.Add(note);
	}

	public void playSong() {
		if (nextNote < song.Count) {
			beatFactor = beatType / (4 * divisions * bps);

			StartCoroutine(playNote(song[nextNote]));
		}
	}

	IEnumerator playNote(Note note) {
		++nextNote;
		//check for rests
		//start playing note
		if (playerPart)
		{
			note.startMovement(noteSpeed, polygonBuilder.hitSizeY, playTime(note));
		}
		else {
			StartCoroutine(playNoteDelayed(note, noteDelay));
		}
		//check for chords
		if (nextNote < song.Count && song[nextNote].isChord())
		{
			StartCoroutine(playNoteSingle(song[nextNote]));
			++nextNote;
			while (song[nextNote].isChord() && nextNote < song.Count)
			{
				++nextNote;
			}
		}

		yield return new WaitForSeconds(playTime(note));

		if (nextNote < song.Count)
		{
			yield return StartCoroutine(playNote(song[nextNote]));
		}
		else {
			yield return null;
		}
	}

	IEnumerator playNoteSingle(Note note) {
		//start playing note
		if (playerPart)
		{
			note.startMovement(noteSpeed, polygonBuilder.hitSizeY, playTime(note));
		}
		else
		{
			StartCoroutine(playNoteDelayed(note, noteDelay));
		}
		yield return null;
	}

	IEnumerator playNoteDelayed(Note note, float delay) {
		yield return new WaitForSeconds(delay);
		note.play();
		yield return new WaitForSeconds(playTime(note));
		note.stop();
	}

	private float playTime(Note note) {
		return note.getDuration() * beatFactor;
	}
}
