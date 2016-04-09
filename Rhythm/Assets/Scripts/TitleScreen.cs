using UnityEngine;
using System.Collections;

public class TitleScreen : MonoBehaviour {
	public BuildSong songBuilderPrefab;


	// Use this for initialization
	void Start () {
	
	}
	
	public void onExit() {
		Application.Quit();
	}

	public void onStart() {
		Instantiate(songBuilderPrefab);
		Destroy(gameObject);
	}
}
