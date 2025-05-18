using Unity.Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float yOffset = 0.6f;


    private void Update()
    {
        transform.position = new Vector3(player.position.x, player.position.y + yOffset, player.position.z);
        
    }
}
