using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class FieldAwareness : MonoBehaviour
{
    private Vector3 _ballDestinationPos;
    private Vector3 _ballPos;
    private bool    _destinationInRange = false;
    private bool    _ballInAir          = false;

    // Properties
    public Vector3  BallDestination     { get { return _ballDestinationPos; } }
    public Vector3  BallPos             { get { return _ballPos; } }
    public bool     DestinationInRange  { get { return _destinationInRange; } set { _destinationInRange = value; } }
    public bool     BallInAir           { get { return _ballInAir; } set { _ballInAir = value; } }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "ballDestination")
        {
            if (!_ballInAir)
                return;

            _ballDestinationPos = other.transform.position;
            _destinationInRange = true;
        }
        else if (other.tag == "ball")
        {
            _ballPos = other.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _destinationInRange = false;
    }
}
