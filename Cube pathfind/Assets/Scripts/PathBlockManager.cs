using System.Collections.Generic;
using UnityEngine;

public class PathBlockManager : MonoBehaviour
{
    public static PathBlockManager Instance;
    public Mesh CornerMesh;
    public Mesh DefaultMesh;

    [System.Serializable]
    public class PathConfiguration
    {
        public CubeType Type;
        public Material PathMaterial;
        public float Width;
        public Vector3 Offset;
    }

    public List<PathConfiguration> pathConfigurations = new List<PathConfiguration>();

    public PathConfiguration GetConfigurationByType(CubeType type)
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

    private void Awake()
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
}