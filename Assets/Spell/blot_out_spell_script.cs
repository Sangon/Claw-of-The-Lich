using UnityEngine;
using System;
using System.Collections;

public class blot_out_spell_script : Spell
{
    private float timer = 0;
    private int damageTimes;
    private float timeAlive;
    private float damage = (Tuner.BASE_BLOT_OUT_DAMAGE * 0.5f) / Tuner.DEFAULT_BLOT_OUT_DURATION;

    void Start()
    {
        //Destroy(gameObject, Tuner.DEFAULT_BLOT_OUT_DURATION);
    }

    void FixedUpdate()
    {
        timer++;
        timeAlive += Time.fixedDeltaTime;
        if (timeAlive >= ((damageTimes + 1) * 0.5f))
        {
            foreach (GameObject g in getUnitsAtPoint(transform.position, Tuner.DEFAULT_BLOT_OUT_RADIUS))
            {
                g.GetComponent<UnitCombat>().takeDamage(damage, getParent());
            }
            damageTimes++;
        }

        //System.Random rand = new System.Random();

        //Vector2 randomVector = new Vector2(transform.position.x - Tuner.DEFAULT_BLOT_OUT_RADIUS + 2 * rand.Next(0, (int)(Tuner.DEFAULT_BLOT_OUT_RADIUS)), transform.position.y - Tuner.DEFAULT_BLOT_OUT_RADIUS / 2 + rand.Next(0, (int)(Tuner.DEFAULT_BLOT_OUT_RADIUS)) + 800);
        if (timer % 2 == 0)
        {
            Vector2 randomVector = Ellipse.getRandomPointInsideEllipse(transform.position, Tuner.DEFAULT_BLOT_OUT_RADIUS);
            randomVector.y += 800f;
            Instantiate(Resources.Load("blot_out_projectile"), randomVector, Quaternion.identity);
        }

        if (timeAlive >= Tuner.DEFAULT_BLOT_OUT_DURATION)
            Destroy(gameObject);
    }
}
