using UnityEngine;
using UnityEditor;

namespace Ferr {
    public partial class ComponentTracker  {
        [Ferr.TrackerRegistration]
        static void RegisterFerr2DT() {
            AddType<Ferr2DT_TerrainMaterial>();
            AddType<Ferr2DT_PathTerrain>();
        }
    }
}