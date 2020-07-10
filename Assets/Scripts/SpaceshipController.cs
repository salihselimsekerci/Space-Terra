using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour {

	/// <summary>
	/// This is the main spaceship controller class which handles bodypart activation, fuel consumption, 
	/// applying force & torque to make the rocket fly upwards, and explosion sequence when player ran out of
	/// fuel or clicks too late. 
	/// 
	/// One important thing to note when editing bodyparts inside the bodyParts array. You need to respect the order of
	/// parts and add them from the bottom to top, meaning that the index 0 of array should be filled with the first part
	/// (the bottom part) and so one. There is an image in the docs that describe this with more detail.
	/// </summary>

	[Header("Bodyparts")]
	public GameObject headPart;				//head of the rocket
	public GameObject[] bodyParts;			//each spaceship prefab saves a reference to all its bodyparts (note the order)	

	[Header("FXs")]
	public GameObject explosionFx;			//explosion prefab used when the ship explodes.
	public GameObject trail;				//trail line under the rocket.

	[Header("Prefab references")]
	public GameObject parachute;
	private GameObject newPlanet;
	public GameObject astronaut;

	[Header("Audio")]
	public AudioClip nozzleFireSfx;
	private bool nozzleSfxPlayFlag;

	//Note that we are using the global position of the child object!
	private float parachuteActivationDistance = 30;		//when distance is less than this, parachute will open
	private float parachuteDestroyDistance = -3;		//we need to destroy the parachute when it reaches the earth
	public static bool parachuteIsActivated;			//private flag
	public static float parachuteActivationTime;

	public static int activePartId;				//which part is already active? we need this index to activate the particles.
	public static bool isHeadActive;			//flag for when all bodyparts has been detached.
	public static float headActivationTime;		//save the time when head became active.
	public static bool isLandingOnNewPlanet;	//true when we reached a certain height and are landing on a new planet

	public static bool rocketExploded;		//flag
	private bool explosionFlag;				//flag

	private Vector3 defaultForce;			//the force we are applying to the rocket to make it fly upward.


	/// <summary>
	/// Init
	/// </summary>
	void Awake () {

		activePartId = 0; 					//default is the first body part in the array
		isHeadActive = false;
		rocketExploded = false;
		explosionFlag = false;
		nozzleSfxPlayFlag = false;
		parachuteIsActivated = false;
		isLandingOnNewPlanet = false;
		headActivationTime = 0;
		defaultForce = new Vector3 (0.65f, 55, 0);
		setBodypartStatus (activePartId);

		newPlanet = GameObject.FindGameObjectWithTag ("NewPlanet");
	}


	/// <summary>
	/// FSM
	/// </summary>
	void Update () {

		//run explosion just once
		if (rocketExploded && !explosionFlag) {
			makeExplosion ();
		}

		//play this just once
		if (CounterController.initCounter && !nozzleSfxPlayFlag) {
			nozzleSfxPlayFlag = true;
			//play sfx
			GetComponent<AudioSource>().clip = nozzleFireSfx;
			GetComponent<AudioSource>().Play();
			GetComponent<AudioSource> ().loop = true;
		}

		//new update v1.3 - land on new planet
		if (GameController.peakHeight >= NewPlanetController.minimumHeightToLand) {

			isLandingOnNewPlanet = true;

			//make sure we land on the surface of new planet
			Vector3 dir = newPlanet.transform.position - transform.position;
			//print ("Landing... | Dir: " + dir);

			GetComponent<Rigidbody> ().AddRelativeForce (dir.normalized * 10, ForceMode.Force);
			float dist = transform.position.y - newPlanet.transform.position.y;
			if (isHeadActive && dist < parachuteActivationDistance && !parachuteIsActivated) {
				StartCoroutine(activateParachute ());
			}
		}

		//Open parachute when head-part is near the earth
		if (isHeadActive && transform.position.y < parachuteActivationDistance && !parachuteIsActivated && !isLandingOnNewPlanet) {
			StartCoroutine(activateParachute ());
		}
			
	}


	/// <summary>
	/// Activates the parachute.
	/// </summary>
	IEnumerator activateParachute() {
		
		parachuteIsActivated = true;
		parachuteActivationTime = Time.timeSinceLevelLoad;

		GameObject p = Instantiate (parachute, headPart.transform.position, Quaternion.Euler (0, 0, 0)) as GameObject;
		p.name = "Parachute";
		p.transform.parent = this.gameObject.transform;

		//set a new drag to slow down the fall
		GetComponent<Rigidbody> ().drag = 4;
		GetComponent<Rigidbody> ().angularDrag = 3;

		while (headPart.transform.position.y > parachuteDestroyDistance)
			yield return 0;

		if (headPart.transform.position.y < parachuteDestroyDistance)
			Destroy (p);
	}


	/// <summary>
	/// Physics update should be performed in another thread
	/// </summary>
	void FixedUpdate () {

		if (GameController.isGameOver || !GameController.isGameStarted) {
			return;
		}

		//we need to apply default power when there is atleast one bodypart available
		if(!GameController.isOutOfFuel)
			applyDefaultForce (true);
	}


	/// <summary>
	/// Apply a constant power to make the rocket fly
	/// </summary>
	void applyDefaultForce (bool autoForce) {

		if (Input.GetKey (KeyCode.Space) || autoForce) {
			//print ("Applying force...");
			GetComponent<Rigidbody> ().AddForce (defaultForce, ForceMode.Force);

			//turn the rocket a little bit when flying
			float dir = transform.eulerAngles.z - 360;
			if (dir > -5.5f)
				GetComponent<Rigidbody> ().AddTorque (0, 0, -0.15f);
			else
				transform.eulerAngles = new Vector3 (0, 0, -5.6f);

		} 
	}


	/// <summary>
	/// Set the state of the given bodypart
	/// </summary>
	void setBodypartStatus(int id) {

		bodyParts[id].GetComponent<SpaceshipBodyController>().isActive = true;

	}


	/// <summary>
	/// Activates the next bodypart and apply a bonus power based on the result of previous detach.
	/// </summary>
	public IEnumerator activateNextPart(float bonusPower) {

		activePartId++;
		//print ("activePartId: " + activePartId);

		//apply bonus force
		GetComponent<Rigidbody> ().AddForce (new Vector3 (0, bonusPower, 0), ForceMode.Impulse);

		//if there is a bodypart available...
		if (activePartId < bodyParts.Length) {

			applyTorque ();
			yield return new WaitForSeconds (0.1f);
			setBodypartStatus (activePartId);

		} else {

			//there is only the head in space
			isHeadActive = true;
			headActivationTime = Time.timeSinceLevelLoad;
			print ("isHeadActive: " + isHeadActive);

			GetComponent<AudioSource>().Stop();

			//apply the final force and cut off the force (out of fuel)
			applyFinalForce();
			GameController.isOutOfFuel = true;

		}
	}


	//// <summary>
	/// apply a little torque
	/// </summary>
	void applyTorque() {
		//GetComponent<Rigidbody> ().AddTorque (0, 0, -0.5f);
		GetComponent<Rigidbody> ().AddForce (new Vector3 (5, 0, 0), ForceMode.Acceleration);
	}


	/// <summary>
	/// Apply the final force.
	/// </summary>
	void applyFinalForce() {
		GetComponent<Rigidbody> ().AddForce (new Vector3 (0, 75, 0), ForceMode.Impulse);
	}


	/// <summary>
	/// Create an explosion when rocket ran out of fuel
	/// </summary>
	void makeExplosion() {
		
		explosionFlag = true;
		print ("Create rocket explosion...");

		GetComponent<AudioSource>().Stop();

		// -> hide/disable/stop rocket
		trail.SetActive (false);
		gameObject.GetComponent<Rigidbody> ().isKinematic = true;
		gameObject.GetComponent<Rigidbody> ().useGravity = false;
		//we will shift the rocket behind the background to hide it from view
		transform.position = new Vector3(transform.position.x, transform.position.y, 5);

		//create explosion
		GameObject expl = Instantiate(explosionFx, transform.position + new Vector3(0, 1.5f + activePartId , -5), Quaternion.Euler(0, 180, 0)) as GameObject;
		expl.name = "explosionFx";

	}


	/// <summary>
	/// Increases the pitch of the nozzle particle sfx
	/// </summary>
	public IEnumerator increasePitch() {
		AudioSource aso = GetComponent<AudioSource> ();
		aso.pitch = 3.0f;
		yield return new WaitForSeconds (1.25f);
		aso.pitch = 1.75f;
	}


	void OnCollisionEnter(Collision c) {
		if (c.collider.gameObject.tag == "NewPlanet") {
			Transform p = gameObject.transform.Find ("Parachute");
			Destroy (p.gameObject);
			GetComponent<Rigidbody> ().isKinematic = true;

			//unload the astronauts, if there is any!
			if(GameController.totalHiredAstronaut > 0) {
				//save total man on the planet
				int currentManOnPlanet = PlayerPrefs.GetInt ("ManOnTheNewPlanet");
				PlayerPrefs.SetInt ("ManOnTheNewPlanet", currentManOnPlanet + GameController.totalHiredAstronaut);
				//let the astronauts out of the ship
				StartCoroutine(unloadAstronauts ());
			}
		}
	}


	IEnumerator unloadAstronauts() {
		//create small astronauts to populate the new planet (you can chnage the starting point here)
		for(int i = 0; i < GameController.totalHiredAstronaut; i++) {
			GameObject astro = Instantiate(astronaut, headPart.transform.position, Quaternion.Euler(0, 180, 0)) as GameObject;
			astro.name = "Astronaut-" + (i+1).ToString();
			astro.GetComponent<AstronautManager> ().dest = 2;	//they need to enter the ship
			yield return new WaitForSeconds(1.1f);
		}
	}

}
