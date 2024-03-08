using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using static UnityEngine.GraphicsBuffer;

public class ReceiverAI : MonoBehaviour
{
    [SerializeField] private GameSystem gameSystem;

    private Vector3         _orignalPos;
    private Animator        _animator;
    private NavMeshAgent    _navMeshAgent;
    private int             _seeking            = 0;
    private bool            _isRunning          = false;
    [SerializeField]
    private LocalAwareness  _localAwareness;
    [SerializeField]
    private FieldAwareness  _fieldAwareness;
    [SerializeField]
    private HoldBall        _holdBallRight;
    [SerializeField]
    private HoldBall        _holdBallLeft;

    // Animation Rigging
    [SerializeField]
    private Rig             _rig;
    private RigBuilder      _rigBuilder;

    [SerializeField] private TwoBoneIKConstraint _ikConstraintRight;
    [SerializeField] private TwoBoneIKConstraint _ikConstraintLeft;

    private int _seekingHash = Animator.StringToHash("Seeking");
    private int _runningHash = Animator.StringToHash("isRunning");

    private void Awake()
    {
    }

    private void Start()
    {
        _orignalPos = transform.position;

        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rigBuilder = GetComponent<RigBuilder>();
    }

    private void Update()
    {
        if (gameSystem.playHasEnded)
        {
            _isRunning = false;
            _seeking = 0;

            if (_rig.weight > 0)
                _rig.weight -= 0.006f;
 
            gameSystem.SetState(gameSystem.endPlayState);
        }

        if (!gameSystem.playHasStarted) return;

        // Pass animator parameters
        if (_animator != null)
        {
            _animator.SetInteger(_seekingHash, _seeking);
            _animator.SetBool(_runningHash, _isRunning);
        }

        // Check if ball was caught
        if (_holdBallRight.BallCaught || _holdBallLeft.BallCaught)
        {
            _seeking = 0;

            if (_rig.weight >= 0.1f)
                _rig.weight -= 0.006f;

            if (_rig.weight < 0.1f)
            {
                // Since ball was caught the play ends and moves on to end state
                gameSystem.SetState(gameSystem.endPlayState);
            }

            return;
        }

        // Ball destination in field awareness
        if (_fieldAwareness.DestinationInRange && _fieldAwareness.BallInAir)
        {
            gameSystem.SetDestination(_fieldAwareness.BallDestination);
            FaceTargetDirection(_fieldAwareness.BallDestination);
        }

        // Ball is inside local awareness
        if (_localAwareness.BallInRange)
        {
            // Calculate angle from ball position to current position
            Vector3 ballDirection = _localAwareness.BallCollider.transform.position - _localAwareness.FOVPivot.transform.position;
            float angle = FindSignedAngle(_localAwareness.FOVPivot.transform.forward, ballDirection);

            // Check if ball is in FOV
            if (Mathf.Abs(angle) < _localAwareness.FOV / 2f)
            {
                // Stop Turning 
                _seeking = 0;

                _ikConstraintRight.data.target = _localAwareness.BallCollider.transform.GetChild(0).transform;
                _ikConstraintLeft.data.target = _localAwareness.BallCollider.transform.GetChild(1).transform;

                if (_rig.weight < 1)
                    _rig.weight += 0.2f;

                Debug.Log("here");

                _rigBuilder.Build();
            }
            else
            {
                if (_rig.weight > 0)
                    _rig.weight -= 0.006f;

                // Turn to where the ball is
                _seeking = (int)Mathf.Sign(angle);

                // Use navmesh angular speed to speed up root motion rotation. 
                FaceTargetDirection(ballDirection);
            }
        }

        if (_navMeshAgent.hasPath)
        {
            // Reached the destination
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                if (gameSystem.CurrentPointIndex < gameSystem.CurrentRoutePoints.Length - 1 && !_fieldAwareness.DestinationInRange)
                    gameSystem.CurrentPointIndex++;

                if (!_fieldAwareness.DestinationInRange && !_fieldAwareness.BallInAir)
                {
                    gameSystem.SetDestination(gameSystem.CurrentRoutePoints[gameSystem.CurrentPointIndex].position);
                }
                else
                {
                    gameSystem.SetDestination(_fieldAwareness.BallDestination);

                    //if (_fieldAwareness.BallPos.magnitude > 0)
                    //{
                    //    Vector3 targetDirection = _fieldAwareness.BallPos - transform.position;
                    //    targetDirection.y = 0;

                    //    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _navMeshAgent.angularSpeed);
                    //}
                }

                _isRunning = false;
            }
            else
            {
                if (_fieldAwareness.DestinationInRange && _fieldAwareness.BallInAir)
                {
                    gameSystem.SetDestination(_fieldAwareness.BallDestination);
                    FaceTargetDirection(_fieldAwareness.BallDestination);
                }

                //Faces target and starts running
                FaceTargetDirection(gameSystem.CurrentRoutePoints[gameSystem.CurrentPointIndex].position);
            }


        }
    }

    // Face target based on navmesh angular speed
    void FaceTargetDirection(Vector3 target)
    {
        Vector3 targetDirection = target - transform.position;
        targetDirection.y = 0;

        float angle = FindSignedAngle(this.transform.forward, targetDirection);

        if (Mathf.Abs(angle) < 360f / 2f)
        {
            if (!_isRunning)
            {
                //_seeking = 0;
                //_fieldAwareness.destinationInRange = false;
                _isRunning = true;
            }

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _navMeshAgent.angularSpeed);
        }
        else
        {
            _seeking = (int)Mathf.Sign(angle);
        }
    }

    public static float FindSignedAngle(Vector3 fromVector, Vector3 toVector)
    {
        if (fromVector == toVector)
            return 0.0f;

        float angle = Vector3.Angle(fromVector, toVector);
        Vector3 cross = Vector3.Cross(fromVector, toVector);
        angle *= Mathf.Sign(cross.y);
        return angle;
    }

    public void ResetReceiverAI()
    {
        _localAwareness.BallCollider = null;
        _localAwareness.BallInRange = false;
        transform.position = _orignalPos;
        transform.rotation = Quaternion.identity;
        _ikConstraintRight.data.target = null;
        _ikConstraintLeft.data.target = null;
        _holdBallRight.BallCaught = false;
        _holdBallLeft.BallCaught = false;
        _fieldAwareness.DestinationInRange = false;
        _navMeshAgent.isStopped = true;
        _navMeshAgent.ResetPath();

        _rig.weight = 0;
        _rigBuilder.Build();

        _seeking = 0;
        _isRunning = false;
    }
}