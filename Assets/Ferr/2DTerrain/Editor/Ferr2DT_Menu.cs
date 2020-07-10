using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;

public class Ferr2DT_Menu {
	public enum SnapType {
		SnapGlobal,   // Snap to global coordinates
		SnapLocal,    // Snap to coordinates relative to transform
		SnapRelative  // Default, Unity-like snapping
	}
	
	static bool     prefsLoaded = false;
	static bool     hideMeshes  = true;
	static float    pathScale   = 1;
	static SnapType snapMode    = SnapType.SnapRelative;
	static float    smartSnapDist = 0.4f;
	static int      updateTerrainSkipFrames = 0;
	static int      ppu           = 64;
	static bool     smoothTerrain = false;
	
	public static bool HideMeshes {
		get { LoadPrefs(); return hideMeshes; }
		set { hideMeshes = value; SavePrefs(); }
	}
	public static float PathScale {
		get { LoadPrefs(); return pathScale;  }
	}
	public static SnapType SnapMode {
		get { LoadPrefs(); return snapMode;   }
		set { snapMode = value; SavePrefs(); }
	}
	public static float SmartSnapDist {
		get { LoadPrefs(); return smartSnapDist;   }
		set { smartSnapDist = value; SavePrefs(); }
	}
	public static int UpdateTerrainSkipFrames {
		get { LoadPrefs(); return updateTerrainSkipFrames; }
	}
	public static int PPU {
		get { LoadPrefs(); return ppu; }
	}
	public static bool SmoothTerrain {
		get { LoadPrefs(); return smoothTerrain; }
	}
	
    [MenuItem("GameObject/Create Ferr2D Terrain/Create Physical 2D Terrain %t", false, 0)]
    static void MenuAddPhysicalTerrain() {
        Ferr2DT_MaterialSelector.Show(AddPhysicalTerrain);
    }
    static void AddPhysicalTerrain(Ferr2DT_TerrainMaterial aMaterial) {
        GameObject obj = CreateBaseTerrain(aMaterial, true);
        Selection.activeGameObject = obj;
        EditorGUIUtility.PingObject(obj);
    }


    [MenuItem("GameObject/Create Ferr2D Terrain/Create Decorative 2D Terrain %#t", false, 0)]
    static void MenuAddDecoTerrain() {
        Ferr2DT_MaterialSelector.Show(AddDecoTerrain);
    }
    static void AddDecoTerrain(Ferr2DT_TerrainMaterial aMaterial) {
        GameObject obj = CreateBaseTerrain(aMaterial, false);
        Selection.activeGameObject = obj;
        EditorGUIUtility.PingObject(obj);
    }
    static GameObject CreateBaseTerrain(Ferr2DT_TerrainMaterial aMaterial, bool aCreateColliders) {
        GameObject          obj     = new GameObject("New Terrain");
        Ferr2D_Path         path    = obj.AddComponent<Ferr2D_Path        >();
        Ferr2DT_PathTerrain terrain = obj.AddComponent<Ferr2DT_PathTerrain>();
        
        bool hasEdges = aMaterial.Has(Ferr2DT_TerrainDirection.Bottom) ||
                        aMaterial.Has(Ferr2DT_TerrainDirection.Left) ||
                        aMaterial.Has(Ferr2DT_TerrainDirection.Right);

        if (hasEdges) {
            path.Add(new Vector2(-6, -3));
            path.Add(new Vector2(-6,  3));
            path.Add(new Vector2( 6,  3));
            path.Add(new Vector2( 6, -3));
            path.closed = true;
        } else {
            path.Add(new Vector2(-6, 6));
            path.Add(new Vector2( 6, 6));
            terrain.splitCorners = false;
            path.closed = false;
        }
        
        if (aMaterial.fillMaterial != null) {
            if (hasEdges) {
                terrain.fill = Ferr2DT_FillMode.Closed;
            } else {
                terrain.fill = Ferr2DT_FillMode.Skirt;
                terrain.splitCorners = true;
            }
        } else {
            terrain.fill = Ferr2DT_FillMode.None;
        }
        terrain.smoothPath     = SmoothTerrain;
        terrain.pixelsPerUnit  = PPU;
        terrain.createCollider = aCreateColliders;
        terrain.SetMaterial (aMaterial);
        terrain.Build(true);

        obj.transform.position = GetSpawnPos();

        return obj;
    }
	

	[MenuItem("GameObject/Create Ferr2D Terrain/Create Terrain Material", false, 11)]
    static void MenuAddTerrainMaterial() {
        AddTerrainMaterial(GetCurrentPath());
    }
    [MenuItem("Assets/Create/Ferr2D Terrain Material", false, 101)]
    static void ContextAddTerrainMaterial() {
        AddTerrainMaterial(GetCurrentPath());
    }
    static void AddTerrainMaterial(string aFolder) {
        GameObject obj = new GameObject("New Terrain Material");
        obj.AddComponent<Ferr2DT_TerrainMaterial>();
        string name = aFolder + "/NewTerrainMaterial.prefab";
        int id = 0;
        while (AssetDatabase.LoadAssetAtPath(name, typeof(GameObject)) != null) {
            id += 1;
            name = aFolder + "/NewTerrainMaterial" + id + ".prefab";
        }
        GameObject prefab = PrefabUtility.CreatePrefab(name, obj);
        GameObject.DestroyImmediate(obj);

        Selection.activeGameObject = prefab;
        EditorGUIUtility.PingObject(prefab);
    }


    [MenuItem("Assets/Prebuild Ferr2D Terrain", false, 101)]
    static void MenuPrebuildTerrain() {
        Debug.Log("Prebuilding...");
        Ferr2DT_Builder.SaveTerrains();
        Debug.Log("Prebuilding complete.");
    }

    [MenuItem("Assets/Rebuild Ferr2D Component Cache", false, 101)]
    static void MenuRebuildComponentCache() {
        Ferr.ComponentTracker.RecreateCache();
    }
	
	
	[PreferenceItem("Ferr")]
	static void Ferr2DT_PreferencesGUI() 
	{
		LoadPrefs();
		
		pathScale  = EditorGUILayout.FloatField   ("Path vertex scale",        pathScale );
		updateTerrainSkipFrames = EditorGUILayout.IntField("Update Terrain Every X Frames", updateTerrainSkipFrames);
		smartSnapDist = EditorGUILayout.FloatField("Smart Snap Distance",      smartSnapDist);
		ppu           = EditorGUILayout.IntField  ("Default PPU",              ppu);
		smoothTerrain = EditorGUILayout.Toggle    ("Default smoothed terrain", smoothTerrain);
		
		if (GUI.changed) {
			SavePrefs();
		}
	}
	
	static void LoadPrefs() {
		if (prefsLoaded) return;
		prefsLoaded   = true;
		hideMeshes    = EditorPrefs.GetBool ("Ferr_hideMeshes", true);
		pathScale     = EditorPrefs.GetFloat("Ferr_pathScale",  1   );
		updateTerrainSkipFrames = EditorPrefs.GetInt("Ferr_updateTerrainAlways", 0);
		snapMode      = (SnapType)EditorPrefs.GetInt("Ferr_snapMode", (int)SnapType.SnapRelative);
		ppu           = EditorPrefs.GetInt  ("Ferr_ppu", 64);
		smoothTerrain = EditorPrefs.GetBool ("Ferr_smoothTerrain", false);
		smartSnapDist = EditorPrefs.GetFloat("Ferr_smartSnapDist", 0.4f);
	}
	static void SavePrefs() {
		if (!prefsLoaded) return;
		EditorPrefs.SetBool ("Ferr_hideMeshes", hideMeshes);
		EditorPrefs.SetFloat("Ferr_pathScale",  pathScale );
		EditorPrefs.SetInt  ("Ferr_updateTerrainAlways", updateTerrainSkipFrames);
		EditorPrefs.SetInt  ("Ferr_snapMode",   (int)snapMode);
		EditorPrefs.SetInt  ("Ferr_ppu",        ppu       );
		EditorPrefs.SetBool ("Ferr_smoothTerrain", smoothTerrain);
		EditorPrefs.SetFloat("Ferr_smartSnapDist", smartSnapDist);
	}

    static Vector3 GetSpawnPos() {
        Plane   plane  = new Plane(new Vector3(0, 0, -1), 0);
        float   dist   = 0;
        Vector3 result = new Vector3(0, 0, 0);
        //Ray     ray    = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
        if (plane.Raycast(ray, out dist)) {
            result = ray.GetPoint(dist);
        }
        return new Vector3(result.x, result.y, 0);
    }
    static string  GetCurrentPath() {
        string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
        if (Path.GetExtension(path) != "") path = Path.GetDirectoryName(path);
        if (path                    == "") path = "Assets";
        return path;
    }
}
