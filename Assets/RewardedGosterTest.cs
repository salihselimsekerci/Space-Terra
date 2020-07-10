using UnityEngine;

public class RewardedGosterTest : MonoBehaviour
{
    private int altin = 0;

    void OnGUI()
    {
        if (GUILayout.Button("Rewarded reklam göster"))
        {
            ReklamScript.RewardedReklamGoster(RewardedReklamGosterildi);
        }
    }

    void RewardedReklamGosterildi(GoogleMobileAds.Api.Reward odul)
    {
        altin += (int)odul.Amount;
        GameController.playerCoin += 50;
      
    }
}