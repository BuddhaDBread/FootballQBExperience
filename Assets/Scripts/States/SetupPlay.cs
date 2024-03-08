using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupPlay : State
{
    public SetupPlay(GameSystem gameSystem) : base(gameSystem)
    {
    }

    public override IEnumerator OnEnter()
    {
        // Turn on routes UI


        yield break;
    }

    public override void OnUpdate()
    {
        // Allow player to choose route and position the throw

        #region Player Input 

        // Move Up
        if (Input.GetKey(KeyCode.UpArrow))
        {
            GameSystem.ThrowPoint.transform.Rotate(new Vector3(-GameSystem.ThrowPointRotation, 0, 0));
        }

        // Move Down
        if (Input.GetKey(KeyCode.DownArrow))
        {
            GameSystem.ThrowPoint.transform.Rotate(new Vector3(GameSystem.ThrowPointRotation, 0, 0));
        }

        // Move Left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            GameSystem.ThrowPoint.transform.Rotate(new Vector3(0, -GameSystem.ThrowPointRotation, 0));
        }

        // Move Right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            GameSystem.ThrowPoint.transform.Rotate(new Vector3(0, GameSystem.ThrowPointRotation, 0));
        }

        // Increase Force
        if (Input.GetKey(KeyCode.W))
        {
            if(GameSystem.ThrowForce <= 40)
                GameSystem.ThrowForce = GameSystem.ThrowForce + 0.1f;
        }

        // Decrease Force
        if (Input.GetKey(KeyCode.S))
        {
            if (GameSystem.ThrowForce >= 5)
                GameSystem.ThrowForce = GameSystem.ThrowForce - 0.1f;
        }

        // Start Play
        if (Input.GetKey(KeyCode.Return))
        {
            GameSystem.canvas.gameObject.SetActive(false);
            // Start Receiver AI 
        }

        // Throw ball
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var spawned = GameSystem.SpawnPrefab(GameSystem.ballPrefab, GameSystem.ThrowPoint.position);

            spawned.GetComponent<Rigidbody>().AddForce(GameSystem.ThrowPoint.forward * GameSystem.ThrowForce, ForceMode.Impulse);
        }

        PredictTrajectory(GameSystem.ThrowPoint);

        #endregion
    }

    public override IEnumerator OnExit()
    {
        // Turn off routes UI 

        yield break;
    }

    public void PredictTrajectory(Transform throwPoint)
    {
        Vector3 velocity = throwPoint.forward * (GameSystem.ThrowForce / GameSystem.BallRigidbody.mass);
        Vector3 position = throwPoint.position;
        Vector3 nextPosition;
        float overlap;

        UpdateLineRender(GameSystem.MaxLinePoints, (0, position));

        for (int i = 1; i < GameSystem.MaxLinePoints; i++)
        {
            // Find velocity and update next position
            velocity = CalculateNewVelocity(velocity, GameSystem.BallRigidbody.drag, GameSystem.LineIncrement);
            nextPosition = position + velocity * GameSystem.LineIncrement;

            // Overlap our rays to make sure we never miss a surface
            overlap = Vector3.Distance(position, nextPosition) * GameSystem.RayOverlap;

            // Show landing position
            if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, overlap))
            {
                if (hit.collider.tag == "field")
                {
                    UpdateLineRender(i, (i - 1, hit.point));
                    GameSystem.LandingPos = hit.point;
                    GameSystem.LandingCollider.transform.position = GameSystem.LandingPos;
                    break;
                }
            }

            position = nextPosition;
            UpdateLineRender(GameSystem.MaxLinePoints, (i, position));
        }
    }

    private void UpdateLineRender(int count, (int point, Vector3 pos) pointPos)
    {
        GameSystem.ThrowTrajectoryLine.positionCount = count;
        GameSystem.ThrowTrajectoryLine.SetPosition(pointPos.point, pointPos.pos);
    }

    private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float increment)
    {
        velocity += Physics.gravity * increment;
        velocity *= Mathf.Clamp01(1f - drag * increment);
        return velocity;
    }
}
