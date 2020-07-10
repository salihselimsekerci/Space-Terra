using UnityEngine;
using System.Collections;

public class TumbleweedMotion : MonoBehaviour {
	
	float wind = 40;
	Rigidbody body;
	
	void Start () {
		body = GetComponent<Rigidbody>();
		body.velocity = new Vector3(0,Random.value * 2,0);
	}
	
	void Update () {
		body.AddForceAtPosition(new Vector3(wind, 2, 0), transform.position);
		if (transform.position.x > 30) Destroy (gameObject);
	}
}
