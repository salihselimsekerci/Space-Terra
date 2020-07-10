using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickDestroy : MonoBehaviour {

	void Start () {
		Destroy (gameObject, 1.25f);
	}

}
