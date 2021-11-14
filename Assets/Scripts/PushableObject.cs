using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [SerializeField] 
    private ForceMode forceMode;
    [SerializeField]
    private float moveAmount;
    //[SerializeField]
    //private float time;

    private bool _push;
    private Vector3 _pushDirection;
    private float _force;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_push)
        {
            _push = false;
            _rigidbody.AddForce(_pushDirection * (_force * Time.deltaTime), forceMode);
        }
    }

    public void Push(Vector3 pushDirection, float force)
    {
        Debug.Log($"Push {pushDirection} {force}");
        _pushDirection = pushDirection;
        _force = force;
        _push = true;
        // var moveTo = transform.position + pushDirection * moveAmount;
        // _rigidbody.DOMove(moveTo, time);
        // _rigidbody.AddForce(pushDirection * (force * Time.deltaTime), forceMode);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, moveAmount);
    }
}
