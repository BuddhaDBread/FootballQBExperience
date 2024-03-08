using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldBall : MonoBehaviour
{
    [SerializeField]
    private Transform   _ballHoldPos;
    private Collider    _ballCollider;
    private bool        _ballCaught      = false;

    // Properties
    public bool BallCaught { get { return _ballCaught; } set { _ballCaught = value; } }

    void Update()
    {
        if (_ballCollider != null)
        {
            _ballCollider.transform.position = _ballHoldPos.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ball")
        {
            _ballCaught = true;

            _ballCollider = other;
            var rb = other.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.Sleep();
            rb.detectCollisions = false;
            rb.isKinematic = true;

            other.transform.SetParent(transform);
            other.transform.position = _ballHoldPos.position;
        }
    }
}
