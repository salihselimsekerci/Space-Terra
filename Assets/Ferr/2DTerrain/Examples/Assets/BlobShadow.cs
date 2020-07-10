using UnityEngine;
using System.Collections;

public class BlobShadow : MonoBehaviour {
	public GameObject shadowedObject;
    public Vector3    offset      = Vector3.zero;
    public float      maxDistance = 5;
    public float      scaleTo     = 1.5f;
    public bool       fadeOut     = true;
    public bool       use3D       = false;

	Renderer   renderCom;
	Collider   col;
#if !(UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
	Collider2D col2D;
#endif

	void Awake() {
		renderCom = GetComponent<Renderer>();
	}

    void Start() {
	    col   = shadowedObject.GetComponentInChildren<Collider>();
#if !(UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
	    col2D = shadowedObject.GetComponentInChildren<Collider2D>();
#endif
    }

	void LateUpdate () {
        Vector3 pos = shadowedObject.transform.position;

        if (use3D) {
            Ray          r    = new Ray(pos, new Vector3(0, -1, 0));
            RaycastHit[] hits = Physics.RaycastAll(r, maxDistance);
            RaycastHit   hit  = new RaycastHit(); ;
            float        closest = maxDistance;
            bool         found   = false;
            
            for (int i = 0; i < hits.Length; i++) {
			    if (hits[i].collider != col && hits[i].distance <= closest) {
                    hit     = hits[i];
                    closest = hit.distance;
                    found   = true;
                }
			}

            if (found) {
                transform.position = hit.point + offset;
                FitGround(hit.normal);
                Modifiers(hit.distance / maxDistance);
				renderCom.enabled = true;
            } else {
				renderCom.enabled = false;
            }
        } else {
#if !(UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
            RaycastHit2D[] hits = Physics2D.RaycastAll(pos, new Vector2(0, -1), maxDistance);
            RaycastHit2D   hit  = new RaycastHit2D();
            float          closest  = maxDistance;
            bool           found    = false;

            for (int i = 0; i < hits.Length; i++) {
                float dist = ((Vector2)pos - hits[i].point).magnitude / maxDistance;
                if (hits[i].collider != col2D && dist <= closest) {
                    hit     = hits[i];
                    closest = dist;
                    found   = true;
                }
            }

            if (found) {
                transform.position = (Vector3)hit.point + offset;
                FitGround(hit.normal);
                Modifiers(closest);
				renderCom.enabled = true;
            } else {
				renderCom.enabled = false;
            }
#else
	        use3D = true;
#endif
        }
	}

    void Modifiers(float aPercent) {
        if (fadeOut) {
			Color c = renderCom.material.color;
            c.a = 1-aPercent;
			renderCom.material.color = c;
        }

        float s = Mathf.Lerp(1, scaleTo, aPercent);
        transform.localScale = new Vector3(s, s, s);
    }

    void FitGround(Vector3 aNormal) {
        transform.rotation = Quaternion.FromToRotation(Vector3.right, aNormal);

        if (transform.eulerAngles.y != 0) {
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - 270);
        } else {
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - 90);
        }
    }
}
