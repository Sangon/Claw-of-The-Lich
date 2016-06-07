using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    private float timeStamp;

    private UnitCombat unitCombat;

    public enum SearchFilter
    {
        byDistance,
        byHealth
    }

    void Start()
    {
        unitCombat = GetComponent<UnitCombat>();
    }

    public GameObject getUnitWithLeastHealth(List<GameObject> units, float maxDistance)
    {
        float health, bestHealth = float.MaxValue;
        GameObject target = null;
        foreach (GameObject unit in units)
        {
            float distance = Ellipse.isometricDistance(unit.transform.position, transform.position);
            if (distance <= maxDistance)
            {
                health = unit.GetComponent<UnitCombat>().getHealth();
                if (target == null || health < bestHealth)
                {
                    //This unit has less health than the old target or there was no old target: change target
                    target = unit;
                    bestHealth = health;
                }
            }
        }
        return target;
    }

    public List<GameObject> getUnitsInSight(float maxDistance)
    {
        List<GameObject> nearbyPlayers = UnitList.getPlayerUnitsInArea(transform.position, maxDistance);

        nearbyPlayers.RemoveAll(u => UnitMovement.lineOfSight(transform.position, u.transform.position, false) == false);

        return nearbyPlayers;
    }

    public GameObject getClosestUnit(List<GameObject> units)
    {
        float distance, bestDistance = float.MaxValue;
        GameObject target = null;
        foreach (GameObject unit in units)
        {
            distance = Ellipse.isometricDistance(unit.transform.position, transform.position);
            if (target == null || distance < bestDistance)
            {
                //This character is closer than the old target or there was no old target: change target
                target = unit;
                bestDistance = distance;
            }
        }
        return target;
    }

    public bool searchForOpponents(ref GameObject closestUnit, SearchFilter filter = SearchFilter.byDistance)
    {
        GameObject target = null;
        List<GameObject> unitsInSight = getUnitsInSight(Tuner.UNIT_AGGRO_RANGE);

        if (unitsInSight.Count == 0)
            return false;

        //Set the closest player character in sight as the default target
        target = getClosestUnit(unitsInSight);
        closestUnit = target;

        switch (filter)
        {
            case SearchFilter.byHealth:
                GameObject lowHealthTarget = getUnitWithLeastHealth(unitsInSight, unitCombat.getAttackRange());
                if (lowHealthTarget != null)
                    target = lowHealthTarget;
                break;
        }

        if (target != null)
        {
            unitCombat.aggro(target);
            return true;
        }
        return false;
    }
}

