using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[InitializeOnLoad]
public class Ferr2DT_Builder {
    static bool wasPlaying;

    static Ferr2DT_Builder() {
        EditorApplication.playmodeStateChanged += StateChanged;
    }

    static void StateChanged() {
        if ((!wasPlaying && EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying) || EditorApplication.isCompiling) {
            SaveTerrains(); 
        }
        wasPlaying = EditorApplication.isPlayingOrWillChangePlaymode;
    }

    public static void SaveTerrains() {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        List<Ferr2DT_PathTerrain> terrains = Ferr.ComponentTracker.GetComponents<Ferr2DT_PathTerrain>(); //Ferr2DT_AssetTracker.GetPrefabs();
        for (int i = 0; i < terrains.Count; i++) {
            terrains[i].Build(true);;
        }

        sw.Stop();
	    if (terrains.Count > 0 && sw.Elapsed.TotalMilliseconds > 500) {
            Debug.Log("Prebuilding terrain prefabs ("+terrains.Count+"): " + Mathf.Round((float)sw.Elapsed.TotalMilliseconds) + "ms");
        }
    }
}
