using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlanetController : MonoBehaviour {

	/// <summary>
	/// When we upgrade our rocket, eventually we can reach new heights. 
	/// If we pass a certain height, we can land on a new planet. This class is the controller of the new planet,
	/// handling its movement, position and other parameters.
	/// </summary>

	public static int minimumHeightToLand = 300;		//our spaceship needs to pass this height to be able to land on the this planet
	private int yPositionOffset = 40;					//new planet needs to be a little lower than the "minimumHeightToLand" value 
	private int xPositionOffset;						//we need to slightly adjust x position of planet in order to land safely
	private int planetID;

	void Awake () {

		planetID = PlayerPrefs.GetInt("StartingPlanetID");

		if (planetID == 0) {
			minimumHeightToLand = 300;
			xPositionOffset = 2;
		}

		if (planetID == 1) {
			minimumHeightToLand = 650;
			xPositionOffset = 7;
		}

	}

	void Start () {
		transform.position = new Vector3 (	transform.position.x + xPositionOffset + (minimumHeightToLand / 50), 
											minimumHeightToLand - yPositionOffset, 
											transform.position.z);
	}

}
