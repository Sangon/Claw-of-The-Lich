using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class projectile_spell_script : Spell
{

    public float velocity;
    public float damage;
    public Vector2 dir;
    private string ownerTag;

    void Awake(){
        spellID = 0;
        velocity = Tuner.DEFAULT_PROJECTILE_VELOCITY;
        damage = Tuner.DEFAULT_PROJECTILE_DAMAGE;
        Destroy(gameObject, Tuner.DEFAULT_PROJECTILE_RANGE / Tuner.DEFAULT_PROJECTILE_VELOCITY);
        transform.position = new Vector3(transform.position.x, transform.position.y + Tuner.DEFAULT_PROJECTILE_OFFSET, transform.position.y / 100.0f + 800.0f);
    }

    public void initAttack(Vector3 enemy, GameObject parent, bool handleOffset){
        castLocation = enemy;
        ownerTag = parent.tag;
        if (handleOffset)
            castLocation.y += Tuner.DEFAULT_PROJECTILE_OFFSET;
        dir = new Vector2(castLocation.x - transform.position.x, castLocation.y - transform.position.y);
        transform.Translate(dir.normalized * velocity);
        transform.position = new Vector3(transform.position.x, transform.position.y, (transform.position.y - Tuner.DEFAULT_PROJECTILE_OFFSET) / 100.0f + 800.0f);
    }

    private bool checkCollision(Vector2 start, Vector2 end, bool ignoreObstacles)
    {
        RaycastHit2D hit = Physics2D.Linecast(start, end, Tuner.LAYER_UNITS | Tuner.LAYER_OBSTACLES);
        Debug.DrawLine(start, end, Color.cyan, Time.fixedDeltaTime);
        if (hit.collider != null)
        {
            if (!ignoreObstacles && hit.collider.name.Equals("Collision"))
            {
                Destroy(gameObject);
                return false;
            } 
            else if (!hit.collider.name.Equals("Collision") && !hit.collider.gameObject.tag.Equals(ownerTag))
            {
                hit.collider.gameObject.GetComponent<UnitCombat>().takeDamage(damage);
                Destroy(gameObject);
                return false;
            }
        }
        return true;
    }

    void FixedUpdate()
    {
        //Liikuttaa projektiiliä kohteen suuntaan
        if (dir != Vector2.zero)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
            transform.Translate(dir.normalized * velocity);
            transform.position = new Vector3(transform.position.x, transform.position.y, (transform.position.y - Tuner.DEFAULT_PROJECTILE_OFFSET) / 100.0f + 800.0f);
            Vector2 pos2DNew = new Vector2(transform.position.x, transform.position.y);

            Vector2 start = pos2D - new Vector2(0, Tuner.DEFAULT_PROJECTILE_OFFSET);
            Vector2 end = pos2DNew - new Vector2(0, Tuner.DEFAULT_PROJECTILE_OFFSET);

            if (checkCollision(start, end, false))
            {
                Vector2 offset = Quaternion.Euler(0, 0, 90) * dir.normalized * Tuner.DEFAULT_PROJECTILE_HITBOX_RADIUS * 0.5f;
                start = pos2D - new Vector2(0, Tuner.DEFAULT_PROJECTILE_OFFSET) + offset;
                end = pos2DNew - new Vector2(0, Tuner.DEFAULT_PROJECTILE_OFFSET) + offset;
                if (checkCollision(start, end, true))
                {
                    start = pos2D - new Vector2(0, Tuner.DEFAULT_PROJECTILE_OFFSET) - offset;
                    end = pos2DNew - new Vector2(0, Tuner.DEFAULT_PROJECTILE_OFFSET) - offset;
                    checkCollision(start, end, true);
                }
            }
        }
    }
}