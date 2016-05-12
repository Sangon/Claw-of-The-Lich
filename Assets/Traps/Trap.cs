using System;
using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{
    public float triggerDistance = 500f;
    public int unitsSpawned = 10;
    public Vector3 unitSpawnPositionCenter;
    public float unitSpawnArea = 500f;
    private float cooldownMax = 2f;
    private float cooldownNow = 0f;
    private string triggererTag = "Player";
    private float retriggerCooldown = 0;
    private bool canBeRetriggered = false;

    void Awake()
    {
        if (unitSpawnPositionCenter == Vector3.zero)
            unitSpawnPositionCenter = transform.position;
    }

    public void trigger(String tag)
    {
        if (cooldownNow <= 0f && tag.Equals(triggererTag) && retriggerCooldown <= 0f)
        {
            int j = 0;
            for (int i = 0; i < unitsSpawned; i++)
            {
                Vector3 unitSpawnPos = Ellipse.getRandomPointInsideEllipse(unitSpawnPositionCenter, unitSpawnArea);
                while (!UnitMovement.lineOfSight(unitSpawnPos, unitSpawnPositionCenter))
                {
                    j++;
                    if (j == 10)
                    {
                        unitSpawnPos = unitSpawnPositionCenter;
                        break;
                    }
                    unitSpawnPos = Ellipse.getRandomPointInsideEllipse(unitSpawnPositionCenter, unitSpawnArea);
                }

                GameObject unit;

                if (UnityEngine.Random.Range(-1f, 1f) >= 0)
                {
                    unit = Instantiate(Resources.Load("Melee"), unitSpawnPos, Quaternion.identity) as GameObject;
                    unit.name = "Melee [" + UnitList.createUnit() + "]";
                }
                else
                {
                    unit = Instantiate(Resources.Load("Ranged"), unitSpawnPos, Quaternion.identity) as GameObject;
                    unit.name = "Ranged [" + UnitList.createUnit() + "]";
                }

                unit.GetComponent<UnitMovement>().moveTo(transform.position);
            }
            cooldownNow = cooldownMax;
        }
        if (tag.Equals(triggererTag))
            retriggerCooldown = Time.fixedDeltaTime * 2f;
    }

    public float getTriggerDistance()
    {
        return triggerDistance;
    }

    void FixedUpdate()
    {
        cooldownNow -= Time.fixedDeltaTime;
        if (canBeRetriggered)
            retriggerCooldown -= Time.fixedDeltaTime;
        if (cooldownNow <= 0f && retriggerCooldown <= 0f)
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        else
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }
}
