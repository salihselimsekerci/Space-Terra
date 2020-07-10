using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour {

	/// <summary>
	/// We need to show a message after each successful body detach. This status controller receives the ID
	/// and displays a message accordingly.
	/// </summary>

	internal int statusID;
	private string statusText;
	public GameObject myLabel;

	void Start () {

		switch (statusID) {
		case 0:
			statusText = "Ok";
			break;
		case 1:
			statusText = "Good";
			break;
		case 2:
			statusText = "Great";
			break;
		case 3:
			statusText = "Awesome";
			break;
		case 4:
			statusText = "Perfect";
			break;
		}

		myLabel.GetComponent<TextMesh> ().text = statusText;

		Destroy (gameObject, 1.5f);
	}

}
