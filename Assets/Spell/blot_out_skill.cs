using UnityEngine;
using System.Collections;
using System;

public class blot_out_skill : Skill {

    public blot_out_skill() {
        spellName = "blot_out";
        skillIcon = null;
        maxCooldown = 1*50;
    }

    public override void cast(GameObject unit)
    {
        if (currentCooldown == maxCooldown)
        {

            GameObject g = Instantiate(Resources.Load(spellName), getCurrentMousePos(), Quaternion.identity) as GameObject;
            g.GetComponent<blot_out_spell_script>().setParent(unit);

            currentCooldown = 0;
        }

    }
    public override void FixedUpdate()
    {

        if (currentCooldown < maxCooldown)
        {
            currentCooldown++;
            if (currentCooldown == maxCooldown)
            {
                //Debug.Log("Ready: " + spellName);
            }
        }
    }

}
