using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WerewolfAggroBox : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other)
    {
        CharacterClass character = other.GetComponent<CharacterClass>();
        if(character != null)
        {
            character.SetIsAlarmed(true);
        }
    }
}