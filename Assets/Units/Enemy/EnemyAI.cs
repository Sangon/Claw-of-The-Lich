using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private float timeStamp;

    //private UnitMovement unitMovement = null;
    private UnitCombat unitCombat = null;
    private PartySystem partySystem = null;

    void Start()
    {
        //unitMovement = GetComponent<UnitMovement>();
        unitCombat = GetComponent<UnitCombat>();
        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();
    }

    public bool lookForOpponents()
    {
        float dis, bestDis = float.MaxValue;
        GameObject target = null;

        foreach (GameObject character in partySystem.aliveCharacters)
        {
            dis = Ellipse.isometricDistance(character.transform.position, transform.position);
            if (dis < Tuner.UNIT_AGGRO_RANGE)
            {
                if (target != null && dis < bestDis)
                {
                    target = character; //This character is closer than the old target: change target
                    bestDis = dis;
                }
                else if (target == null)
                    target = character;
            }
        }
        if (target != null)
        {
            if (UnitMovement.lineOfSight(transform.position, target.transform.position, false))
            {
                unitCombat.aggro(target);
                return true;
            }
        }
        return false;
    }
}

