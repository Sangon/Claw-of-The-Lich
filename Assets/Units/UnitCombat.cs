using UnityEngine;
using System.Collections.Generic;

public class UnitCombat : MonoBehaviour
{
    //Unitin health
    private float health;
    private float maxHealth;

    //Unit type
    private bool melee = false;

    //Targetin seurausta varten.
    private GameObject lockedTarget = null;
    private HealthBar healthBar = null;
    private PlayerHUD playerHUD = null;

    private float attackRange;
    private bool attacking = false;
    private bool attacked = true;
    private bool lockedAttack = false;
    private int attackTimer = 30;
    private int maxAttackTimer = 30;
    private int attackPoint = 15;

    //TODO: Parempi spellilista
    private Skill[] spellList = new Skill[2];
    private PartySystem partySystem;
    private UnitMovement unitMovement;
    private CameraScripts cameraScripts;

    private unit_attributes attributes;

    //Targets hit
    List<GameObject> hits = null;

    void Start()
    {
        attributes = new unit_attributes(gameObject.name);
        health = attributes.health;
        maxHealth = attributes.health;

        if (gameObject.name.Contains("Melee"))
            melee = true;

        melee = attributes.isMelee;

        attackRange = (isMelee()) ? Tuner.UNIT_BASE_MELEE_RANGE : Tuner.UNIT_BASE_RANGED_RANGE;

        spellList[1] = attributes.skill1;
        spellList[0] = attributes.skill2;
        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();
        unitMovement = GetComponent<UnitMovement>();
        healthBar = GetComponent<HealthBar>();

        if (gameObject.tag.Equals("Player"))
            playerHUD = GetComponent<PlayerHUD>();

        cameraScripts = Camera.main.GetComponent<CameraScripts>();
    }

    public Skill[] getSpellList()
    {
        return spellList;
    }

    public void changeWeapon()
    {
        melee = !melee;
        attackRange = (isMelee()) ? Tuner.UNIT_BASE_MELEE_RANGE : Tuner.UNIT_BASE_RANGED_RANGE;
    }
    public void changeWeaponTo(bool melee)
    {
        if ((isMelee() && !melee) || (!isMelee() && melee))
            changeWeapon();
    }

    public bool isAlive()
    {
        if (getHealth() > 0f)
            return true;
        return false;
    }

    public bool isMelee()
    {
        return melee;
    }

    void checkForDeath()
    {
        if (health <= 0)
        {
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

        if (lockedAttack)
        {
            if (lockedTarget != null && lockedTarget.activeSelf)
            {
                if (!inRange(lockedTarget) && !attacking)
                    unitMovement.moveTo(lockedTarget.transform.position);
                else if (inRange(lockedTarget))
                    startAttack();
            }
            else // Target is dead!
                stopAttack();
        }

        if (isAttacking())
        {
            //Nappaa targetit v‰h‰n ennen kuin tekee damage, est‰‰ sit‰ ett‰ targetit kerke‰‰ juosta rangesta pois joka kerta jos ne juoksee karkuun.
            if (attackTimer == maxAttackTimer)
            {
                //Mathf.Floor(maxAttackTimer*0.9f)){
                if (isMelee())
                    hits = getUnitsInMelee(unitMovement.getDirection());
            }

            attackTimer--;

            if (attackTimer <= 0)
                resetAttack();
            else if (attackTimer == attackPoint)
            {
                attacked = true;
                if (isMelee())
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
                    GameObject projectile = Instantiate(Resources.Load("testSpell"), transform.position, Quaternion.identity) as GameObject;
                    if (lockedAttack)
                    {
                        Vector3 polygonColliderCenter = lockedTarget.GetComponent<PolygonCollider2D>().bounds.center;
                        projectile.GetComponent<projectile_spell_script>().initAttack(polygonColliderCenter, gameObject, true);
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
            GameObject bestTarget = null;
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
                float currentDistance = Vector2.Distance(g.transform.position, hit);

                if (currentDistance < distance)
                {
                    bestTarget = g;
                    distance = currentDistance;
                }
            }
            if (distance <= Tuner.ATTACKMOVE_MAX_SEARCH_DISTANCE_FROM_CLICK_POINT)
            {
                lockedAttack = true;
                lockedTarget = bestTarget;
            }
        }

    }

    public GameObject getClosestTargetToPoint(Vector2 hit)
    {
        List<GameObject> targetList = new List<GameObject>();
        GameObject[] hostileList = GameObject.FindGameObjectsWithTag("Hostile");
        if (hostileList.Length == 0)
            return null;

        targetList.AddRange(hostileList);

        GameObject target = null;

        float distance = float.MaxValue;
        foreach (GameObject g in targetList)
        {
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
        //GetComponent<Animator>().Play("Attacking_SW");
    }

    public void resetAttack()
    {
        attacking = false;
        attackTimer = maxAttackTimer;
    }

    public void stopAttack()
    {
        lockedTarget = null;
        lockedAttack = false;
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
        return lockedAttack;
    }

    public bool hasAttacked()
    {
        return attacked;
    }

    private void setLockedTarget(GameObject target)
    {
        lockedTarget = target;
        lockedAttack = true;
    }
    public GameObject getLockedTarget()
    {
        return lockedTarget;
    }
    public bool lineOfSight(GameObject target)
    {
        return unitMovement.lineOfSight(transform.position, target.transform.position);
    }

    //Tarkastaa nyt vain jos kyseinen kohde on attack rangen sis‰ll‰. Tarkistukset siit‰ jos vihollinen on liian kaukana en‰‰n seuraamiseen pit‰‰ tehd‰ itse.
    public bool inRange(GameObject target)
    {
        if (target != null && target.activeSelf)
        {
            if (getRange(target) < attackRange && lineOfSight(target))
                return true;
            else
                return false;
        }
        else {
            return false;
        }
    }
    //Can also be used to heal with negative argument
    public void takeDamage(float damage, GameObject source)
    {
        if ((health - damage) > maxHealth)
            health = maxHealth;
        else
            health -= damage;

        checkForDeath();
        updateHUD();

        if (source != null && gameObject.activeSelf && source.activeSelf && !gameObject.tag.Equals("Player") && source != gameObject)
        {
            // AI: Aggro on the attacker
            aggro(source);
        }

    }
    //Can also be used to heal with negative argument
    public void dealDamage(GameObject enemy, float amount) //Onko t‰‰ oikeesti tarpeellinen?
    {
        if (enemy != null && enemy.activeSelf)
        {
            enemy.GetComponent<UnitCombat>().takeDamage(amount, gameObject);
        }
        //Debug.Log("DEALT DAMAGE." + enemy + " REMAINING HEALTH:" + enemy.GetComponent<UnitCombat>().getHealth());
    }

    private void updateHUD()
    {
        if (healthBar != null)
            healthBar.update(getHealth() / getMaxHealth());
        if (playerHUD != null)
            playerHUD.updateStats(getHealth() / getMaxHealth(), 1f);
    }

    public void aggro(GameObject target)
    {
        if (!isAttacking())
        {
            setLockedTarget(target);
        }
    }
    public float getHealth()
    {
        return health;
    }
    public void setHealth(float hp)
    {
        health = hp;
        updateHUD();
    }
    public void resetHealth()
    {
        health = maxHealth;
        updateHUD();
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
                if (Mathf.Abs((unitMovement.getFacingAngle()) - attackAngle) < Tuner.DEFAULT_MELEE_ATTACK_CONE_DEGREES)
                    hostilesInRange.Add(hostile);
                //print(Mathf.Abs((unitMovement.getFacingAngle()) - attackAngle));
                //print("facingAngle: " + (unitMovement.getFacingAngle()) + " attackAngle: " + attackAngle);
            }
        }
        return hostilesInRange;
    }
}
