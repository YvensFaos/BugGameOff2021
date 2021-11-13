using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Push(Vector3 pushDirection, float force)
    {
        Debug.Log($"Push {pushDirection} {force}");
        _rigidbody.AddForce(pushDirection * force);
    }
}
