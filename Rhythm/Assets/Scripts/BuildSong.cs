using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildSong : MonoBehaviour {
	public List<TextAsset> sheets;
	public List<int> bpms;
	private int bpm;
	private TextAsset sheet;
	private XmlDocument xmlDoc;
	private Canvas choosePart;
	public Canvas choosePartPrefab;
	private Transform buttonGrid;
	public GameObject layoutButtonPrefab;
	public GameObject songPrefab;
	public Canvas songUiPrefab;
	private BuildPolygon polygonBuilder;
	public Player playerPrefab;
	public GameObject audioPlayerPrefab;
	private List<string> partList = new List<string>();
	private Canvas chooseSong;
	public Canvas chooseSongPrefab;
	public BuildPolygon polygonBuilderPrefab;
	public GameObject titleScreenPrefab;
	public GameObject instructionsPrefab;
	private GameObject instructions;
	private List<Song> songList = new List<Song>();
	private Player player;
	private Canvas songUi;
	private GameObject pauseMenu;
	public GameObject pauseMenuPrefab;
	private float prevFixedDeltaTime;
	private string playerPartId;
	private List<GameObject> audioPlayers = new List<GameObject>();


	// Use this for initialization
	void Start () {
		chooseSong = Instantiate(chooseSongPrefab);
		int i = 0;
		foreach (Transform child in chooseSong.transform.FindChild("Songs").transform) {
			int index = i;
			child.GetComponent<Button>().onClick.AddListener(() => selectSong(index));
			i++;
		}
		EventSystem.current.SetSelectedGameObject(chooseSong.transform.FindChild("Songs").transform.GetChild(0).gameObject, null);
		chooseSong.transform.FindChild("Back").GetComponent<Button>().onClick.AddListener(() => backSong());
		chooseSong.enabled = true;
	}
	

	public void selectSong(int i) {
		Destroy(chooseSong.gameObject);
		choosePart = Instantiate(choosePartPrefab);
		buttonGrid = choosePart.transform.FindChild("ButtonGrid");
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
			layoutButton.GetComponent<Text>().text = name;
			layoutButton.GetComponent<Button>().onClick.AddListener(() => readMusic(id));
		}
		choosePart.enabled = true;
		EventSystem.current.SetSelectedGameObject(buttonGrid.transform.GetChild(0).gameObject, null);
		choosePart.transform.FindChild("Back").GetComponent<Button>().onClick.AddListener(() => backPart());

	}

	private void readMusic(string playerId) {
		//eventually need to check which type of xml doc this is

		if (choosePart != null)
		{
			Destroy(choosePart.gameObject);
		}
		instructions = Instantiate(instructionsPrefab);
		playerPartId = playerId;


		songList = new List<Song>();
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
			audioPlayers.Add(audioPlayerObject);
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
		instructions.transform.FindChild("Continue").GetComponent<Button>().onClick.AddListener(() => cont());
		EventSystem.current.SetSelectedGameObject(instructions.transform.FindChild("Continue").gameObject, null);
	}

	void startSong(List<Song> songList) {
		songUi = Instantiate(songUiPrefab);
		player = Instantiate(playerPrefab);
		polygonBuilder = Instantiate(polygonBuilderPrefab);
		player.songBuilder = this;
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

	void backSong() {
		if (polygonBuilder != null) {
			Destroy(polygonBuilder.gameObject);
		}
		if (choosePart != null) {
			Destroy(polygonBuilder.gameObject);
		}
		if (chooseSong != null) {
			Destroy(chooseSong.gameObject);
		}
		GameObject titleScreen = Instantiate(titleScreenPrefab);
		EventSystem.current.SetSelectedGameObject(titleScreen.transform.FindChild("Start").gameObject, null);
		Destroy(gameObject);
	}

	void backPart() {
		partList.Clear();
		if (choosePart != null)
		{
			Destroy(choosePart.gameObject);
		}
		chooseSong = Instantiate(chooseSongPrefab);
		int i = 0;
		foreach (Transform child in chooseSong.transform.FindChild("Songs").transform)
		{
			int index = i;
			child.GetComponent<Button>().onClick.AddListener(() => selectSong(index));
			i++;
		}
		EventSystem.current.SetSelectedGameObject(chooseSong.transform.FindChild("Songs").transform.GetChild(0).gameObject, null);
		chooseSong.transform.FindChild("Back").GetComponent<Button>().onClick.AddListener(() => backSong());
	}

	void cont()
	{
		Destroy(instructions.gameObject);
		startSong(songList);
	}

	void quitSong() {
		clean();
		backPart();
	}

	public void pause() {
		if (pauseMenu == null)
		{

			Time.timeScale = 0;
			prevFixedDeltaTime = Time.fixedDeltaTime;
			Time.fixedDeltaTime = 0;

			foreach (Song song in songList)
			{
				song.pause();
			}
			foreach (GameObject obj in audioPlayers)
			{
				AudioPlayer audioPlayer = (AudioPlayer)obj.GetComponent(typeof(AudioPlayer));
				audioPlayer.pauseAll();
			}

			pauseMenu = Instantiate(pauseMenuPrefab);
			EventSystem.current.SetSelectedGameObject(pauseMenu.transform.FindChild("Resume").gameObject, null);
			pauseMenu.transform.FindChild("Resume").GetComponent<Button>().onClick.AddListener(() => resume());
			pauseMenu.transform.FindChild("Quit").GetComponent<Button>().onClick.AddListener(() => quitSong());
			pauseMenu.transform.FindChild("Restart").GetComponent<Button>().onClick.AddListener(() => restart());
		}
	}

	void resume()
	{
		Destroy(pauseMenu);
		Time.timeScale = 1;
		Time.fixedDeltaTime = prevFixedDeltaTime;
		player.resume();
	}

	void restart() {
		clean();
		readMusic(playerPartId);
	}

	void clean() {
		foreach (Song song in songList)
		{
			song.destroy();
		}
		songList.Clear();
		foreach (GameObject audioPlayer in audioPlayers) {
			Destroy(audioPlayer.gameObject);
		}
		audioPlayers.Clear();
		Destroy(polygonBuilder.gameObject);
		Destroy(player.gameObject);
		Destroy(songUi.gameObject);
		Destroy(pauseMenu.gameObject);
		Time.timeScale = 1;
		Time.fixedDeltaTime = prevFixedDeltaTime;
	}
}
