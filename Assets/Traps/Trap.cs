using System;
using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{
    public float triggerDistance = 200f;
    private float cooldownMax = 2f;
    private float cooldownNow = 0f;
    private string triggererTag = "Player";
    private float retriggerCooldown = 0;

    public void trigger(String tag)
    {
        if (cooldownNow <= 0f && tag.Equals(triggererTag) && retriggerCooldown <= 0f)
        {
            if (UnityEngine.Random.Range(-1f, 1f) >= 0)
            {
                GameObject unit = Instantiate(Resources.Load("Melee"), transform.position, Quaternion.identity) as GameObject;
                unit.name = "Melee";
            } else
            {
                GameObject unit = Instantiate(Resources.Load("Ranged"), transform.position, Quaternion.identity) as GameObject;
                unit.name = "Ranged";
            }
            cooldownNow = cooldownMax;
        }
        else if (tag.Equals(triggererTag))
            retriggerCooldown = Time.fixedDeltaTime * 2;
    }

    public float getTriggerDistance()
    {
        return triggerDistance;
    }

    void FixedUpdate()
    {
        cooldownNow -= Time.fixedDeltaTime;
        retriggerCooldown -= Time.fixedDeltaTime;
        if (cooldownNow <= 0f && retriggerCooldown <= 0f)
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        else
            this.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }
}
