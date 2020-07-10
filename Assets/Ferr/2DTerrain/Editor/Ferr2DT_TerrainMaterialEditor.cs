using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

[CustomEditor(typeof(Ferr2DT_TerrainMaterial))]
public class Ferr2DT_TerrainMaterialEditor : Editor {

	public override void OnInspectorGUI() {
		Undo.RecordObject(target, "Modified Terrain Material");
        
		Ferr2DT_TerrainMaterial mat = target as Ferr2DT_TerrainMaterial;
		Material                newMat;
		
		newMat = mat.edgeMaterial = (Material)EditorGUILayout.ObjectField("Edge Material", mat.edgeMaterial, typeof(Material), true);
		if (mat.edgeMaterial != newMat) {
			mat.edgeMaterial  = newMat;
			Ferr2DT_TerrainMaterialUtility.CheckMaterialMode(mat.edgeMaterial, TextureWrapMode.Clamp);
		}
		
		newMat = (Material)EditorGUILayout.ObjectField("Fill Material", mat.fillMaterial, typeof(Material), true);
		if (mat.fillMaterial != newMat) {
			mat.fillMaterial  = newMat;
			Ferr2DT_TerrainMaterialUtility.CheckMaterialMode(mat.fillMaterial, TextureWrapMode.Repeat);
		}
		
        if (mat.edgeMaterial == null) EditorGUILayout.HelpBox("Please add an edge material to enable the material editor!", MessageType.Warning);
        else {
            if (GUILayout.Button("Open Material Editor")) Ferr2DT_TerrainMaterialWindow.Show(mat);
        }
		if (GUI.changed) {
			EditorUtility.SetDirty(target);

            Ferr2DT_PathTerrain[] terrain = GameObject.FindObjectsOfType(typeof(Ferr2DT_PathTerrain)) as Ferr2DT_PathTerrain[];
            for (int i = 0; i < terrain.Length; i++)
            {
                if(terrain[i].TerrainMaterial == mat)
                    terrain[i].Build(true);
            }
		}
	}
}