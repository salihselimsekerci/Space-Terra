using UnityEngine;
using System.Collections;
using System;
using GoogleMobileAds.Api;

public class ReklamScript : MonoBehaviour
{
    private static ReklamScript instance = null;
     private GameObject GC;
    [Header("Ad Unit ID'ler")]
    public string bannerID;
    public string interstitialID;
    public string rewardedVideoID;

    [Header("Test Modu")]
    public bool testMod = false;
    public string testDeviceID;

    [Header("Diğer Ayarlar")]
    public bool cocuklaraYonelikReklamGoster = false;
    public AdPosition bannerPozisyonu = AdPosition.Top;

    private BannerView bannerReklam;
    private InterstitialAd interstitialReklam;

    private float interstitialIstekTimeoutZamani;
    private float rewardedVideoIstekTimeoutZamani;

    private IEnumerator interstitialGosterCoroutine;
    private IEnumerator rewardedVideoGosterCoroutine;

    public delegate void RewardedVideoOdul(Reward odul);
    private RewardedVideoOdul odulDelegate;

    void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (this != instance)
        {
            Destroy(gameObject);
            return;
        }
        GC=GameObject.FindGameObjectWithTag("GameController");
    }

    void Start()
    {
        if (this != instance)
            return;

        BannerReklamYukle();
        InterstitialReklamYukle();
        RewardedReklamYukle();
    }

    void BannerReklamYukle()
    {
        if (string.IsNullOrEmpty(bannerID))
            return;

        bannerReklam = new BannerView(bannerID, AdSize.SmartBanner, bannerPozisyonu);

        AdRequest reklamiAl = ReklamIstegiOlustur(testMod);
        bannerReklam.LoadAd(reklamiAl);

        bannerReklam.Hide();
    }

    void InterstitialReklamYukle()
    {
        if (string.IsNullOrEmpty(interstitialID))
            return;

        if (interstitialReklam != null)
            interstitialReklam.Destroy();

        interstitialReklam = new InterstitialAd(interstitialID);
        interstitialReklam.OnAdClosed += InterstitialDelegate;

        AdRequest reklamiAl = ReklamIstegiOlustur(testMod);
        interstitialReklam.LoadAd(reklamiAl);

        interstitialIstekTimeoutZamani = Time.realtimeSinceStartup + 10f;
    }

    void RewardedReklamYukle()
    {
        if (string.IsNullOrEmpty(rewardedVideoID))
            return;

        RewardBasedVideoAd rewardedReklam = RewardBasedVideoAd.Instance;
        rewardedReklam.OnAdClosed -= RewardedVideoDelegate;
        rewardedReklam.OnAdClosed += RewardedVideoDelegate;
        rewardedReklam.OnAdRewarded -= RewardedVideoOdullendir;
        rewardedReklam.OnAdRewarded += RewardedVideoOdullendir;

        AdRequest reklamiAl = ReklamIstegiOlustur(false);

        if (testMod) // test id: https://groups.google.com/d/msg/google-admob-ads-sdk/k-kVwVu2XBc/57YBTdxPBgAJ
            rewardedReklam.LoadAd(reklamiAl, "ca-app-pub-6442067588290070/4128212231");
        else
            rewardedReklam.LoadAd(reklamiAl, rewardedVideoID);

        rewardedVideoIstekTimeoutZamani = Time.realtimeSinceStartup + 30f;
    }

    AdRequest ReklamIstegiOlustur(bool testModu)
    {
        AdRequest.Builder reklamIstegi = new AdRequest.Builder();

        if (testModu)
            reklamIstegi.AddTestDevice(AdRequest.TestDeviceSimulator).AddTestDevice(testDeviceID);

        if (cocuklaraYonelikReklamGoster)
            reklamIstegi.TagForChildDirectedTreatment(true).AddExtra("is_designed_for_families", "true");

        return reklamIstegi.Build();
    }

    void InterstitialDelegate(object sender, EventArgs args)
    {
        InterstitialReklamYukle();
    }

    void RewardedVideoDelegate(object sender, EventArgs e)
    {
        RewardedReklamYukle();
    }

    void OnGUI()
    {
        Color c = GUI.color;
  
        //if( GUI.Button( new Rect( Screen.width / 2 - 150, 0, 300, 120 ), "Banner Goster" ) )
        //    ReklamScript.BannerGoster();
                
        //if( GUI.Button( new Rect( Screen.width / 2 - 150, 120, 300, 120 ), "Banner Gizle" ) )
        //    ReklamScript.BannerGizle();
          
        //GUI.color = InterstitialHazirMi() ? Color.green : Color.red;
        //if( GUI.Button( new Rect( Screen.width / 2 - 150, 240, 300, 120 ), "Interstitial Goster" ) )
        //    ReklamScript.InsterstitialGoster();
  
        //GUI.color = RewardedReklamHazirMi() ? Color.green : Color.red;
        //if( GUI.Button( new Rect( Screen.width / 2 - 150, 360, 300, 120 ), "Rewarded Goster" ) )
            //ReklamScript.RewardedReklamGoster( null );
  
        //GUI.color = c;
    }
    public void reklamgoster()
    {
        ReklamScript.RewardedReklamGoster(null);
        GC.GetComponent<GameController>().altinEkle();
    }
    public static void BannerGoster() // Banner Gösterceğin Zaman CAğır
    {
        if (instance == null)
            return;

        if (instance.bannerReklam == null)
            instance.BannerReklamYukle();

        instance.bannerReklam.Show();
    }

    public static void BannerGizle()
    {
        if (instance == null)
            return;

        if (instance.bannerReklam == null)
            return;

        instance.bannerReklam.Hide();
    }

    public static bool InterstitialHazirMi()
    {
        if (instance == null)
            return false;

        if (instance.interstitialReklam == null)
            return false;

        return instance.interstitialReklam.IsLoaded();
    }

    public static void InterstitialReklamAl()
    {
        if (instance == null)
            return;

        if (instance.interstitialReklam != null && instance.interstitialReklam.IsLoaded())
            return;

        instance.InterstitialReklamYukle();
    }

    public static void InsterstitialGoster()
    {
        if (instance == null)
            return;

        if (instance.interstitialReklam == null)
            instance.InterstitialReklamYukle();

        if (instance.interstitialGosterCoroutine != null)
        {
            instance.StopCoroutine(instance.interstitialGosterCoroutine);
            instance.interstitialGosterCoroutine = null;
        }

        if (instance.interstitialReklam.IsLoaded())
            instance.interstitialReklam.Show();
        else
        {
            if (Time.realtimeSinceStartup >= instance.interstitialIstekTimeoutZamani)
                instance.InterstitialReklamYukle();

            instance.interstitialGosterCoroutine = instance.InsterstitialGosterCoroutine();
            instance.StartCoroutine(instance.interstitialGosterCoroutine);
        }
    }

    public static bool RewardedReklamHazirMi()
    {
        if (instance == null)
            return false;

        return RewardBasedVideoAd.Instance.IsLoaded();
    }

    public static void RewardedReklamAl()
    {
        if (instance == null)
            return;

        if (RewardBasedVideoAd.Instance.IsLoaded())
            return;

        instance.RewardedReklamYukle();
    }

    public static void RewardedReklamGoster(RewardedVideoOdul odulFonksiyonu)
    {
        if (instance == null)
            return;

        if (instance.rewardedVideoGosterCoroutine != null)
        {
            instance.StopCoroutine(instance.rewardedVideoGosterCoroutine);
            instance.rewardedVideoGosterCoroutine = null;
        }

        instance.odulDelegate = odulFonksiyonu;

        RewardBasedVideoAd rewardedReklam = RewardBasedVideoAd.Instance;
        if (rewardedReklam.IsLoaded())
            rewardedReklam.Show();
        else
        {
            if (Time.realtimeSinceStartup >= instance.rewardedVideoIstekTimeoutZamani)
                instance.RewardedReklamYukle();

            instance.rewardedVideoGosterCoroutine = instance.RewardedVideoGosterCoroutine();
            instance.StartCoroutine(instance.rewardedVideoGosterCoroutine);
        }
    }

    IEnumerator InsterstitialGosterCoroutine()
    {
        float istekTimeoutAni = Time.realtimeSinceStartup + 2.5f;
        while (!interstitialReklam.IsLoaded())
        {
            if (Time.realtimeSinceStartup > istekTimeoutAni)
                yield break;

            yield return null;
        }

        interstitialReklam.Show();
    }

    IEnumerator RewardedVideoGosterCoroutine()
    {
        RewardBasedVideoAd rewardedReklam = RewardBasedVideoAd.Instance;
        float istekTimeoutAni = Time.realtimeSinceStartup + 10f;
        while (!rewardedReklam.IsLoaded())
        {
            if (Time.realtimeSinceStartup > istekTimeoutAni)
                yield break;

            yield return null;
        }

        rewardedReklam.Show();
    }

    void RewardedVideoOdullendir(object sender, Reward odul)
    {
        if (odulDelegate != null)
            odulDelegate(odul);

        GameController.playerCoin += 50;
        Debug.Log(GameController.playerCoin);
    }
}