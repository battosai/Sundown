using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    private int nodeID;
    private PlayerClass player;
    private SpriteList sprites;
    private GameObject topMidNotif;
    private SpriteRenderer topMidNotifRend;

    public void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerClass>();
        sprites = GetComponent<SpriteList>();
        topMidNotif = GameObject.Find("TopMidNotification");
        topMidNotifRend = topMidNotif.GetComponent<SpriteRenderer>();
    } 

    public void Update()
    {
        if(player.foundMap)
            foundMap();
        if(player.nodeID > nodeID)
        {
            nodeID++;
            nextZone();
        }
    }

    private void foundMap()
    {
        topMidNotifRend.sprite = sprites.map;
        StartCoroutine(displayNotification(topMidNotifRend, 3f));
    }

    private void nextZone()
    {
        topMidNotifRend.sprite = sprites.zone;
        StartCoroutine(displayNotification(topMidNotifRend, 3f));
    }

    private IEnumerator displayNotification(SpriteRenderer rend, float seconds)
    {
        float start = Time.time;
        while(Time.time-start < seconds)
        {
            yield return null;
        }
        rend.sprite = null;
    }

    public void Reset()
    {
        topMidNotifRend.sprite = null;
        nodeID = 0;
    }
}