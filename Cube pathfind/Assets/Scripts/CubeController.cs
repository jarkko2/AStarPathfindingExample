using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public bool Connection;
    public CubeManager.CubeType type;
    public MeshRenderer meshRend;
    public List<GridManager.Path> paths = new List<GridManager.Path>();
    public bool Occupied;
    public GameObject originalSeeker;

    // Engine is default connection required
    /// Add more, for example two yellows required etc
    public List<CubeManager.CubeType> connectionsRequired = new List<CubeManager.CubeType>() { CubeManager.CubeType.YELLOW };

    public void ClearPath(CubeManager.CubeType type)
    {
        foreach (GridManager.Path path in paths)
        {
            if (path.pathType == type)
            {
                path.ClearPath();
            }
        }
    }

    private void OnEnable()
    {
        if (GridManager.Instance)
        {
            GridManager.Instance.AddBlockToGrid(gameObject);
            CubeManager.Instance.AddToList(type, gameObject);
        }
        else
        {
            Debug.LogError("GridManager was not ready");
        }
        meshRend = GetComponent<MeshRenderer>();
        meshRend.enabled = true;
        transform.GetComponent<BoxCollider>().enabled = true;
    }

    private void Update()
    {
        if (type == CubeManager.CubeType.SOURCE)
        {
            meshRend.material = Connection ? CubeManager.Instance.Connected : CubeManager.Instance.NotConnected;
        }
        if (CubeManager.Instance.IsForceNotOccupied(type))
        {
            Occupied = false;
        }

        if (paths.Count > 0)
        {
            foreach (GridManager.Path path in paths)
            {
                if (path.pathObjects.Count <= 0 && path.path.Count > 0)
                {
                    StartCoroutine(path.GeneratePathObjects());
                }
            }
        }
    }

    public void CheckConnection()
    {
        bool noConnectionAtSomePoint = false;

        foreach (CubeManager.CubeType type in connectionsRequired)
        {
            foreach (var item in CubeManager.Instance.SortClosestType(gameObject, type))
            {
                bool connection = GridManager.Instance.CheckConnectionBetweenPoints(gameObject, item, type);
                if (!connection)
                {
                    noConnectionAtSomePoint = true;
                }
            }
        }
        Connection = !noConnectionAtSomePoint;
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            if (type == CubeManager.CubeType.PATH)
            {
                Debug.Log("Deleting");
                GridManager.Instance.RemoveBlockFromGrid(gameObject);
                meshRend.enabled = false;
                transform.GetComponent<BoxCollider>().enabled = false;
                Destroy(gameObject);
            }
            CubeManager.Instance.CheckConnections();
        }
    }
}
