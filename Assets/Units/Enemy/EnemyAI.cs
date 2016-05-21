using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private float timeStamp;

    private UnitCombat unitCombat;

    void Start()
    {
        unitCombat = GetComponent<UnitCombat>();
    }

    public bool lookForOpponents()
    {
        float dis, bestDis = float.MaxValue;
        GameObject target = null;

        foreach (GameObject character in UnitList.getPlayerUnitsInArea(transform.position, Tuner.UNIT_AGGRO_RANGE))
        {
            if (UnitMovement.lineOfSight(transform.position, character.transform.position, false))
            {
                dis = Ellipse.isometricDistance(character.transform.position, transform.position);
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
            unitCombat.aggro(target);
            return true;
        }
        return false;
    }
}

