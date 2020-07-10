using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class InitManager : MonoBehaviour {

	/// <summary>
	/// We need to use this loader to init the AdManager singleton 
	/// </summary>

	IEnumerator Start () {
		yield return new WaitForSeconds (0.5f);
		PlayerPrefs.SetInt ("StartingPlanetID", 0);		//Always start on earth
		SceneManager.LoadScene ("Game");
	}

}
