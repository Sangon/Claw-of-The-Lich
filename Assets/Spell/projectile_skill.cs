using UnityEngine;
using System.Collections;

public class projectile_skill : Skill
{
    private int spellOffSet = 155;

    public projectile_skill()
    {
        spellName = "testSpell";
        skillIcon = null;
    }

    public override void cast(GameObject owner)
    {
        if (currentCooldown <= 0)
        {
            Instantiate(Resources.Load(spellName), owner.transform.position + new Vector3(0, spellOffSet, 0), Quaternion.identity);
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
