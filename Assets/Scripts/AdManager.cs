//using UnityEngine;
//using System.Collections;
//using GoogleMobileAds;

//public class AdManager : MonoBehaviour {

//	/// <summary>
//	/// This is the main AdMob manager class that can be used/modified by you.
//	/// You can set different IDs for different types of Ads (obtainable from Admob developer panel)
//	/// And you can define new public functions here and call them later inside your game
//	/// </summary>

//	public string admobBannerID = 		"ca-app-pub-3940256099942544/2934735716";
//	public string admobInterstitialID = "ca-app-pub-3940256099942544/2934735716";
//	public string admobVideoID =		"ca-app-pub-3940256099942544/2934735716";

//	void Awake () {
//		DontDestroyOnLoad(gameObject);
//	}

//	void Start () {
//		initAdmob();
//	}

//	Admob ad;
//	//bool isAdmobInited = false;

//	void initAdmob() {
		
//		//  isAdmobInited = true;
//		ad = Admob.Instance();
//		ad.bannerEventHandler += onBannerEvent;
//		ad.interstitialEventHandler += onInterstitialEvent;
//		ad.rewardedVideoEventHandler += onRewardedVideoEvent;
//		ad.nativeBannerEventHandler += onNativeBannerEvent;
//		ad.initAdmob(admobBannerID, admobInterstitialID);
//		//ad.setTesting(true);
//		Debug.Log("Admob Inited.");

//		//showBannerAd (always)
//		Admob.Instance().showBannerRelative(AdSize.Banner, AdPosition.BOTTOM_CENTER, 0);

//		//cache an Interstitial ad for later use
//		ad.loadInterstitial();
//	}

//	//gets called from other classes inside the game
//	public void showInterstitial() {

//		print ("Request for Full AD.");
//		if (ad.isInterstitialReady()) {
//			ad.showInterstitial();
//		}
//	}


//	void onInterstitialEvent(string eventName, string msg)
//	{
//		Debug.Log("handler onAdmobEvent---" + eventName + "   " + msg);
//		if (eventName == AdmobEvent.onAdLoaded)
//		{
//			Admob.Instance().showInterstitial();
//		}
//	}
//	void onBannerEvent(string eventName, string msg)
//	{
//		Debug.Log("handler onAdmobBannerEvent---" + eventName + "   " + msg);
//	}
//	void onRewardedVideoEvent(string eventName, string msg)
//	{
//		Debug.Log("handler onRewardedVideoEvent---" + eventName + "   " + msg);
//	}
//	void onNativeBannerEvent(string eventName, string msg)
//	{
//		Debug.Log("handler onAdmobNativeBannerEvent---" + eventName + "   " + msg);
//	}
//}
