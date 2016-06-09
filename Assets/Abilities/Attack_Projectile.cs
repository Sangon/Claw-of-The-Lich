using UnityEngine;
using System.Collections;
using System;

public class Attack_Projectile : Ability
{
    private int spellOffSet = 155;

    public Attack_Projectile()
    {
        abilityName = "Projectile";
    }

    public override float startCast(Vector2 targetPosition)
    {
        checkForParent();
        if (currentCooldown <= 0)
        {
            this.parent = parent;
            return castTime;
        }

        return 0;
    }

    public override void finishCast()
    {
        Instantiate(Resources.Load(abilityName), parent.transform.position + new Vector3(0, spellOffSet, 0), Quaternion.identity);
        currentCooldown = maxCooldown;
    }

    public override void FixedUpdate()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.fixedDeltaTime;
        }
    }
}
