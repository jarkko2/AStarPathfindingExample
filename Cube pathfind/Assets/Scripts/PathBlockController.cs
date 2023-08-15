using UnityEngine;

public class PathBlockController : MonoBehaviour
{
    private Vector3 targetPos;
    public Material material;

    private float moveSpeed = 2.0f;
    private float minDistance = 0.01f;

    private void OnEnable()
    {
        targetPos = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        if (material != null)
        {
            transform.GetComponent<MeshRenderer>().material = material;
        }
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, targetPos) > minDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
       
    }
}