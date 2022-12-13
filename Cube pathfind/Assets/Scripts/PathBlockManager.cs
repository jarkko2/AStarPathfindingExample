using System.Collections.Generic;
using UnityEngine;

public class PathBlockManager : MonoBehaviour
{
    public static PathBlockManager Instance;
    public Mesh CornerMesh;
    public Mesh DefaultMesh;

    public List<Material> Materials = new List<Material>();

    [System.Serializable]
    public class PathConfiguration
    {
        public CubeManager.CubeType Type;
        public Material PathMaterial;
        public float Width;
        public Vector3 Offset;
    }

    public List<PathConfiguration> pathConfigurations = new List<PathConfiguration>();

    public PathConfiguration GetConfigurationByType(CubeManager.CubeType type)
    {
        foreach (PathConfiguration pathConf in pathConfigurations)
        {
            if (pathConf.Type == type)
            {
                return pathConf;
            }
        }
        // Return first if not found
        return pathConfigurations[0];
    }

    // Start is called before the first frame update
    private void Start()
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

    // Update is called once per frame
    private void Update()
    {

    }
}
