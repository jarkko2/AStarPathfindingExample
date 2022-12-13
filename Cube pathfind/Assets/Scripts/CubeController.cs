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

    public List<GameObject> ConnectionNodes = new List<GameObject>();

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
        meshRend.enabled = type != CubeManager.CubeType.PATH;
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

    public void CheckNodeConnections()
    {
        Debug.Log("ASDASD");
        // Clear first
        foreach (GameObject node in ConnectionNodes)
        {
            node.GetComponent<MeshRenderer>().enabled = false;
        }
        foreach (GameObject node in ConnectionNodes)
        {
            Collider[] hits = Physics.OverlapSphere(node.transform.position, 0.25f);
            List<Collider> templistHits = new List<Collider>();
            List<Collider> listHits = new List<Collider>();
            for (int i = 0; i < hits.Length; i++)
            {
                templistHits.Add(hits[i]);
            }
            for (int i = 0; i < templistHits.Count; i++)
            {
                if (!ConnectionNodes.Contains(templistHits[i].gameObject) && templistHits[i].gameObject != gameObject
                    && CubeManager.Instance.IsBuildable(templistHits[i].GetComponent<CubeController>().type))
                {
                    Debug.Log("Hit" + templistHits[i].gameObject.name);
                    listHits.Add(templistHits[i]);
                }
            }
            if (listHits.Count > 0)
            {
                node.GetComponent<MeshRenderer>().enabled = true;
            }
        }
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
