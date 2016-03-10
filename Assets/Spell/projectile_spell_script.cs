using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class projectile_spell_script : Spell
{

    public float velocity;
    public int damage;
    public Vector2 dir;
    private GameObject parent;

    void Start(){
        spellID = 0;
        velocity = Tuner.DEFAULT_PROJECTILE_VELOCITY;
        damage = Tuner.DEFAULT_PROJECTILE_DAMAGE;
        Destroy(gameObject, Tuner.DEFAULT_PROJECTILE_RANGE / Tuner.DEFAULT_PROJECTILE_VELOCITY);
    }

    public void initAttack(Vector3 enemy, GameObject parent){
        castLocation = enemy;
        this.parent = parent;
        dir = new Vector2(castLocation.x - transform.position.x, castLocation.y - transform.position.y);
    }


    void FixedUpdate()
    {
        //Liikuttaa projektiiliä kohteen suuntaan
        if (dir != null)
        {
            transform.Translate(dir.normalized * velocity);

            RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(dir.normalized.x * 10, dir.normalized.y * 10, 0), transform.position + new Vector3(dir.normalized.x * 10, dir.normalized.y * 10, 0), 0, (1 << 9) | (1 << 8));

            if (hit.collider != null)
            {
                if (hit.collider.name != "Collision" && hit.collider.name != parent.name){
                    hit.collider.gameObject.GetComponent<UnitCombat>().takeDamage(damage);
                }
                Destroy(gameObject);
            }

            Debug.DrawLine(transform.position - new Vector3(dir.normalized.x * 10, dir.normalized.y * 10, 0), transform.position + new Vector3(dir.normalized.x * 10, dir.normalized.y * 10, 0));
            Debug.DrawLine(transform.position - new Vector3(dir.normalized.y * 10, -dir.normalized.x * 10, 0), transform.position + new Vector3(dir.normalized.y * 10, -dir.normalized.x * 10, 0));
        }
    }
}