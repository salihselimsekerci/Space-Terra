using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    [Header("Butonlar")]
    public bool acik = false;
    public GameObject ayarlarbuton;
    public GameObject ayarlarslider;

   public void ayarlar()
    {
        if (ayarlarbuton!=true)
        {
            ayarlarslider.SetActive(true);
        }
    }
}
