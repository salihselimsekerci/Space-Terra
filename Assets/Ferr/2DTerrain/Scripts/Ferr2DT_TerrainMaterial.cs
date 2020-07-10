using UnityEngine;
using System.Collections;

/// <summary>
/// A direction used to describe the surface of terrain.
/// </summary>
public enum Ferr2DT_TerrainDirection
{
	Top    = 0,
	Left   = 1,
	Right  = 2,
	Bottom = 3,
    None   = 100
}

/// <summary>
/// Describes a terrain segment, and how it should be drawn.
/// </summary>
[System.Serializable]
public class Ferr2DT_SegmentDescription {
    /// <summary>
    /// Applies only to terrain segments facing this direction.
    /// </summary>
    public Ferr2DT_TerrainDirection applyTo;
    /// <summary>
    /// Z Offset, for counteracting depth issues.
    /// </summary>
    public float  zOffset;
    /// <summary>
    /// Just in case you want to adjust the height of the segment
    /// </summary>
    public float  yOffset;
    /// <summary>
    /// UV coordinates for the left ending cap.
    /// </summary>
	public Rect   leftCap;
	/// <summary>
    /// UV coordinates for the left ending cap.
    /// </summary>
	public Rect   innerLeftCap;
    /// <summary>
    /// UV coordinates for the right ending cap.
    /// </summary>
	public Rect   rightCap;
	/// <summary>
    /// UV coordinates for the right ending cap.
    /// </summary>
	public Rect   innerRightCap;
    /// <summary>
    /// A list of body UVs to randomly pick from.
    /// </summary>
	public Rect[] body;
    /// <summary>
    /// How much should the end of the path slide to make room for the caps? (Unity units)
    /// </summary>
    public float  capOffset = 0f;

	public Ferr2DT_SegmentDescription() {
		body    = new Rect[] { new Rect(0,0,50,50) };
		applyTo = Ferr2DT_TerrainDirection.Top;
	}
}

/// <summary>
/// Describes a material that can be applied to a Ferr2DT_PathTerrain
/// </summary>
public class Ferr2DT_TerrainMaterial : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The material of the interior of the terrain.
    /// </summary>
    public Material                     fillMaterial;
    /// <summary>
    /// The material of the edges of the terrain.
    /// </summary>
    public Material                     edgeMaterial;
    /// <summary>
    /// These describe all four edge options, how the top, left, right, and bottom edges should be drawn.
    /// </summary>
    [SerializeField]
    private Ferr2DT_SegmentDescription[] descriptors = new Ferr2DT_SegmentDescription[4];
    [SerializeField]
    private bool isPixel = true;
    #endregion

    #region Constructor
    public Ferr2DT_TerrainMaterial() {
        for (int i = 0; i < descriptors.Length; i++) {
            descriptors[i] = new Ferr2DT_SegmentDescription();
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Gets the edge descriptor for the given edge, defaults to the Top, if none by that type exists, or an empty one, if none are defined at all.
    /// </summary>
    /// <param name="aDirection">Direction to get.</param>
    /// <returns>The given direction, or the first direction, or a default, based on what actually exists.</returns>
    public Ferr2DT_SegmentDescription GetDescriptor(Ferr2DT_TerrainDirection aDirection) {
        ConvertToPercentage();
        for (int i = 0; i < descriptors.Length; i++) {
            if (descriptors[i].applyTo == aDirection) return descriptors[i];
        }
        if (descriptors.Length > 0) {
            return descriptors[0];
        }
        return new Ferr2DT_SegmentDescription();
    }
    /// <summary>
    /// Finds out if we actually have a descriptor for the given direction
    /// </summary>
    /// <param name="aDirection">Duh.</param>
    /// <returns>is it there, or is it not?</returns>
	public bool                       Has          (Ferr2DT_TerrainDirection aDirection) {
		for (int i = 0; i < descriptors.Length; i++) {
            if (descriptors[i].applyTo == aDirection) return true;
        }
		return false;
	}
    /// <summary>
    /// Sets a particular direction as having a valid descriptor. Or not. That's a bool.
    /// </summary>
    /// <param name="aDirection">The direction!</param>
    /// <param name="aActive">To active, or not to active? That is the question!</param>
	public void                       Set          (Ferr2DT_TerrainDirection aDirection, bool aActive) {
		if (aActive) {
			if (descriptors[(int)aDirection].applyTo != aDirection) {
				descriptors[(int)aDirection] = new Ferr2DT_SegmentDescription();
				descriptors[(int)aDirection].applyTo = aDirection;
			}
		} else if (descriptors[(int)aDirection].applyTo != Ferr2DT_TerrainDirection.Top) {
			descriptors[(int)aDirection] = new Ferr2DT_SegmentDescription();
		}
	}
    /// <summary>
    /// Converts our internal pixel UV coordinates to UV values Unity will recognize.
    /// </summary>
    /// <param name="aNativeRect">A UV rect, using pixels.</param>
    /// <returns>A UV rect using Unity coordinates.</returns>
	public Rect                       ToUV    (Rect aNativeRect) {
		if (edgeMaterial == null) return aNativeRect;
        return new Rect(
            aNativeRect.x ,
            (1.0f - aNativeRect.height) - aNativeRect.y,
            aNativeRect.width,
            aNativeRect.height);
	}
    /// <summary>
    /// Converts our internal pixel UV coordinates to UV values we can use on the screen! As 0-1.
    /// </summary>
    /// <param name="aNativeRect">A UV rect, using pixels.</param>
    /// <returns>A UV rect using standard UV coordinates.</returns>
	public Rect                       ToScreen(Rect aNativeRect) {
		if (edgeMaterial == null) return aNativeRect;
        return aNativeRect;
    }

    public Rect GetLCap     (Ferr2DT_TerrainDirection aDirection) {
        return GetDescriptor(aDirection).leftCap;
    }
    public Rect GetRCap     (Ferr2DT_TerrainDirection aDirection) {
        return GetDescriptor(aDirection).rightCap;
    }
	public Rect GetInnerLCap(Ferr2DT_TerrainDirection aDirection) {
		return GetDescriptor(aDirection).innerLeftCap;
	}
	public Rect GetInnerRCap(Ferr2DT_TerrainDirection aDirection) {
		return GetDescriptor(aDirection).innerRightCap;
	}
    public Rect GetBody     (Ferr2DT_TerrainDirection aDirection, int aBodyID) {
        return GetDescriptor(aDirection).body[aBodyID];
    }
    public int  GetBodyCount(Ferr2DT_TerrainDirection aDirection) {
        return GetDescriptor(aDirection).body.Length;
    }

    private void ConvertToPercentage() {
        if (isPixel) {
            for (int i = 0; i < descriptors.Length; i++) {
                for (int t = 0; t < descriptors[i].body.Length; t++) {
                    descriptors[i].body[t] = ToNative(descriptors[i].body[t]);
                }
                descriptors[i].leftCap  = ToNative(descriptors[i].leftCap );
                descriptors[i].rightCap = ToNative(descriptors[i].rightCap);
            }
            isPixel = false;
        }
    }
    public Rect ToNative(Rect aPixelRect) {
        if (edgeMaterial == null) return aPixelRect;
        return new Rect(
            aPixelRect.x      / edgeMaterial.mainTexture.width,
            aPixelRect.y      / edgeMaterial.mainTexture.height,
            aPixelRect.width  / edgeMaterial.mainTexture.width,
            aPixelRect.height / edgeMaterial.mainTexture.height);
    }
    public Rect ToPixels(Rect aNativeRect) {
        if (edgeMaterial == null) return aNativeRect;
        return new Rect(
            aNativeRect.x      * edgeMaterial.mainTexture.width,
            aNativeRect.y      * edgeMaterial.mainTexture.height,
            aNativeRect.width  * edgeMaterial.mainTexture.width,
            aNativeRect.height * edgeMaterial.mainTexture.height);
    }
    #endregion
}
