using UnityEngine;
using System.Collections;

public class InfiniteTerrainCamera : MonoBehaviour {
    public GameObject followObject;
    public float      maxXOffset;
   
    float screenWidth;
    float camX;

	void Start      () {
        screenWidth = GetViewSizeAtDistance(Mathf.Abs(followObject.transform.position.z - transform.position.z)).x;
	}
	void FixedUpdate () {
        if (followObject.transform.position.x < camX - screenWidth / 2) {
            followObject.transform.position = new Vector3(0, 9, 0);
            transform.position = new Vector3(0, 9, transform.position.z);
            camX = 0;
        }

        camX += GetSpeed() * Time.deltaTime;
        if (followObject.transform.position.x > camX + maxXOffset) {
            camX = followObject.transform.position.x - maxXOffset;
        }
        transform.position = new Vector3(camX, followObject.transform.position.y, transform.position.z);
	}

    float GetSpeed() {
        return Mathf.Log( camX + 1) * 2;
    }

    public static Vector2 GetViewSizeAtDistance(float aDist) {
        float frustumHeight = 2f * aDist * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
        return new Vector2(frustumHeight * Camera.main.aspect, frustumHeight);
    }
}
