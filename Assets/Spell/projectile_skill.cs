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

    public override void cast(GameObject parent)
    {
        if (currentCooldown <= 0)
        {
            this.parent = parent;

            Instantiate(Resources.Load(spellName), parent.transform.position + new Vector3(0, spellOffSet, 0), Quaternion.identity);
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
