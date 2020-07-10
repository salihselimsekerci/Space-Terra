using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Ferr2D_Path), typeof(Ferr2DT_PathTerrain))]
public class InfiniteTerrain : MonoBehaviour {
    public GameObject   centerAround;
    public int          vertCount = 10;
    public float        vertSpacing = 1;
    public float        minHeight = 2;
    public float        maxHeight = 10;
    public float        heightVariance = 4;
    public float        cliffChance = 0.1f;
    
    Ferr2DT_PathTerrain terrain;
    List<float>         terrainHeights;
    List<float>         terrainSecondaryHeights;
    int                 currentOffset;
    
	void Start  () {
        terrain = GetComponent<Ferr2DT_PathTerrain>();

        terrainHeights          = new List<float>();
        terrainSecondaryHeights = new List<float>();
        for (int i = 0; i < vertCount; i++) {
            NewRight();
        }
        RebuildTerrain();
	}
	void Update () {
        UpdateTerrain();
	}

    void  UpdateTerrain () {
        bool updated = false;

        // generate points to the right if we need 'em
        while (centerAround.transform.position.x > ((currentOffset+1) * vertSpacing)) {
            currentOffset += 1;
            NewRight();
            terrainHeights         .RemoveAt(0);
            terrainSecondaryHeights.RemoveAt(0);
            updated = true;
        }

        // generate points to the left, if we need 'em
        while (centerAround.transform.position.x < ((currentOffset-1) * vertSpacing)) {
            currentOffset -= 1;
            NewLeft();
            terrainHeights         .RemoveAt(terrainHeights         .Count - 1);
            terrainSecondaryHeights.RemoveAt(terrainSecondaryHeights.Count - 1);
            updated = true;
        }

        // rebuild the terrain if we added any points
        if (updated) {
            RebuildTerrain();
        }
    }
    void  RebuildTerrain() {
        float startX = (currentOffset * vertSpacing) - ((vertCount / 2) * vertSpacing);
        terrain.ClearPoints();
        for (int i = 0; i < terrainHeights.Count; i++) {
            Vector2 pos = new Vector2(startX + i * vertSpacing, terrainHeights[i]);
            terrain.AddPoint(pos);
            if (terrainSecondaryHeights[i] != terrainHeights[i]) {
                pos = new Vector2(startX + i * vertSpacing, terrainSecondaryHeights[i]);
                terrain.AddPoint(pos);
            }
        }

        terrain.Build    (false);
        terrain.RecreateCollider();
    }
    void  NewRight      () {
        float right  = GetRight();
        float right2 = Random.value < cliffChance ? GetRight() : right;

        if (Mathf.Abs(right - right2) < 3) {
            right = right2;
        }

        terrainHeights         .Add(right );
        terrainSecondaryHeights.Add(right2);
    }
    void  NewLeft       () {
        float left = GetLeft();
        float left2 = Random.value < cliffChance ? GetLeft() : left;

        if (Mathf.Abs(left - left2) < 3) {
            left = left2;
        }

        terrainHeights         .Insert(0, left );
        terrainSecondaryHeights.Insert(0, left2);
    }
    float GetRight      () {
        if (terrainHeights.Count <= 0) return minHeight + (maxHeight - minHeight) / 2;
        return Mathf.Clamp(terrainSecondaryHeights[terrainHeights.Count - 1] + (-1 + Random.value * 2) * heightVariance, minHeight, maxHeight);
    }
    float GetLeft       () {
        if (terrainHeights.Count <= 0) return minHeight + (maxHeight - minHeight) / 2;
        return Mathf.Clamp(terrainSecondaryHeights[0                       ] + (-1 + Random.value * 2) * heightVariance, minHeight, maxHeight);
    }
}
