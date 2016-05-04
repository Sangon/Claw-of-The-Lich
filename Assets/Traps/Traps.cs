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
        List<GameObject> trapObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Trap"));
        foreach (GameObject trapObject in trapObjects)
        {
            traps.Add(trapObject.GetComponent<Trap>());
        }
    }

    public void checkTrigger(Vector2 position, String tag)
    {
        foreach (Trap trap in traps)
        {
            float dis = Vector2.Distance(trap.transform.position, position);
            if (dis <= trap.getTriggerDistance())
            {
                trap.trigger(tag);
            }
        }
    }
}
