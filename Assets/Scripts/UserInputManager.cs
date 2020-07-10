#pragma warning disable 414

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UserInputManager : MonoBehaviour {

	/// <summary>
	/// This class manages pause and unpause states.
	/// It is also responsible for handling all input/touch events on ui buttons and elements.
	/// </summary>

	internal Camera uiCam;					//UI camera
	internal bool isPaused;					//Flag
	private float savedTimeScale;
	public GameObject pausePlane;
	private GameObject GC;
	public GameObject astronautPrefab;

	public GameObject uiNewSkinCell;
	private bool isSkinCellVisible;

	private GameObject AdManagerObject;
	private bool canTap = true;	

	public AudioClip canBuy;
	public AudioClip cantBuy;
	public AudioClip tapSfx;

	enum Status { PLAY, PAUSE }
	private Status currentStatus = Status.PLAY;

    ReklamScript rg;
	/// <summary>
	/// Init.
	/// </summary>
	void Awake (){		

		GC = GameObject.FindGameObjectWithTag ("GameController");
		uiCam = GameObject.FindGameObjectWithTag ("UICamera").GetComponent<Camera>();
		isPaused = false;
		
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02f;
		
		if(pausePlane)
	    	pausePlane.SetActive(false); 

		uiNewSkinCell.SetActive (false);
		isSkinCellVisible = false;

		//AdManagerObject = GameObject.FindGameObjectWithTag("AdManager");
	}


	/// <summary>
	/// FSM
	/// </summary>
	void Update (){
		
		//touch control
		if(canTap)
			touchManager();
		
		//debug restart
		if(Input.GetKeyDown(KeyCode.R)) {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		}
	}


	/// <summary>
	/// This function monitor player touches on UI buttons.
	/// detects both touch and clicks and can be used with editor, handheld device and 
	/// every other platforms at once.
	/// </summary>
	void touchManager (){
		
		if(Input.GetMouseButtonUp(0)) {
			
			RaycastHit hitInfo;
			Ray ray = uiCam.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hitInfo)) {
				
				GameObject objectHit = hitInfo.transform.gameObject;

				//if we are selecting a new skin
				if (objectHit.tag == "BtnSkin") {

					StartCoroutine (animateButton (hitInfo.transform.gameObject));
					playSfx (tapSfx);
					//get btn skin ID
					int btnID = objectHit.GetComponent<SkinButtonController>().skinButtonID;
					//save selected skin
					PlayerPrefs.SetInt ("SkinID", btnID);
					//apply selected skin
					print("btnID: " + btnID);
					GC.GetComponent<GameController>().manageSpaceshipSkin(btnID);
				}


				//if we are interacting with other UI elements
				switch(objectHit.name) {

				/*
				 * we do not need pause system in this fast paced game...
				 * 
				case "Button-Pause":
					switch (currentStatus) {
			            case Status.PLAY: 
			            	PauseGame();
			            	break;
			            case Status.PAUSE: 
			            	UnPauseGame(); 
			            	break;
			            default: 
			            	currentStatus = Status.PLAY;
			            	break;
			        }
					break;
				
				case "Btn-Resume":
					switch (currentStatus) {
			            case Status.PLAY: 
			            	PauseGame();
			            	break;
			            case Status.PAUSE: 
			            	UnPauseGame(); 
			            	break;
			            default: 
			            	currentStatus = Status.PLAY;
			            	break;
			        }
					break;
				*/

				case "Btn-Restart":
				case "uiButton-Play":
					UnPauseGame();
					SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
					break;
						
				case "uiButton-Upgrade":

					StartCoroutine (animateButton (hitInfo.transform.gameObject));

					//check if player has enough coins to upgrade
					if (GameController.playerCoin >= GameController.upgradePrice && GameController.canUpgrade) {
						print ("We have enough coins to upgrade the rocket");
						//playSfx
						playSfx (canBuy);
						canTap = false;	
						//save available coins
						PlayerPrefs.SetInt ("PlayerCoin", GameController.playerCoin - GameController.upgradePrice);
						//upgrade the rocket
						int rocketLevel = PlayerPrefs.GetInt ("RocketLevel");
						PlayerPrefs.SetInt ("RocketLevel", ++rocketLevel);
						//reload level with new rocket
						SceneManager.LoadScene (SceneManager.GetActiveScene ().name);

					} else {
						print ("Insufficient coins");
						playSfx (cantBuy);
					}
					
					break;

				case "uiButton-SwitchStartingPlanet":

					StartCoroutine (animateButton (hitInfo.transform.gameObject));
					playSfx (tapSfx);

					if (GameController.startingPlanetID == 0) {
						
						PlayerPrefs.SetInt ("StartingPlanetID", 1);
						SceneManager.LoadScene (SceneManager.GetActiveScene ().name);

					} else if (GameController.startingPlanetID == 1) {
						
						PlayerPrefs.SetInt ("StartingPlanetID", 0);
						SceneManager.LoadScene (SceneManager.GetActiveScene ().name);

					}

					break;

				case "uiButton-NewSkin":

					StartCoroutine (animateButton (hitInfo.transform.gameObject));
					playSfx (tapSfx);

					isSkinCellVisible = !isSkinCellVisible;
					uiNewSkinCell.SetActive (isSkinCellVisible);

					break;

				case "uiButton-HireAstronaut":

					StartCoroutine (animateButton (hitInfo.transform.gameObject));

					//check if player has enough money to hire new astronaut, or if there is space for new hiring
					if (GameController.playerCoin >= GameController.astronautHirePrice && GameController.totalHiredAstronaut < GameController.maximumAstronautToHire) {
						//playSfx
						GetComponent<AudioSource> ().PlayOneShot (canBuy);
						//deduct the price from player's money
						PlayerPrefs.SetInt ("PlayerCoin", GameController.playerCoin - GameController.astronautHirePrice);
						//increase hire counter
						GameController.totalHiredAstronaut++;
						//increase new hiring price
						GameController.astronautHirePrice = 1 + (GameController.totalHiredAstronaut * GameController.totalHiredAstronaut);
						//create small astronauts to enter the ship (you can chnage the starting point here)
						GameObject astro = Instantiate(astronautPrefab, new Vector3(-0.9f, 0.5f, 0.15f), Quaternion.Euler(0, 180, 0)) as GameObject;
						astro.name = "Astronaut";
						astro.GetComponent<AstronautManager> ().dest = 1;	//they need to enter the ship
						//update players coin in GameController class
						GC.GetComponent<GameController>().updatePlayersCoin();

					} else {
						playSfx (cantBuy);

					}

					break;

                    case "ReklamGoster":

                        StartCoroutine(animateButton(hitInfo.transform.gameObject));
                        playSfx(tapSfx);
                        rg.GetComponent<ReklamScript>().reklamgoster();

                        canTap = false;

                        break;
                }
			}
		}
	}


	void PauseGame (){
		print("Game is Paused...");

		//show an Interstitial Ad when the game is paused
		if(AdManagerObject)
			//AdManagerObject.GetComponent<AdManager>().showInterstitial();

		isPaused = true;
		savedTimeScale = Time.timeScale;
	    Time.timeScale = 0;
	    AudioListener.volume = 0;
	    if(pausePlane)
	    	pausePlane.SetActive(true);
	    currentStatus = Status.PAUSE;
	}


	void UnPauseGame (){
		print("Unpause");
	    isPaused = false;
	    Time.timeScale = savedTimeScale;
		Time.fixedDeltaTime = 0.02f;
	    AudioListener.volume = 1.0f;
		if(pausePlane)
	    	pausePlane.SetActive(false);   
	    currentStatus = Status.PLAY;
	}


	/// <summary>
	/// This function animates a button by modifying it's scales on x-y plane.
	/// can be used on any element to simulate the tap effect.
	/// </summary>
	IEnumerator animateButton ( GameObject _btn  ){
		canTap = false;
		Vector3 startingScale = _btn.transform.localScale;	//initial scale	
		Vector3 destinationScale = startingScale * 1.1f;	//target scale

		//Scale up
		float t = 0.0f; 
		while (t <= 1.0f) {
			t += Time.deltaTime * 9;
			_btn.transform.localScale = new Vector3(Mathf.SmoothStep(startingScale.x, destinationScale.x, t),
				_btn.transform.localScale.y,
				Mathf.SmoothStep(startingScale.z, destinationScale.z, t));
			yield return 0;
		}

		//Scale down
		float r = 0.0f; 
		if(_btn.transform.localScale.x >= destinationScale.x) {
			while (r <= 1.0f) {
				r += Time.deltaTime * 9;
				_btn.transform.localScale = new Vector3(Mathf.SmoothStep(destinationScale.x, startingScale.x, r),
					_btn.transform.localScale.y,
					Mathf.SmoothStep(destinationScale.z, startingScale.z, r));
				yield return 0;
			}
		}

		if(r >= 1)
			canTap = true;
	}


	/// <summary>
	/// Plays the given audio
	/// </summary>
	void playSfx ( AudioClip _clip  ){
		GetComponent<AudioSource>().clip = _clip;
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().Play();
		}
	}

}