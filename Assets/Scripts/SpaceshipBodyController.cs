using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipBodyController : MonoBehaviour {

	/// <summary>
	/// Each body part has its own controller. Please remember that we need to carefully set each part's position
	/// inside the main rocket and then add it to bodypart array of the rocket to make it work. We also need to 
	/// add the parts in a correct order -> index 0 is the bottom part and with last index is the top part. look at the
	/// structure of on spaceship prefab to find out more about this setup.
	/// </summary>

	public AudioClip detachSfx;
	public bool isActive;				//state flag

	public GameObject fuelBar;			//the green (safe) fuel bar
	public GameObject mainBody;			//the body which decrease in scale when rocket is flying
	public GameObject exhaust;			//fire nozzle position
	public GameObject perfectFx;		//particle system used for perfect disengages

	public GameObject statusPrefab;		//text status prefab which appears on screen when detach happens

	private GameObject mainSpaceship;
	private Vector3 startingScale;
	private float burnSpeed = 0.2f;
	private bool canBurn;
	private bool isDisengaged;

	private bool canTap;


	/// <summary>
	/// Init
	/// </summary>
	void Awake () {

		mainSpaceship = GameObject.FindGameObjectWithTag ("Player");
		canBurn = true;
		isDisengaged = false;
		canTap = true;
		startingScale = mainBody.transform.localScale;

	}


	/// <summary>
	/// FSM
	/// </summary>
	void Update () {

		if (GameController.isGameOver || !GameController.isGameStarted) {

			fuelBar.SetActive (false);
			exhaust.SetActive (false);

			return;
		}

		//Only show fuel bar when this part is active
		if (!isActive) {
			fuelBar.SetActive (false);
			exhaust.SetActive (false);
			return;
		}

		//change size to simulate fuel burning
		StartCoroutine(burn());

		//detach upon player touch/click
		if (Input.GetMouseButtonUp (0) && canTap && !isDisengaged) {
			StartCoroutine(disengage ());
		}
	}


	/// <summary>
	/// Burn this body part just once
	/// </summary>
	IEnumerator burn() {

		if (!canBurn)
			yield break;

		canBurn = false;
		fuelBar.SetActive (true);
		exhaust.SetActive (true);

		float t = 0;
		while (t <= 1 && !isDisengaged) {
			t += Time.deltaTime * ( burnSpeed + (SpaceshipController.activePartId * 0.1f) );
			mainBody.transform.localScale = new Vector3(startingScale.x, 
														Mathf.SmoothStep (startingScale.y, 0, t), 
														startingScale.z);

			//check for fuel miss-use and explosion
			if (mainBody.transform.localScale.y <= 0.01f) {
				//game over
				print ("Rocket exploded! Game over...");
				SpaceshipController.rocketExploded = true;
				GameController.isGameOver = true;
			}

			yield return 0;
		}
	}
		

	/// <summary>
	/// Disengage this bodypart.
	/// </summary>
	IEnumerator disengage() {

		print ("Disengage body...");

		playSfx (detachSfx);

		canTap = false;
		canBurn = false;
		isDisengaged = true;
		isActive = false;

		//detach this part
		transform.parent = null;
		gameObject.AddComponent<Rigidbody> ();
		gameObject.GetComponent<Rigidbody> ().drag = 0.5f;
		gameObject.GetComponent<Rigidbody> ().angularDrag = 0.5f;
		gameObject.GetComponent<Rigidbody> ().AddTorque (new Vector3 (0, 0, 5), ForceMode.Impulse);

		GameObject statusLabel;
		statusLabel = Instantiate (statusPrefab, transform.position + new Vector3(-2,0,0), Quaternion.Euler (0, 180, 0)) as GameObject;
		statusLabel.transform.parent = Camera.main.transform;

		//set bonus power
		float bonusPower = 0;
		if (mainBody.transform.localScale.y <= 1 && mainBody.transform.localScale.y > 0.5f) {
			
			statusLabel.GetComponent<StatusController> ().statusID = 0;
			bonusPower = 0;

		} else if (mainBody.transform.localScale.y <= 0.5f && mainBody.transform.localScale.y > 0.15f) {
			
			statusLabel.GetComponent<StatusController> ().statusID = 1;
			bonusPower = 3;
			
		} else if (mainBody.transform.localScale.y <= 0.15f && mainBody.transform.localScale.y > 0.09f) {
			
			statusLabel.GetComponent<StatusController> ().statusID = 2;
			bonusPower = 6;

		} else if (mainBody.transform.localScale.y <= 0.09f && mainBody.transform.localScale.y > 0.05f) {
			
			statusLabel.GetComponent<StatusController> ().statusID = 3;
			bonusPower = 15;

		} else if (mainBody.transform.localScale.y <= 0.05f && mainBody.transform.localScale.y > 0.01f) {
			
			statusLabel.GetComponent<StatusController> ().statusID = 4;
			bonusPower = 35;

			//activate a nice particle fx for scoring a perfect disengage
			if (SpaceshipController.activePartId < mainSpaceship.GetComponent<SpaceshipController> ().bodyParts.Length - 1) {
				GameObject pp = Instantiate (perfectFx, transform.position, Quaternion.Euler (90, 90, 0)) as GameObject;
				pp.name = "perfectFx";
				pp.transform.parent = mainSpaceship.GetComponent<SpaceshipController> ().bodyParts [SpaceshipController.activePartId + 1].transform.Find ("MainBody").transform.Find ("Exhaust").transform;
				pp.transform.localScale = new Vector3 (1, 0.5f, 1);
				StartCoroutine(mainSpaceship.GetComponent<SpaceshipController> ().increasePitch ());
			}

		}

		//tell spaceship controller to activate next fuel part
		StartCoroutine(mainSpaceship.GetComponent<SpaceshipController>().activateNextPart(bonusPower));

		yield return new WaitForSeconds(0.2f);
		canTap = true;
	}


	/// <summary>
	/// Plaies the given audio
	/// </summary>
	/// <param name="_clip">Clip.</param>
	void playSfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}

}
