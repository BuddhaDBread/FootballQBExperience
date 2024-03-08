using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : State
{
    public StartGame(GameSystem gameSystem) : base(gameSystem)
    {
    }

    public override IEnumerator OnEnter()
    {
        // Turn on UI
        GameSystem.canvas.gameObject.SetActive(true);

        // Change state to player turn state
        GameSystem.SetState(GameSystem.setupPlayState);
        yield break;
    }
}
