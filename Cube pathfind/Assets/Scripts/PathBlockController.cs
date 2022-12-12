using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBlockController : MonoBehaviour
{
    private Vector3 targetPos;
    public Material material;

    void OnEnable()
    {
        targetPos = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        if (material != null)
        {
            transform.GetComponent<MeshRenderer>().material = material;
        }
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.01f);
    }
}
