using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private float timeStamp;

    private UnitMovement unitMovement = null;
    private UnitCombat unitCombat = null;
    private PartySystem partySystem = null;

    void Start()
    {
        unitMovement = GetComponent<UnitMovement>();
        unitCombat = GetComponent<UnitCombat>();
        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();
    }

    public bool lookForOpponents()
    {
        if (partySystem.noneAlive())
            return false;

        float dis = 0;
        GameObject target = null;

        foreach (GameObject character in partySystem.aliveCharacters)
        {
            dis = Vector2.Distance(character.transform.position, transform.position);
            if (dis < Tuner.UNIT_AGGRO_RANGE)
            {
                if (target != null && dis < Vector2.Distance(target.transform.position, transform.position))
                    target = character; //This character is closer than the old target: change target
                else if (target == null)
                    target = character;
            }
        }
        if (target != null)
        {
            if (unitMovement.lineOfSight(transform.position, target.transform.position))
            {
                unitCombat.aggro(target);
                return true;
            }
        }
        return false;
    }
}

