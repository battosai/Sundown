using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WerewolfAggroBox : MonoBehaviour
{
    private PlayerClass player;

    public void Awake()
    {
        player = transform.parent.GetComponent<PlayerClass>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        CharacterClass character = other.GetComponent<CharacterClass>();
        if(character != null)
        {
            // Debug.Log(other.name+" is scared of werewolf!");
            character.SetIsAlarmed(true);
            // character.SetAlarmPoint(player.floorPosition);
        }
    }
}