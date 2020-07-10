#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Ferr {
	public static class LayerUtil {
		static SerializedObject GetLayerManager() {
			return new UnityEditor.SerializedObject(UnityEditor.AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
		}
		static SerializedProperty[] GetLayers(SerializedObject aLayerManager) {
			SerializedProperty[] result = new SerializedProperty[32];
			
			for (int i=0; i<32; i+=1) {
				string name = i < 8 ? "Builtin Layer "+i : "User Layer "+i;
				result[i]   = aLayerManager.FindProperty(name);
			}
			
			return result;
		}
		public static int GetOrCreateLayer(string aName) {
			SerializedObject     lMan   = GetLayerManager();
			SerializedProperty[] layers = GetLayers(lMan);
			for (int i = 0; i < layers.Length; i+=1) {
				if (layers[i].stringValue == aName) {
					return i;
				}
			}
			
			for (int i = 8; i < layers.Length; i+=1) {
				if (layers[i].stringValue == "") {
					layers[i].stringValue = aName;
					lMan.ApplyModifiedProperties();
					return i;
				}
			}
			
			return -1;
		}
		public static string GetLayerName(int aIndex) {
			if (aIndex < 0 || aIndex >= 32) return "";
			
			SerializedObject     lMan   = GetLayerManager();
			SerializedProperty[] layers = GetLayers(lMan);
			return layers[aIndex].stringValue;
		}
		public static void SetLayerName(int aIndex, string aName) {
			if (aIndex < 8 || aIndex >= 32) return;
			
			SerializedObject     lMan   = GetLayerManager();
			SerializedProperty[] layers = GetLayers(lMan);
			layers[aIndex].stringValue = aName;
			lMan.ApplyModifiedProperties();
		}
		public static int GetFirstUnnamedUserLayer() {
			SerializedObject     lMan   = GetLayerManager();
			SerializedProperty[] layers = GetLayers(lMan);
			for (int i = 8; i < layers.Length; i+=1) {
				if (layers[i].stringValue == "") {
					return i;
				}
			}
			return -1;
		}
	}
}

#endif