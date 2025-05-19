using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private bool chase = false;
    [SerializeField] private bool returnToSpawn = false;
    [SerializeField] private float searchRange;
    [SerializeField] private LayerMask searchMask;

    Vector3 spawnPosition;
    Vector3 lastTargetPosition;
    Rigidbody rb;

    private void Start()
    {
        spawnPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        chase = SearchForPlayer();
        if (!chase)
        {
            if(returnToSpawn)
            {
                if (Vector3.Distance(transform.position, spawnPosition) > 0.2f) MoveToDestination(spawnPosition);
            }
            else if (Vector3.Distance(transform.position, lastTargetPosition) > 0.2f)
            {
                if (Vector3.Distance(transform.position, lastTargetPosition) > 0.2f) MoveToDestination(lastTargetPosition);
                if(Vector3.Distance(transform.position, lastTargetPosition) < 0.3f) returnToSpawn = true;
            }
        }
        else
        {
            returnToSpawn = false;
            MoveToDestination(player.position);
            lastTargetPosition = player.position;
        }
    }

    private void RotateTowardsDestination(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void MoveToDestination(Vector3 destination)
    {
        RotateTowardsDestination(destination);
        Vector3 direction = (destination - transform.position).normalized;
        Vector3 newPosition = rb.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private bool SearchForPlayer()
    {
        Ray ray = new Ray(transform.position, (player.position - transform.position).normalized);
        if(Physics.Raycast(ray, out RaycastHit hitInfo, searchRange, searchMask))
        {
            if (hitInfo.collider.CompareTag("Player")) return true;
        }
        return false;
    }

    //Old Search Method
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            chase = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            chase = false;
        }
    }*/
}
