using DG.Tweening;
using UnityEngine;

public class MoveEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToMove;
    [SerializeField]
    private Transform moveTo;
    [SerializeField]
    private float velocity;
    [SerializeField]
    private float time;
    [SerializeField]
    private bool calculateTime;

    public void Move()
    {
        if (calculateTime)
        {
            var distance = Vector3.Distance(moveTo.position, objectToMove.transform.position);
            time = distance / velocity;
        }

        objectToMove.transform.DOMove(moveTo.position, time);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(objectToMove.transform.position, moveTo.position);
    }
}