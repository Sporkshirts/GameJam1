using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] GameObject nervePrefab, parentObject, player, brain, flesh1, flesh2;
    [SerializeField] float fleshYOffset, brainYOffset;

    GameObject fleshAttached, brainAtttached;

    public NerveSpawn nerveSpawn;

    public void OnTriggerEnter(Collider other)
    {

        if (other.name == "Flesh1")
        {
            Debug.Log("It's 1.");

            nerveSpawn.Spawn2(flesh1);

            // update "score that Flesh1 is now tethered.
        }
        else if (other.name == "Flesh2")
        {
            Debug.Log("It's 2.");
            // update "score that Flesh1 is now tethered.
        }


    }



}
