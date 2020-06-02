using System;
using UnityEngine;

public abstract class BaseState
{
    //can't do in constructor, race condition
    protected static PlayerClass player
    {
        get
        {
            if(_player == null)
                _player = GameObject.Find("Player").GetComponent<PlayerClass>();
            return _player;
        }
        private set
        {
            _player = value;
        }
    }
    protected static PlayerClass _player;
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