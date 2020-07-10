using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(Ferr2D_Path))]
public class Ferr2D_PathEditor : Editor { 
	static Texture2D texMinus;
    static Texture2D texMinusSelected;
    static Texture2D texDot;
    static Texture2D texDotSnap;
    static Texture2D texDotPlus;
    static Texture2D texDotSelected;
    static Texture2D texDotSelectedSnap;

    static Texture2D texLeft;
    static Texture2D texRight;
    static Texture2D texTop;
    static Texture2D texBottom;
	static Texture2D texAuto;
	static Texture2D texReset;
	static Texture2D texScale;

    private void CapDotMinus        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texMinus);}
    private void CapDotMinusSelected(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texMinusSelected);}
    private void CapDot             (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot);}
    private void CapDotSnap         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotSnap);}
    private void CapDotPlus         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotPlus);}
    private void CapDotSelected     (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotSelected);}
    private void CapDotSelectedSnap (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotSelectedSnap);}
    private void CapDotLeft         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texLeft);}
    private void CapDotRight        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texRight);}
    private void CapDotTop          (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texTop);}
    private void CapDotBottom       (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texBottom);}
    private void CapDotAuto         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texAuto);}
    private void CapDotScale        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texScale);}
    private void CapDotReset        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texReset);}

	
	public static Action OnChanged = null;
	static int updateCount    = 0;
	bool       showVerts      = false;
	bool       prevChanged    = false;
	List<int>  selectedPoints = new List<int>();
	Vector2    dragStart;
	bool       drag           = false;

    void LoadTextures() {
        if (texMinus != null) return;

        texMinus           = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-minus.png"         );
        texMinusSelected   = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-minus-selected.png");
        texDot             = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot.png"               );
        texDotSnap         = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-snap.png"          );
        texDotPlus         = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-plus.png"          );
        texDotSelected     = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-selected.png"      );
        texDotSelectedSnap = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-selected-snap.png" );

        texLeft   = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-left.png" );
        texRight  = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-right.png");
        texTop    = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-top.png"  );
        texBottom = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-down.png" );
	    texAuto   = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-auto.png" );
	    texReset  = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-reset.png");
	    texScale  = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-scale.png");
    }
	private         void OnEnable      () {
		selectedPoints.Clear();
        LoadTextures();
	}
	private         void OnSceneGUI    () {
		Ferr2D_Path  path      = (Ferr2D_Path)target;
		GUIStyle     iconStyle = new GUIStyle();
		iconStyle.alignment    = TextAnchor.MiddleCenter;
		
		// setup undoing things
		Undo.RecordObject(target, "Modified Path");
		
        // draw the path line
		if (Event.current.type == EventType.Repaint)
			DoPath(path);
		
		// Check for drag-selecting multiple points
		DragSelect(path);
		
        // do adding verts in when the shift key is down!
		if (Event.current.shift && !Event.current.control) {
			DoShiftAdd(path, iconStyle);
		}
		
        // draw and interact with all the path handles
		DoHandles(path, iconStyle);
		
		// update everything that relies on this path, if the GUI changed
		if (GUI.changed) {
			UpdateDependentsSmart(path, false, false);
			EditorUtility.SetDirty (target);
			prevChanged = true;
		} else if (Event.current.type == EventType.Used) {
			if (prevChanged == true) {
				UpdateDependentsSmart(path, false, true);
			}
			prevChanged = false;
		}
	}
	public override void OnInspectorGUI() {
		Undo.RecordObject(target, "Modified Path");
		
		Ferr2D_Path path = (Ferr2D_Path)target;
		
		// if this was an undo, refresh stuff too
		if (Event.current.type == EventType.ValidateCommand) {
			switch (Event.current.commandName) {
			case "UndoRedoPerformed":
				
				path.UpdateDependants(true);
				if (OnChanged != null) OnChanged();
				return;
			}
		}
		
		path.closed = EditorGUILayout.Toggle ("Closed", path.closed);
		if (path)
			
        // display the path verts list info
			showVerts   = EditorGUILayout.Foldout(showVerts, "Path Vertices");
		EditorGUI.indentLevel = 2;
		if (showVerts)
		{
			int size = EditorGUILayout.IntField("Count: ", path.pathVerts.Count);
			while (path.pathVerts.Count > size) path.pathVerts.RemoveAt(path.pathVerts.Count - 1);
			while (path.pathVerts.Count < size) path.pathVerts.Add     (new Vector2(0, 0));
		}
        // draw all the verts! Long list~
		for (int i = 0; showVerts && i < path.pathVerts.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("#" + i, GUILayout.Width(60));
			path.pathVerts[i] = new Vector2(
				EditorGUILayout.FloatField(path.pathVerts[i].x),
				EditorGUILayout.FloatField(path.pathVerts[i].y));
			EditorGUILayout.EndHorizontal();
		}
		
        // button for updating the origin of the object
		if (GUILayout.Button("Center Position")) path.ReCenter();
		
		bool updateClosed = false;
		Ferr2DT_PathTerrain terrain = path.GetComponent<Ferr2DT_PathTerrain>();
		if (!path.closed && terrain != null && (terrain.fill == Ferr2DT_FillMode.Closed || terrain.fill == Ferr2DT_FillMode.InvertedClosed || terrain.fill == Ferr2DT_FillMode.FillOnlyClosed)) {
			path.closed  = true;
			updateClosed = true;
		}
		if (terrain != null &&  path.closed && (terrain.fill == Ferr2DT_FillMode.FillOnlySkirt || terrain.fill == Ferr2DT_FillMode.Skirt)) {
			path.closed  = false;
			updateClosed = true;
		}
		
        // update dependants when it changes
		if (GUI.changed || updateClosed)
		{
			path.UpdateDependants(false);
			EditorUtility.SetDirty(target);
		}
	}
	
	private void    UpdateDependentsSmart(Ferr2D_Path aPath, bool aForce, bool aFullUpdate) {
		if (aForce || Ferr2DT_Menu.UpdateTerrainSkipFrames == 0 || updateCount % Ferr2DT_Menu.UpdateTerrainSkipFrames == 0) {
			aPath.UpdateDependants(aFullUpdate);
			if (Application.isPlaying) aPath.UpdateColliders();
			if (OnChanged != null) OnChanged();
		}
		updateCount += 1;
	}
	private void    DragSelect           (Ferr2D_Path path) {
		
		if (Event.current.type == EventType.Repaint) {
			if (drag) {
				Vector3 pt1 = HandleUtility.GUIPointToWorldRay(dragStart).GetPoint(0.2f);
				Vector3 pt2 = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).GetPoint(0.2f);
				Vector3 pt3 = HandleUtility.GUIPointToWorldRay(new Vector2(dragStart.x, Event.current.mousePosition.y)).GetPoint(0.2f);
				Vector3 pt4 = HandleUtility.GUIPointToWorldRay(new Vector2(Event.current.mousePosition.x, dragStart.y)).GetPoint(0.2f);
				Handles.DrawSolidRectangleWithOutline(new Vector3[] { pt1, pt3, pt2, pt4 }, new Color(0, 0.5f, 0.25f, 0.25f), new Color(0, 0.5f, 0.25f, 0.5f));
			}
		}
		
		if (Event.current.shift && Event.current.control) {
			switch(Event.current.type) {
			case EventType.MouseDrag:
				SceneView.RepaintAll();
				break;
			case EventType.MouseMove:
				SceneView.RepaintAll();
				break;
			case EventType.MouseDown:
				dragStart = Event.current.mousePosition;
				drag = true;
				
				break;
			case EventType.MouseUp:
				Vector2 dragEnd = Event.current.mousePosition;
				selectedPoints.Clear();
				for	(int i=0;i<path.pathVerts.Count;i+=1) {
					float left   = Mathf.Min(dragStart.x, dragStart.x + (dragEnd.x - dragStart.x));
					float right  = Mathf.Max(dragStart.x, dragStart.x + (dragEnd.x - dragStart.x));
					float top    = Mathf.Min(dragStart.y, dragStart.y + (dragEnd.y - dragStart.y));
					float bottom = Mathf.Max(dragStart.y, dragStart.y + (dragEnd.y - dragStart.y));
					
					Rect r = new Rect(left, top, right-left, bottom-top);
					if (r.Contains(HandleUtility.WorldToGUIPoint(path.transform.TransformPoint( path.pathVerts[i]) ) )) {
						selectedPoints.Add(i);
					}
				}
				
				HandleUtility.AddDefaultControl(0);
				drag = false;
				Repaint();
				break;
			case EventType.Layout :
				HandleUtility.AddDefaultControl(GetHashCode());
				break;
			}
		} else if (drag == true) {
			drag = false;
			Repaint();
		}
	}
	private void    DoHandles            (Ferr2D_Path path, GUIStyle iconStyle)
	{
        Vector3             snap         = Event.current.control ? new Vector3(EditorPrefs.GetFloat("MoveSnapX"), EditorPrefs.GetFloat("MoveSnapY"), EditorPrefs.GetFloat("MoveSnapZ")) : Vector3.zero;
        Transform           transform    = path.transform;
        Matrix4x4           mat          = transform.localToWorldMatrix;
        Matrix4x4           invMat       = transform.worldToLocalMatrix;
        Transform           camTransform = SceneView.lastActiveSceneView.camera.transform;
		Ferr2DT_PathTerrain terrain      = path.GetComponent<Ferr2DT_PathTerrain>();
		if (terrain) terrain.MatchOverrides();
		
		Handles.color = new Color(1, 1, 1, 1);
		for (int i = 0; i < path.pathVerts.Count; i++)
		{
			Vector3 pos        = mat.MultiplyPoint3x4(path.pathVerts[i]);
			Vector3 posStart   = pos;
			bool    isSelected = false;
               if (selectedPoints!= null) isSelected = selectedPoints.Contains(i);
			
            // check if we want to remove points
			if (Event.current.alt)
			{
				float                   handleScale = HandleScale(posStart);
				Handles.DrawCapFunction cap         = (isSelected || selectedPoints.Count <= 0) ? (Handles.DrawCapFunction)CapDotMinusSelected : (Handles.DrawCapFunction)CapDotMinus;
				if (Handles.Button(posStart, camTransform.rotation, handleScale, handleScale, cap))
				{
					if (!isSelected) {
						selectedPoints.Clear();
						selectedPoints.Add(i);
					}
					for (int s = 0; s < selectedPoints.Count; s++) {
						if (terrain) terrain.RemovePoint(selectedPoints[s]);
						else  path.pathVerts.RemoveAt   (selectedPoints[s]);
						if (selectedPoints[s] <= i) i--;
						
						for (int u = 0; u < selectedPoints.Count; u++) {
							if (selectedPoints[u] > selectedPoints[s]) selectedPoints[u] -= 1;
						}
					}
					selectedPoints.Clear();
					GUI.changed = true;
				} else if (Ferr2DT_SceneOverlay.editMode != Ferr2DT_EditMode.None) {
					
					if (terrain && i+1 < path.pathVerts.Count) {
						float   scale     = handleScale * 0.5f;
						Vector3 dirOff    = GetTickerOffset(path, pos, i);
						Vector3 posDirOff = posStart + dirOff;
						
						if (IsVisible(posDirOff)) {
							cap = null;
							if      (Ferr2DT_SceneOverlay.editMode == Ferr2DT_EditMode.Override) cap  = GetDirIcon(terrain.directionOverrides[i]);
							else if (Ferr2DT_SceneOverlay.editMode == Ferr2DT_EditMode.Scale   ) cap  = CapDotScale;
							if      (Event.current.alt)                                          cap  = CapDotReset;
							
							if (Handles.Button(posDirOff, camTransform.rotation, scale, scale, cap)) {
								if (selectedPoints.Count < 2 || isSelected == false) {
									selectedPoints.Clear();
									selectedPoints.Add(i);
									isSelected = true;
								}
								
								for (int s = 0; s < selectedPoints.Count; s++) {
									if (Ferr2DT_SceneOverlay.editMode == Ferr2DT_EditMode.Override)
										terrain.directionOverrides[selectedPoints[s]] = Ferr2DT_TerrainDirection.None;
									else if (Ferr2DT_SceneOverlay.editMode == Ferr2DT_EditMode.Scale)
										terrain.vertScales        [selectedPoints[s]] = 1;
								}
								GUI.changed = true;
							}
						}
					}
				}
			} else {
                // check for moving the point
				Handles.DrawCapFunction cap = CapDot;
				if (Event.current.control) cap = isSelected ? (Handles.DrawCapFunction)CapDotSelectedSnap : (Handles.DrawCapFunction)CapDotSnap;
				else                       cap = isSelected ? (Handles.DrawCapFunction)CapDotSelected     : (Handles.DrawCapFunction)CapDot;
				
				Vector3 result = Handles.FreeMoveHandle(
					posStart,
					camTransform.rotation,
					HandleScale(pos),
					snap, 
					cap);
				
				if (result != posStart) {
					
					if (selectedPoints.Count < 2 || isSelected == false) {
						selectedPoints.Clear();
						selectedPoints.Add(i);
						isSelected = true;
					}
					
					if (!(Event.current.control && Ferr2DT_Menu.SnapMode == Ferr2DT_Menu.SnapType.SnapRelative))
						result = GetRealPoint(result, transform);

					Vector3 global = result;
					if (Event.current.control && Ferr2DT_Menu.SnapMode == Ferr2DT_Menu.SnapType.SnapGlobal) global = SnapVector(global, snap);
					Vector3 local  = invMat.MultiplyPoint3x4(global);
					if (Event.current.control && Ferr2DT_Menu.SnapMode == Ferr2DT_Menu.SnapType.SnapLocal ) local  = SnapVector(local, snap);
					if (!Event.current.control && Ferr2DT_SceneOverlay.smartSnap) {
						local = SmartSnap(local, path.pathVerts, selectedPoints, Ferr2DT_Menu.SmartSnapDist);
					}
					
					Vector2 relative = new Vector2( local.x, local.y) - path.pathVerts[i];
					
					for (int s = 0; s < selectedPoints.Count; s++) {
						path.pathVerts[selectedPoints[s]] += relative;
					}
				}
				
                // if using terrain, check to see for any edge overrides
				if (Ferr2DT_SceneOverlay.showIndices) {
					Vector3 labelPos = posStart + (Vector3)Ferr2D_Path.GetNormal(path.pathVerts, i, path.closed);
					Handles.color    = Color.white;
					Handles.Label(labelPos, "" + i);
					Handles.color    = new Color(1, 1, 1, 0);
				}
				
				if (terrain) {
					float   scale     = HandleScale (pos) * 0.5f;
					Vector3 dirOff    = GetTickerOffset (path, pos, i);
					Vector3 posDirOff = posStart + dirOff;
					
					if (Ferr2DT_SceneOverlay.editMode == Ferr2DT_EditMode.Override && i+1 < path.pathVerts.Count) {
						if (IsVisible(posDirOff) && terrain.directionOverrides != null) {
							cap = Event.current.alt ? (Handles.DrawCapFunction)CapDotReset : GetDirIcon(terrain.directionOverrides[i]);
							if (Handles.Button(posDirOff, camTransform.rotation, scale, scale, cap)) {
								if (selectedPoints.Count < 2 || isSelected == false) {
									selectedPoints.Clear();
									selectedPoints.Add(i);
									isSelected = true;
								}
								
								Ferr2DT_TerrainDirection dir = NextDir(terrain.directionOverrides[i]);
								for (int s = 0; s < selectedPoints.Count; s++) {
									terrain.directionOverrides[selectedPoints[s]] = dir;
								}
								GUI.changed = true;
							}
						}
						
					} else if (Ferr2DT_SceneOverlay.editMode == Ferr2DT_EditMode.Scale) {
						if (IsVisible(posDirOff)) {
                            cap = Event.current.alt ? (Handles.DrawCapFunction)CapDotReset : (Handles.DrawCapFunction)CapDotScale;
							
							Vector3 scaleMove = Handles.FreeMoveHandle(posDirOff, camTransform.rotation, scale, Vector3.zero, cap);
							float   scaleAmt  = scaleMove.y - posDirOff.y;
							if (Mathf.Abs(scaleAmt) > 0.01f ) {
								if (selectedPoints.Count < 2 || isSelected == false) {
									selectedPoints.Clear();
									selectedPoints.Add(i);
									isSelected = true;
								}
								
								float vertScale = terrain.vertScales[i] - Event.current.delta.y / 100f;
								vertScale = Mathf.Clamp(vertScale, 0.2f, 3f);
								for (int s = 0; s < selectedPoints.Count; s++) {
									terrain.vertScales[selectedPoints[s]] = vertScale;
								}
								GUI.changed = true;
							}
						}
					}
				}
				
                // make sure we can add new point at the midpoints!
				if (i + 1 < path.pathVerts.Count || path.closed == true) {
					int     index       = path.closed && i + 1 == path.pathVerts.Count ? 0 : i + 1;
					Vector3 pos2        = mat.MultiplyPoint3x4(path.pathVerts[index]);
					Vector3 mid         = (pos + pos2) / 2;
					float   handleScale = HandleScale(mid);
					
					if (Handles.Button(mid, camTransform.rotation, handleScale, handleScale, CapDotPlus)) {
						Vector2 pt = invMat.MultiplyPoint3x4(mid);
						if (terrain)
							terrain.AddPoint(pt, index);
						else
							path.pathVerts.Insert(index, pt);
					}
				}
			}
		}
		
		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete && selectedPoints.Count > 0) {
			for (int s = 0; s < selectedPoints.Count; s++) {
				if (terrain) terrain.RemovePoint(selectedPoints[s]);
				else  path.pathVerts.RemoveAt   (selectedPoints[s]);
				
				for (int u = 0; u < selectedPoints.Count; u++) {
					if (selectedPoints[u] > selectedPoints[s]) selectedPoints[u] -= 1;
				}
			}
			selectedPoints.Clear();
			GUI.changed = true;
			Event.current.Use();
		}
	}
	private Vector3 GetTickerOffset      (Ferr2D_Path path, Vector3  aRootPos, int aIndex) {
		float   scale  = HandleScale(aRootPos) * 0.5f;
		Vector3 result = Vector3.zero;
		
		int     index  = (aIndex + 1) % path.pathVerts.Count;
		Vector3 delta  = Vector3.Normalize(path.pathVerts[index] - path.pathVerts[aIndex]);
		Vector3 norm   = new Vector3(-delta.y, delta.x, 0);
		result = delta * scale * 3 + new Vector3(norm.x, norm.y, 0) * scale * 2;

		return result;
	}
	private void    DoShiftAdd           (Ferr2D_Path path, GUIStyle iconStyle)
	{
        Vector3             snap      = Event.current.control ? new Vector3(EditorPrefs.GetFloat("MoveSnapX"), EditorPrefs.GetFloat("MoveSnapY"), EditorPrefs.GetFloat("MoveSnapZ")) : Vector3.zero;
		Ferr2DT_PathTerrain terrain   = path.gameObject.GetComponent<Ferr2DT_PathTerrain>();
        Transform           transform = path.transform;
        Transform           camTransform = SceneView.lastActiveSceneView.camera.transform;
		Vector3             pos       = transform.InverseTransformPoint( GetMousePos(Event.current.mousePosition, transform) );
		bool                hasDummy  = path.pathVerts.Count <= 0;
		
		if (hasDummy) path.pathVerts.Add(Vector2.zero);
		
		int   closestID  = path.GetClosestSeg(pos);
		int   secondID   = closestID + 1 >= path.Count ? 0 : closestID + 1;
		
		float firstDist  = Vector2.Distance(pos, path.pathVerts[closestID]);
		float secondDist = Vector2.Distance(pos, path.pathVerts[secondID]);
		
		Vector3 local  = pos;
		if (Event.current.control && Ferr2DT_Menu.SnapMode == Ferr2DT_Menu.SnapType.SnapLocal ) local  = SnapVector(local,  snap);
		Vector3 global = transform.TransformPoint(pos);
		if (Event.current.control && Ferr2DT_Menu.SnapMode == Ferr2DT_Menu.SnapType.SnapGlobal) global = SnapVector(global, snap);
		
		Handles.color = Color.white;
		if (!(secondID == 0 && !path.closed && firstDist > secondDist))
		{
			Handles.DrawLine( global, transform.TransformPoint(path.pathVerts[closestID]));
		}
		if (!(secondID == 0 && !path.closed && firstDist < secondDist))
		{
			Handles.DrawLine( global, transform.TransformPoint(path.pathVerts[secondID]));
		}
		Handles.color = new Color(1, 1, 1, 1);
		
		Vector3 handlePos = transform.TransformPoint(pos);
        float   scale     = HandleScale(handlePos);
		if (Handles.Button(handlePos, camTransform.rotation, scale, scale, CapDotPlus))
		{
			Vector3 finalPos = transform.InverseTransformPoint(global);
			if (secondID == 0) {
				if (firstDist < secondDist) {
					if (terrain)
						terrain.AddPoint(finalPos);
					else
						path.pathVerts.Add(finalPos);
				} else {
					if (terrain)
						terrain.AddPoint(finalPos, 0);
					else
						path.pathVerts.Insert(0, finalPos);
				}
			} else {
				if (terrain)
					terrain.AddPoint(finalPos, Mathf.Max(closestID, secondID));
				else
					path.pathVerts.Insert(Mathf.Max(closestID, secondID), finalPos);
			}
			selectedPoints.Clear();
			GUI.changed = true;
		}
		
		if (hasDummy) path.pathVerts.RemoveAt(0);
	}
	private void    DoPath               (Ferr2D_Path path)
	{
		Handles.color = Color.white;
		List<Vector2> verts     = path.GetVertsRaw();
        Matrix4x4     mat       = path.transform.localToWorldMatrix;

		for (int i = 0; i < verts.Count - 1; i++)
		{
			Vector3 pos  = mat.MultiplyPoint3x4(verts[i]);
			Vector3 pos2 = mat.MultiplyPoint3x4(verts[i + 1]);
			Handles.DrawLine(pos, pos2);
		}
		if (path.closed)
		{
			Vector3 pos  = mat.MultiplyPoint3x4(verts[0]);
			Vector3 pos2 = mat.MultiplyPoint3x4(verts[verts.Count - 1]);
			Handles.DrawLine(pos, pos2);
		}
	}
	
	private Handles.DrawCapFunction  GetDirIcon(Ferr2DT_TerrainDirection aDir) {
		if      (aDir == Ferr2DT_TerrainDirection.Top   ) return CapDotTop;
		else if (aDir == Ferr2DT_TerrainDirection.Right ) return CapDotRight;
		else if (aDir == Ferr2DT_TerrainDirection.Left  ) return CapDotLeft;
		else if (aDir == Ferr2DT_TerrainDirection.Bottom) return CapDotBottom;
		return CapDotAuto;
	}
	private Ferr2DT_TerrainDirection NextDir   (Ferr2DT_TerrainDirection aDir) {
		if      (aDir == Ferr2DT_TerrainDirection.Top   ) return Ferr2DT_TerrainDirection.Right;
		else if (aDir == Ferr2DT_TerrainDirection.Right ) return Ferr2DT_TerrainDirection.Bottom;
		else if (aDir == Ferr2DT_TerrainDirection.Left  ) return Ferr2DT_TerrainDirection.Top;
		else if (aDir == Ferr2DT_TerrainDirection.Bottom) return Ferr2DT_TerrainDirection.None;
		return Ferr2DT_TerrainDirection.Left;
	}
	
	public static Vector3 GetMousePos  (Vector2 aMousePos, Transform aTransform) {
		Ray   ray   = SceneView.lastActiveSceneView.camera.ScreenPointToRay(new Vector3(aMousePos.x, aMousePos.y, 0));
		Plane plane = new Plane(aTransform.TransformDirection(new Vector3(0,0,-1)), aTransform.position);
		float dist  = 0;
		Vector3 result = new Vector3(0,0,0);
		
		ray = HandleUtility.GUIPointToWorldRay(aMousePos);
		if (plane.Raycast(ray, out dist)) {
			result = ray.GetPoint(dist);
		}
		return result;
	}
	public static float   GetCameraDist(Vector3 aPt) {
		return Vector3.Distance(SceneView.lastActiveSceneView.camera.transform.position, aPt);
	}
	public static bool    IsVisible    (Vector3 aPos) {
		Transform t = SceneView.lastActiveSceneView.camera.transform;
		if (Vector3.Dot(t.forward, aPos - t.position) > 0)
			return true;
		return false;
	}
	public static void    SetScale     (Vector3 aPos, Texture aIcon, ref GUIStyle aStyle, float aScaleOverride = 1) {
		float max      = (Screen.width + Screen.height) / 2;
		float dist     = SceneView.lastActiveSceneView.camera.orthographic ? SceneView.lastActiveSceneView.camera.orthographicSize / 0.5f : GetCameraDist(aPos);
		
		float div = (dist / (max / 160));
		float mul = Ferr2DT_Menu.PathScale * aScaleOverride;
		
		aStyle.fixedWidth  = (aIcon.width  / div) * mul;
		aStyle.fixedHeight = (aIcon.height / div) * mul;
	}
	public static float   HandleScale  (Vector3 aPos) {
		float dist = SceneView.lastActiveSceneView.camera.orthographic ? SceneView.lastActiveSceneView.camera.orthographicSize / 0.45f : GetCameraDist(aPos);
		return Mathf.Min(0.4f * Ferr2DT_Menu.PathScale, (dist / 5.0f) * 0.4f * Ferr2DT_Menu.PathScale);
	}
	
	private static Vector3 SnapVector  (Vector3 aVector, Vector3 aSnap) {
		return new Vector3(
			((int)(aVector.x / aSnap.x + (aVector.x > 0 ? 0.5f : -0.5f))) * aSnap.x,
			((int)(aVector.y / aSnap.y + (aVector.y > 0 ? 0.5f : -0.5f))) * aSnap.y,
			((int)(aVector.z / aSnap.z + (aVector.z > 0 ? 0.5f : -0.5f))) * aSnap.z);
	}
	private static Vector2 SnapVector  (Vector2 aVector, Vector2 aSnap) {
		return new Vector2(
			((int)(aVector.x / aSnap.x + (aVector.x > 0 ? 0.5f : -0.5f))) * aSnap.x,
			((int)(aVector.y / aSnap.y + (aVector.y > 0 ? 0.5f : -0.5f))) * aSnap.y);
	}
	private static Vector3 GetRealPoint(Vector3 aPoint, Transform aTransform) {
		Plane p = new Plane( aTransform.TransformDirection(new Vector3(0, 0, -1)), aTransform.position);
		Ray   r = new Ray  (SceneView.lastActiveSceneView.camera.transform.position, aPoint - SceneView.lastActiveSceneView.camera.transform.position);
		float d = 0;
		
		if (p.Raycast(r, out d)) {
			return r.GetPoint(d);;
		}
		return aPoint;
	}
	private Vector3 SmartSnap(Vector3 aPoint, List<Vector2> aPath, List<int> aIgnore, float aSnapDist) {
		float   minXDist = aSnapDist;
		float   minYDist = aSnapDist;
		Vector3 result   = aPoint;
		
		for (int i = 0; i < aPath.Count; ++i) {
			if (aIgnore.Contains(i)) continue;
			
			float xDist = Mathf.Abs(aPoint.x - aPath[i].x);
			float yDist = Mathf.Abs(aPoint.y - aPath[i].y);
			
			if (xDist < minXDist) {
				minXDist = xDist;
				result.x = aPath[i].x;
			}
			
			if (yDist < minYDist) {
				minYDist = yDist;
				result.y = aPath[i].y;
			}
		}
		return result;
	}
}