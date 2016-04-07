using UnityEngine;
using System.Collections;
using System;
using System.ComponentModel;

public class whirlwind_skill : Skill
{
    private float wwtimer = 0;
    private float maxwwTimer = 20;
    private GameObject parent = null;
    public whirlwind_skill () {
        spellName = "whirlwind";
        skillIcon = null;
        targetable = false;
    }

    public override void cast(GameObject unit){

        parent = unit;
        if (currentCooldown == maxCooldown)
        {
            currentCooldown = 0;
            foreach (GameObject g in getUnitsAtPoint(unit.transform.position, Tuner.DEFAULT_WHIRLWIND_RADIUS))
            {
                g.GetComponent<UnitCombat>().takeDamage(Tuner.BASE_WHIRLWIND_DAMAGE);
            }
        }

        parent.gameObject.GetComponent<UnitMovement>().stop();
        parent.gameObject.GetComponent<UnitMovement>().lockMovement();


        wwtimer = maxwwTimer;
    }

    public override void FixedUpdate()
    {
        if (currentCooldown < maxCooldown){
            currentCooldown++;
            if (currentCooldown == maxCooldown)
            {
                Debug.Log("Ready: " + spellName);
            }
        }

        if (parent == null)
            return;

        if (wwtimer <= 0)
        {
            parent.GetComponent<UnitMovement>().unlockMovement();
        }
        else{
            if (wwtimer > (maxwwTimer / 4) * 3)
            {
                parent.GetComponent<UnitMovement>().setDirection(UnitMovement.Direction.E);
            }
            else if (wwtimer > (maxwwTimer / 4) * 2)
            {
                parent.GetComponent<UnitMovement>().setDirection(UnitMovement.Direction.S);
            }
            else if (wwtimer > (maxwwTimer / 4))
            {
                parent.GetComponent<UnitMovement>().setDirection(UnitMovement.Direction.W);
            }
            else if (wwtimer > 0)
            {
                parent.GetComponent<UnitMovement>().setDirection(UnitMovement.Direction.N);
            }

            Debug.Log("WWTIMER" + wwtimer);
        }

        wwtimer--;

    }


}
