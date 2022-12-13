using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public static CubeManager Instance;

    public GameObject cube;
    public List<GameObject> cubes = new List<GameObject>();
    public GameObject cursor;

    public Material Connected;
    public Material NotConnected;

    public enum CubeType
    {
        PATH,
        YELLOW,
        SOURCE,
        RED,
        PURPLE
    }

    [System.Serializable]
    public class CubeSetting
    {
        public CubeType Type;
        public bool ForceNotOccupied;
    }
    public List<CubeSetting> cubeSettings = new List<CubeSetting>();

    public bool IsForceNotOccupied (CubeType type)
    {
        for (int i = 0; i < cubeSettings.Count; i++)
        {
            if (cubeSettings[i].Type == type)
            {
                return cubeSettings[i].ForceNotOccupied;
            }
        }
        return false;
    }

    [System.Serializable]
    public class BlockType
    {
        public CubeType Type;
        public List<GameObject> Objects = new List<GameObject>();
    }

    public List<BlockType> blockTypes = new List<BlockType>();

    // Update is called once per frame
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 50))
        {
            float x = Mathf.RoundToInt(hit.point.x);
            float y = Mathf.RoundToInt(hit.point.y);
            float z = Mathf.RoundToInt(hit.point.z);

            cursor.transform.position = new Vector3(x, y, z);

            // Do not rotate on plane
            if (hit.transform.GetComponent<CubeController>())
            {
                cursor.transform.position = (hit.transform.position + hit.transform.up) + (hit.normal - Vector3.up);
            }

            if (Input.GetMouseButtonDown(1))
            {
                CreateCube(cursor.transform.position);
                CheckConnections();
            }
        }

    }

    private void Start()
    {
        CheckConnections();
    }

    private void CreateCube(Vector3 pos)
    {
        GameObject go = Instantiate(cube, pos, Quaternion.identity);
        cubes.Add(go);
    }

    public void CheckConnections()
    {
        for (int i = 0; i < cubes.Count; i++)
        {
            cubes[i].transform.GetComponent<CubeController>().Occupied = false;
        }
        for (int i = 0; i < blockTypes.Count; i++)
        {
            if (blockTypes[i].Type == CubeType.SOURCE)
            {
                foreach (GameObject source in blockTypes[i].Objects)
                {
                    source.transform.GetComponent<CubeController>().CheckConnection();
                }
            }
        }
    }

    public List<GameObject> SortClosestType(GameObject source, CubeType type)
    {
        for (int i = 0; i < blockTypes.Count; i++)
        {
            if (blockTypes[i].Type == type)
            {
                return blockTypes[i].Objects.OrderBy(
                x => Vector3.Distance(source.transform.position, x.transform.position)
                ).ToList();
            }
        }
        Debug.LogError("Did not found object types!");
        return null;
    }


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void AddToList(CubeType type, GameObject cube)
    {
        for (int i = 0; i < blockTypes.Count; i++)
        {
            if (blockTypes[i].Type == type)
            {
                blockTypes[i].Objects.Add(cube);
                return;
            }
        }
        BlockType blockType = new BlockType();
        blockType.Type = type;
        blockType.Objects.Add(cube);
        blockTypes.Add(blockType);
    }
}
