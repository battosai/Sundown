using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    private int nodeID;
    private PlayerClass player;
    private SpriteList sprites;
    public Image topMidNotif;
    public Image[] hpSegments;

    public void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerClass>();
        sprites = GetComponent<SpriteList>();
    } 

    public void Start()
    {
        topMidNotif.enabled = false;
        Hurtbox.OnPlayerHurt += UpdateHP;
        Wildlife.OnPlayerWildlifeHeal += UpdateHP;
        Container.OnFoundMap += foundMap;
    }

    public void Update()
    {
        if(player.nodeID > nodeID)
        {
            nodeID++;
            nextZone();
        }
    }

    private void UpdateHP()
    {
        for(int i = 0; i < player.maxHealth; i++)
        {
            Image hpSegment = hpSegments[i];
            hpSegment.sprite = (player.health >= i+1) ? sprites.hp : sprites.emptyHp;
        }
    }

    private void foundMap()
    {
        topMidNotif.sprite = sprites.map;
        StartCoroutine(displayNotification(topMidNotif, 3f));
    }

    private void nextZone()
    {
        topMidNotif.sprite = sprites.zone;
        StartCoroutine(displayNotification(topMidNotif, 3f));
    }

    private IEnumerator displayNotification(Image img, float seconds)
    {
        img.enabled = true;
        float start = Time.time;
        while(Time.time-start < seconds)
        {
            yield return null;
        }
        img.sprite = null;
        img.enabled = false;
    }

    public void Reset()
    {
        topMidNotif.sprite = null;
        nodeID = 0;
    }
}