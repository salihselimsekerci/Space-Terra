using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinButtonController : MonoBehaviour {

	//range starts from 0 to n
	public int skinButtonID;

	private Color lockedColor;
	private Color unlockedColor;

	private Renderer ren;
	private BoxCollider bc;


	void Awake() {
		
		lockedColor = new Color (1, 1, 1, 0.2f);
		unlockedColor = new Color (1, 1, 1, 1);
		ren = GetComponent<Renderer> ();
		bc = GetComponent<BoxCollider> ();

	}


	void Start() {

		//check if this skin is open or not
		if (PlayerPrefs.GetInt ("SkinOpen-" + skinButtonID.ToString()) == 1) {
			ren.material.color = unlockedColor;
			bc.enabled = true;
		} else {
			ren.material.color = lockedColor;
			bc.enabled = false;
		}

	}


	public void unlockSkin(int id) {
		if (skinButtonID == id) {
			ren.material.color = unlockedColor;
			bc.enabled = true;
		}
	}

}
