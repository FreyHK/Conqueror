using UnityEngine;
using System.Collections;

public class MouseManager : MonoBehaviour {

	public Transform unit;
	public Transform planet;

	public Vector3[] vertices;

	// Use this for initialization
	void Start () {
		//planet = GameObject.FindWithTag ("Planet").tra;

		vertices = planet.GetComponentInChildren<MeshFilter> ().mesh.vertices;
		for (int i = 0; i < vertices.Length; i++) {
			vertices [i] *= 100f;
		}


		unit.transform.position = vertices [Random.Range(0, vertices.Length)];
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonUp (0) || Input.GetTouch(0).phase == TouchPhase.Stationary) {

			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			RaycastHit hitInfo;

			if (Physics.Raycast (ray, out hitInfo)) {

				print("We hit " + hitInfo.collider.name);

				if (hitInfo.collider.transform.root.gameObject == planet.gameObject) {
					//We hit the planet
					print("We hit at: " + hitInfo.point + " the closest point is " + GetClosestOfVertices(hitInfo.point));

					unit.transform.position = GetClosestOfVertices(hitInfo.point);
					unit.transform.rotation = Quaternion.LookRotation (planet.position - unit.position);
				}
			}
		}
	}

	Vector3 GetClosestOfVertices(Vector3 point){

		float dist = Mathf.Infinity;
		Vector3 closest = Vector3.zero;

		foreach (Vector3 v in vertices) {

			float d = Vector3.Distance (v, point);
			if (d < dist) {
				closest = v;
				dist = d;
			}
		}

		return closest;
	}
}
