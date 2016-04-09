using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using InControl;


public class Menu : MonoBehaviour {
	private InputDevice device;
	public Button back;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		device = InputManager.ActiveDevice;

		if (device.Action2.WasPressed) {
			back.onClick.Invoke();
		}
	}
}
