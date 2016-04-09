using UnityEngine;
using System.Collections;
using InControl;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	public GameObject leftCursorPrefab;
	public GameObject rightCursorPrefab;
	private GameObject leftCursor;
	private GameObject rightCursor;
	private GameObject leftCursorVisible;
	private GameObject rightCursorVisible;
	private InputDevice device;
	public BuildPolygon polygonBuilder;
	private float sideLength;
	private HitZone activeZoneLeft;
	private HitZone activeZoneRight;
	public Canvas songUi;
	public int score = 0;
	private Text scoreText;
	public BuildSong songBuilder;

	// Use this for initialization
	void Start () {
		leftCursor = Instantiate(leftCursorPrefab);
		rightCursor = Instantiate(rightCursorPrefab);
		leftCursor.GetComponent<MeshRenderer>().enabled = false;
		rightCursor.GetComponent<MeshRenderer>().enabled = false;
		leftCursorVisible = Instantiate(leftCursorPrefab);
		rightCursorVisible = Instantiate(rightCursorPrefab);

		leftCursorVisible.transform.SetParent(transform);
		leftCursor.transform.SetParent(transform);
		rightCursor.transform.SetParent(transform);
		rightCursorVisible.transform.SetParent(transform);

		rightCursorVisible.transform.position = new Vector3(0, 0, 1);


		sideLength = polygonBuilder.sideLength;
		scoreText = songUi.transform.FindChild("Score").GetComponent<Text>();
	}

	public void resume() {
		if (activeZoneLeft != null)
		{
			activeZoneLeft.resume();
		}
		if (activeZoneRight != null)
		{
			activeZoneRight.resume();
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		device = InputManager.ActiveDevice;

		if (Input.GetKeyDown(KeyCode.Escape) || device.Command.WasPressed) {
			songBuilder.pause();
			if (activeZoneLeft != null)
			{
				activeZoneLeft.pause();
			}
			if (activeZoneRight != null)
			{
				activeZoneRight.pause();
			}
		}

		if (device.LeftStick.Value != device.LeftStick.LastValue) {
			if (device.LeftStick.Value[0] == 0 && device.LeftStick.Value[1] == 0)
			{
				leftCursor.transform.position = Vector3.zero;
				leftCursorVisible.transform.position = Vector3.zero;

				if (activeZoneLeft != null) {
					activeZoneLeft.deactivate(device.LeftBumper, rightCursor.GetComponent<Renderer>().material);
					activeZoneLeft = null;
				}
			}
			else
			{
				float angle = Mathf.Atan2(device.LeftStick.Value[0], device.LeftStick.Value[1]);
				angle = -angle * 180 / Mathf.PI;
				leftCursor.transform.position = new Vector3(0, 3.8637f * sideLength / 2 + 1, 0);
				leftCursor.transform.RotateAround(Vector3.zero, Vector3.forward, angle);
				leftCursorVisible.transform.position = new Vector3(0, 3.8637f * sideLength / 2 - 0.75f, 0);
				leftCursorVisible.transform.RotateAround(Vector3.zero, Vector3.forward, angle);

				int layerMask = 1 << 8;
				RaycastHit hit;
				if (Physics.Linecast(leftCursor.transform.position, Vector3.zero, out hit, layerMask))
				{
					HitZone newZone = (HitZone)hit.transform.GetComponent(typeof(HitZone));
					if (activeZoneLeft != newZone) {
						if (activeZoneLeft != null) {
							activeZoneLeft.deactivate(device.LeftBumper, rightCursor.GetComponent<Renderer>().material);
						}
						newZone.activate(device.LeftBumper, leftCursor.GetComponent<Renderer>().material);
						activeZoneLeft = newZone;
					}
				}
			}
		}
		if (device.RightStick.Value != device.RightStick.LastValue)
		{
			if (device.RightStick.Value[0] == 0 && device.RightStick.Value[1] == 0)
			{
				rightCursor.transform.position = Vector3.zero;
				rightCursorVisible.transform.position = new Vector3(0, 0, 1);

				if (activeZoneRight != null)
				{
					activeZoneRight.deactivate(device.RightBumper, leftCursor.GetComponent<Renderer>().material);
					activeZoneRight = null;
				}
			}
			else
			{
				float angle = Mathf.Atan2(device.RightStick.Value[0], device.RightStick.Value[1]);
				angle = -angle * 180 / Mathf.PI;
				rightCursor.transform.position = new Vector3(0, 3.8637f * sideLength / 2 + 1, 0);
				rightCursor.transform.RotateAround(Vector3.zero, Vector3.forward, angle);
				rightCursorVisible.transform.position = new Vector3(0, 3.8637f * sideLength / 2 - 0.75f, 1);
				rightCursorVisible.transform.RotateAround(Vector3.zero, Vector3.forward, angle);

				int layerMask = 1 << 8;
				RaycastHit hit;
				if (Physics.Linecast(rightCursor.transform.position, Vector3.zero, out hit, layerMask))
				{
					HitZone newZone = (HitZone)hit.transform.GetComponent(typeof(HitZone));
					if (activeZoneRight != newZone)
					{
						if (activeZoneRight != null)
						{
							activeZoneRight.deactivate(device.RightBumper, leftCursor.GetComponent<Renderer>().material);
						}
						newZone.activate(device.RightBumper, rightCursor.GetComponent<Renderer>().material);
						activeZoneRight = newZone;
					}
				}
			}
		}
		scoreText.text = "" + score;
	}

}
