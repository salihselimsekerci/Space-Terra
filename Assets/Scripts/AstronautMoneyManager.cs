using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class AstronautMoneyManager : MonoBehaviour {

	/// <summary>
	/// Astronauts we send to the new planet will make us some coins every minute. Here we check the last time
	/// user played the game and if a certain amount of time has been passed, grant him the coins.
	/// </summary>

	public static int checkDelay = 1800;	//we will check for player return to game every 30 minutes

	public GameObject prizePanel;			//the main prize panel
	public GameObject coinTextUI;			//coins amount on game scene
	public AudioClip collectSfx;			//audio

	private DateTime baseTime;
	private int currentTime;
	private int lastPlayedTime;
	private bool canTap;
	private bool canShowUI;
	private int generatedCoins;


	/// <summary>
	/// Init
	/// </summary>
	void Awake () {

		prizePanel.SetActive (false);
		canTap = true;
		generatedCoins = 0;
		canShowUI = false;

		//if there is no astronaut on other planets, don't continue
		if (PlayerPrefs.GetInt ("ManOnTheNewPlanet") < 1)
			gameObject.SetActive (false);
	}


	/// <summary>
	/// Check if the required delay has been passed between the last and the new prize coins collected by the astronauts.
	/// if so, save the new time and show the collection UI panel. If not, show nothing and continue as normal.
	/// </summary>
	void checkTime () {

		//save the last time user player this game. So we can calculate how much time has been passed since the last game
		baseTime = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
		currentTime = (int)(System.DateTime.UtcNow - baseTime).TotalSeconds;

		if (PlayerPrefs.HasKey ("lastPlayedTime"))
			lastPlayedTime = PlayerPrefs.GetInt ("lastPlayedTime");
		else
			lastPlayedTime = currentTime;

		print ("currentTime: " + currentTime);
		print ("lastPlayedTime: " + lastPlayedTime);

		if (currentTime > lastPlayedTime + checkDelay) {
			
			//save into playerprefs
			PlayerPrefs.SetInt ("lastPlayedTime", currentTime);
			print ("New Play-Time saved.");
			canShowUI = true;

		} else {
			
			print ("You need to wait atleast " + (checkDelay - (currentTime - lastPlayedTime)) + " seconds before being able to check for money!");
			canShowUI = false;

		}

		//coin generation formula
		generatedCoins = (int)( ((currentTime - lastPlayedTime) / 600.0f) * PlayerPrefs.GetInt ("ManOnTheNewPlanet") );
		print ("generatedCoins: " + generatedCoins);
	}


	void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	/// <summary>
	/// We need to check if we are inside the correct scene before showing prize coins panel to the player.
	/// </summary>
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {

		checkTime ();

		//enable prize collection if on game scene
		if (SceneManager.GetActiveScene ().name == "Game" && canShowUI && generatedCoins > 0) {
			prizePanel.SetActive (true);
			coinTextUI.GetComponent<TextMesh> ().text = generatedCoins.ToString ();
		}

	}


	/// <summary>
	/// FSM
	/// </summary>
	void Update() {

		if(canTap)
			StartCoroutine(tapManager());

	}
		


	/// <summary>
	/// Check player inputs. Notice that you can merge this event inside "UserInputManager". but we had to use a distinct class
	/// to maintain backward [and update] compatibility.
	/// </summary>
	private RaycastHit hitInfo;
	private Ray ray;
	IEnumerator tapManager (){

		//Mouse of touch?
		if(	Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)  
			ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
		else if(Input.GetMouseButtonUp(0))
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		else
			yield break;

		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			switch (objectHit.name) {
			case "CollectButton":
				
				//set flags
				canTap = true;
				playSfx (collectSfx);

				//get available coins
				int availableCoins = PlayerPrefs.GetInt("PlayerCoin");
				PlayerPrefs.SetInt("PlayerCoin", availableCoins + generatedCoins);

				//save new time
				PlayerPrefs.SetInt ("lastPlayedTime", currentTime);

				//set delayed flags
				canShowUI = false;
				generatedCoins = 0;

				//wait
				yield return new WaitForSeconds(0.5f);

				//reload
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				break;
			}
		}
	}


	/// <summary>
	/// Plaies the given audio
	/// </summary>
	void playSfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}

}
