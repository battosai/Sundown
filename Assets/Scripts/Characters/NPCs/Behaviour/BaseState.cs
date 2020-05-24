using System;
using UnityEngine;

public abstract class BaseState
{
    protected GameObject obj;
    protected Transform trans;
    protected Rigidbody2D rb;
    protected SpriteRenderer rend;
    protected Collider2D pushBox;
    protected CharacterClass character;

    public BaseState(CharacterClass ch)
    {
        this.obj = ch.gameObject;
        this.trans = ch.transform;
        this.rb = ch.rb;
        this.rend = ch.rend;
        this.pushBox = ch.pushBox;
        this.character = ch;
    }

    public abstract Type Tick();
}