using UnityEngine;
using System;
using System.Collections;

public class blot_out_spell_script : Spell {


    private GameObject parent;
    private float timer = 0;

    void Start () {
        Destroy(gameObject, Tuner.DEAULT_BLOT_OUT_DURATION);
    }
	

	void FixedUpdate () {
        timer++;

            foreach (GameObject g in getUnitsAtPoint(transform.position, Tuner.DEFAULT_BLOT_OUT_RADIUS))
            {
                g.GetComponent<UnitCombat>().takeDamage(Tuner.BASE_BLOT_OUT_DAMAGE);
            }

        if (true)
        {
            Debug.Log("asd");
            System.Random rand = new System.Random();
            Vector2 randomVector = new Vector2(transform.position.x - Tuner.DEFAULT_BLOT_OUT_RADIUS / 2 + rand.Next(0, (int)(Tuner.DEFAULT_BLOT_OUT_RADIUS)), transform.position.y - Tuner.DEFAULT_BLOT_OUT_RADIUS / 2 + rand.Next(0, (int)(Tuner.DEFAULT_BLOT_OUT_RADIUS))+800);
            Instantiate(Resources.Load("blot_out_projectile"), randomVector, Quaternion.identity);
        }

    }
}
