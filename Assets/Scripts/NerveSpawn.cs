using UnityEngine;

public class NerveSpawn : MonoBehaviour
{
    
    [SerializeField] GameObject nervePrefab, parentObject, player, brain, flesh1, flesh2;
    [SerializeField] float playerYOffset, brainYOffset, flesh1YOffset, flesh2YOffset;
    [SerializeField] LayerMask brainLayer;

    [SerializeField]
    [Range(1, 1000)]

    int length = 1;

    [SerializeField] float nerveDistance = 0.21f;

    [SerializeField] bool reset, spawn, snapFirst, snapLast;

    GameObject playerAttached, brainAtttached, flesh1Attached, flesh2Attached;

   




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
        int count = (int)(length / nerveDistance);

        for (int x = 0; x < count; x++)
        {
            GameObject tmp = Instantiate(
                nervePrefab,
                new Vector3(transform.position.x, transform.position.y * nerveDistance * (x + 1), transform.position.z),
                Quaternion.identity,
                parentObject.transform
            );

            tmp.transform.eulerAngles = new Vector3(180, 0, 0);
            tmp.name = parentObject.transform.childCount.ToString();

            if (x == 0)
            {
                Destroy(tmp.GetComponent<CharacterJoint>());
            }
            else
            {
                tmp.GetComponent<CharacterJoint>().connectedBody =
                    parentObject.transform.Find((parentObject.transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
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

        AttachFlesh(flesh);
    }




    private void AttachFlesh(GameObject flesh)
    {
        if (flesh == null) return;

        float yOffset = (flesh == flesh1) ? flesh1YOffset :
                        (flesh == flesh2) ? flesh2YOffset : 0;

        GameObject attached = parentObject.transform.Find(parentObject.transform.childCount.ToString())?.gameObject;

        if (attached)
        {
            attached.transform.position = new Vector3(
                flesh.transform.position.x,
                flesh.transform.position.y + yOffset,
                flesh.transform.position.z
            );

            flesh.AddComponent<CharacterJoint>().connectedBody = attached.GetComponent<Rigidbody>();

            if (flesh == flesh1) flesh1Attached = attached;
            else if (flesh == flesh2) flesh2Attached = attached;
        }
    }



}



