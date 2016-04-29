using UnityEngine;
using System.Collections;
using System;

public class whirlwind_skill : Skill
{

    // Use this for initialization
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
                g.GetComponent<UnitCombat>().takeDamage(Tuner.BASE_WHIRLWIND_DAMAGE, owner);
            }
        }
    }

    public override void FixedUpdate()
    {

        if (currentCooldown < maxCooldown)
        {
            currentCooldown++;
            if (currentCooldown == maxCooldown)
            {
                Debug.Log("Ready: " + spellName);
            }
        }
    }


}
