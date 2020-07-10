using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketHolderController : MonoBehaviour {

	public GameObject[] holderPoles;
	private bool closePoles;

	void Start () {
		closePoles = false;
	}
	
	void Update () {

		if (CounterController.initCounter && !closePoles)
			StartCoroutine(closeHolderPoles ());
	}


	IEnumerator closeHolderPoles() {

		//run once
		closePoles = true;

		float t = 0;
		while(t < 1) {

			t += Time.deltaTime * 0.5f;
			for (int i = 0; i < holderPoles.Length; i++) {
				holderPoles [i].transform.rotation = Quaternion.Euler (0, 0, Mathf.Lerp (0, 80, t));
			}

			yield return 0;
		}
	}

}
