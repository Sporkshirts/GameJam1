using System;
using UnityEditor.Callbacks;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform damagePoint;
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private bool chase = false;
    [SerializeField] private bool returnToSpawn = false;
    [SerializeField] private float searchRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float cooldown;
    [SerializeField] private LayerMask searchMask;

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
        chase = SearchForPlayer();
        cooldownTimer -= Time.fixedDeltaTime;

        if (!chase)
        {
            if (returnToSpawn)
            {
                if (Vector3.Distance(transform.position, spawnPosition) > 0.2f) MoveToDestination(spawnPosition);
                else
                {
                    if (walk == true)
                    {
                        walk = false;
                        animator.SetBool("Walk", false);
                    }
                }
            }
            else if (Vector3.Distance(transform.position, lastTargetPosition) > 0.2f)
            {
                if (Vector3.Distance(transform.position, lastTargetPosition) > 0.2f) MoveToDestination(lastTargetPosition);
                if (Vector3.Distance(transform.position, lastTargetPosition) < 0.3f) returnToSpawn = true;
            }
            return;
        }

        if(cooldownTimer > 0)
        {
            return;
        }


        returnToSpawn = false;
        MoveToDestination(player.position);
        lastTargetPosition = player.position;


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
        cooldownTimer = cooldown;
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
