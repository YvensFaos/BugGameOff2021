using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    private Rigidbody _selfRigidBody;

    [SerializeField] 
    private ForceMode forceMode;

    private bool _push;
    private Vector3 _pushDirection;
    private float _force;

    private void Awake()
    {
        _selfRigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_push)
        {
            _push = false;
            _selfRigidBody.AddForce(_pushDirection * _force, forceMode);
        }
    }

    public void Push(Vector3 pushDirection, float force)
    {
        if (!_push)
        {
            _pushDirection = pushDirection;
            _force = force;
            _push = true;
        }
    }
}
