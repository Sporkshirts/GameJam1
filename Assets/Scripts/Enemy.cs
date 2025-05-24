using UnityEngine;

public class Enemy : MonoBehaviour
{
    private enum EnemyState { Idle, Chasing, Searching, Returning }

    [SerializeField] private EnemyState currentState = EnemyState.Idle;

    [SerializeField] private Transform player;
    [SerializeField] private Transform damagePoint;
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float searchRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float cooldown;
    [SerializeField] private LayerMask searchMask;
    [SerializeField] AudioSource Grr;

    float cooldownTimer = 0;

    Vector3 spawnPosition;
    Vector3 lastTargetPosition;
    Rigidbody rb;

    bool walk = false;

    [SerializeField] Animator animator;

    private void Start()
    {
        spawnPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        cooldownTimer -= Time.fixedDeltaTime;

        bool seesPlayer = SearchForPlayer();

        switch (currentState)
        {
            case EnemyState.Idle:
                if (seesPlayer)
                {
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Chasing:
                if (seesPlayer)
                {
                    lastTargetPosition = player.position;
                    MoveToDestination(player.position);

                    if (Vector3.Distance(transform.position, player.position) <= attackRange && cooldownTimer <= 0)
                    {
                        Attack();
                        
                    }
                }
                else
                {
                    currentState = EnemyState.Searching;
                }
                break;

            case EnemyState.Searching:
                if (seesPlayer)
                {
                    currentState = EnemyState.Chasing;
                }

                if (Vector3.Distance(transform.position, lastTargetPosition) > 0.2f)
                {
                    MoveToDestination(lastTargetPosition);
                }
                else
                {
                    currentState = EnemyState.Returning;
                }
                break;

            case EnemyState.Returning:
                if (seesPlayer)
                {
                    currentState = EnemyState.Chasing;
                }

                if (Vector3.Distance(transform.position, spawnPosition) > 0.2f)
                {
                    MoveToDestination(spawnPosition);
                }
                else
                {
                    if (walk)
                    {
                        walk = false;
                        animator.SetBool("Walk", false);
                    }
                    currentState = EnemyState.Idle;
                }
                break;
        }
    }


    public float GetTimer()
    {
        return cooldownTimer;
    }

    public void Attack()
    {
        walk = false;
        animator.SetBool("Walk", false);
        animator.SetTrigger("Attack");
        Grr.Play();
        cooldownTimer = cooldown;

        NerveSpawn.Instance.DestroyConnectionToPlayer();
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
        if (walk == false)
        {
            walk = true;
            animator.SetBool("Walk", true);
        }

        RotateTowardsDestination(destination);
        Vector3 direction = (destination - transform.position).normalized;
        Vector3 newPosition = rb.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private bool SearchForPlayer()
    {
        Ray ray = new Ray(transform.position, (player.position - transform.position).normalized);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, searchRange, searchMask))
        {
            if (hitInfo.collider.CompareTag("Player")) return true;
        }
        return false;
    }

}
