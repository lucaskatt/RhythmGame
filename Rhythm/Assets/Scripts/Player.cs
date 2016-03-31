using UnityEngine;
using System.Collections;
using InControl;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	public GameObject cursorPrefab;
	private GameObject leftCursor;
	private GameObject rightCursor;
	private InputDevice device;
	public BuildPolygon polygonBuilder;
	private float sideLength;
	private HitZone activeZoneLeft;
	private HitZone activeZoneRight;
	public Canvas songUi;
	public int score = 0;
	private Text scoreText;

	// Use this for initialization
	void Start () {
		leftCursor = Instantiate(cursorPrefab);
		rightCursor = Instantiate(cursorPrefab);
		sideLength = polygonBuilder.sideLength;
		scoreText = songUi.transform.FindChild("Score").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		device = InputManager.ActiveDevice;

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}

		if (device.LeftStick.Value != device.LeftStick.LastValue) {
			if (device.LeftStick.Value[0] == 0 && device.LeftStick.Value[1] == 0)
			{
				leftCursor.transform.position = Vector3.zero;
				if (activeZoneLeft != null) {
					activeZoneLeft.deactivate(device.LeftBumper);
					activeZoneLeft = null;
				}
			}
			else
			{
				float angle = Mathf.Atan2(device.LeftStick.Value[0], device.LeftStick.Value[1]);
				angle = -angle * 180 / Mathf.PI;
				leftCursor.transform.position = new Vector3(0, 3.8637f * sideLength / 2 + 1, 0);
				leftCursor.transform.RotateAround(Vector3.zero, Vector3.forward, angle);

				int layerMask = 1 << 8;
				RaycastHit hit;
				if (Physics.Linecast(leftCursor.transform.position, Vector3.zero, out hit, layerMask))
				{
					HitZone newZone = (HitZone)hit.transform.GetComponent(typeof(HitZone));
					if (activeZoneLeft != newZone) {
						if (activeZoneLeft != null) {
							activeZoneLeft.deactivate(device.LeftBumper);
						}
						newZone.activate(device.LeftBumper);
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
				if (activeZoneRight != null)
				{
					activeZoneRight.deactivate(device.RightBumper);
					activeZoneRight = null;
				}
			}
			else
			{
				float angle = Mathf.Atan2(device.RightStick.Value[0], device.RightStick.Value[1]);
				angle = -angle * 180 / Mathf.PI;
				rightCursor.transform.position = new Vector3(0, 3.8637f * sideLength / 2 + 1, 0);
				rightCursor.transform.RotateAround(Vector3.zero, Vector3.forward, angle);

				int layerMask = 1 << 8;
				RaycastHit hit;
				if (Physics.Linecast(rightCursor.transform.position, Vector3.zero, out hit, layerMask))
				{
					HitZone newZone = (HitZone)hit.transform.GetComponent(typeof(HitZone));
					if (activeZoneRight != newZone)
					{
						if (activeZoneRight != null)
						{
							activeZoneRight.deactivate(device.RightBumper);
						}
						newZone.activate(device.RightBumper);
						activeZoneRight = newZone;
					}
				}
			}
		}
		scoreText.text = "" + score;
	}

}
