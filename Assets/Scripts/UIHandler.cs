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

    public void FoundMap()
    {
        topMidNotifRend.sprite = SpriteList.map;
        StartCoroutine(DisplayNotification(topMidNotifRend, 3f));
    }

    public IEnumerator DisplayNotification(SpriteRenderer rend, float seconds)
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
    }
}