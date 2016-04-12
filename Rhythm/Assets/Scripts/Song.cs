using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Song : MonoBehaviour {
	private List<NoteStruct> song = new List<NoteStruct>();
	private float bps;
	private int divisions;
	private int beatType;
	private int nextNote = 0;
	public float noteSpeed = 3;
	public BuildPolygon polygonBuilder;
	bool playerPart = true;
	private float beatFactor;
	private float noteDelay;
	public int poolSize = 20;
	private List<Note> pool = new List<Note>();
	private int poolPos = 0;
	public GameObject notePrefab;
	private bool paused = false;
	public bool test = false;
	private List<AudioSource> stems = new List<AudioSource>();
	public float stemDelay = 2f;

	public struct NoteStruct{
		public char step;
		public int duration;
		public AudioPlayer player;
		public int alter;
		public int octave;
		public bool chord;
		public string key;
		public int noteNumber;

		public NoteStruct(string _step, int _duration, AudioPlayer _player, int _alter = 0, int _octave = 0, bool _chord = false)
		{
			step = _step[0];
			alter = _alter;
			octave = _octave;
			duration = _duration;
			chord = _chord;
			player = _player;
			key = "";
			noteNumber = 0;
			buildKey();
		}

		private void buildKey()
		{
			switch (step)
			{
				case 'A':
					if (alter <= -1)
					{
						key = "G#";
						noteNumber = 8;
					}
					else if (alter >= 1)
					{
						key = "A#";
						noteNumber = 10;
					}
					else
					{
						key = "A";
						noteNumber = 9;
					}
					break;
				case 'B':
					if (alter <= -1)
					{
						key = "A#";
						noteNumber = 10;
					}
					else if (alter >= 1)
					{
						key = "C";
						noteNumber = 0;
					}
					else
					{
						key = "B";
						noteNumber = 11;
					}
					break;
				case 'C':
					if (alter <= -1)
					{
						key = "B";
						noteNumber = 11;
					}
					else if (alter >= 1)
					{
						key = "C#";
						noteNumber = 1;
					}
					else
					{
						key = "C";
						noteNumber = 0;
					}
					break;
				case 'D':
					if (alter <= -1)
					{
						key = "C#";
						noteNumber = 1;
					}
					else if (alter >= 1)
					{
						key = "D#";
						noteNumber = 3;
					}
					else
					{
						key = "D";
						noteNumber = 2;
					}
					break;
				case 'E':
					if (alter <= -1)
					{
						key = "D#";
						noteNumber = 3;
					}
					else if (alter >= 1)
					{
						key = "F";
						noteNumber = 5;
					}
					else
					{
						key = "E";
						noteNumber = 4;
					}
					break;
				case 'F':
					if (alter <= -1)
					{
						key = "E";
						noteNumber = 4;
					}
					else if (alter >= 1)
					{
						key = "F#";
						noteNumber = 6;
					}
					else
					{
						key = "F";
						noteNumber = 5;
					}
					break;
				case 'G':
					if (alter <= -1)
					{
						key = "F#";
						noteNumber = 6;
					}
					else if (alter >= 1)
					{
						key = "G#";
						noteNumber = 8;
					}
					else
					{
						key = "G";
						noteNumber = 7;
					}
					break;
				default:
					key = "" + step;
					noteNumber = -1;
					break;
			}

			//get octave
			if (octave < 1)
			{
				key += "1";
			}
			else if (octave > 7)
			{
				key += "7";
			}
			else
			{
				key += "" + octave;
			}
			key += " (1)";
		}

		public void play(float playTime) {
			player.startNoteStructAudio(this, playTime);
		}

		public void stop()
		{
			player.stopNoteStructAudio(this);
		}

	};

	public void init(int _bpm, int _divisions, int _beatType, bool _playerPart) {
		bps = _bpm / 60f;
		divisions = _divisions;
		beatType = _beatType;
		nextNote = 0;
		playerPart = _playerPart;
		noteDelay = (Note.startDist - (3.8637f * polygonBuilder.sideLength / 2) - polygonBuilder.hitSizeY / 2) / noteSpeed;
		if (playerPart)
		{
			song = new List<NoteStruct>();

			for (int i = 0; i < poolSize; i++)
			{
				GameObject noteObject = Instantiate(notePrefab, new Vector3(100, 100), Quaternion.identity) as GameObject;
				Note note = (Note)noteObject.GetComponent(typeof(Note));
				pool.Add(note);
			}
		}
		if (test) {
			stems.Add(GameObject.Find("DrumsStem").GetComponent<AudioSource>());
			stems.Add(GameObject.Find("GuitarStem").GetComponent<AudioSource>());
			stems.Add(GameObject.Find("KeysStem").GetComponent<AudioSource>());
			stems.Add(GameObject.Find("RhythmStem").GetComponent<AudioSource>());
			stems.Add(GameObject.Find("SongStem").GetComponent<AudioSource>());
			stems.Add(GameObject.Find("VocalsStem").GetComponent<AudioSource>());
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void addNote(string step, int duration, AudioPlayer audioPlayer, int alter=0, int octave=0, bool chord=false) {
		song.Add(new NoteStruct(step, duration, audioPlayer, alter, octave, chord));
	}

	public void playSong() {
		if (nextNote < song.Count) {
			beatFactor = beatType / (4 * divisions * bps);

			if (test) {
				StartCoroutine(playStems());
				StartCoroutine(delayStart());
			}

			else{
				StartCoroutine(playNote(song[nextNote]));
			}
		}
	}

	IEnumerator delayStart() {
		yield return new WaitForSeconds(stemDelay);
		StartCoroutine(playNote(song[nextNote]));
		yield return null;
	}

	IEnumerator playNote(NoteStruct noteStruct) {
		++nextNote;
		//start playing note
		Note note = null;
		if (playerPart)
		{
			note = pool[poolPos];
			poolPos++;
			if (poolPos >= poolSize) {
				poolPos = 0;
			}
			note.init(noteStruct);
			note.startMovement(noteSpeed, polygonBuilder.hitSizeY, playTimeNote(note));
		}
		else {
			StartCoroutine(playNoteDelayed(noteStruct, noteDelay));
		}
		//check for chords
		if (nextNote < song.Count && song[nextNote].chord)
		{
			if (note != null)
			{
				note.chordColor();
			}
			StartCoroutine(playNoteSingle(song[nextNote]));
			++nextNote;
			while (song[nextNote].chord && nextNote < song.Count)
			{
				++nextNote;
			}
		}

		yield return new WaitForSeconds(playTimeNoteStruct(noteStruct));

		if (nextNote < song.Count)
		{
			yield return StartCoroutine(playNote(song[nextNote]));
		}
		else {
			yield return null;
		}
	}

	IEnumerator playNoteSingle(NoteStruct noteStruct) {
		//start playing note
		if (playerPart)
		{
			Note note = pool[poolPos];
			poolPos++;
			if (poolPos >= poolSize)
			{
				poolPos = 0;
			}
			note.init(noteStruct);
			note.startMovement(noteSpeed, polygonBuilder.hitSizeY, playTimeNote(note));
			note.chordColor();
		}
		else
		{
			StartCoroutine(playNoteDelayed(noteStruct, noteDelay));
		}
		yield return null;
	}

	IEnumerator playNoteDelayed(NoteStruct noteStruct, float delay) {
		if (!test)
		{
			yield return new WaitForSeconds(delay);
			float playTime = playTimeNoteStruct(noteStruct);
			noteStruct.play(playTime);
			yield return new WaitForSeconds(playTime);
			noteStruct.stop();
		}
	}

	IEnumerator playStems() {
		yield return new WaitForSeconds(noteDelay);
		foreach (AudioSource stem in stems) {
			stem.Play();
		}
		yield return null;
	}

	private float playTimeNote(Note note) {
		return note.getDuration() * beatFactor;
	}

	private float playTimeNoteStruct(NoteStruct noteStruct) {
		return noteStruct.duration * beatFactor;
	}

	public void destroy() {
		foreach(Note note in pool) {
			note.stop();
			Destroy(note.gameObject);
		}
		Destroy(gameObject);
	}

	public void pause() {
		paused = true;
		foreach (Note note in pool)
		{
			note.stop();
		}
	}
}
