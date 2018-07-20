using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Townsperson : CharacterClass
{
    public bool isAlarmed {get; private set;}

    public void Reset()
    {
        base.Reset();
        isAlarmed = false;
    }
}