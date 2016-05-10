using System;
using UnityEngine;
using System.Collections.Generic;

public class Traps : MonoBehaviour
{
    public List<Trap> traps;

    //private PartySystem partySystem = null;

    // Use this for initialization
    void Start()
    {
        //partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();
        foreach (GameObject trapObject in UnitList.getTraps())
        {
            traps.Add(trapObject.GetComponent<Trap>());
        }
    }

    public void checkTrigger(Vector2 position, String tag)
    {
        foreach (Trap trap in traps)
        {
            float dis = Ellipse.isometricDistance(trap.transform.position, position);
            if (dis <= trap.getTriggerDistance())
            {
                trap.trigger(tag);
            }
        }
    }
}
