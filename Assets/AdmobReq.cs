using UnityEngine;
using System.Collections;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;

public class AdmobReq : MonoBehaviour {

	private BannerView bannerView;
	private InterstitialAd interstitial;



	public void Start () {
        DontDestroyOnLoad (this);
		RequestInterstitial ();
       


    }
    int reklamsayi;
    public void reklamkontrol ()
    {
     
        if (reklamsayi % 4 == 0)
        {
            ShowInterstitial();
        }


    }
	
	private void RequestBanner()
	{
		#if UNITY_ANDROID
		string adUnitId = "";
		#elif UNITY_IPHONE
		string adUnitId = "INSERT_IOS_BANNER_AD_UNIT_ID_HERE";
		#else
		string adUnitId = "unexpected_platform";
		#endif
		
		// Create a 320x50 banner at the top of the screen.
		bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
		// Register for ad events.
		bannerView.OnAdLoaded += HandleAdLoaded;
		bannerView.OnAdFailedToLoad += HandleAdFailedToLoad;
		bannerView.OnAdOpening += HandleAdOpened;
		bannerView.OnAdClosed += HandleAdClosing;
		bannerView.OnAdClosed += HandleAdClosed;
		bannerView.OnAdLeavingApplication += HandleAdLeftApplication;
		// Load a banner ad.
		bannerView.LoadAd(createAdRequest());
	}
	
	private void RequestInterstitial()
	{

#if UNITY_ANDROID
		string adUnitId = "ca-app-pub-8608124455720890/7832528805";
#elif UNITY_IPHONE
		string adUnitId = "INSERT_IOS_INTERSTITIAL_AD_UNIT_ID_HERE";
#else
        string adUnitId = "unexpected_platform";
		#endif

		// Create an interstitial.
		interstitial = new InterstitialAd(adUnitId);
		// Register for ad events.
		interstitial.OnAdLoaded += HandleInterstitialLoaded;
		interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
		interstitial.OnAdOpening += HandleInterstitialOpened;
		interstitial.OnAdClosed += HandleInterstitialClosing;
		interstitial.OnAdClosed += HandleInterstitialClosed;
		interstitial.OnAdLeavingApplication += HandleInterstitialLeftApplication;
		// Load an interstitial ad.
		interstitial.LoadAd(createAdRequest());

	}
		// bu reklm gösterimi tamam diyo ya , o reklamı gösterıyo yanı demi şuan = ? evet fakat sen oyun başlangıcına koyarsan bunu öper seni google :) AS:DAS:D hala anlamadım mk kanka şşimmdi ben ontriggerla cagırcam bunu qama bu oyun başlar başlamaz acılıyo neden mk 
	// Returns an ad request with custom ad targeting.
	private AdRequest createAdRequest()
	{
		return new AdRequest.Builder()
				.AddKeyword("game")
				.SetGender(Gender.Male)
				.SetBirthday(new DateTime(1985, 1, 1))
				.TagForChildDirectedTreatment(false)
				.AddExtra("color_bg", "9B30FF")
		.AddTestDevice(AdRequest.TestDeviceSimulator)
		.AddTestDevice("3e379b7afa67f945")
				.Build();
		
	}
	
	public void ShowInterstitial()
	{ 
		if (interstitial.IsLoaded())
		{
			interstitial.Show();
			Debug.LogError("yüklenmiş gösteriliyor demmekki sorun yok ");
		}
		else
		{
			Debug.LogError ("yüklenmemiş");
		}
		RequestInterstitial ();
	}

	
	#region Banner callback handlers
	
	public void HandleAdLoaded(object sender, EventArgs args)
	{
		print("HandleAdLoaded event received.");
	}
	
	public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		print("HandleFailedToReceiveAd event received with message: " + args.Message);
	}
	
	public void HandleAdOpened(object sender, EventArgs args)
	{
		print("HandleAdOpened event received");
	}
	
	void HandleAdClosing(object sender, EventArgs args)
	{
		print("HandleAdClosing event received");
	}
	
	public void HandleAdClosed(object sender, EventArgs args)
	{
		print("HandleAdClosed event received");
	}
	
	public void HandleAdLeftApplication(object sender, EventArgs args)
	{
		print("HandleAdLeftApplication event received");
	}
	
	#endregion
	void hidePanel(){
	
	}
	#region Interstitial callback handlers
	
	public void HandleInterstitialLoaded(object sender, EventArgs args)
	{
		Debug.LogError ("Rekalm gösterimi  loading işlemitamam");
		
		//ShowInterstitial ();
	}
	
	public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		print("HandleInterstitialFailedToLoad event received with message: " + args.Message);
		Debug.LogError ("Rekalm gösterimi hatası");
		Invoke ("hidePanel", 3f);

	}
	
	public void HandleInterstitialOpened(object sender, EventArgs args)
	{
		print("HandleInterstitialOpened event received");
		
	}
	
	void HandleInterstitialClosing(object sender, EventArgs args)
	{
		print("HandleInterstitialClosing event received");
	}
	
	public void HandleInterstitialClosed(object sender, EventArgs args)
	{
		print("HandleInterstitialClosed event received");
		Debug.LogError ("Rekalm gösteriminden çıkıldı");
		
		Invoke ("hidePanel", 3f);
		
		
	}
	
	public void HandleInterstitialLeftApplication(object sender, EventArgs args)
	{
		print("HandleInterstitialLeftApplication event received");
	}
	
	#endregion
}