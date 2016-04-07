using UnityEngine;
using System.Collections.Generic;
using System;

public class UnitCombat : MonoBehaviour
{
    //Unitin health
    private float health;
    private float maxHealth;

    //Unit type
    public bool isMelee = false;

    //Targetin seurausta varten.
    private GameObject lockedTarget = null;
    private HealthBar healthBar = null;

    private float attackRange;
    private bool attacking = false;
    private bool attacked = true;
    private int attackTimer = 30;
    private int maxAttackTimer = 30;
    private int attackPoint = 15;

    //TODO: Parempi spellilista
    private Skill[] spellList = new Skill[2];
    private PartySystem partySystem;
    private UnitMovement unitMovement;
    private CameraScripts cameraScripts;

    //Targets hit
    List<GameObject> hits = null;

    void Start()
    {

        health = Tuner.UNIT_BASE_HEALTH;
        maxHealth = Tuner.UNIT_BASE_HEALTH;
        //isMelee = false;
        attackRange = (isMelee) ? Tuner.UNIT_BASE_MELEE_RANGE : Tuner.UNIT_BASE_RANGED_RANGE;

        spellList[0] = ScriptableObject.CreateInstance("blot_out_skill") as blot_out_skill;
        spellList[1] = ScriptableObject.CreateInstance("charge_skill") as charge_skill;
        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();
        unitMovement = GetComponent<UnitMovement>();
        healthBar = GetComponent<HealthBar>();
        cameraScripts = Camera.main.GetComponent<CameraScripts>();

    }

    void checkForDeath()
    {
        if (health <= 0){
            if (Camera.main.transform.parent == transform)
                Camera.main.transform.parent = null;
            gameObject.SetActive(false);
            gameObject.tag = "Dead";
            partySystem.updateCharacterList();
            cameraScripts.updateTarget();
        }
    }

    void FixedUpdate()
    {
        checkForDeath();

        if (isLockedAttack())
        {
            if (!inRange(lockedTarget) && !attacking)
            {
                unitMovement.moveTo(lockedTarget.transform.position);
            }
            else if (inRange(lockedTarget))
            {
                startAttack();
            }
        }

        if (isAttacking())
        {
            //Nappaa targetit v‰h‰n ennen kuin tekee damage, est‰‰ sit‰ ett‰ targetit kerke‰‰ juosta rangesta pois joka kerta jos ne juoksee karkuun.
            if (attackTimer == maxAttackTimer)
            {
                //Mathf.Floor(maxAttackTimer*0.9f)){
                if (isMelee)
                    hits = getUnitsInMelee(unitMovement.direction);
            }

            attackTimer--;

            if (attackTimer <= 0)
            {
                resetAttack();
            }

            //Debug.Log(attackTimer);
            //Tehd‰‰n damage, otetaan kaikki targetit jotka olivat rangessa ja niihin damage.

            if (attackTimer == attackPoint)
            {
                attacked = true;
                if (isMelee)
                {
                    if (hits != null)
                    {
                        foreach (GameObject hit in hits)
                        {
                            if (hit != null && hit.activeSelf && hit.GetComponent<UnitCombat>() != null && hit.transform.tag != this.transform.tag)
                                dealDamage(hit, Tuner.UNIT_BASE_MELEE_DAMAGE);
                        }
                        hits = null;
                    }

                }
                else {

                    GameObject projectile = Instantiate(Resources.Load("fireball_projectile"), transform.position, Quaternion.identity) as GameObject;
                    if (isLockedAttack())
                    {
                        projectile.GetComponent<projectile_spell_script>().initAttack(lockedTarget.transform.position, gameObject, true);
                    }
                    else
                    {
                        projectile.GetComponent<projectile_spell_script>().initAttack(PlayerMovement.getCurrentMousePos(), gameObject, false);
                    }
                }
            }

        }

        //P‰ivitet‰‰n spellien logiikka.
        foreach (Skill s in spellList)
        {
            s.FixedUpdate();
        }


    }

    public void attackClosestTargetToPoint(Vector2 hit)
    {
        if (partySystem.getGroupID(gameObject) != -1)
        {
            List<GameObject> targetList = new List<GameObject>();

            //Hakee kaikki mobit Hostile tagill‰ ja lis‰‰ ne potentiaalisten vihollisten listaan.
            GameObject[] hostileList = GameObject.FindGameObjectsWithTag("Hostile");
            if (hostileList.Length == 0)
                return;
                
            targetList.AddRange(hostileList);

            //Laskee kuka potentiaalisten vihollisten listasta on l‰himp‰n‰ ja lockinnaa siihen.
            float distance = Mathf.Infinity;
            foreach (GameObject g in targetList)
            {
                float currentDistance = Vector3.Distance(g.transform.position, hit);

                if (currentDistance < distance)
                {
                    lockedTarget = g;
                    distance = currentDistance;
                }
            }
        }

    }

    internal Skill getSkill(int selectedSpellSlot)
    {
        return spellList[selectedSpellSlot];
    }

    public GameObject getClosestTargetToPoint(Vector2 hit)
    {
        List<GameObject> targetList = new List<GameObject>();
        GameObject[] hostileList = GameObject.FindGameObjectsWithTag("Hostile");
        if (hostileList.Length == 0)
            return null;

        targetList.AddRange(hostileList);

        GameObject target = null;

        float distance = Mathf.Infinity;
        foreach (GameObject g in targetList){
            float currentDistance = Vector3.Distance(g.transform.position, hit);

            if (currentDistance < distance)
            {
                target = g;
                distance = currentDistance;
            }
        }

        return target;
    }

    public void startAttack()
    {
        unitMovement.stop();
        attacking = true;
        attacked = false;
    }

    public void resetAttack()
    {
        attacking = false;
        attackTimer = maxAttackTimer;
    }

    public void stopAttack()
    {
        lockedTarget = null;
        attacking = false;
        attackTimer = maxAttackTimer;
    }

    //Hakee et‰isyyden t‰m‰n ja parametrin‰ annetun unitin v‰lill‰.
    public float getRange(GameObject target)
    {
        return Vector2.Distance(transform.position, lockedTarget.transform.position);
    }

    public bool isAttacking()
    {
        return attacking;
    }

    public bool isLockedAttack()
    {
        return lockedTarget == null ? false : true;
    }

    public bool hasAttacked()
    {
        return attacked;
    }

    public void setLockedTarget(GameObject target)
    {
        lockedTarget = target;
    }
    public GameObject getLockedTarget()
    {
        return lockedTarget;
    }
    public bool lineOfSight(GameObject target)
    {
        if (target != null && target.activeSelf)
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, target.transform.position, Tuner.LAYER_OBSTACLES);
            if (hit.collider == null)
                return true;
        }
        return false;
    }

    //Tarkastaa nyt vain jos kyseinen kohde on attack rangen sis‰ll‰. Tarkistukset siit‰ jos vihollinen on liian kaukana en‰‰n seuraamiseen pit‰‰ tehd‰ itse.
    public bool inRange(GameObject target)
    {
        //Asettaa vaan ranged unitille pidemm‰n rangen, LOS tarkistukset voi tehd‰ vaikka jokasen mobin omassa scriptiss‰, tai luoda uuden function t‰nne tarkistusta varten.
        if (target != null && target.activeSelf)
        {
            if (getRange(target) < attackRange)
            {
                if (!isMelee && lineOfSight(target))
                    return true;
                else if (isMelee)
                    return true;
                else
                    return false;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    }
    //Can also be used to heal with negative argument
    public void takeDamage(float damage)
    {
        if ((health - damage) > maxHealth)
            health = maxHealth;
        else
            health -= damage;

        checkForDeath();

        if (healthBar != null)
            healthBar.update(getHealth() / getMaxHealth());
    }

    //Can also be used to heal with negative argument
    public void dealDamage(GameObject enemy, float amount)
    {

        if (enemy != null && enemy.activeSelf){
            enemy.GetComponent<UnitCombat>().takeDamage(amount);
        }

        //Debug.Log("DEALT DAMAGE." + enemy + " REMAINING HEALTH:" + enemy.GetComponent<UnitCombat>().getHealth());
    }

    public float getHealth()
    {
        return health;
    }
    public void setHealth(float hp)
    {
        health = hp;
        if (healthBar != null)
            healthBar.update(getHealth() / getMaxHealth());
    }
    public void resetHealth()
    {
        health = maxHealth;
        if (healthBar != null)
            healthBar.update(getHealth() / getMaxHealth());
    }
    public float getMaxHealth()
    {
        return maxHealth;
    }
    public float getAttackRange()
    {
        return attackRange;
    }

    public void castSpellInSlot(int slot, GameObject unit)
    {

        if (partySystem.getGroupID(gameObject) != -1)
        {
            spellList[slot].cast(unit);
        }

    }

    //Haetaan meleerangessa olevat viholliset ja tehd‰‰n juttuja.
    public List<GameObject> getUnitsInMelee(UnitMovement.Direction dir)
    {
        //int mod = 3;
        GameObject[] hostiles = null;
        List<GameObject> hostilesInRange = new List<GameObject>();

        if (transform.tag != "Hostile")
            hostiles = GameObject.FindGameObjectsWithTag("Hostile");
        else
            hostiles = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject hostile in hostiles)
        {
            float dis = Vector2.Distance(transform.position, hostile.transform.position);
            if (dis <= getAttackRange())
            {
                Vector2 relative = transform.InverseTransformPoint(hostile.transform.position);
                float attackAngle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
                if (Mathf.Abs((unitMovement.getFacingAngle()) - attackAngle) < 45f)
                    hostilesInRange.Add(hostile);
                //print(Mathf.Abs((unitMovement.getFacingAngle()) - attackAngle));
                //print("facingAngle: " + (unitMovement.getFacingAngle()) + " attackAngle: " + attackAngle);
            }
        }

        return hostilesInRange;
    }
}
