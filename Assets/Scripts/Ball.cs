using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private GameSystem _gameSystem;

    private Rigidbody       _rb;
    private FieldAwareness  _fieldAwareness;

    private void Awake()
    {
        var gameSystem = GameObject.FindWithTag("gameSystem");
        _gameSystem = gameSystem.GetComponent<GameSystem>();

        var fieldAwareness = GameObject.FindWithTag("fieldAwareness");
        _fieldAwareness = fieldAwareness.GetComponent<FieldAwareness>();

        _fieldAwareness.BallInAir = true;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Rotate ball for a spin effect
        if (_rb.velocity.magnitude > 0.1f && _fieldAwareness.BallInAir)
        {
            // Store the current rotation
            Quaternion originalRotation = transform.rotation;

            transform.LookAt(transform.position + _rb.velocity);

            // Restore the original rotation on the z-axis
            transform.rotation = Quaternion.Euler(0f, 0f, originalRotation.eulerAngles.z);

            // Apply continuous rotation on the z-axis
            transform.Rotate(0f, 0f, 400 * Time.deltaTime);

        }
    }

    public void OnCollisionEnter(Collision col)
    {
        // Ball is grounded
        _fieldAwareness.BallInAir = false;
        _gameSystem.playHasEnded = true;

    }

    private void OnDestroy()
    {
        _fieldAwareness.BallInAir = false;
    }
}
