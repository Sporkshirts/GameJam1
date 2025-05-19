using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private bool chase = false;
    Vector3 spawnPosition;
    Rigidbody rb;

    private void Start()
    {
        spawnPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!chase)
        {
            if (Vector3.Distance(transform.position, spawnPosition) > 0.2f)
            {
                RotateTowardsDestination(spawnPosition);
                Vector3 direction = (spawnPosition - transform.position).normalized;
                Vector3 newPosition = rb.position + direction * speed * Time.fixedDeltaTime;
                rb.MovePosition(newPosition);
            }

        }
        else
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Vector3 newPosition = rb.position + direction * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
            RotateTowardsDestination(player.position);
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

    private void OnTriggerEnter(Collider other)
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
    }
}
