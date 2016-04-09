using UnityEngine;
using System.Collections;

public class BuildPolygon : MonoBehaviour {
	public float sideLength = 1;
	public GameObject sidePrefab;
	public float hitSizeY = 0.5f;
	public Player player;
	public GameObject circleRendererPrefab;
	public int numCircles = 3;
	public float separation = 1f;
	private float startRadius = 2f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void build() {
		for(int i = 0; i < 12; ++i) {
			GameObject side = Instantiate(sidePrefab);
			side.transform.SetParent(transform);
			HitZone hitZone = (HitZone) side.GetComponent(typeof(HitZone));
			hitZone.player = player;
			side.transform.localScale = new Vector3(sideLength, hitSizeY, 0.1f);
			side.transform.position = new Vector3(0, 3.8637f * sideLength / 2, 0);
			side.transform.RotateAround(Vector3.zero, Vector3.forward, 30f * i);
		}

		
		for (int i = 0; i < numCircles; ++i) {
			GameObject circleRenderer = Instantiate(circleRendererPrefab);
			circleRenderer.transform.SetParent(transform);
			DrawCircle drawCircle = (DrawCircle)circleRenderer.GetComponent(typeof(DrawCircle));
			drawCircle.draw(startRadius + (i+1) * separation);
    }
		
	}
}
