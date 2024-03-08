using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public string Key = null;
    public string Value = null;
}

[RequireComponent(typeof(LineRenderer))]
public class GameSystem : StateMachine
{
    public static GameSystem instance = null;
    
    private enum RunRoute
    {
        Fly,
        Corner,
        Comeback
    }

    // Game States
    public  StartGame   startGameState;
    public  SetupPlay   setupPlayState;
    public  Play        playState;
    public  EndPlay     endPlayState;

    public Canvas       canvas;

    // Prefabs
    public  GameObject  ballPrefab;

    // Private Variables                    
    private Rigidbody                       _ballRigidbody;
    [SerializeField]
    private Transform                       _throwPoint;
    private float                           _throwPointRotation     = 0.07f;
    [SerializeField]
    private float                           _throwForce             = 10;

    private LineRenderer                    _throwTrajectoryLine;
    private Vector3                         _landingPos;
    [SerializeField]
    private Collider                        _landingCollider;
    //[SerializeField, Range(10, 100)]
    private int                             _maxLinePoints          = 100;
    //[SerializeField, Range(0.01f, 0.5f)]    
    private float                           _lineIncrement          = 0.025f;
    private float                           _rayOverlap             = 1.1f;

    // Route transformation points
    [SerializeField]
    private Transform[] _flyRoutePoints;
    [SerializeField]    
    private Transform[] _cornerRoutePoints;
    [SerializeField]    
    private Transform[] _comebackRoutePoints;
    [SerializeField]    
    private Transform[] _currentRoutePoints;
    [SerializeField]
    private int         _currentPointIndex = 0;

    // Audio Collections
    //public AudioCollection winLoseSounds = null;

    // Properties
    public Rigidbody    BallRigidbody       { get { return _ballRigidbody; } }
    public Transform    ThrowPoint          { get { return _throwPoint;} }
    public float        ThrowPointRotation  { get { return _throwPointRotation;} }
    public float        ThrowForce          { get { return _throwForce;} set { _throwForce = value; } }
    public LineRenderer ThrowTrajectoryLine { get { return _throwTrajectoryLine; } set { _throwTrajectoryLine = value; } }
    public Vector3      LandingPos          { get { return _landingPos; } set { _landingPos = value; } }
    public Collider     LandingCollider     { get { return _landingCollider; } set { _landingCollider = value; } }
    public int          MaxLinePoints       { get { return _maxLinePoints; } }
    public float        LineIncrement       { get { return _lineIncrement;} }
    public float        RayOverlap          { get { return _rayOverlap; } }
    public Transform[]  CurrentRoutePoints  { get { return _currentRoutePoints; } }
    public int          CurrentPointIndex   { get { return _currentPointIndex; } set { _currentPointIndex = value; } }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);

        #region States Initialization

        // Initialize the game states
        if (startGameState == null)
            startGameState = new StartGame(instance);

        if (setupPlayState == null)
            setupPlayState = new SetupPlay(instance);

        if (playState == null)
            playState = new Play(instance);

        if (endPlayState == null)
            endPlayState = new EndPlay(instance);

        #endregion

        _throwTrajectoryLine = GetComponent<LineRenderer>();
        _ballRigidbody = ballPrefab.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        StartGameState();
    }

    public GameObject SpawnPrefab(GameObject prefab, Vector3 pos)
    {
        return Instantiate(prefab, pos, _throwPoint.rotation) as GameObject;
    }

    public void DestroyPrefab(GameObject prefab)
    {
        Destroy(prefab);
    }

    public void StartGameState()
    {
        SetState(startGameState);
    }

    private void Update()
    {
        if (State != null)
            State.OnUpdate();

    }

    private void FixedUpdate()
    {
        if (State != null)
            State.OnFixedUpdate();
    }

    // Initialize on button press
    public void InitializeRoute(int route)
    {
        switch (route)
        {
            case 0:
                _currentRoutePoints = _flyRoutePoints;
                break;
            case 1:
                _currentRoutePoints = _cornerRoutePoints;
                break;
            case 2:
                _currentRoutePoints = _comebackRoutePoints;
                break;
            default:
                _currentRoutePoints = _flyRoutePoints;
                break;
        }
    }

}