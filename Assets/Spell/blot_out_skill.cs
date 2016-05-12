using UnityEngine;
using System.Collections;
using System;

public class blot_out_skill : Skill
{
    public blot_out_skill()
    {
        spellName = "blot_out";
        skillIcon = null;
        maxCooldown = Tuner.BASE_BLOT_OUT_COOLDOWN;
    }

    public override void cast(GameObject owner)
    {
        if (currentCooldown <= 0)
        {
            GameObject g = Instantiate(Resources.Load(spellName), getCurrentMousePos(), Quaternion.identity) as GameObject;
            g.GetComponent<blot_out_spell_script>().setParent(owner);

            currentCooldown = maxCooldown;
        }
    }
    public override void FixedUpdate()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.fixedDeltaTime;
        }
    }
}
