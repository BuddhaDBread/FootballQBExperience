using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LocalAwareness : MonoBehaviour
{
    [SerializeField]
    private Transform   _fovPivot;
    private float       _fov            = 90f;
    private bool        _ballInRange    = false;
    private Collider    _ballCollider   = null;

    // Properties
    public Transform    FOVPivot        { get { return _fovPivot; } }
    public float        FOV             { get { return _fov; } }
    public bool         BallInRange     { get { return _ballInRange; } set { _ballInRange = value; } }
    public Collider     BallCollider    { get { return _ballCollider; } set { _ballCollider = value; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ball")
        {
            
            _ballCollider = other;
            _ballInRange = true;
        }
    }
}
