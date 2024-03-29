using UnityEngine;

[System.Serializable]
public class Cube
{
    public Vector3 worldPosition;
    public GameObject cubeObject;
    public Cube parent;
    public int gridX;
    public int gridY;
    public int gridZ;
    public int gCost;
    public int hCost;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public bool Occupied;

    public Cube(Vector3 _worldPos, GameObject _cubeObject, int _gridX, int _gridY, int _gridZ)
    {
        worldPosition = _worldPos;
        cubeObject = _cubeObject;
        gridX = _gridX;
        gridY = _gridY;
        gridZ = _gridZ;
    }
}