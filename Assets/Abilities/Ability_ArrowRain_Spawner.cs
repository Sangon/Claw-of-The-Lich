using UnityEngine;
using System;
using System.Collections;

public class Ability_ArrowRain_Spawner : AbilitySpawner
{
    private float timer = 0;
    private int damageTimes;
    private float timeAlive;
    private float damage = 0;
    private float areaRadius = Tuner.BASE_BLOT_OUT_RADIUS;

    public void setAreaRadius(float areaRadius)
    {
        this.areaRadius = areaRadius;
    }

    void Start()
    {
        //Destroy(gameObject, Tuner.DEFAULT_BLOT_OUT_DURATION);
        damage = (getParent().GetComponent<UnitCombat>().calculateDamage(Tuner.DamageType.ranged, Tuner.BASE_BLOT_OUT_DAMAGE_MULTIPLIER) * 0.5f) / Tuner.BASE_BLOT_OUT_DURATION;
    }

    void FixedUpdate()
    {
        timer++;
        timeAlive += Time.fixedDeltaTime;
        if (timeAlive >= ((damageTimes + 1) * 0.5f))
        {
            foreach (GameObject unit in getUnitsAtPoint(transform.position, areaRadius))
            {
                unit.GetComponent<UnitCombat>().takeDamage(damage, getParent());
            }
            damageTimes++;
        }

        if (timer % 4 == 0)
        {
            Vector2 randomVector = Ellipse.getRandomPointInsideEllipse(transform.position, areaRadius);
            randomVector.y += 800f;
            Instantiate(Resources.Load("Ability Prefabs/Projectile_ArrowRain"), randomVector, Quaternion.identity);
        }

        if (timeAlive >= Tuner.BASE_BLOT_OUT_DURATION)
            Destroy(gameObject);
    }
}
