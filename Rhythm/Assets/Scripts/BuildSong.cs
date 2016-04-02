using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class BuildSong : MonoBehaviour {
	public List<TextAsset> sheets;
	public List<int> bpms;
	private int bpm;
	private TextAsset sheet;
	private XmlDocument xmlDoc;
	public Canvas choosePart;
	private Transform buttonGrid;
	public GameObject layoutButtonPrefab;
	public GameObject songPrefab;
	public Canvas songUiPrefab;
	public BuildPolygon polygonBuilder;
	public Player playerPrefab;
	public GameObject audioPlayerPrefab;
	private List<string> partList = new List<string>();
	public Canvas chooseSong;

	// Use this for initialization
	void Start () {
		choosePart.enabled = false;
		chooseSong.enabled = true;
		buttonGrid = choosePart.transform.FindChild("ButtonGrid");
	}
	

	public void selectSong(int i) {
		chooseSong.enabled = false;
		sheet = sheets[i];
		bpm = bpms[i];
		getParts();
	}

	private void getParts() {
		xmlDoc = new XmlDocument();
		xmlDoc.XmlResolver = null;
		xmlDoc.LoadXml(sheet.text);
		XmlNodeList scoreParts = xmlDoc.GetElementsByTagName("score-part");
		foreach (XmlNode scorePart in scoreParts) {
			string id = scorePart.Attributes["id"].InnerText;
			string name = scorePart.SelectSingleNode("part-name").InnerText;
			partList.Add(id);

			GameObject layoutButton = Instantiate(layoutButtonPrefab);
			layoutButton.transform.SetParent(buttonGrid);
			layoutButton.transform.GetChild(0).GetComponent<Text>().text = name;
			layoutButton.GetComponent<Button>().onClick.AddListener(() => readMusic(id));
		}
		choosePart.enabled = true;
	}

	private void readMusic(string playerId) {
		//eventually need to check which type of xml doc this is
		choosePart.enabled = false;
		List<Song> songList = new List<Song>();
		foreach (string id in partList)
		{
			XmlNode part = xmlDoc.SelectSingleNode("//part[@id='" + id + "']");
			XmlNode attributes;
			if (part.SelectSingleNode("measure[@number='0']/attributes") != null)
			{
				attributes = part.SelectSingleNode("measure[@number='0']/attributes");
			}
			else {
				attributes = part.SelectSingleNode("measure[@number='1']/attributes");
			}

			int divisions = 0;
			if (!int.TryParse(attributes.SelectSingleNode("divisions").InnerText, out divisions))
			{
				print("divisions select failed");
			}

			int beatType = 4;
			if (attributes.SelectSingleNode("time") != null)
			{
				if (!int.TryParse(attributes.SelectSingleNode("time/beat-type").InnerText, out beatType))
				{
					print("beatType select failed");
				}
			}

			GameObject songObject = Instantiate(songPrefab);
			Song song = (Song)songObject.GetComponent(typeof(Song));
			//check if playerPart
			GameObject audioPlayerObject = Instantiate(audioPlayerPrefab);
			AudioPlayer audioPlayer = (AudioPlayer)audioPlayerObject.GetComponent(typeof(AudioPlayer));
			if (id == playerId) {
				audioPlayer.volume = 0.5f;
				song.init(bpm, divisions, beatType, true);
			}
			else {
				song.init(bpm, divisions, beatType, false);
			}
			audioPlayer.assignKeys();

			foreach (XmlNode note in part.SelectNodes("measure/note"))
			{
				if(note.SelectSingleNode("unpitched") != null) {
					continue;
				}

				if (note.SelectSingleNode("rest") != null)
				{
					string step = "R";
					int duration = 0;
					if (note.SelectSingleNode("duration") == null)
					{
						continue;
					}
					if (!int.TryParse(note.SelectSingleNode("duration").InnerText, out duration))
					{
						print("get duration failed");
					}
					if (duration != 0)
					{
						song.addNote(step, duration, audioPlayer);
					}
				}

				else
				{
					if (note.SelectSingleNode("pitch/step") == null) {
						continue;
					}
					string step = note.SelectSingleNode("pitch/step").InnerText;
					int alter = 0;
					if (note.SelectSingleNode("pitch/alter") != null)
					{
						if (!int.TryParse(note.SelectSingleNode("pitch/alter").InnerText, out alter))
						{
							print("get alter failed");
						}
					}
					int duration = 0;
					if (note.SelectSingleNode("duration") == null)
					{
						continue;
					}
					if (!int.TryParse(note.SelectSingleNode("duration").InnerText, out duration))
					{
						print("get duration failed");
					}
					int octave = 0;
					if (note.SelectSingleNode("pitch/octave") == null)
					{
						continue;
					}
					if (!int.TryParse(note.SelectSingleNode("pitch/octave").InnerText, out octave))
					{
						print("get octave failed");
					}
					bool chord = note.SelectSingleNode("chord") != null;
					if (duration != 0)
					{
						song.addNote(step, duration, audioPlayer, alter, octave, chord);
					}
				}

			}
			songList.Add(song);
		}

		startSong(songList);
	}

	void startSong(List<Song> songList) {
		Canvas songUi = Instantiate(songUiPrefab);
		Player player = Instantiate(playerPrefab);
		polygonBuilder.player = player;
		polygonBuilder.build();
		player.songUi = songUi;
		player.polygonBuilder = polygonBuilder;
		foreach (Song song in songList) {
			StartCoroutine(playPart(song));
		}
	}

	IEnumerator playPart(Song song) {
		song.playSong();
		yield return null;
	}
}
