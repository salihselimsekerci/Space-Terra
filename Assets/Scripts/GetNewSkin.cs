using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetNewSkin : MonoBehaviour {

	public static int availableSkins;				//Total available skins
	public GameObject[] skinButtons;				//all available skins buttons in UI
	public static List<int> lockedSkinIDs;			//IDs of all locked skins available to unlock
	public AudioClip unlockSfx;
	public AudioClip notPossibleSfx;
	private bool canTap;
	private GameObject GC;

	void Awake () {

		//init
		lockedSkinIDs = new List<int>();
		availableSkins = skinButtons.Length;
		GC = GameObject.FindGameObjectWithTag ("GameController");

		//always mark the first skin as unlocked
		PlayerPrefs.SetInt("SkinOpen-0", 1);

		//Get locked skins (starts from 1 because index 0 is always open by default)
		getLockedSkins();

		canTap = true;
	}

	void Start () {

		//set price
		GetComponent<TextMesh>().text = "+ Get New (" + GC.GetComponent<GameController>().skinUnlockPrice + ")";

	}


	/// <summary>
	/// Get all locked skin ids and store them in a temporary list
	/// </summary>
	void getLockedSkins() {
		lockedSkinIDs = new List<int>();
		for (int i = 1; i < availableSkins; i++) {
			if (PlayerPrefs.GetInt ("SkinOpen-" + i.ToString ()) == 0) {
				lockedSkinIDs.Add (i);
			}
		}
		print ("Total number of locked skins: " + lockedSkinIDs.Count);
	}

	
	void Update () {

		//touch control
		if(canTap)
			touchManager();

	}


	private RaycastHit hitInfo;
	private Ray ray;
	void touchManager (){

		//Mouse of touch?
		if(	Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)  
			ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
		else if(Input.GetMouseButtonUp(0))
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		else
			return;

		if (Physics.Raycast(ray, out hitInfo)) {
			GameObject objectHit = hitInfo.transform.gameObject;
			switch(objectHit.name) {

			//Game Modes
			case "GetNewButton":
				//if all skins are already unlocked, or if we do not have enough coins to unlock a new skin
				if (lockedSkinIDs.Count <= 0 || PlayerPrefs.GetInt("PlayerCoin") < GC.GetComponent<GameController>().skinUnlockPrice) {
					//no unlocked skin available!
					playSfx (notPossibleSfx);				
				} else {
					//we have atleast one skin to unlock
					playSfx (unlockSfx);
					//deduct and save available coins
					PlayerPrefs.SetInt ("PlayerCoin", GameController.playerCoin - GC.GetComponent<GameController>().skinUnlockPrice);
					//call GC to reflect the new available coins
					GC.GetComponent<GameController>().updatePlayersCoin();
					//unlock a skin
					unlockNewSkin ();
				}
				break;
			}	
		}
	}


	/// <summary>
	/// This function randomly selects and unlock a locked skin ID
	/// </summary>
	void unlockNewSkin() {
		//create a random id to unlock
		int randomID = lockedSkinIDs[Random.Range(0, lockedSkinIDs.Count)];
		print ("Unlocking skin id " + randomID);
		//save the new unlocked skin
		PlayerPrefs.SetInt ("SkinOpen-" + randomID.ToString(), 1);
		//update the unlocked skin's material in UI
		skinButtons[randomID].GetComponent<SkinButtonController> ().unlockSkin (randomID);
		//update locked skin list for further use
		getLockedSkins();
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
