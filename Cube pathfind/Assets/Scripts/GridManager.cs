using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    private List<Cube> grid = new List<Cube>();
    //private List<Cube> path = new List<Cube>();
    public GameObject pathObject;

    //public List<GameObject> pathObjects;
    public float delayOnSpawning;

    [System.Serializable]
    public class Path
    {
        public CubeManager.CubeType pathType;
        public List<Cube> path = new List<Cube>();
        public List<GameObject> pathObjects = new List<GameObject>();

        public void ClearPath()
        {
            path.Clear();
            for (int i = 0; i < pathObjects.Count; i++)
            {
                Destroy(pathObjects[i]);
            }
            pathObjects.Clear();
        }

        public IEnumerator GeneratePathObjects()
        {
            for (int i = 0; i < path.Count; i++)
            {
                PathBlockManager.PathConfiguration configuration = PathBlockManager.Instance.GetConfigurationByType(pathType);
                Material material = configuration.PathMaterial;
                Vector3 offset = configuration.Offset;
                float Width = configuration.Width;
                float Height = 1.1f;

                GameObject pathObj = Instantiate(Instance.pathObject, path[i].worldPosition + new Vector3(0, 0.1f, 0), Quaternion.identity);
                pathObj.GetComponent<MeshRenderer>().material = material;

                if (i + 1 >= path.Count || i <= 0)
                {
                    pathObj.transform.localScale = new Vector3(Width + offset.x, Height + offset.y, Width + offset.z);
                    pathObjects.Add(pathObj);
                }
                else
                {
                    Mesh mesh;
                    int rotation;
                    pathObj.transform.localScale = FigureOutScaleRotationAndMesh(path[i].worldPosition, path[i + 1].worldPosition, path[i - 1].worldPosition, offset, Width, Height, out mesh, out rotation);
                    pathObj.transform.GetComponent<MeshFilter>().mesh = mesh;
                    pathObj.transform.Rotate(new Vector3(0, rotation, 0));
                    pathObjects.Add(pathObj);
                }
                yield return new WaitForSeconds(Instance.delayOnSpawning);
            }       
        }
        public Vector3 FigureOutScaleRotationAndMesh(Vector3 start, Vector3 next, Vector3 behind, Vector3 offset, float width, float height, out Mesh mesh, out int rotation)
        {
            Vector3 scale = new Vector3(0.5f + offset.x, height + offset.y, 0.5f + offset.z);
            Mesh tempmesh = PathBlockManager.Instance.DefaultMesh;
            int tempRotation = 0;
            float DefaultWidthLine = 0.2f;
            float DefaultWidthFull = 1.0f;
            // - - right
            if (start.x < next.x && start.z == next.z && behind.z == start.z && behind.x < start.x)
            {
                scale = new Vector3(DefaultWidthFull, height + offset.y, DefaultWidthLine + offset.z);
            }
            // left
            if (start.x > next.x && start.z == next.z && behind.z == start.z && behind.x > start.x)
            {
                scale = new Vector3(DefaultWidthFull, height + offset.y, DefaultWidthLine + offset.z);
            }

            // | |
            // down
            if (start.x == next.x && start.z > next.z && behind.z > start.z && behind.x == start.x)
            {
                scale = new Vector3(DefaultWidthLine + offset.x, height + offset.y, DefaultWidthFull);
            }
            // up
            if (start.x == next.x && start.z < next.z && behind.z < start.z && behind.x == start.x)
            {
                scale = new Vector3(DefaultWidthLine + offset.x, height + offset.y, DefaultWidthFull);
            }

            // corner from up to left
            if (next.x < start.x && next.z == start.z && behind.z > start.z && behind.x == start.x)
            {
                tempmesh = PathBlockManager.Instance.CornerMesh;
                scale = new Vector3(DefaultWidthFull + offset.x, height + offset.y, DefaultWidthFull + offset.z);
            }
            // corner from up to right
            if (next.x > start.x && next.z == start.z && behind.z > start.z && behind.x == start.x)
            {
                tempmesh = PathBlockManager.Instance.CornerMesh;
                scale = new Vector3(DefaultWidthFull + offset.x, height + offset.y, DefaultWidthFull + offset.z);
                tempRotation = 90;
            }
            // corner from down to left
            if (next.x < start.x && next.z == start.z && behind.z < start.z && behind.x == start.x)
            {
                tempmesh = PathBlockManager.Instance.CornerMesh;
                scale = new Vector3(DefaultWidthFull + offset.x, height + offset.y, DefaultWidthFull + offset.z);
                tempRotation = 270;
            }
            // corner from left to down
            if (next.x == start.x && next.z < start.z && behind.x < start.x && behind.z == start.z)
            {
                tempmesh = PathBlockManager.Instance.CornerMesh;
                scale = new Vector3(DefaultWidthFull + offset.x, height + offset.y, DefaultWidthFull + offset.z);
                tempRotation = 270;
            }
            // corner from right to up
            if (next.x == start.x && next.z > start.z && behind.x > start.x && behind.z == start.z)
            {
                tempmesh = PathBlockManager.Instance.CornerMesh;
                scale = new Vector3(DefaultWidthFull + offset.x, height + offset.y, DefaultWidthFull + offset.z);
                tempRotation = 90;
            }
            // corner from right to down
            if (next.x == start.x && next.z < start.z && behind.x > start.x && behind.z == start.z)
            {
                tempmesh = PathBlockManager.Instance.CornerMesh;
                scale = new Vector3(DefaultWidthFull + offset.x, height + offset.y, DefaultWidthFull + offset.z);
                tempRotation = 180;
            }
            // corner from down to right
            if (next.x > start.x && next.z == start.z && behind.x == start.x && behind.z < start.z)
            {
                tempmesh = PathBlockManager.Instance.CornerMesh;
                scale = new Vector3(1f + offset.x, height + offset.y, DefaultWidthFull + offset.z);
                tempRotation = 180;
            }
            // corner from left to up
            if (next.x == start.x && next.z > start.z && behind.x < start.x && behind.z == start.z)
            {
                tempmesh = PathBlockManager.Instance.CornerMesh;
                scale = new Vector3(DefaultWidthFull + offset.x, height + offset.y, DefaultWidthFull + offset.z);
                tempRotation = 0;
            }
            mesh = tempmesh;
            rotation = tempRotation;
            return scale;
        }
    }

    public void Awake()
    {
        Instance = this;
    }

    public void AddBlockToGrid(GameObject cube)
    {
        int x = Mathf.RoundToInt(cube.transform.position.x);
        int y = Mathf.RoundToInt(cube.transform.position.y);
        int z = Mathf.RoundToInt(cube.transform.position.z);
        Debug.Log("Position of cube: " + cube.transform.position);
        grid.Add(new Cube(cube.transform.position, cube, x, y, z));
    }

    public void RemoveBlockFromGrid(GameObject cube)
    {
        Cube cubeRef = FindCubeOfGameObjectInGrid(cube);
        grid.Remove(cubeRef);
        CubeManager.Instance.cubes.Remove(cube);
    }

    //private void OnDrawGizmos()
    //{
    //    if (grid != null)
    //    {
    //        foreach (Cube n in grid)
    //        {
    //            Debug.Log(n.worldPosition);
    //            Gizmos.color = Color.green;
    //            //if (path != null)
    //            //{
    //            //    if (path.Contains(n))
    //            //    {
    //            //        Gizmos.color = Color.black;
    //            //    }
    //            //}
    //            Gizmos.DrawCube(n.worldPosition, Vector3.one - new Vector3(0.1f, -1.1f, 0.1f));
    //        }
    //    }
    //}

    public Cube FindCubeOfGameObjectInGrid(GameObject cube)
    {
        for (int i = 0; i < grid.Count; i++)
        {
            if (grid[i].cubeObject == cube)
            {
                Debug.Log("Found: " + grid[i]);
                return grid[i];
            }
        }
        Debug.Log("Did not found");
        return null;
    }

    public bool CheckConnectionBetweenPoints(GameObject seeker, GameObject target, CubeManager.CubeType type)
    {
        List<Cube> open = new List<Cube>();
        List<Cube> closed = new List<Cube>();

        open.Add(FindCubeOfGameObjectInGrid(seeker));

        while (open.Count > 0)
        {
            Cube current = open[0];

            for (int i = 1; i < open.Count; i++)
            {
                if (open[i].fCost < current.fCost || open[i].fCost == current.fCost)
                {
                    if (open[i].hCost < current.hCost)
                    {
                        current = open[i];
                    }
                }
            }

            open.Remove(current);
            closed.Add(current);

            if (current == FindCubeOfGameObjectInGrid(target))
            {
                seeker.transform.GetComponent<CubeController>().ClearPath(type);
                RetracePath(FindCubeOfGameObjectInGrid(seeker), FindCubeOfGameObjectInGrid(target), type);
                return true;
            }

            foreach (Cube neigbour in GetNeighbours(current, seeker))
            {
                if (closed.Contains(neigbour))
                {
                    continue;
                }

                int newMovementCostToNeigbour = current.gCost + GetDistance(current, neigbour);
                if (newMovementCostToNeigbour < neigbour.gCost || !open.Contains(neigbour))
                {
                    neigbour.gCost = newMovementCostToNeigbour;
                    neigbour.hCost = GetDistance(neigbour, FindCubeOfGameObjectInGrid(target));
                    neigbour.parent = current;
                    if (!open.Contains(neigbour))
                    {
                        open.Add(neigbour);
                    }
                }
            }
        }

        if (seeker.transform.GetComponent<CubeController>())
        {
            seeker.transform.GetComponent<CubeController>().ClearPath(type);
        }
        return false;
    }

    private void RetracePath(Cube startNode, Cube endNode, CubeManager.CubeType type)
    {
        List<Cube> foundPath = new List<Cube>();
        Cube currentNode = endNode;
        
        while (currentNode != startNode)
        {
            foundPath.Add(currentNode);
            currentNode = currentNode.parent;
        }
        // Add the source too
        foundPath.Add(startNode);

        foundPath.Reverse();

        for (int i = 0; i < foundPath.Count; i++)
        {
            foundPath[i].cubeObject.transform.GetComponent<CubeController>().Occupied = true;
            foundPath[i].cubeObject.transform.GetComponent<CubeController>().originalSeeker = startNode.cubeObject;
        }
        foundPath[0].cubeObject.transform.GetComponent<CubeController>().Occupied = false;
        foundPath[foundPath.Count - 1].cubeObject.transform.GetComponent<CubeController>().Occupied = false;

        Path path = new Path
        {
            pathType = type,
            path = foundPath
        };
        startNode.cubeObject.transform.GetComponent<CubeController>().paths.Add(path);
    }

    //TODO call in start to get neighbours already
    public List<Cube> GetNeighbours(Cube cube, GameObject origSeeker)
    {
        List<Cube> neighbours = new List<Cube>();
        Collider[] hits = Physics.OverlapSphere(cube.cubeObject.transform.position, 0.5f);
       
        foreach (Collider hit in hits)
        {
            Debug.Log(hit);
            if (hit.transform.gameObject != cube.cubeObject && hit.transform.GetComponent<CubeController>())
            {
                Debug.Log(hit.transform.position);

                if (hit.transform.GetComponent<CubeController>().Occupied == false)
                {
                    neighbours.Add(FindCubeOfGameObjectInGrid(hit.transform.gameObject));
                }
                if (hit.transform.GetComponent<CubeController>().Occupied == true && hit.transform.GetComponent<CubeController>().originalSeeker == origSeeker)
                {
                    Debug.Log("Same sourcecontroller");
                    neighbours.Add(FindCubeOfGameObjectInGrid(hit.transform.gameObject));
                }
            }
        }
        return neighbours;
    }

    private int GetDistance(Cube nodeA, Cube nodeB)
    {
        //int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        //int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        //int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        //int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        //int dstZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);


        //if (dstX > dstY)
        //{
        //    return 14 * dstY + 10 * (dstX - dstY);
        //}

        //return 14 * dstX + 10 * (dstY - dstX);
        return Mathf.RoundToInt(Vector3.Distance(new Vector3(nodeA.gridX, nodeA.gridY, nodeA.gridZ), new Vector3(nodeB.gridX, nodeB.gridY, nodeB.gridZ)));
    }

}
