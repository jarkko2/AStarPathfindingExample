using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public bool connectionToEngine;
    public bool connectionToFuel;
    public CubeManager.CubeType type;
    public MeshRenderer meshRend;
    public List<GridManager.Path> paths = new List<GridManager.Path>();
    public bool Occupied;
    public GameObject originalSeeker;

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
        if (type == CubeManager.CubeType.WHEEL)
        {
            meshRend.material = connectionToEngine && connectionToFuel ? CubeManager.Instance.Connected : CubeManager.Instance.NotConnected;
        }
        if (type == CubeManager.CubeType.FUEL)
        {
            Occupied = false;
        }
        if (type == CubeManager.CubeType.ENGINE)
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
        //connectionToEngine = GridManager.Instance.CheckConnectionBetweenPoints(gameObject, CubeManager.Instance.GetClosestType(gameObject, CubeManager.CubeType.ENGINE), CubeManager.CubeType.ENGINE);
        //connectionToFuel = GridManager.Instance.CheckConnectionBetweenPoints(gameObject, CubeManager.Instance.GetClosestType(gameObject, CubeManager.CubeType.FUEL), CubeManager.CubeType.FUEL);

        foreach (var item in CubeManager.Instance.SortClosestType(gameObject, CubeManager.CubeType.ENGINE))
        {
            connectionToEngine = GridManager.Instance.CheckConnectionBetweenPoints(gameObject, item, CubeManager.CubeType.ENGINE);
            if (connectionToEngine)
            {
                break;
            }
        }

        foreach (var item in CubeManager.Instance.SortClosestType(gameObject, CubeManager.CubeType.FUEL))
        {
            connectionToFuel = GridManager.Instance.CheckConnectionBetweenPoints(gameObject, item, CubeManager.CubeType.FUEL);
            if (connectionToFuel)
            {
                break;
            }
        } 
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            if (type == CubeManager.CubeType.NORMAL)
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
