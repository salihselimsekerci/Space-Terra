using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstronautManager : MonoBehaviour {

	/// <summary>
	/// Astronaut manager simply moves the astronaut prefabs to their designated destinations upon instantiation.
	/// </summary>

	public int dest = 1;		//1 = move towards spaceship
								//2 = move to the new planet

	private Vector3 startingPos;
	private float randDestX;
	private float randDestY;


	void Start () {
		startingPos = transform.position;
		randDestX = Random.Range (-2.5f, 2.5f);
		randDestY = Random.Range (0, -3.5f);
		StartCoroutine(moveToDest (dest));
	}

	IEnumerator moveToDest(int destID) {

		float t = 0;
		while (t < 1) {
			t += Time.deltaTime * 1;

			if (destID == 1) {
				transform.position = new Vector3 (Mathf.Lerp (startingPos.x, -0.25f, t), startingPos.y, startingPos.z);
				if (t >= 1)
					Destroy (gameObject);
			}

			if (destID == 2) {
				transform.position = new Vector3 (	Mathf.Lerp (startingPos.x, startingPos.x + randDestX, t), 
													Mathf.Lerp (startingPos.y, startingPos.y + randDestY, t), 
													startingPos.z);
			}

			yield return 0;
		}

	}

}
