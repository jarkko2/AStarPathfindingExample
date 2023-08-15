using UnityEngine;

public class PathBlockController : MonoBehaviour
{
    private Vector3 targetPos;
    public Material material;

    private void OnEnable()
    {
        targetPos = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        if (material != null)
        {
            transform.GetComponent<MeshRenderer>().material = material;
        }
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.01f);
    }
}