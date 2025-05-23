using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && enemy.GetTimer() <= 0f)
        {
            enemy.Attack();
        }
    }
}
