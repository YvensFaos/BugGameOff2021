using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public void ActivateRespawnPoint()
    {
        GameLogic.Logic.SetRespawnPoint(transform.position);
    }
}
