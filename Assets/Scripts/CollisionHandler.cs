using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] GameObject nervePrefab, parentObject, player, brain, flesh1, flesh2, BLink1, BLink2;
    [SerializeField] float fleshYOffset, brainYOffset;

    GameObject fleshAttached, brainAtttached;

    public NerveSpawn nerveSpawn;

    public void OnTriggerEnter(Collider other)
    {

        if (other.name == "Flesh1")
        {
            Debug.Log("It's 1.");

            nerveSpawn.Spawn2(flesh1);
            flesh1.transform.position = flesh1.transform.position + new Vector3(2.11f, 0.33f, 0.82f);
            Rigidbody rb = flesh1.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeAll;


            Rigidbody rb1 = BLink1.GetComponent<Rigidbody>();
            rb1.constraints = RigidbodyConstraints.FreezeAll;

            // update "score that Flesh1 is now tethered.
        }
        else if (other.name == "Flesh2")
        {
            Debug.Log("It's 2.");

            nerveSpawn.Spawn2(flesh2);
            flesh2.transform.position = flesh2.transform.position + new Vector3(-4.8f, 2.3f, 1.82f);
            Rigidbody rb = flesh2.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeAll;

            Rigidbody rb1 = BLink2.GetComponent<Rigidbody>();
            rb1.constraints = RigidbodyConstraints.FreezeAll;

            // update "score that Flesh1 is now tethered.
        }


    }



}
