using UnityEngine;
using System.Collections;

public class DrawCircle : MonoBehaviour {

	public float thetaScale = 0.01f;
	private int size;
	private LineRenderer lineRenderer;
	private float theta = 0f;

	void Start()
	{

	}

	public void draw(float radius)
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
		theta = 0f;
		size = (int)((1f / thetaScale) + 1f);
		lineRenderer.SetVertexCount(size);
		for (int i = 0; i < size; i++)
		{
			theta += (2.0f * Mathf.PI * thetaScale);
			float x = radius * Mathf.Cos(theta);
			float y = radius * Mathf.Sin(theta);
			lineRenderer.SetPosition(i, new Vector3(x, y, 0));
		}
	}
}
