using UnityEngine;
using System.Collections;
using System;

public class whirlwind_skill : Skill
{
    public whirlwind_skill()
    {
        spellName = "whirlwind";
        skillIcon = null;
    }

    public override void cast(GameObject owner)
    {
        if (currentCooldown == maxCooldown)
        {
            currentCooldown = 0;
            foreach (GameObject g in getUnitsAtPoint(owner.transform.position, Tuner.DEFAULT_WHIRLWIND_RADIUS))
            {
                //Check for line of sight before dealing damage
                if (g.GetComponent<UnitMovement>().lineOfSight(owner.transform.position, g.transform.position))
                    g.GetComponent<UnitCombat>().takeDamage(Tuner.BASE_WHIRLWIND_DAMAGE, owner, Tuner.DamageType.melee);
            }
        }
    }

    public override void FixedUpdate()
    {
        if (currentCooldown < maxCooldown)
        {
            currentCooldown++;
            /*
            if (currentCooldown == maxCooldown)
            {
                Debug.Log("Ready: " + spellName);
            }
            */
        }
    }
}
