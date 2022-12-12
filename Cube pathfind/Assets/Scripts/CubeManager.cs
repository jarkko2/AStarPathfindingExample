using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public static CubeManager Instance;
    public List<GameObject> Wheels;
    public List<GameObject> Engines;
    public List<GameObject> Fuels;

    public GameObject cube;
    private Vector3 worldPosition;
    public GameObject cursor;

    public Material Connected;
    public Material NotConnected;

    public List<GameObject> cubes = new List<GameObject>();

    // Update is called once per frame
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 50))
        {
            worldPosition = hit.point;
            float x = Mathf.RoundToInt(hit.point.x);
            float y = Mathf.RoundToInt(hit.point.y);
            float z = Mathf.RoundToInt(hit.point.z);
            cursor.transform.position = new Vector3(x, y, z);
            //cursor.transform.rotation = Quaternion.FromToRotation(cursor.transform.up, hitData.normal) * cursor.transform.rotation;

            if (hit.transform.GetComponent<CubeController>())
            {
                cursor.transform.position = (hit.transform.position + hit.transform.up) + (hit.normal - Vector3.up);
            }

            //Debug.Log(hitData.point);
            if (Input.GetMouseButtonDown(1))
            {
                //CreateCube(new Vector3(x, y, z));
                CreateCube(cursor.transform.position);
                //Grid.Instance.CreateCube(new Vector3(x, y, z));
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
        foreach (var wheel in Wheels)
        {
            wheel.transform.GetComponent<CubeController>().CheckConnection();
        }
    }

    public List<GameObject> SortClosestType(GameObject source, CubeType type)
    {
        if (type == CubeType.ENGINE)
        {
            return Engines.OrderBy(
               x => Vector3.Distance(source.transform.position, x.transform.position)
              ).ToList();
        }
        if (type == CubeType.FUEL)
        {
            return Fuels.OrderBy(
               x => Vector3.Distance(source.transform.position, x.transform.position)
              ).ToList();
        }
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

    public enum CubeType
    {
        NORMAL,
        ENGINE,
        WHEEL,
        FUEL
    }

    public void AddToList(CubeType type, GameObject cube)
    {
        if (type == CubeType.ENGINE)
        {
            Engines.Add(cube);
        }
        if (type == CubeType.WHEEL)
        {
            Wheels.Add(cube);
        }
        if (type == CubeType.FUEL)
        {
            Fuels.Add(cube);
        }
    }

}
