using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    private int nodeID;
    private PlayerClass player;
    private SpriteList sprites;
    private GameObject bloodBar;
    private SpriteRenderer bloodBarRend;
    private GameObject healthBar;
    private SpriteRenderer healthBarRend;
    private GameObject topMidNotif;
    private SpriteRenderer topMidNotifRend;

    public void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerClass>();
        sprites = GetComponent<SpriteList>();
        bloodBar = GameObject.Find("BloodBar");
        bloodBarRend = bloodBar.GetComponent<SpriteRenderer>();
        healthBar = GameObject.Find("HealthBar");
        healthBarRend = healthBar.GetComponent<SpriteRenderer>();
        topMidNotif = GameObject.Find("TopMidNotification");
        topMidNotifRend = topMidNotif.GetComponent<SpriteRenderer>();
    } 

    public void Update()
    {
        updateBloodBar(player.hunger);
        if(!player.isHuman)
            updateHealthBar(player.health);
        if(player.foundMap)
            foundMap();
        if(player.nodeID > nodeID)
        {
            nodeID++;
            nextZone();
        }
    }

    private void updateBloodBar(int progress)
    {
        if(progress > sprites.bloodBar.Length-1 || progress < 0)
        {
            Debug.Log("[Error] Invalid Blood Bar Progress: "+progress);
            return;
        }
        bloodBarRend.sprite = sprites.bloodBar[progress];
    }

    private void updateHealthBar(int progress)
    {
        if(progress > sprites.healthBar.Length-1 || progress < 0)
        {
            Debug.Log("[Error] Invalid Health Bar Progress: "+progress);
            return;
        }
        healthBarRend.sprite = sprites.healthBar[progress];
    }

    private void tooHungry()
    {
        topMidNotifRend.sprite = sprites.hunger;
        StartCoroutine(displayNotification(topMidNotifRend, 3f));
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