using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	/// <summary>
	/// Main camera manager. Handles camera movement, smooth follow (main rocket), and bounding by movement limiters.
	/// Note that this is the game-play camera. All UI rendering is done by UICamera in another thread.
	/// </summary>

	public static float	cps = 6; 			//camera's default projection size
	public static float	currentCps;			//camera's current projection size

	internal Vector3 cameraStartingPos;
	internal Vector3 cameraCurrentPos;

	internal GameObject targetToLock;		//the target object we need to have a static view on
	internal GameObject targetToFollow;		//the target we need to follow as it moves

	//reference to game objects
	private GameObject player;


	/// <summary>
	/// Init
	/// </summary>
	void Awake () {

		currentCps = cps;
		GetComponent<Camera> ().orthographicSize = cps;
		cameraStartingPos = new Vector3 (0, 1.35f, -10);
		cameraCurrentPos = cameraStartingPos;
		transform.position = cameraStartingPos;
		targetToLock = null;
		targetToFollow = null;

		player = GameObject.FindGameObjectWithTag ("Player");
		targetToFollow = player;
		//targetToFollow = player.transform.FindChild("SpaceshipHead").gameObject;
	}
		

	/// <summary>
	/// FSM
	/// </summary>
	void FixedUpdate () {

		if(!SpaceshipController.isHeadActive)
			targetToFollow = player.GetComponent<SpaceshipController>().bodyParts[SpaceshipController.activePartId];
		else 
			targetToFollow = player.transform.Find("SpaceshipHead").gameObject;

		//if the game has not started yet, or the game is finished, just return
		//if (!GameController.gameIsStarted || GameController.gameIsFinished)
		//	return;

		//follow the target (if any)
		if (targetToFollow) {
			StartCoroutine (smoothFollow (targetToFollow.transform.position));
		}

	}
		

	/// <summary>
	/// set limiters
	/// </summary>
	void LateUpdate () {
		
		//Limit camera's movement
		if (transform.position.y < 1.35F) {
			transform.position = new Vector3 (transform.position.x, 1.35F, transform.position.z);
		}

		//you might need to change this limit in your own game!
		if (transform.position.y > 1500) {
			transform.position = new Vector3 (transform.position.x, 1500, transform.position.z);
		}

	}


	/// <summary>
	/// Smooth follow the target object.
	/// </summary>
	[Range(0, 0.5f)]
	public float followSpeedDelay = 0.1f;
	private float xVelocity = 0.0f;
	private float yVelocity = 0.0f;
	IEnumerator smoothFollow(Vector3 p) {

		if (targetToFollow.transform.position.y <= -2)
			yield break;

		float posX = Mathf.SmoothDamp(transform.position.x, p.x, ref xVelocity, followSpeedDelay);
		//float posY = Mathf.SmoothDamp(transform.position.y, p.y + 2.5f + (GameController.rocketLevel / 3), ref yVelocity, followSpeedDelay);
		float posY = Mathf.SmoothDamp(transform.position.y, p.y + 2.5f + (SpaceshipController.activePartId / 2.5f), ref yVelocity, followSpeedDelay);
		transform.position = new Vector3(posX, posY, transform.position.z);

		//always save camera's current pos in an external variable, for later use
		cameraCurrentPos = transform.position;

		handleCps ();

		yield return 0;
	}


	/// <summary>
	/// Change camera's projection size based on game events
	/// if spaceship has any parts, we need to maintain a closeup, but if all parts has been dropped and
	/// there is only the head in the sky, we need to change to longshot.
	/// </summary>
	void handleCps() {

		if (!SpaceshipController.isHeadActive) {
			/*
			currentCps = cps - (player.transform.position.y / 100);
			//limiters
			if (currentCps < 5) currentCps = 5;
			if (currentCps > 6) currentCps = 6;
			*/

			currentCps = 6 - (SpaceshipController.activePartId / 5.1f);

		} else {

			if (!SpaceshipController.parachuteIsActivated)
				currentCps = Mathf.Lerp (currentCps, 30, (Time.timeSinceLevelLoad - SpaceshipController.headActivationTime) / 100);
			else
				currentCps = Mathf.Lerp (currentCps, 8.5f, (Time.timeSinceLevelLoad - SpaceshipController.parachuteActivationTime) / 100);
		}

		//set the new projection size
		GetComponent<Camera> ().orthographicSize = currentCps;
	}


	/// <summary>
	/// move the camera to a given position
	/// </summary>
	/// <returns>The to position.</returns>
	/// <param name="from">From.</param>
	/// <param name="to">To.</param>
	/// <param name="speed">Speed.</param>
	public IEnumerator goToPosition(Vector3 from, Vector3 to, float speed) {
		float t = 0;
		while (t < 1) {
			t += Time.deltaTime * speed;
			transform.position = new Vector3 (	Mathf.SmoothStep (from.x, to.x, t),
												Mathf.SmoothStep (from.y, to.y, t),
												transform.position.z);
			yield return 0;
		}

		if(t >= 1) {
			//always save camera's current pos in an external variable, for later use
			cameraCurrentPos = transform.position;
		}
	}


}
