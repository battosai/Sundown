using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    private CharacterClass character;

    public void Awake()
    {
        character = GetComponent<CharacterClass>();
    }

    public void Hurt(int damage)
    {
        character.SetHealth(character.health-damage);
    }
}