using UnityEngine;
using System.Collections;

public class TumbleweedSpawner : MonoBehaviour {
	public GameObject tumbleweedPrefab;
	float timer = 0;
	
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0) {
			timer = Random.value * 5;
			
			Instantiate(tumbleweedPrefab,transform.position, Quaternion.identity);
		}
	}
}
