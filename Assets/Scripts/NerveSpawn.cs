using System;
using System.Collections;
using UnityEngine;

public class NerveSpawn : MonoBehaviour
{

    [SerializeField] GameObject nervePrefab, parentObject, player, brain, flesh1, flesh2, BLink1, BLink2;
    [SerializeField] float playerYOffset, brainYOffset, flesh1YOffset, flesh2YOffset, bLink1YOffset, bLink2YOffset;
    [SerializeField] LayerMask brainLayer;

    [SerializeField]
    [Range(1, 1000)]

    int length = 1;

    [SerializeField] float nerveDistance = 0.21f;

    [SerializeField] bool reset, spawn, snapFirst, snapLast;

    GameObject playerAttached, brainAtttached, flesh1Attached, flesh2Attached, bLink1Attached, bLink2Attached;






    // Update is called once per frame
    void Update()
    {
        if (reset)
        {
            foreach (GameObject tmp in GameObject.FindGameObjectsWithTag("Nerve"))
            {
                Destroy(tmp);
            }
            reset = false;
        }

        if (spawn)
        {
            Spawn();

            spawn = false;
        }

    }

    public void Spawn()
    {
        if (parentObject == null || nervePrefab == null) return;

        int count = Mathf.CeilToInt(length / nerveDistance);

        GameObject previousSegment = null;

        Vector3 startPoint = brain.transform.position + Vector3.up * brainYOffset;
        Vector3 endPoint = player.transform.position + Vector3.up * playerYOffset;
        Vector3 direction = (endPoint - startPoint).normalized;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.down, direction);

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = startPoint + direction * (nerveDistance * i);

            GameObject segment = Instantiate(nervePrefab, spawnPos, rotation, parentObject.transform);
            segment.name = (i + 1).ToString();

            if (i == 0)
            {
                brainAtttached = segment;
            }
            if (i == count - 1)
            {
                playerAttached = segment;
            }

            Rigidbody rb = segment.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (i > 0 && previousSegment != null)
            {
                CharacterJoint joint = segment.GetComponent<CharacterJoint>();
                if (joint != null)
                {
                    joint.connectedBody = previousSegment.GetComponent<Rigidbody>();
                }
            }
            else
            {
                Destroy(segment.GetComponent<CharacterJoint>());
            }

            previousSegment = segment;
        }

        if (brain != null && Physics.Raycast(player.transform.position, brain.transform.position - player.transform.position, out RaycastHit hit, 100f, brainLayer))
        {
            Rigidbody brainRb = brain.GetComponent<Rigidbody>();

            if (brainRb != null && brainAtttached != null)
            {
                brainAtttached.transform.position = hit.point + Vector3.up * brainYOffset;

                CharacterJoint brainJoint = brainAtttached.AddComponent<CharacterJoint>();
                brainJoint.connectedBody = brainRb;
            }
        }

        if (player != null)
        {

            if (playerAttached != null)
            {
                playerAttached.GetComponent<Renderer>().enabled = false;
                playerAttached.transform.position = player.transform.position + Vector3.up * playerYOffset;

                Rigidbody playerRb = player.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    CharacterJoint playerJoint = player.AddComponent<CharacterJoint>();
                    playerJoint.connectedBody = playerAttached.GetComponent<Rigidbody>();
                }
            }
        }


    }

    public void MoveConnectionToFlesh(GameObject flesh, Vector3 contactPoint)
    {
        if (playerAttached == null)
        {
            return;
        }

        Destroy(player.GetComponent<CharacterJoint>());

        GameObject fleshAttached = playerAttached;
        playerAttached = null;

        fleshAttached.transform.position = contactPoint;

        CharacterJoint fleshJoint = flesh.AddComponent<CharacterJoint>();
        fleshJoint.connectedBody = fleshAttached.GetComponent<Rigidbody>();
    }

    public bool IsAttachedToPlayer()
    {
        return playerAttached;
    }


    public void Spawn2(GameObject flesh)
    {
        if (flesh == null) return;

        int count = Mathf.Max(1, (int)(length / nerveDistance));
        GameObject firstLink = null;
        GameObject lastLink = null;

        // Spawn the chain of nerves
        for (int x = 0; x < count; x++)
        {
            GameObject link = Instantiate(
                nervePrefab,
                transform.position + Vector3.up * nerveDistance * x,
                Quaternion.Euler(180, 0, 0),
                parentObject.transform
            );

            link.name = parentObject.transform.childCount.ToString();

            if (x == 0)
            {
                firstLink = link;
                Destroy(link.GetComponent<CharacterJoint>());
            }
            else
            {
                Rigidbody previousBody = parentObject.transform
                    .GetChild(parentObject.transform.childCount - 2)
                    .GetComponent<Rigidbody>();

                link.GetComponent<CharacterJoint>().connectedBody = previousBody;
            }

            if (x == count - 1)
            {
                lastLink = link;
            }
        }

        // Determine the proper BLink and Y offsets
        GameObject targetBLink = null;
        float fleshYOffset = 0f;
        float bLinkYOffset = 0f;

        if (flesh == flesh1)
        {
            targetBLink = BLink1;
            fleshYOffset = flesh1YOffset;
            bLinkYOffset = bLink1YOffset;
        }
        else if (flesh == flesh2)
        {
            targetBLink = BLink2;
            fleshYOffset = flesh2YOffset;
            bLinkYOffset = bLink2YOffset;
        }

        // Connect first link to flesh
        if (firstLink != null && flesh != null)
        {
            firstLink.transform.position = flesh.transform.position + Vector3.up * fleshYOffset;
            CharacterJoint joint = flesh.AddComponent<CharacterJoint>();
            joint.connectedBody = firstLink.GetComponent<Rigidbody>();
        }

        // Connect last link to BLink
        if (lastLink != null && targetBLink != null)
        {
            lastLink.transform.position = targetBLink.transform.position + Vector3.up * bLinkYOffset;
            CharacterJoint joint = lastLink.AddComponent<CharacterJoint>();
            joint.connectedBody = targetBLink.GetComponent<Rigidbody>();
        }
    }
}



