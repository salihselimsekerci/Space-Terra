using UnityEngine;
using System.Collections;

namespace Ferr {
	public interface IBlendPaintable {
	    bool OverridePainting {get;}
	
	    void OnPaint         (Vector3 aBrushPoint, float aBrushSize, float aBrushStrength, Color aColor);
	    bool OnSetPaintObject();
	    void OnBeginPaint    ();
	    void OnEndPaint      ();
	}
}