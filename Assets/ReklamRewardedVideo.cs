using UnityEngine;
using System;
using GoogleMobileAds.Api;

public class ReklamRewardedVideo : MonoBehaviour
{
    private int altin = 0;

    void Start()
    {
        RewardBasedVideoAd reklamObjesi = RewardBasedVideoAd.Instance;

        YeniReklamAl(null, null);

        reklamObjesi.OnAdClosed -= YeniReklamAl;
        reklamObjesi.OnAdClosed += YeniReklamAl;
        reklamObjesi.OnAdRewarded -= OyuncuyuOdullendir;
        reklamObjesi.OnAdRewarded += OyuncuyuOdullendir;
    }

  public void reklamgoster()
    {
        RewardBasedVideoAd.Instance.Show();
    }

    public void YeniReklamAl(object sender, EventArgs args)
    {
        RewardBasedVideoAd reklamObjesi = RewardBasedVideoAd.Instance;

        AdRequest reklamiAl = new AdRequest.Builder().Build();
        reklamObjesi.LoadAd(reklamiAl, "ca-app-pub-6442067588290070/4128212231");
    }

    private void OyuncuyuOdullendir(object sender, Reward odul)
    {
        Debug.Log("Ödül türü: " + odul.Type);

        altin += (int)odul.Amount;
    }
}