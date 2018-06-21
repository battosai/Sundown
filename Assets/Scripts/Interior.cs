using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interior : MonoBehaviour
{
    public List<GameObject> objects {get; private set;}    

    public void SetObjects(List<GameObject> objects){this.objects = objects;}
}