using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	/// <summary>
	/// This is the main game controller class. 
	/// It is responsible for maintaining player coins, rocket upgrade level, current and best scores, 
	/// monitoring game over event, enabling/disabling UI elements and saving game progress.
	/// </summary>

	public static bool isGameStarted;				//static flag
	public static bool isGameOver;					//static flag

	public static int skinID;						//skin image (index) used for the current spaceship
	public static bool isOutOfFuel;					//has this space ship burned all its fuel?
	public static float playerScore;				//Score oof player for the current game. Will be calculated based on the fly distance.
	public static int playerCoin;					//total player saved coins
	public static int prizeCoin;					//coins received only in this round
	public static int rocketLevel;					//upgrade level of the rocket
	public static int upgradePrice;					//coins needed to upgrade the rocket
	public static bool canUpgrade;					//flag for other controllers
	public static float peakHeight;					//maximum height of the spaceship is this round
	public static int savedBestScore;				//Best score of all time (saved in playerprefs)
	public static int startingPlanetID;				//Id of the starting planet
	private float bestScore;						//saved best score

	//new update - astronaut1
	public static int astronautHirePrice;			//for each new astronaut we need to hire, we increase the price 
	public static int totalHiredAstronaut;			//total hired astronaut in this round, already inside the spaceship
	public static int maximumAstronautToHire = 10;	//for now, we limit the total astronauts player can hire in each round.


	[Header("Available Rockets")]
	public GameObject[] availableRockets;

	[Header("Available Spaceship Skins")]
	public SpaceshipSkin[] skin;
	public int skinUnlockPrice = 500;

	[Header("UI Elements")]
	public GameObject uiPlayerScore;
	public GameObject uiPlayerBestScore;
	public GameObject uiPlayerBestScoreStatic;
	public GameObject uiPlayerCoin;
	public GameObject uiPlayerPrizeCoin;
	public GameObject uiUpgradePrice;
	public GameObject uiCurrentSkinIcon;
	//new update
	public GameObject uiManOnTheNewPlanet;
	public GameObject uiHireAstronautButton;
	public GameObject uiAstronautCell;
	public GameObject uiAstronautHirePrice;
	public GameObject uiHiredAstronaut;
	//v1.5
	public GameObject uiButtonSwitchStartingPlanet;
	public GameObject uiStartingPlanetIcon;
	public Material[] uiStartingPlanetImages;
    public GameObject odullureklambtun;

	[Header("UI Buttons")]
	public GameObject uiButtonPlay;

	[Header("Audio")]
	public AudioClip scoreSfx;

	private GameObject spaceShip;
	private Vector3 spaceShipStartPosition;
	private GameObject AdManagerObject;


	void Awake () {

		//Debug - Test
		//PlayerPrefs.DeleteAll();
		//PlayerPrefs.SetInt ("PlayerCoin", 10000);

		isGameStarted = false;
		isGameOver = false;
		playerScore = 0;
		bestScore = -1;
		isOutOfFuel = false;
		canUpgrade = true;
		peakHeight = 0;
		astronautHirePrice = 1;		//always fixed at 1 for the first astonaut
		totalHiredAstronaut = 0;

		uiPlayerPrizeCoin.SetActive (false);
		uiButtonPlay.SetActive (false);

		playerCoin = PlayerPrefs.GetInt ("PlayerCoin");
		rocketLevel = PlayerPrefs.GetInt ("RocketLevel");

		print ("rocketLevel: " + rocketLevel);

		//Create the rocket & set upgrade price
		GameObject rocket;
		if (rocketLevel >= availableRockets.Length - 1) {
			rocketLevel = availableRockets.Length - 1;
			canUpgrade = false;
		}

		//rocket upgrade rules
		switch(rocketLevel) {
		case 0:
			rocket = Instantiate (availableRockets [0], new Vector3 (0, -2.3f, 0), Quaternion.Euler (0, 0, 0)) as GameObject;
			rocket.name = "SpaceShip";
			upgradePrice = 100;
			break;
		case 1:
			rocket = Instantiate (availableRockets [1], new Vector3 (0, -2.3f, 0), Quaternion.Euler (0, 0, 0)) as GameObject;
			rocket.name = "SpaceShip";
			upgradePrice = 250;
			break;
		case 2:
			rocket = Instantiate (availableRockets [2], new Vector3 (0, -2.3f, 0), Quaternion.Euler (0, 0, 0)) as GameObject;
			rocket.name = "SpaceShip";
			upgradePrice = 500;
			break;
		case 3:
			rocket = Instantiate (availableRockets [3], new Vector3 (0, -2.3f, 0), Quaternion.Euler (0, 0, 0)) as GameObject;
			rocket.name = "SpaceShip";
			upgradePrice = 1000;
			break;
		case 4:
			rocket = Instantiate (availableRockets [4], new Vector3 (0, -2.3f, 0), Quaternion.Euler (0, 0, 0)) as GameObject;
			rocket.name = "SpaceShip";
			upgradePrice = 3000;
			break;
		}

		//AdManagerObject = GameObject.FindGameObjectWithTag("AdManager");
		spaceShip = GameObject.FindGameObjectWithTag ("Player");
		spaceShipStartPosition = spaceShip.transform.position;

		//Skin settings
		skinID = PlayerPrefs.GetInt ("SkinID");
		//print ("Current skinID: " + skinID);
		//Set spaceship skin
		manageSpaceshipSkin(skinID);

		//set correct icon for switch planet icon
		startingPlanetID = PlayerPrefs.GetInt("StartingPlanetID");
		if (startingPlanetID == 0) {
			uiStartingPlanetIcon.GetComponent<Renderer> ().material = uiStartingPlanetImages [1];
		} else if (startingPlanetID == 1) {
			uiStartingPlanetIcon.GetComponent<Renderer> ().material = uiStartingPlanetImages [0];
		}

		//Astronaut hiring addition
		savedBestScore = PlayerPrefs.GetInt ("PlayerBestScore");
		print ("savedBestScore: " + savedBestScore);
		//print ("savedBestScore: " + savedBestScore + " || Required score to open Astronaut system: " + NewPlanetController.minimumHeightToLand);
		if (savedBestScore < NewPlanetController.minimumHeightToLand) {
			//if we dont have a powerful spaceship and are not able to reach the new planet, do not show these buttons
			uiHireAstronautButton.SetActive(true);
            
			uiAstronautCell.SetActive (false);
			print ("Hiring system is not activated. You need to reach higher peaks!");
			//also hide switch planets icon
			uiButtonSwitchStartingPlanet.SetActive(false);
		}

		//we need to always enable switch-planets button when we are not on earth, so...
		if (startingPlanetID != 0) {
			uiButtonSwitchStartingPlanet.SetActive(true);
		}

		//debug
		//playerCoin = 1000;
	}


	/// <summary>
	/// Update UI elements with useful information
	/// </summary>
	void Start () {
		uiUpgradePrice.GetComponent<TextMesh> ().text = upgradePrice.ToString ();
		uiPlayerBestScore.GetComponent<TextMesh> ().text = PlayerPrefs.GetInt ("PlayerBestScore").ToString();
		uiPlayerCoin.GetComponent<TextMesh> ().text = playerCoin.ToString();
		uiManOnTheNewPlanet.GetComponent<TextMesh> ().text = PlayerPrefs.GetInt ("ManOnTheNewPlanet").ToString ();
	}
    public void altinEkle()
    {
        playerCoin += 50;
        uiPlayerCoin.GetComponent<TextMesh>().text = playerCoin.ToString();
        PlayerPrefs.SetInt("PlayerCoin",playerCoin);


    }

    /// <summary>
    /// Set correct spaceship skin images
    /// </summary>
    public void manageSpaceshipSkin(int id) {

		//Test/Cheat
		//id = 2;

		//we have 1 head and multiple body, fuel & divider objects
		GameObject spHead = GameObject.FindGameObjectWithTag ("SpaceshipHead");
		GameObject[] spBody = GameObject.FindGameObjectsWithTag ("SpaceshipBody");
		GameObject[] spDivider = GameObject.FindGameObjectsWithTag ("SpaceshipDivider");

		spHead.GetComponent<Renderer> ().material.mainTexture = skin [id].spaceshipHead;
		uiCurrentSkinIcon.GetComponent<Renderer> ().material.mainTexture = skin [id].spaceshipHead;

		//print ("SpaceshipBody count: " + spBody.Length);
		//print ("SpaceshipDivider count: " + spDivider.Length);

		for (int i = 0; i < spBody.Length; i++) {
			spBody[i].GetComponent<Renderer> ().material.mainTexture = skin [id].spaceshipBody;
			spDivider[i].GetComponent<Renderer> ().material.mainTexture = skin [id].spaceshipDivider;
		}
	}


	/// <summary>
	/// FSM
	/// </summary>
	void Update () {

		//Run just once
		if (isGameOver) {
            runGameover ();
            return;
            

        }

		updatePlayerScore ();
	}


	/// <summary>
	/// Set/save player score
	/// </summary>
	void updatePlayerScore() {

		if (!isGameStarted)
          
			return;
        //odullureklambtun.SetActive(false);

        //calculate score based on how much the rocket has travelled upward
        playerScore = spaceShip.transform.position.y + Mathf.Abs(spaceShipStartPosition.y);

		//save best Score (height)
		if (playerScore >= bestScore) {
			bestScore = playerScore;
		}

		//print ("bestScore: " + bestScore + " ||| playerScore: " + playerScore);

		//check peak hitting event (fast game finish)
		if (isGameStarted) {
			if (playerScore < bestScore) {

				//we have hit the peak
				print ("Peak Reached...");
				peakHeight = bestScore;			//maxdffimum height travelled
				playSfx (scoreSfx);

				//animate scores
				StartCoroutine(roundScores());

				isGameOver = true;
			}
		}

		//calculate and save player money/prize
		prizeCoin = (int)(bestScore / 10);
		PlayerPrefs.SetInt ("PlayerCoin", playerCoin + prizeCoin);

		//set limiters
		if (playerScore < 0)
			playerScore = 0;

		//show on UI
		uiPlayerScore.GetComponent<TextMesh> ().text = ((int)bestScore).ToString ();
		uiPlayerCoin.GetComponent<TextMesh> ().text = playerCoin.ToString ();
		uiPlayerPrizeCoin.GetComponent<TextMesh> ().text = "+" + prizeCoin.ToString ();

	}


	/// <summary>
	/// When the game is over, we use this function to animate the current coins text with the amount of coins 
	/// we have received as the reward.
	/// </summary>
	IEnumerator roundScores() {
		yield return new WaitForSeconds (1.5f);

		print ("Rounding...");
        FindObjectOfType<AdmobReq>().GetComponent<AdmobReq>().ShowInterstitial();
        float t = 0;
		float pc = playerCoin;
		float pz = prizeCoin;

		while (t < 1) {
			t += Time.deltaTime * 1.5f;
			playerCoin = (int)Mathf.Lerp (pc, pc + pz, t);
			prizeCoin = (int)Mathf.Lerp (pz, 0, t);

			uiPlayerCoin.GetComponent<TextMesh> ().text = playerCoin.ToString ();
			uiPlayerPrizeCoin.GetComponent<TextMesh> ().text = "+" + prizeCoin.ToString ();
			yield return 0;
		}
	}


	/// <summary>
	/// Makes the UI elements appear/disappear
	/// </summary>
	public void enableUI (bool state) {

		//score
		uiPlayerBestScore.SetActive (state);
		uiPlayerBestScoreStatic.SetActive (state);

		//coin
		uiPlayerCoin.SetActive(state);
		uiPlayerPrizeCoin.SetActive (state);
		uiManOnTheNewPlanet.SetActive (state);

		//buttons
		uiButtonPlay.SetActive (state);
	}


	/// <summary>
	/// Game over sequence
	/// </summary>
	private bool runOnce = false;
	void runGameover() {

		if (runOnce)
			return;

		runOnce = true;

		//save score/bestScore
		int lastBestScore = PlayerPrefs.GetInt("PlayerBestScore");
		if (playerScore > lastBestScore) {
			//save new best score
			PlayerPrefs.SetInt("PlayerBestScore", (int)playerScore);
			uiPlayerBestScore.GetComponent<TextMesh> ().text = ((int)playerScore).ToString();
		}

		enableUI (true);

		//show an Ad on screen every 1 out of 5 gameover
		if (Random.value > 0.80f) {
			//if (AdManagerObject) 
			//	//AdManagerObject.GetComponent<AdManager> ().showInterstitial ();
		}

	}


	/// <summary>
	/// Sometimes we need to update player coins form other classes (incase we consume player's money for something)
	/// </summary>
	public void updatePlayersCoin() {
		playerCoin = PlayerPrefs.GetInt ("PlayerCoin");
		uiPlayerCoin.GetComponent<TextMesh> ().text = playerCoin.ToString ();
		uiAstronautHirePrice.GetComponent<TextMesh> ().text = astronautHirePrice.ToString ();
		uiHiredAstronaut.GetComponent<TextMesh> ().text = totalHiredAstronaut.ToString ();
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
