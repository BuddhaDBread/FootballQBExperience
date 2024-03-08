using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected GameSystem GameSystem;


    public State(GameSystem gameSystem)
    {
        GameSystem = gameSystem;
    }

    // Update for a state
    public virtual void OnUpdate()
    {
    }

    // Fixed update for a state
    public virtual void OnFixedUpdate()
    {

    }

    // What happens when a state is entered
    public virtual IEnumerator OnEnter()
    {
        yield break;
    }
    // What happens when you exit a state
    public virtual IEnumerator OnExit()
    {
        yield break;
    }
}
