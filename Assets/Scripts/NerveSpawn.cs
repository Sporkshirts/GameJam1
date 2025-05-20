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
        int count = (int)(length / nerveDistance);

        for (int x = 0; x < count; x++)
        {
            GameObject tmp;

            tmp = Instantiate(nervePrefab, new Vector3(transform.position.x, transform.position.y * nerveDistance * (x + 1), transform.position.z), Quaternion.identity, parentObject.transform);
            tmp.transform.eulerAngles = new Vector3(180, 0, 0);

            tmp.name = parentObject.transform.childCount.ToString();

            if (x == 0)
            {
                Destroy(tmp.GetComponent<CharacterJoint>());
            }
            else
            {
                tmp.GetComponent<CharacterJoint>().connectedBody = parentObject.transform.Find((parentObject.transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
            }
        }

        if (brain)
        {
            if (Physics.Raycast(player.transform.position, brain.transform.position - player.transform.position, out RaycastHit hit, 100f, brainLayer))
            {
                brainAtttached = parentObject.transform.Find("1").gameObject;
                brainAtttached.transform.position = new Vector3(hit.point.x, hit.point.y + brainYOffset, hit.point.z);
                brainAtttached.AddComponent<CharacterJoint>().connectedBody = brain.GetComponent<Rigidbody>();
            }
        }

        if (player)
        {
            playerAttached = parentObject.transform.Find(parentObject.transform.childCount.ToString()).gameObject;
            playerAttached.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + playerYOffset, player.transform.position.z);
            player.AddComponent<CharacterJoint>().connectedBody = playerAttached.GetComponent<Rigidbody>();
        }


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



