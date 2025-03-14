using UnityEngine;

public abstract class HeightMapper : ScriptableObject
{
    public abstract Vector3[] ApplyHeightMap(Vector3 positionOffset, Vector3[] vertices, int xSize, int zSize);
}
