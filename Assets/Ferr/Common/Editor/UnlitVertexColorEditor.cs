using UnityEditor;
using UnityEngine;
using System.Collections;

public class UnlitVertexColorEditor : MaterialEditor {
	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();
		if (!isVisible)
			return;
		
		Material targetMat = target as Material;
		string[] keyWords  = targetMat.shaderKeywords;
		
		bool noTex = System.Array.IndexOf(keyWords, "NO_TEX") != -1;
		EditorGUI.BeginChangeCheck();
		noTex = EditorGUILayout.Toggle ("Don't use texture", noTex);
		if (EditorGUI.EndChangeCheck()) {
			string[] keywords = new string[] { noTex ? "NO_TEX" : "USE_TEX" };
			targetMat.shaderKeywords = keywords;
			EditorUtility.SetDirty (targetMat);
		}
	}
}