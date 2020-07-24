using System;
using UnityEngine;
using Game;

public class RangerRepositionState : BaseState
{
    private static readonly float SHOVE_TIME = 1f;      //how long it takes ranger to do
    private static readonly float REPOSITION_TIME = 2f;
    private bool didShove;
    private float repoTimer;
    private float shoveTimer;
    private Vector2 reposition;
    private Ranger ranger;
    private Hitbox hitbox;

    public RangerRepositionState(Ranger ch) : base(ch)
    {
        ranger = ch;
        hitbox = ch.hitbox;
        didShove = false;
        repoTimer = 0f;
        shoveTimer = 0f;
    }

    public override Type Tick()
    {
        if(!didShove)
            ShoveCheck();
        //ideally this timer is to let the shove animation play
        else if(shoveTimer < SHOVE_TIME)
            shoveTimer += Time.deltaTime;
        //ideally this timer is to let the repo animation play
        else if(repoTimer < REPOSITION_TIME)
            repoTimer += Time.deltaTime;
        else
        {
            Reset();
            //maybe replace this method with a
            //more interesting fnc that finds
            //the relative "opposite" pos to place
            //the ranger
            reposition = Arena.GetOpenPosition();
            trans.position = reposition;
            return typeof(RangerIdleState);
        }
        return null;
    }

    private void ShoveCheck()
    {
        int dir = character.isLeft ? -1 : 1;
        hitbox.mask.useTriggers = false;
        hitbox.SetAct(Act.SHOVE);
        hitbox.SetOffset(new Vector2(Ranger.SHOVE[0].x*dir, Ranger.SHOVE[0].y));
        hitbox.SetSize(Ranger.SHOVE[1]);
        hitbox.StartCheckingCollision();
        hitbox.CheckCollision();
        hitbox.StopCheckingCollision();
        didShove = true;
    }

    private void Reset()
    {
        didShove = false;
        repoTimer = 0f;
        shoveTimer = 0f;
    }
}