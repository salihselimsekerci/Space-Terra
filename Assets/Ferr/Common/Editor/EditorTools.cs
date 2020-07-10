using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Ferr {
	public class EditorTools {
		public enum MultiType {
			None,
			All,
			Some
		}
		
        #region Fields
        static int    handleID       = 0;
		static int    selectedHandle = -1;

		static Material _CapMaterial2D = null;
		static Material _CapMaterial3D = null;
		
		internal static Matrix4x4 capDir = Matrix4x4.identity;
		#endregion

		#region Properties
		static Material CapMaterial2D {
			get {
				if (_CapMaterial2D == null) { _CapMaterial2D = new Material(Shader.Find("Hidden/Ferr Gizmo Shader 2D")); }
				return _CapMaterial2D;
			}
		}
		static Material CapMaterial3D {
			get {
				if (_CapMaterial3D == null) { _CapMaterial3D = new Material(Shader.Find("Hidden/Ferr Gizmo Shader 3D")); }
				return _CapMaterial3D;
			}
		}
		#endregion

		#region General utilities
		public static Vector3   GetUnitySnap() {
            return new Vector3(EditorPrefs.GetFloat("MoveSnapX", 1), EditorPrefs.GetFloat("MoveSnapY", 1), EditorPrefs.GetFloat("MoveSnapZ", 1));
        }
		public static MultiType IsStatic(Object[] aItems) {
			MultiType result = MultiType.None;
			for (int i = 0; i < aItems.Length; ++i) {
				if ((aItems[i] as Component).gameObject.isStatic) {
					if (result == MultiType.None) result = MultiType.All;
				} else {
					if (result == MultiType.All ) result = MultiType.Some;
				}
			}
			return result;
		}
        #endregion

        #region File and resource methods
        public static string    GetFerrDirectory   () {
	        Dummy      dummy  = ScriptableObject.CreateInstance<Dummy>();
	        MonoScript script = MonoScript.FromScriptableObject(dummy);
	        string     path   = AssetDatabase.GetAssetPath(script);
	        
	        dummy.hideFlags = HideFlags.HideAndDontSave;
	        
	        int    start = path.IndexOf("Ferr/Common/Editor");
	        string dir   = path.Substring(0, start);
	        ScriptableObject.DestroyImmediate(dummy);
	        
	        return dir;
		}
	    public static Texture2D GetGizmo           (string aFileName) {
		    Texture2D tex = AssetDatabase.LoadAssetAtPath(GetFerrDirectory()+"Ferr/" + aFileName, typeof(Texture2D)) as Texture2D;
	        if (tex == null) {
	            tex = EditorGUIUtility.whiteTexture;
	            Debug.Log("Couldn't load Gizmo tex " + aFileName);
	        }
	        return tex;
	    }
	    public static List<T>   GetPrefabsOfType<T>() where T:Component {
	        string[] fileNames  = System.IO.Directory.GetFiles(Application.dataPath, "*.prefab", System.IO.SearchOption.AllDirectories);
	        int      pathLength = Application.dataPath.Length + 1;
	        List<T>  result     = new List<T>();
	
	        for (int i = fileNames.Length; i > 0; i--) {
	            fileNames[i - 1] = "Assets\\" + fileNames[i - 1].Substring(pathLength);
	            GameObject go = UnityEditor.AssetDatabase.LoadAssetAtPath(fileNames[i - 1], typeof(GameObject)) as GameObject;
	            if (go != null) {
	                T source = go.GetComponent<T>();
	                if (source) result.Add(source);
	            }
	        }
	        return result;
	    }
        public static Material  GetDefaultMaterial () {
			System.Reflection.BindingFlags bfs                            = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static;
	        System.Reflection.MethodInfo   getBuiltinExtraResourcesMethod = typeof( EditorGUIUtility ).GetMethod( "GetBuiltinExtraResource", bfs );
	        #if UNITY_5
	        string matName = "Default-Material.mat";
	        #else
	        string matName = "Default-Diffuse.mat";
	        #endif
	        return (Material)getBuiltinExtraResourcesMethod.Invoke( null, new object[] { typeof( Material ), matName } );
		}
        #endregion

        #region UI Drawing methods
        public static void DrawRect (Rect aRect) {
	        DrawRect(aRect, new Rect(0,0,1,1));
	    }
	    public static void DrawRect (Rect aRect, Rect aBounds) {
			float x      = aBounds.x + aRect.x * aBounds.width;
			float y      = aBounds.y + aRect.y * aBounds.height;
			float width  = aRect.width  * aBounds.width;
			float height = aRect.height * aBounds.height;
			
			GUI.DrawTexture(new Rect(x,       y,         width, 1     ), EditorGUIUtility.whiteTexture);
			GUI.DrawTexture(new Rect(x,      (y+height), width, 1     ), EditorGUIUtility.whiteTexture);
			GUI.DrawTexture(new Rect(x,       y,         1,     height), EditorGUIUtility.whiteTexture);
			GUI.DrawTexture(new Rect(x+width, y,         1,     height), EditorGUIUtility.whiteTexture);
		}
	    public static void DrawHLine(Vector2 aPos, float aLength) {
	        GUI.DrawTexture(new Rect(aPos.x, aPos.y, aLength, 1), EditorGUIUtility.whiteTexture);
	    }
	    public static void DrawVLine(Vector2 aPos, float aLength) {
	        GUI.DrawTexture(new Rect(aPos.x, aPos.y, 1, aLength), EditorGUIUtility.whiteTexture);
	    }
		public static void DrawDepthLine(Vector3 aP1, Vector3 aP2) {
			if (Event.current.type != EventType.Repaint) {
				return;
			}
			CapMaterial3D.SetPass(1);

			GL.PushMatrix();
			GL.MultMatrix(Handles.matrix);
			GL.Begin(GL.LINES);
			GL.Color(Handles.color);
			GL.Vertex(aP1);
			GL.Vertex(aP2);
			GL.End();
			GL.PopMatrix();
		}
        public static void Box      (int aBorder, System.Action inside) {
            Box(aBorder, inside, 0, 0);
        }
	    public static void Box      (int aBorder, System.Action inside, int aWidthOverride, int aHeightOverride)
	    {
	        Rect r = EditorGUILayout.BeginHorizontal(GUILayout.Width(aWidthOverride));
	        if (aWidthOverride != 0)
	        {
	            r.width = aWidthOverride;
	        }
	        GUI.Box(r, GUIContent.none);
	        GUILayout.Space(aBorder);
	        if (aHeightOverride != 0)
	            EditorGUILayout.BeginVertical(GUILayout.Height(aHeightOverride));
	        else
	            EditorGUILayout.BeginVertical();
	        GUILayout.Space(aBorder);
	        inside();
	        GUILayout.Space(aBorder);
	        EditorGUILayout.EndVertical();
	        GUILayout.Space(aBorder);
	        EditorGUILayout.EndHorizontal();
	    }
        #endregion

        #region Custom handles
        public static Rect    UVRegionRect (Rect    aRect, Rect aBounds) {
	        Vector2 pos = RectHandle(new Vector2(aBounds.x+aRect.x, aBounds.y+aRect.y), aRect, aBounds);
	        aRect.x = pos.x - aBounds.x;
	        aRect.y = pos.y - aBounds.y;
	
	        float left  = MouseHandle(new Vector2(aBounds.x+aRect.x,   aBounds.y+aRect.y+aRect.height/2), 10).x - aBounds.x;
	        float right = MouseHandle(new Vector2(aBounds.x+aRect.xMax,aBounds.y+aRect.y+aRect.height/2), 10).x - aBounds.x;
	
	        float top    = MouseHandle(new Vector2(aBounds.x+aRect.x+aRect.width/2,aBounds.y+aRect.y   ), 10).y - aBounds.y;
	        float bottom = MouseHandle(new Vector2(aBounds.x+aRect.x+aRect.width/2,aBounds.y+aRect.yMax), 10).y - aBounds.y;
	
	        return new Rect(left, top, right-left, bottom-top);
	    }
	    public static Vector2 MouseHandle  (Vector2 aPos, int aSize) {
	        Rect button = new Rect(aPos.x-aSize/2, aPos.y-aSize/2, aSize, aSize);
	        GUI.DrawTexture(button, EditorGUIUtility.whiteTexture);
	        return RectHandle(aPos, button);
	    }
	    public static Vector2 RectHandle   (Vector2 aPos, Rect aRect) {
	        return RectHandle(aPos, aRect, new Rect(0,0,1,1));
	    }
	    public static Vector2 RectHandle   (Vector2 aPos, Rect aRect, Rect aBounds) {
	        handleID += 1;
	
	        EditorTools.DrawRect(new Rect(aBounds.x+aRect.x, aBounds.y+aRect.y, aRect.width, aRect.height));
	        if (Event.current.type == EventType.MouseDown) {
	            if (new Rect(aBounds.x+aRect.x, aBounds.y+aRect.y, aRect.width, aRect.height).Contains(Event.current.mousePosition)) {
	                selectedHandle = handleID;
	            }
	        }
	        if (selectedHandle == handleID && Event.current.type == EventType.MouseDrag) {
	            aPos += Event.current.delta;
	        }
	        return aPos;
	    }
        public static bool    ResetHandles () {
	        handleID = 0;
	        if (Event.current.type == EventType.MouseUp) {
	            selectedHandle = -1;
	            return true;
	        }
	        return false;
	    }
	    public static bool    HandlesMoving() {
	        return selectedHandle != -1;
	    }
        #endregion

        #region Cap methods
		public static void ImageCapBase(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, Texture2D aTex)
		{
			if (Event.current.type != EventType.Repaint) {
				return;
			}

			aPosition = Handles.matrix.MultiplyPoint(aPosition);
			Vector3 right = Camera.current.transform.right * aSize;
			Vector3 top   = Camera.current.transform.up    * aSize;
			CapMaterial2D.mainTexture = aTex;
			CapMaterial2D.SetPass(0);
			
			GL.Begin(GL.QUADS);
			GL.Color(Handles.color);
			GL.TexCoord2(1, 1);
			GL.Vertex(aPosition + right + top);
			
			GL.TexCoord2(1, 0);
			GL.Vertex(aPosition + right - top);
			
			GL.TexCoord2(0, 0);
			GL.Vertex(aPosition - right - top);
			
			GL.TexCoord2(0, 1);
			GL.Vertex(aPosition - right + top);
			
			GL.End();
		}
		public static void CubeCapDirBase(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, Vector3 aScale) {
			if (Event.current.type != EventType.Repaint) {
				return;
			}

			aPosition  = Handles.matrix.MultiplyPoint(aPosition);
			
			for (int i = 0; i < CapMaterial3D.passCount; ++i) {
				CapMaterial3D.SetPass(i);
				
				GL.PushMatrix();
				GL.Begin(GL.QUADS);
				GL.Color(Handles.color);
				
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1, 1,-1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1, 1,-1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1,-1,-1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1,-1,-1) * aSize, aScale)));
				
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1,-1, 1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1,-1, 1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1, 1, 1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1, 1, 1) * aSize, aScale)));
				
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1,-1,-1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1,-1,-1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1,-1, 1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1,-1, 1) * aSize, aScale)));
				
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1, 1, 1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1, 1, 1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1, 1,-1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1, 1,-1) * aSize, aScale)));
				
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1,-1, 1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1, 1, 1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1, 1,-1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1,-1,-1) * aSize, aScale)));
				
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1,-1,-1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1, 1,-1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1, 1, 1) * aSize, aScale)));
				GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1,-1, 1) * aSize, aScale)));
				
				GL.End();
				GL.PopMatrix();
			}
		}
		public static void DiamondCapDir(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {
			if (Event.current.type != EventType.Repaint) {
				return;
			}
			aPosition  = Handles.matrix.MultiplyPoint(aPosition);
			
			for (int i = 0; i < CapMaterial3D.passCount; ++i) {
				CapMaterial3D.SetPass(i);
				
				GL.PushMatrix();
				GL.Begin(GL.TRIANGLES);
				GL.Color(Handles.color);
				
				GL.Color(Handles.color);
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3(-1,0,-1) * aSize * .5f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3(-1,0, 1) * aSize * .5f));
				GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 0, 1, 0) * aSize));
				
				GL.Color(Handles.color);
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 1,0, 1) * aSize * .5f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 1,0,-1) * aSize * .5f));
				GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 0, 1, 0) * aSize));
				
				GL.Color(Handles.color);
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 1,0,-1) * aSize * .5f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3(-1,0,-1) * aSize * .5f));
				GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 0, 1, 0) * aSize));
				
				GL.Color(Handles.color);
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3(-1,0, 1) * aSize * .5f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 1,0, 1) * aSize * .5f));
				GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 0, 1, 0) * aSize));
				
				
				GL.Color(Handles.color);
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3(-1,0, 1) * aSize * .5f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3(-1,0,-1) * aSize * .5f));
				GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 0, -1, 0) * aSize));
				
				GL.Color(Handles.color);
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 1,0,-1) * aSize * .5f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 1,0, 1) * aSize * .5f));
				GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 0, -1, 0) * aSize));
				
				GL.Color(Handles.color);
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3(-1,0,-1) * aSize * .5f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 1,0,-1) * aSize * .5f));
				GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 0, -1, 0) * aSize));
				
				GL.Color(Handles.color);
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 1,0, 1) * aSize * .5f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3(-1,0, 1) * aSize * .5f));
				GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 0, -1, 0) * aSize));
				
				GL.End();
				GL.PopMatrix();
			}
		}
		public static void ArrowCapDirBase(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, Vector3 aDir) {
			if (Event.current.type != EventType.Repaint) {
				return;
			}
			aPosition  = Handles.matrix.MultiplyPoint(aPosition);
			Quaternion rot = Quaternion.LookRotation(aDir);
			
			for (int i = 0; i < CapMaterial3D.passCount; ++i) {
				CapMaterial3D.SetPass(i);
			
				GL.PushMatrix();
				GL.Begin(GL.TRIANGLES);
				GL.Color(Handles.color);
				
				//GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3(-1, 1,0) * aSize * .5f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3(-1,-1,0) * aSize * .5f));
				//GL.Color(Handles.color);
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 0, 0, 1) * aSize));
				
				//GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 1,-1,0) * aSize * .5f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 1, 1,0) * aSize * .5f));
				//GL.Color(Handles.color);
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 0, 0, 1) * aSize));
				
				//GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3(-1,-1,0) * aSize * .5f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 1,-1,0) * aSize * .5f));
				//GL.Color(Handles.color);
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 0, 0, 1) * aSize));
				
				//GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 1, 1,0) * aSize * .5f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3(-1, 1,0) * aSize * .5f));
				//GL.Color(Handles.color);
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 0, 0, 1) * aSize));
				
				//GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3(-1, 1,0) * aSize * .5f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 1, 1,0) * aSize * .5f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3(-1,-1,0) * aSize * .5f));
				
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 1, 1,0) * aSize * .5f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 1,-1,0) * aSize * .5f));
				GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3(-1,-1,0) * aSize * .5f));
				
				GL.End();
				GL.PopMatrix();
			}
			
		}
		public static void ArrowCapXP(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {
			ArrowCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(1, 0, 0));
		}
		public static void ArrowCapXN(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {
			ArrowCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(-1, 0, 0));
		}
		public static void ArrowCapZP(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {
			ArrowCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(0, 0, 1));
		}
		public static void ArrowCapZN(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {
			ArrowCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(0, 0, -1));
		}
		public static void ArrowCapYP(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {
			ArrowCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(0, 1, 0));
		}
		public static void ArrowCapYN(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {
			ArrowCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(0, -1, 0));
		}
		
		public static void BarCapX(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {
			CubeCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(1, .25f, .25f));
		}
		public static void BarCapY(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {
			CubeCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(.25f, 1, .25f));
		}
		public static void BarCapZ(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {
			CubeCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(.25f, .25f, 1));
		}
		public static void BarCapXZ(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {
			CubeCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(.25f, .25f, .25f));
		}
        #endregion
    }
}