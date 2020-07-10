using UnityEngine;
using System.Collections;

public class AdmobGoster : MonoBehaviour {
  void Start()
    {
       FindObjectOfType<AdmobReq>().GetComponent<AdmobReq>().ShowInterstitial();


    }


}
