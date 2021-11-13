using UnityEngine;
using UnityEngine.Events;

enum TriggerCollisionPhase
{
    A, B
}

[RequireComponent(typeof(Collider))]
public class TriggerOnCollision : MonoBehaviour
{
    [SerializeField] private LayerMask checkCollisionWith;
    [SerializeField] private UnityEvent eventsToInvoke;
    [SerializeField] private bool twoPhase;
    [SerializeField] private UnityEvent eventsToInvokePhaseTwo;

    private TriggerCollisionPhase _phase;

    private void Awake()
    {
        _phase = TriggerCollisionPhase.A;
    }

    private void OnTriggerEnter(Collider other)
    {
        InvokeEvents(other.gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        InvokeEvents(other.gameObject);
    }

    private void InvokeEvents(GameObject anotherGameObject)
    {
        if ((checkCollisionWith.value & (1 << anotherGameObject.layer)) > 0) {
            switch (_phase)
            {
                case TriggerCollisionPhase.A:
                    eventsToInvoke.Invoke();
                    if (twoPhase)
                    {
                        _phase = TriggerCollisionPhase.B;
                    }
                    break;
                case TriggerCollisionPhase.B:
                    eventsToInvokePhaseTwo.Invoke();
                    if (twoPhase)
                    {
                        _phase = TriggerCollisionPhase.A;
                    }
                    break;
            }
        }
    }
}
