using UnityEngine;
using System.Collections.Generic;

public class UnitCombat : MonoBehaviour
{
    //Unit's health & stamina
    private float health;
    private float maxHealth;
    private float stamina;
    private float maxStamina;

    //Unit's melee/ranged attack damage
    private float damage;
    private float meleeDamage;
    private float rangedDamage;

    private float movementSpeed;
    private float baseMovementSpeed;

    //Unit type
    private bool melee = false;

    //Targetin seurausta varten.
    private GameObject lockedTarget = null;

    private float attackRange;
    private bool attacking = false;
    private bool attacked = true;
    private bool lockedAttack = false;
    private int attackTimer = 30;
    private int maxAttackTimer = 30;
    private int maxMeleeAttackTimer = 30;
    private int maxRangedAttackTimer = 30;
    private int attackPoint = 24; //Should be a bit less than maxAttackTimer, depending on animation

    private bool stopAttackAfter = false;

    //TODO: Parempi spellilista
    private Skill[] spellList = new Skill[2];
    private PartySystem partySystem;
    private UnitMovement unitMovement;
    private Buffs buffs;
    private CameraScripts cameraScripts;

    private unit_attributes attributes;

    //Targets hit
    List<GameObject> hits = null;

    void Awake()
    {
        attributes = new unit_attributes(gameObject.name);
        health = attributes.health;
        maxHealth = attributes.health;
        stamina = Tuner.UNIT_BASE_STAMINA;
        maxStamina = Tuner.UNIT_BASE_STAMINA;

        melee = attributes.isMelee;

        meleeDamage = attributes.meleedamage;
        maxMeleeAttackTimer = attributes.meleeattackframes;
        rangedDamage = attributes.rangeddamage;
        maxRangedAttackTimer = attributes.rangedattackframes;

        if (isMelee())
        {
            attackRange = Tuner.UNIT_BASE_MELEE_RANGE;
            maxAttackTimer = maxMeleeAttackTimer;
            damage = meleeDamage;
        }
        else
        {
            attackRange = Tuner.UNIT_BASE_RANGED_RANGE;
            maxAttackTimer = maxRangedAttackTimer;
            damage = rangedDamage;
        }

        attackPoint = (int)(maxAttackTimer * 0.8);
        attackTimer = maxAttackTimer;

        spellList[0] = attributes.skill1;
        spellList[1] = attributes.skill2;

        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();
        unitMovement = GetComponent<UnitMovement>();

        movementSpeed = attributes.movementspeed;
        baseMovementSpeed = movementSpeed;
        unitMovement.setMovementSpeed(movementSpeed);

        buffs = GetComponent<Buffs>();

        cameraScripts = Camera.main.GetComponent<CameraScripts>();
    }

    public float getMovementSpeed()
    {
        return movementSpeed;
    }

    public float getBaseMovementSpeed()
    {
        return baseMovementSpeed;
    }

    public int getMaxAttackTimer()
    {
        return maxAttackTimer;
    }

    public Skill[] getSpellList()
    {
        return spellList;
    }

    public void changeWeapon()
    {
        melee = !melee;
        if (isMelee())
        {
            attackRange = Tuner.UNIT_BASE_MELEE_RANGE;
            maxAttackTimer = maxMeleeAttackTimer;
        }
        else
        {
            attackRange = Tuner.UNIT_BASE_RANGED_RANGE;
            maxAttackTimer = maxRangedAttackTimer;
        }
        attackPoint = (int)(maxAttackTimer * 0.8);
        attackTimer = maxAttackTimer;
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
        if (health <= 0 && !gameObject.tag.Equals("Dead"))
        {
            setStamina(0);

            //Disable most of the scripts on the gameobject (UnitMovement is needed for death animations)
            if (GetComponent<AstarAI>())
                GetComponent<AstarAI>().enabled = false;
            if (GetComponent<Seeker>())
                GetComponent<Seeker>().enabled = false;
            if (GetComponent<Buffs>())
                GetComponent<Buffs>().enabled = false;
            if (GetComponent<ArrowIndicator>())
                GetComponent<ArrowIndicator>().enabled = false;
            if (GetComponent<EnemyAI>())
                GetComponent<EnemyAI>().enabled = false;
            if (GetComponent<AIStates>())
                GetComponent<AIStates>().enabled = false;
            if (GetComponent<HealthBar>())
                GetComponent<HealthBar>().enabled = false;
            stopAttack();
            unitMovement.stop();

            if (tag.Equals("Hostile"))
            {
                //Hide minimap icon for enemies
                GameObject minimapIcon = GameObject.Find("Icon_" + gameObject.name);
                if (minimapIcon != null)
                    minimapIcon.SetActive(false);
                if (GetComponent<MiniMapMark>())
                    GetComponent<MiniMapMark>().enabled = false;
            }

            GameObject canvas = transform.Find("Canvas").gameObject;
            if (canvas != null)
                canvas.SetActive(false);

            if (Camera.main.transform.parent == transform)
                Camera.main.transform.parent = null;
            FMODUnity.RuntimeManager.PlayOneShot("event:/sfx/enemy_down", AudioScript.get3DAudioPositionVector3(transform.position));
            //gameObject.SetActive(false);

            if (tag.Equals("Player"))
            {
                partySystem.updateCharacterList();
                cameraScripts.updateTarget();
            } else
            {
                GameObject.Find("HUD").GetComponent<GameHUD>().addMana(3);
            }

            gameObject.tag = "Dead";
        }
    }

    private bool canAttack()
    {
        if (!isAlive() || buffs.isStunned())
            return false;
        return true;
    }

    void FixedUpdate()
    {
        checkForDeath();
        if (!canAttack())
            stopAttack();
        else if (lockedAttack)
        {
            if (lockedTarget != null && lockedTarget.GetComponent<UnitCombat>().isAlive())
            {
                if (!buffs.isUncontrollable())
                {
                    if (!inRange(lockedTarget) && !isAttacking())
                        unitMovement.moveTo(lockedTarget.transform.position);
                    else if (inRange(lockedTarget))
                        startAttack();
                }
            }
            else //Target is dead!
                stopAttack();
        }

        if (isAttacking() && !buffs.isUncontrollable())
        {
            //Nappaa targetit v�h�n ennen kuin tekee damage, est�� sit� ett� targetit kerke�� juosta rangesta pois joka kerta jos ne juoksee karkuun.
            if (attackTimer == maxAttackTimer && isMelee())
            {
                hits = getUnitsInMelee(unitMovement.getDirection());
            }
            if (attackTimer == attackPoint)
            {
                attacked = true;
                if (isMelee())
                {
                    if (gameObject.tag.Equals("Hostile"))
                        FMODUnity.RuntimeManager.PlayOneShot("event:/sfx/attack__dagger", AudioScript.get3DAudioPositionVector3(transform.position));
                    else
                        FMODUnity.RuntimeManager.PlayOneShot("event:/sfx/attack_sword", AudioScript.get3DAudioPositionVector3(transform.position));
                    if (hits != null)
                    {
                        foreach (GameObject hit in hits)
                        {
                            if (hit != null && hit.GetComponent<UnitCombat>() != null && hit.GetComponent<UnitCombat>().isAlive() && hit.transform.tag != this.transform.tag)
                                dealDamage(hit, damage, Tuner.DamageType.melee);
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
                    else if (tag.Equals("Player"))
                    {
                        projectile.GetComponent<projectile_spell_script>().initAttack(gameObject.GetComponent<PlayerMovement>().getClickPosition(), gameObject, false);
                    }
                }
            }

            attackTimer--;

            if (attackTimer <= 0)
                if (stopAttackAfter)
                    stopAttack();
                else
                    resetAttack();
        }

        //P�ivitet��n spellien logiikka.
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

            if (UnitList.getHostiles().Length == 0)
                return;

            //Laskee kuka potentiaalisten vihollisten listasta on l�himp�n� ja lockinnaa siihen.
            float distance = Mathf.Infinity;
            foreach (GameObject g in UnitList.getHostiles())
            {
                float currentDistance = Ellipse.isometricDistance(g.transform.position, hit);

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
    /*
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
                float currentDistance = Ellipse.isometricDistance(g.transform.position, hit);

                if (currentDistance < distance)
                {
                    target = g;
                    distance = currentDistance;
                }
            }

            return target;
        }
    */

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
        stopAttackAfter = false;
    }

    public void stopAttack()
    {
        lockedTarget = null;
        lockedAttack = false;
        attacking = false;
        attackTimer = maxAttackTimer;
        stopAttackAfter = false;
    }

    public void stopAttackAfterAttacked()
    {
        stopAttackAfter = true;
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

    public bool inRange(GameObject target)
    {
        if (target != null && target.GetComponent<UnitCombat>().isAlive())
        {
            if (Ellipse.isometricDistance(transform.position, target.transform.position) < attackRange && UnitMovement.lineOfSight(transform.position, target.transform.position, false))
                return true;
            else
                return false;
        }
        else {
            return false;
        }
    }
    //Can also be used to heal with negative argument
    public void takeDamage(float damage, GameObject source, Tuner.DamageType damageType = Tuner.DamageType.none)
    {
        if (!isAlive())
            return;
        if (damageType == Tuner.DamageType.ranged)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/sfx/hit_flesh", AudioScript.get3DAudioPositionVector3(transform.position));
        }
        else if (damageType == Tuner.DamageType.melee)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/sfx/hit_metal", AudioScript.get3DAudioPositionVector3(transform.position));
            //Knockback
            if (!gameObject.tag.Equals("Player"))
                unitMovement.knockback(source, Tuner.PLAYER_MELEE_ATTACK_KNOCKBACK_DISTANCE);
        }

        if ((health - damage) > maxHealth)
            health = maxHealth;
        else
            health -= damage;

        checkForDeath();

        if (source != null && isAlive() && source.GetComponent<UnitCombat>() != null && source.GetComponent<UnitCombat>().isAlive() && !gameObject.tag.Equals("Player") && !GetComponent<AIStates>().inCombat() && source != gameObject)
        {
            // AI: Aggro on the attacker
            aggro(source);
        }
    }

    //Can also be used to heal with negative argument
    public void dealDamage(GameObject enemy, float amount, Tuner.DamageType damageType = Tuner.DamageType.none)
    {
        if (enemy != null && enemy.GetComponent<UnitCombat>() != null && enemy.GetComponent<UnitCombat>().isAlive())
        {
            enemy.GetComponent<UnitCombat>().takeDamage(amount, gameObject, damageType);
        }
        //if (name.Equals("Character#1"))
        //Debug.Log("DEALT DAMAGE." + enemy + " REMAINING HEALTH:" + enemy.GetComponent<UnitCombat>().getHealth());
    }

    public bool canCastSpell(int spellID)
    {
        if (getSpellList()[spellID].isOnCooldown() || buffs.isStunned() || buffs.isUncontrollable())
            return false;
        return true;
    }

    //Used by AI
    public void aggro(GameObject target)
    {
        setLockedTarget(target);
    }
    public float getHealth()
    {
        return health;
    }
    public void setHealth(float value)
    {
        health = value;
    }
    public void resetHealth()
    {
        health = maxHealth;
    }
    public float getMaxHealth()
    {
        return maxHealth;
    }
    public void setStamina(float value)
    {
        stamina = value;
    }
    public float getStamina()
    {
        return stamina;
    }
    public float getMaxStamina()
    {
        return maxStamina;
    }
    public float getAttackRange()
    {
        return attackRange;
    }

    public void castSpellInSlot(int slot)
    {
        spellList[slot].cast(gameObject);
    }

    //Haetaan meleerangessa olevat viholliset ja tehd��n juttuja.
    public List<GameObject> getUnitsInMelee(UnitMovement.Direction dir)
    {
        //int mod = 3;
        GameObject[] targets = null;
        List<GameObject> targetsInRange = new List<GameObject>();

        if (transform.tag != "Hostile")
            targets = UnitList.getHostiles();
        else
            targets = UnitList.getPlayers();

        foreach (GameObject target in targets)
        {
            float dis = Ellipse.isometricDistance(transform.position, target.transform.position);
            if (dis <= getAttackRange())
            {
                Vector2 relative = transform.InverseTransformPoint(target.transform.position);
                float attackAngle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
                if (Mathf.Abs((unitMovement.getFacingAngle()) - attackAngle) <= Tuner.DEFAULT_MELEE_ATTACK_CONE_DEGREES)
                    targetsInRange.Add(target);

                //print(Mathf.Abs((unitMovement.getFacingAngle()) - attackAngle));
                //print("facingAngle: " + (unitMovement.getFacingAngle()) + " attackAngle: " + attackAngle);
            }
        }
        return targetsInRange;
    }
}
