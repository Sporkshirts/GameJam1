using UnityEngine;

public class NerveSpawn : MonoBehaviour
{
    [SerializeField] GameObject nervePrefab, parentObject;
        
    [SerializeField]
    [Range(1, 1000)]

    int length = 1;

    [SerializeField] float nerveDistance = 0.21f;

    [SerializeField] bool reset, spawn, snapFirst, snapLast;



    // Update is called once per frame
    void Update()
    {
        if(reset)
        {
            foreach (GameObject tmp in GameObject.FindGameObjectsWithTag("Player"))
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

            tmp = Instantiate(nervePrefab, new Vector3(transform.position.x, transform.position.y * nerveDistance* (x+1), transform.position.z), Quaternion.identity, parentObject.transform);
            tmp.transform.eulerAngles = new Vector3(180,0,0);

            tmp.name = parentObject.transform.childCount.ToString();

            if(x == 0)
            { 
                Destroy(tmp.GetComponent<CharacterJoint>());
            }
            else
            {
                tmp.GetComponent<CharacterJoint>().connectedBody = parentObject.transform.Find((parentObject.transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
            }


        }
    }

}
