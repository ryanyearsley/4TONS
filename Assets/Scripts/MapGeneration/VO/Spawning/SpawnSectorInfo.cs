using UnityEngine;
using System;
[Serializable]
public class SpawnSectorInfo {
    public Vector2 minNormalized;
    public Vector2 maxNormalized;
    public Vector2Int minCoord;
    public Vector2Int maxCoord;

    public SpawnSectorInfo (Vector2 min, Vector2 max) {

        this.minNormalized = min;
        this.maxNormalized = max;
    }

}
