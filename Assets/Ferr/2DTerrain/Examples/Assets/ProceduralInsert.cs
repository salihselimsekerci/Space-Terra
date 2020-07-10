using UnityEngine;
using System.Collections;

public class ProceduralInsert : MonoBehaviour {
	public Ferr2DT_PathTerrain terrain;

	void Update () {
		if (Input.GetButtonDown("Fire1")) {
			Plane p    = new Plane(new Vector3(0, 0, 1), Vector3.zero);
			Ray   r    = Camera.main.ScreenPointToRay(Input.mousePosition);
			float dist = 0;

			if (p.Raycast(r, out dist)) {
				Vector3 intersection = r.GetPoint(dist) - terrain.transform.position;
				terrain.AddAutoPoint(new Vector2(intersection.x, intersection.y));
				terrain.Build(false);
				terrain.RecreateCollider();
			}
		}
	}
}
