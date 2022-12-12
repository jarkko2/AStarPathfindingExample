using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBlockManager : MonoBehaviour
{
    public static PathBlockManager Instance;
    public Mesh CornerMesh;
    public Mesh DefaultMesh;
    // Start is called before the first frame update
    void Start()
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
    void Update()
    {
        
    }
}
