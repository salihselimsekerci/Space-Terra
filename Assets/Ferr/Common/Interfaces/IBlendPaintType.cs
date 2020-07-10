using UnityEngine;
using System.Collections.Generic;

namespace Ferr {
	public interface IBlendPaintType {
		string Name { get; }
		
		Color PaintColor    { get; set; }
		float PaintSize     { get; set; }
		float PaintStrength { get; set; }
		float PaintFalloff  { get; set; }
		bool  PaintBackfaces{ get; set; }
		
		void  StartPainting     (List<GameObject> aPaintObjs);
		void  EndPainting       (List<GameObject> aPaintObjs);
		
		bool  IsValidPaintObject(GameObject aPaintObj);
		float TypeAffinity      (GameObject aPaintObj);
		void  DoPaint           (GameObject aPaintObj, RaycastHit aRayHit, Vector3 aPrevHitPt, Vector3 aHitPt);
		void  SavePaintObject   (GameObject aPaintObj);
		void  OnGUI             (ref LayoutAdvancer aLayout, float aWidth, float aHeight);
		bool  OnInput           ();
	}
}