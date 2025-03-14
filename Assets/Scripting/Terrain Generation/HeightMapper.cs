using UnityEngine;

public abstract class HeightMapper : MonoBehaviour
{
    public abstract Vector3[] ApplyHeightMap(Vector3[] vertices, int xSize, int zSize);
}
