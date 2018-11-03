using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    private GameObject topMidNotif;
    private SpriteRenderer topMidNotifRend;

    public void Awake()
    {
        topMidNotif = GameObject.Find("TopMidNotification");
        topMidNotifRend = topMidNotif.GetComponent<SpriteRenderer>();
    } 

    public void restart()
    {
        topMidNotifRend.sprite = null;
    }
}