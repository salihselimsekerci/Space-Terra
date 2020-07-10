using UnityEngine;
using UnityEditor;

using System;
using System.Collections;

public static class Ferr2DT_TerrainMaterialUtility {
    public static Rect AtlasField          (Ferr2DT_TerrainMaterial aMat, Rect aRect, Texture aTexture) {
		EditorGUILayout.BeginHorizontal(GUILayout.Height(64));
		GUILayout.Space(5);
		GUILayout.Space(64);
		
		Rect  r   = GUILayoutUtility.GetLastRect();
		float max = Mathf.Max (1, Mathf.Max ( aRect.width, aRect.height ));
		r.width   = Mathf.Max (1, (aRect.width  / max) * 64);
		r.height  = Mathf.Max (1, (aRect.height / max) * 64);
		
		GUI      .DrawTexture(new Rect(r.x-1,  r.y-1,    r.width+2, 1),          EditorGUIUtility.whiteTexture);
		GUI      .DrawTexture(new Rect(r.x-1,  r.yMax+1, r.width+2, 1),          EditorGUIUtility.whiteTexture);
		GUI      .DrawTexture(new Rect(r.x-1,  r.y-1,    1,         r.height+2), EditorGUIUtility.whiteTexture);
		GUI      .DrawTexture(new Rect(r.xMax, r.y-1,    1,         r.height+2), EditorGUIUtility.whiteTexture);
		GUI      .DrawTextureWithTexCoords(r, aTexture, aMat.ToUV(aRect));
		GUILayout.Space(10);

        Rect result = aMat.ToNative(EditorGUILayout.RectField(aMat.ToPixels(aRect)));
		EditorGUILayout.EndHorizontal();
		
		return result;
	}
    public static void ShowPreview         (Ferr2DT_TerrainMaterial aMat, Ferr2DT_TerrainDirection aDir, bool aSimpleUVs, bool aEditable, float aWidth) {
		if (aMat.edgeMaterial == null || aMat.edgeMaterial.mainTexture == null) return;
		
		GUILayout.Label(aMat.edgeMaterial.mainTexture);
		
		Rect texRect   = GUILayoutUtility.GetLastRect();
        texRect.width  = Mathf.Min(Screen.width-aWidth, aMat.edgeMaterial.mainTexture.width);
        texRect.height = (texRect.width / aMat.edgeMaterial.mainTexture.width) * aMat.edgeMaterial.mainTexture.height;
		
		ShowPreviewDirection(aMat, aDir, texRect, aSimpleUVs, aEditable);
	}
	public static void ShowPreviewDirection(Ferr2DT_TerrainMaterial aMat, Ferr2DT_TerrainDirection aDir, Rect aBounds, bool aSimpleUVs, bool aEditable) {
		Ferr2DT_SegmentDescription desc = aMat.GetDescriptor(aDir);
        if (!aMat.Has(aDir)) return;

        if (!aEditable) {
            for (int i = 0; i < desc.body.Length; i++)
            {
                Ferr.EditorTools.DrawRect(aMat.ToScreen( desc.body[i]  ), aBounds);    
            }
		    Ferr.EditorTools.DrawRect(aMat.ToScreen( desc.leftCap  ), aBounds);
	        Ferr.EditorTools.DrawRect(aMat.ToScreen( desc.rightCap ), aBounds);
	        Ferr.EditorTools.DrawRect(aMat.ToScreen( desc.innerLeftCap ), aBounds);
	        Ferr.EditorTools.DrawRect(aMat.ToScreen( desc.innerRightCap), aBounds);
        }
        else if (aSimpleUVs) {
	        float   height    = MaxHeight(desc);
            float   capWidth  = Mathf.Max(desc.leftCap.width, desc.rightCap.width);
            float   bodyWidth = desc.body[0].width;
	        int     bodyCount = desc.body.Length;
	        float   texWidth  = aMat.edgeMaterial.mainTexture != null ? aMat.edgeMaterial.mainTexture.width  : 1;
	        float   texHeight = aMat.edgeMaterial.mainTexture != null ? aMat.edgeMaterial.mainTexture.height : 1;
            Vector2 pos       = new Vector2(desc.leftCap.x, desc.leftCap.y);
            if (desc.leftCap.width == 0 && desc.leftCap.height == 0) pos = new Vector2(desc.body[0].x, desc.body[0].y);

            Rect bounds = new Rect(pos.x, pos.y, capWidth*2+bodyWidth*bodyCount, height);
            bounds = aMat.ToNative(Ferr.EditorTools.UVRegionRect(aMat.ToPixels(bounds),  aBounds));
	        bounds = ClampRect(bounds, (Texture2D)aMat.edgeMaterial.mainTexture);
	        
	        Ferr.EditorTools.DrawVLine(new Vector2((pos.x + capWidth)* texWidth + aBounds.x, (pos.y * texHeight)+2), height * texHeight);
	        for (int i = 1; i <= desc.body.Length; i++) {
		        Ferr.EditorTools.DrawVLine(new Vector2((pos.x + capWidth + bodyWidth*i) * texWidth + aBounds.x, (pos.y * texHeight)+2), height * texHeight);
            }

            height    = bounds.height;
            bodyWidth = (bounds.width - capWidth * 2) / bodyCount;
            pos.x     = bounds.x;
            pos.y     = bounds.y;

            float currX = pos.x;
            desc.leftCap.x      = currX;
            desc.leftCap.y      = pos.y;
            desc.leftCap.width  = capWidth;
            desc.leftCap.height = capWidth == 0 ? 0 : height;
            currX += capWidth;

            for (int i = 0; i < desc.body.Length; i++)
            {
                desc.body[i].x      = currX;
                desc.body[i].y      = pos.y;
                desc.body[i].width  = bodyWidth;
                desc.body[i].height = height;
                currX += bodyWidth;
            }

            desc.rightCap.x      = currX;
            desc.rightCap.y      = pos.y;
            desc.rightCap.width  = capWidth;
            desc.rightCap.height = capWidth == 0 ? 0 : height;

        } else {
            for (int i = 0; i < desc.body.Length; i++) {
                desc.body[i]  = ClampRect(aMat.ToNative(Ferr.EditorTools.UVRegionRect(aMat.ToPixels( desc.body[i] ), aBounds)),  (Texture2D)aMat.edgeMaterial.mainTexture);
            }
            if (desc.leftCap.width  != 0 && desc.leftCap.height  != 0)
                desc.leftCap  = ClampRect(aMat.ToNative(Ferr.EditorTools.UVRegionRect(aMat.ToPixels( desc.leftCap ),  aBounds)), (Texture2D)aMat.edgeMaterial.mainTexture);
            if (desc.rightCap.width != 0 && desc.rightCap.height != 0)
	            desc.rightCap = ClampRect(aMat.ToNative(Ferr.EditorTools.UVRegionRect(aMat.ToPixels( desc.rightCap ), aBounds)), (Texture2D)aMat.edgeMaterial.mainTexture);
	        
	        if (desc.innerLeftCap.width  != 0 && desc.innerLeftCap.height  != 0)
		        desc.innerLeftCap  = ClampRect(aMat.ToNative(Ferr.EditorTools.UVRegionRect(aMat.ToPixels( desc.innerLeftCap ),  aBounds)), (Texture2D)aMat.edgeMaterial.mainTexture);
	        if (desc.innerRightCap.width != 0 && desc.innerRightCap.height != 0)
		        desc.innerRightCap = ClampRect(aMat.ToNative(Ferr.EditorTools.UVRegionRect(aMat.ToPixels( desc.innerRightCap ), aBounds)), (Texture2D)aMat.edgeMaterial.mainTexture);
        }
	}
    public static void ShowSample          (Ferr2DT_TerrainMaterial aMat, Ferr2DT_TerrainDirection aDir, float aWidth) {
        if (aMat.edgeMaterial == null || aMat.edgeMaterial.mainTexture == null)  return;

        Ferr2DT_SegmentDescription desc = aMat.GetDescriptor(aDir);
        float totalWidth                = desc.leftCap.width + desc.rightCap.width + (desc.body[0].width * 3);
        float sourceHeight              = MaxHeight(desc);

        float scale = Mathf.Min(aWidth/totalWidth, 64 / sourceHeight);

        GUILayout.Space(sourceHeight* scale);
        float x = GUILayoutUtility.GetLastRect().x;
        float y = GUILayoutUtility.GetLastRect().y;
        if (desc.leftCap.width != 0) {
            float yOff = ((sourceHeight - desc.leftCap.height) / 2) * scale;
            GUI.DrawTextureWithTexCoords(new Rect(x,y+yOff,desc.leftCap.width * scale, desc.leftCap.height * scale), aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture, aMat.ToUV(desc.leftCap));
            x += desc.leftCap.width * scale;
        }
        for (int i = 0; i < 3; i++)
        {
            int id = (2-i) % desc.body.Length;
            float yOff = ((sourceHeight - desc.body[id].height) / 2) * scale;
            GUI.DrawTextureWithTexCoords(new Rect(x,y+yOff,desc.body[id].width * scale, desc.body[id].height * scale), aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture, aMat.ToUV(desc.body[id]));
            x += desc.body[id].width * scale;
        }
        if (desc.leftCap.width != 0) {
            float yOff = ((sourceHeight - desc.rightCap.height) / 2) * scale;
            GUI.DrawTextureWithTexCoords(new Rect(x,y+yOff,desc.rightCap.width * scale, desc.rightCap.height * scale), aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture, aMat.ToUV(desc.rightCap));
        }
    }
    
    public static bool IsSimple      (Ferr2DT_SegmentDescription aDesc) {
        float y       = aDesc.body[0].y;
        bool  hasCaps = aDesc.leftCap.width != 0 || aDesc.rightCap.width != 0;
        float height  = aDesc.body[0].height;

        if (hasCaps && (aDesc.leftCap.y      != y      || aDesc.rightCap.y != y))           return false;
        if (hasCaps &&  aDesc.leftCap.xMax   != aDesc.body[0].x)                            return false;
        if (hasCaps &&  aDesc.rightCap.x     != aDesc.body[aDesc.body.Length - 1].xMax)     return false;
        if (hasCaps && (aDesc.leftCap.height != height || aDesc.rightCap.height != height)) return false;

        for (int i = 0; i < aDesc.body.Length; i++) {
            if (aDesc.body[i].y      != aDesc.leftCap.y)                                return false;
            if (aDesc.body[i].height != height)                                         return false;
            if (i + 1 < aDesc.body.Length && aDesc.body[i].xMax != aDesc.body[i + 1].x) return false;
        }
        return true;
    }
    public static void EditUVsSimple (Ferr2DT_TerrainMaterial    aMat, Ferr2DT_SegmentDescription desc)
    {
        Rect cap  = aMat.ToPixels(desc.leftCap);
        Rect body = aMat.ToPixels(desc.body[0]);

        float   height    = body.height;
        float   capWidth  = cap .width;
        float   bodyWidth = body.width;
        int     bodyCount = desc.body.Length;
        Vector2 pos       = new Vector2(cap.x, cap.y);
        if (cap.width == 0 && cap.height == 0) pos = new Vector2(body.x, body.y);

        pos       = EditorGUILayout.Vector2Field("Position",    pos      );
        height    = EditorGUILayout.FloatField  ("Height",      height   );
        capWidth  = EditorGUILayout.FloatField  ("Cap Width",   capWidth );
        bodyWidth = EditorGUILayout.FloatField  ("Body Width",  bodyWidth);
        bodyCount = Mathf.Max(1, EditorGUILayout.IntField    ("Body slices", bodyCount));

        if (bodyCount != desc.body.Length) {
            Array.Resize<Rect>(ref desc.body, bodyCount);
        }

        float currX = pos.x;
        desc.leftCap.x      = currX;
        desc.leftCap.y      = pos.y;
        desc.leftCap.width  = capWidth;
        desc.leftCap.height = capWidth == 0 ? 0 : height;
        desc.leftCap = aMat.ToNative(desc.leftCap);
        currX += capWidth;

        for (int i = 0; i < desc.body.Length; i++)
        {
            desc.body[i].x      = currX;
            desc.body[i].y      = pos.y;
            desc.body[i].width  = bodyWidth;
            desc.body[i].height = height;
            desc.body[i] = aMat.ToNative(desc.body[i]);
            currX += bodyWidth;
        }

        desc.rightCap.x      = currX;
        desc.rightCap.y      = pos.y;
        desc.rightCap.width  = capWidth;
        desc.rightCap.height = capWidth == 0 ? 0 : height;
        desc.rightCap = aMat.ToNative(desc.rightCap);
    }
    public static void EditUVsComplex(Ferr2DT_TerrainMaterial    aMat, Ferr2DT_SegmentDescription desc, float aWidth, ref int aCurrBody)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Body", GUILayout.Width(40f));

        int bodyID = Mathf.Clamp(aCurrBody, 0, desc.body.Length);
        if (GUILayout.Button("<", GUILayout.Width(20f))) aCurrBody = Mathf.Clamp(aCurrBody - 1, 0, desc.body.Length - 1);
        EditorGUILayout.LabelField("" + (bodyID + 1), GUILayout.Width(12f));
        if (GUILayout.Button(">", GUILayout.Width(20f))) aCurrBody = Mathf.Clamp(aCurrBody + 1, 0, desc.body.Length - 1);
        bodyID = Mathf.Clamp(aCurrBody, 0, desc.body.Length - 1);
        int length = Math.Max(1, EditorGUILayout.IntField(desc.body.Length, GUILayout.Width(32f)));
        EditorGUILayout.LabelField("Total", GUILayout.Width(40f));
        if (length != desc.body.Length) Array.Resize<Rect>(ref desc.body, length);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        desc.body[bodyID] = AtlasField(aMat, desc.body[bodyID], aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture);
        if (desc.leftCap.width == 0 && desc.leftCap.height == 0) {
            if (EditorGUILayout.Toggle("Left Cap", false)) {
                desc.leftCap = new Rect(0, 0, 30, 30);
            }
        } else {
            if (EditorGUILayout.Toggle("Left Cap", true)) {
                desc.leftCap = AtlasField(aMat, desc.leftCap, aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture);
            } else {
                desc.leftCap = new Rect(0, 0, 0, 0);
            }
        }
	    if (desc.innerLeftCap.width == 0 && desc.innerLeftCap.height == 0) {
		    if (EditorGUILayout.Toggle("Inner Left Cap", false)) {
			    desc.innerLeftCap = new Rect(0, 0, 30, 30);
		    }
	    } else {
		    if (EditorGUILayout.Toggle("Inner Left Cap", true)) {
			    desc.innerLeftCap = AtlasField(aMat, desc.innerLeftCap, aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture);
		    } else {
			    desc.innerLeftCap = new Rect(0, 0, 0, 0);
		    }
	    }
	    
        if (desc.rightCap.width == 0 && desc.rightCap.height == 0) {
            if (EditorGUILayout.Toggle("Right Cap", false)) {
                desc.rightCap = new Rect(0, 0, 30, 30);
            }
        } else  {
            if (EditorGUILayout.Toggle("Right Cap", true)) {
                desc.rightCap = AtlasField(aMat, desc.rightCap, aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture);
            } else {
                desc.rightCap = new Rect(0, 0, 0, 0);
            }
        }
	    if (desc.innerRightCap.width == 0 && desc.innerRightCap.height == 0) {
		    if (EditorGUILayout.Toggle("Inner Right Cap", false)) {
			    desc.innerRightCap = new Rect(0, 0, 30, 30);
		    }
	    } else  {
		    if (EditorGUILayout.Toggle("Inner Right Cap", true)) {
			    desc.innerRightCap = AtlasField(aMat, desc.innerRightCap, aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture);
		    } else {
			    desc.innerRightCap = new Rect(0, 0, 0, 0);
		    }
	    }
    }

    public static float MaxHeight (Ferr2DT_SegmentDescription aDesc) {
        float sourceHeight = Mathf.Max( aDesc.leftCap.height, aDesc.rightCap.height );
        float max          = 0;
        for (int i = 0; i < aDesc.body.Length; i++)
        {
            if (aDesc.body[i].height > max) max = aDesc.body[i].height;
        }
        return Mathf.Max(max, sourceHeight);
    }
    public static Rect  ClampRect (Rect aRect, Texture2D aTex) {
        if (aRect.width  > 1) aRect.width  = 1;
        if (aRect.height > 1) aRect.height = 1;
        if (aRect.xMax   > 1) aRect.xMax   = 1;
        if (aRect.yMax   > 1) aRect.yMax   = 1;
        if (aRect.x      < 0) aRect.x      = 0;
        if (aRect.y      < 0) aRect.y      = 0;
        if (aRect.width  < 0) aRect.width  = 0;
        if (aRect.height < 0) aRect.height = 0;
        return aRect;
    }
	public static void CheckMaterialMode(Material aMat, TextureWrapMode aDesiredMode) {
		if (aMat != null && aMat.mainTexture != null && aMat.mainTexture.wrapMode != aDesiredMode) {
			if (EditorUtility.DisplayDialog("Ferr2D Terrain", "The Material's texture 'Wrap Mode' generally works best when set to "+aDesiredMode+"! Would you like this texture to be updated?", "Yes", "No")) {
				string          path = AssetDatabase.GetAssetPath(aMat.mainTexture);
				TextureImporter imp  = AssetImporter.GetAtPath   (path) as TextureImporter;
				if (imp != null) {
					imp.wrapMode = aDesiredMode;
					AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
				}
			}
		}
	}
}

public class Ferr2DT_TerrainMaterialWindow : EditorWindow {
    private Ferr2DT_TerrainMaterial material;
    
    int                      currBody  = 0;
    bool                     simpleUVs = true;
    const int                width     = 250;
    Ferr2DT_TerrainDirection currDir   = Ferr2DT_TerrainDirection.Top;
    Ferr2DT_TerrainDirection prevDir   = Ferr2DT_TerrainDirection.Top;
    GUIStyle                 foldoutStyle;
    Vector2                  scroll;

    public static void Show(Ferr2DT_TerrainMaterial aMaterial) {
        Ferr2DT_TerrainMaterialWindow window = EditorWindow.GetWindow<Ferr2DT_TerrainMaterialWindow>();
        window.material       = aMaterial;
        window.wantsMouseMove = true;
        window.title          = "Ferr2DT Editor";
        if (aMaterial != null && aMaterial.edgeMaterial != null) {
	        window.minSize = new Vector2(400, 400);
        }
        window.foldoutStyle           = EditorStyles.foldout;
        window.foldoutStyle.fontStyle = FontStyle.Bold;
        window.currDir                = Ferr2DT_TerrainDirection.None;
	    window.prevDir                = Ferr2DT_TerrainDirection.None;
    }

    void OnGUI        () {
        if (material == null) return;

        // if this was an undo, repaint it
        if (Event.current.type == EventType.ValidateCommand)
        {
            switch (Event.current.commandName)
            {
                case "UndoRedoPerformed":
                    Repaint ();
                    return;
            }
        }

		Undo.RecordObject(material, "Modified Terrain Material");

        if (Ferr.EditorTools.ResetHandles()) {
            GUI.changed = true;
        }
        
        EditorGUILayout .BeginHorizontal ();
        EditorGUILayout .BeginVertical   (GUILayout.Width(width));

        Ferr.EditorTools.Box(5, () => {
            if (currDir != Ferr2DT_TerrainDirection.None) {
                Ferr2DT_TerrainMaterialUtility.ShowSample(material, currDir, width-10);
            }
            Ferr.EditorTools.Box(2, () => {
                if (GUILayout.Button("Top")) currDir = Ferr2DT_TerrainDirection.Top;
                if (currDir == Ferr2DT_TerrainDirection.Top) {
                    if (prevDir != currDir) simpleUVs = Ferr2DT_TerrainMaterialUtility.IsSimple(material.GetDescriptor(currDir));

                    bool showTop = GUILayout.Toggle(material.Has(Ferr2DT_TerrainDirection.Top), "Use Top");
                    material.Set(Ferr2DT_TerrainDirection.Top, showTop);
                    if (showTop) ShowDirection(material, currDir);
                }
            }, width-10, 0);
            Ferr.EditorTools.Box(2, () => {
                if (GUILayout.Button("Left")) currDir = Ferr2DT_TerrainDirection.Left;
                if (currDir == Ferr2DT_TerrainDirection.Left) {
                    if (prevDir != currDir) simpleUVs = Ferr2DT_TerrainMaterialUtility.IsSimple(material.GetDescriptor(currDir));

                    bool showLeft = GUILayout.Toggle(material.Has(Ferr2DT_TerrainDirection.Left), "Use Left");
                    material.Set(Ferr2DT_TerrainDirection.Left, showLeft);
                    if (showLeft) ShowDirection(material, currDir);
                }
            }, width - 10, 0);
            Ferr.EditorTools.Box(2, () => {
                if (GUILayout.Button("Right")) currDir = Ferr2DT_TerrainDirection.Right;
                if (currDir == Ferr2DT_TerrainDirection.Right) {
                    if (prevDir != currDir) simpleUVs = Ferr2DT_TerrainMaterialUtility.IsSimple(material.GetDescriptor(currDir));

                    bool showRight = GUILayout.Toggle(material.Has(Ferr2DT_TerrainDirection.Right), "Use Right");
                    material.Set(Ferr2DT_TerrainDirection.Right, showRight);
                    if (showRight) ShowDirection(material, currDir);
                }
            }, width - 10, 0);
            Ferr.EditorTools.Box(2, () => {
                if (GUILayout.Button("Bottom")) currDir = Ferr2DT_TerrainDirection.Bottom;
                if (currDir == Ferr2DT_TerrainDirection.Bottom) {
                    if (prevDir != currDir) simpleUVs = Ferr2DT_TerrainMaterialUtility.IsSimple(material.GetDescriptor(currDir));

                    bool showBottom = GUILayout.Toggle(material.Has(Ferr2DT_TerrainDirection.Bottom), "Use Bottom");
                    material.Set(Ferr2DT_TerrainDirection.Bottom, showBottom);
                    if (showBottom) ShowDirection(material, currDir);
                }
            }, width - 10, 0);
        }, 0, (int)this.position.height);

        EditorGUILayout.EndVertical  ();
        EditorGUILayout.BeginVertical();
        scroll = EditorGUILayout.BeginScrollView(scroll);
        if (currDir != Ferr2DT_TerrainDirection.None) {
            Ferr2DT_TerrainMaterialUtility.ShowPreview(material, currDir, simpleUVs, true, width);
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical  ();
        EditorGUILayout.EndHorizontal();

        if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDrag)
			Repaint ();

        if (GUI.changed) {
            EditorUtility.SetDirty(material);

            Ferr2DT_PathTerrain[] terrain = GameObject.FindObjectsOfType(typeof(Ferr2DT_PathTerrain)) as Ferr2DT_PathTerrain[];
            for (int i = 0; i < terrain.Length; i++)
            {
                if(terrain[i].TerrainMaterial == material)
                    terrain[i].Build(true);
            }
		}

        prevDir = currDir;
    }
    void ShowDirection(Ferr2DT_TerrainMaterial aMat, Ferr2DT_TerrainDirection aDir) {
		Ferr2DT_SegmentDescription desc = aMat.GetDescriptor(aDir);

		desc.zOffset   = EditorGUILayout.FloatField( "Z Offset",   desc.zOffset  );
		desc.yOffset   = EditorGUILayout.FloatField( "Y Offset",   desc.yOffset  );
        desc.capOffset = EditorGUILayout.FloatField( "Cap Offset", desc.capOffset);

        simpleUVs = EditorGUILayout.Toggle("Simple", simpleUVs);
        if (simpleUVs) {
            Ferr2DT_TerrainMaterialUtility.EditUVsSimple(aMat, desc);
        } else {
            Ferr2DT_TerrainMaterialUtility.EditUVsComplex(aMat, desc, width, ref currBody);
        }
    }
}