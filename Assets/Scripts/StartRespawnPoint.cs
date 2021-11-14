using UnityEngine;

public class StartRespawnPoint : MonoBehaviour
{
    private void Start()
    {
        GameLogic.Logic.SetRespawnPoint(transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 1.0f);
    }
}
