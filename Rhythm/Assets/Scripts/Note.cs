using UnityEngine;
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
	public static float startDist = 30f;
	public bool isPlaying = false;
	private float speed;

	void Start() {
		rigid = GetComponent<Rigidbody>();
	}

	public void init(string _step, int _duration, AudioPlayer _player, int _alter = 0, int _octave = 0, bool _chord = false)
	{
		step = _step[0];
		alter = _alter;
		octave = _octave;
		duration = _duration;
		chord = _chord;
		player = _player;
		buildKey();
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
			transform.RotateAround(Vector3.zero, Vector3.forward, 30 * noteNumber);
			Vector3 norm = (Vector3.zero - transform.position).normalized;
			GetComponent<Rigidbody>().velocity = norm * speed;
		}
	}

	private void buildKey()
	{
		switch (getStep())
		{
			case 'A':
				if (getAlter() <= -1)
				{
					key = "G#";
					noteNumber = 8;
				}
				else if (getAlter() >= 1)
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
				if (getAlter() <= -1)
				{
					key = "A#";
					noteNumber = 10;
				}
				else if (getAlter() >= 1)
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
				if (getAlter() <= -1)
				{
					key = "B";
					noteNumber = 11;
				}
				else if (getAlter() >= 1)
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
				if (getAlter() <= -1)
				{
					key = "C#";
					noteNumber = 1;
				}
				else if (getAlter() >= 1)
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
				if (getAlter() <= -1)
				{
					key = "D#";
					noteNumber = 3;
				}
				else if (getAlter() >= 1)
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
				if (getAlter() <= -1)
				{
					key = "E";
					noteNumber = 4;
				}
				else if (getAlter() >= 1)
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
				if (getAlter() <= -1)
				{
					key = "F#";
					noteNumber = 6;
				}
				else if (getAlter() >= 1)
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
				key = "" + getStep();
				noteNumber = -1;
				break;
		}

		//get octave
		if (getOctave() < 1)
		{
			key += "1";
		}
		else if (getOctave() > 7)
		{
			key += "7";
		}
		else
		{
			key += "" + getOctave();
		}
		key += " (1)";
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