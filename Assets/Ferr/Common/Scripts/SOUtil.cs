using UnityEngine;
using System.Collections;

namespace Ferr {
	public class SOUtil {
		#if UNITY_EDITOR
		public static void CreateAsset(System.Type aType, string aBaseName) {
			ScriptableObject style = ScriptableObject.CreateInstance(aType);
			
			string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeInstanceID);
			if (System.IO.Path.GetExtension(path) != "") path = System.IO.Path.GetDirectoryName(path);
			if (path == "") path = "Assets";
			
			string name = path + "/"+aBaseName+".asset";
			int id = 0;
			while (UnityEditor.AssetDatabase.LoadAssetAtPath(name, aType) != null) {
				id += 1;
				name = path + "/" + aBaseName + id + ".asset";
			}
			
			UnityEditor.AssetDatabase.CreateAsset(style, name);
			UnityEditor.AssetDatabase.SaveAssets();
			
			UnityEditor.EditorUtility.FocusProjectWindow();
			UnityEditor.Selection.activeObject = style;
		}
		#endif
	}
}