using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPlay : State
{
    public EndPlay(GameSystem gameSystem) : base(gameSystem)
    {
    }

    public override IEnumerator OnEnter()
    {
        // Turn on UI
        //GameSystem.canvas.gameObject.SetActive(true);
        //GameSystem.flyButton.SetActive(true);
        //GameSystem.cornerButton.SetActive(true);
        //GameSystem.comebackButton.SetActive(true);

        GameSystem.resetButton.SetActive(true);

        yield break;
    }
}
