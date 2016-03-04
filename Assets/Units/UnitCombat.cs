using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitCombat : MonoBehaviour {


	//Unitin health
	public int health;

	//Targetin seurausta varten.
	public GameObject lockedTarget = null;

	private float attackRange;
	private bool attacking = false;
	private int attackTimer = 30;
	private int maxAttackTimer = 30;
	private int attackPoint = 15;

	//TODO: Parempi spellilista
	private Skill[] spellList = new Skill[2];
    private PartySystem partySystem;

    void Start () {
		health = Tuner.UNIT_BASE_HEALTH;
		attackRange = Tuner.UNIT_BASE_MELEE_RANGE;

		//TODO: Parempi spellien initialisointi.
		spellList [0] = new projectile_skill();
		spellList [1] = new projectile_skill();
        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();

    }

	RaycastHit2D[] hits;

	void FixedUpdate () {

		if(health <= 0){
			Destroy(gameObject);
		}

        if (lockedTarget != null)
        {
            if (!inRange(lockedTarget) && !attacking)
            {
                GetComponent<UnitMovement>().moveTo(lockedTarget.transform.position);
            }else{
                startAttack();
            }
        }
        
		if(attacking){

			//Nappaa targetit v‰h‰n ennen kuin tekee damage, est‰‰ sit‰ ett‰ targetit kerke‰‰ juosta rangesta pois joka kerta jos ne juoksee karkuun.

			if(attackTimer == maxAttackTimer) {//Mathf.Floor(maxAttackTimer*0.9f)){
				hits = getUnitsInMelee(GetComponent<UnitMovement>().direction);
			}

			attackTimer--;

			if(attackTimer <= 0){
                resetAttack();
			}
            Debug.Log(attackTimer);
			//Tehd‰‰n damage, otetaan kaikki targetit jotka olivat rangessa ja niihin damage.
			if(attackTimer == attackPoint){
				
				if (hits != null) {
					foreach (RaycastHit2D hit in hits) {
                        if(hit.collider.GetComponent<UnitCombat>() != null)
                            dealDamage(hit.collider.gameObject,10);
					}
                    hits = null;
				}

			}

		}

		DebugRay(GetComponent<UnitMovement>().direction);

		//P‰ivitet‰‰n spellien logiikka.
		foreach(Skill s in spellList){
			s.FixedUpdate();
		}


	}

	public void attackClosestTargetToPoint(Vector2 hit){

        if (partySystem.isSelected2(gameObject)) {
            List<GameObject> targetList = new List<GameObject>();

            //Hakee kaikki mobit Hostile tagill‰ ja lis‰‰ ne potentiaalisten vihollisten listaan.
            GameObject[] hostileList = GameObject.FindGameObjectsWithTag("Hostile");
            targetList.AddRange(hostileList);

            //Laskee kuka potentiaalisten vihollisten listasta on l‰himp‰n‰ ja lockinnaa siihen.
            float distance = Mathf.Infinity;
            foreach (GameObject g in targetList) {
                float currentDistance = Vector3.Distance(g.transform.position, hit);

                if (currentDistance < distance) {
                    lockedTarget = g;
                    distance = currentDistance;
                }
            }
        }

	}

	public GameObject getClosestTargetToPoint(Vector2 hit){

		List<GameObject> targetList = new List<GameObject>();
		GameObject[] hostileList = GameObject.FindGameObjectsWithTag("Hostile");
		targetList.AddRange (hostileList);

		GameObject target = null;

		float distance = Mathf.Infinity;
		foreach(GameObject g in targetList){
			float currentDistance = Vector3.Distance(g.transform.position,hit);

			if(currentDistance < distance){
				target = g;
				distance = currentDistance;
			}
		}

		return target;
	}
		
	public void startAttack(){
        GetComponent<UnitMovement>().stop();
        attacking = true;
	}

    public void resetAttack() {
        attacking = false;
        attackTimer = maxAttackTimer;
    }

	public void stopAttack(){
        lockedTarget = null;
		attacking = false;
		attackTimer = maxAttackTimer;
	}

	//Hakee et‰isyyden t‰m‰n ja parametrin‰ annetun unitin v‰lill‰.
	public float getRange(GameObject target){
		return Vector2.Distance(transform.position, lockedTarget.transform.position);
	}

	//Tarkastaa nyt vain jos kyseinen kohde on attack rangen sis‰ll‰. Tarkistukset siit‰ jos vihollinen on liian kaukana en‰‰n seuraamiseen pit‰‰ tehd‰ itse.
	public bool inRange(GameObject target){

		//Asettaa vaan ranged unitille pidemm‰n rangen, LOS tarkistukset voi tehd‰ vaikka jokasen mobin omassa scriptiss‰, tai luoda uuden function t‰nne tarkistusta varten.
		if (target != null) {

			if (getRange(target) < attackRange){
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}

	public void takeDamage(int damage){
		health -= damage;
	}

	public void dealDamage(GameObject enemy, int amount){

        if (enemy != null) {
            enemy.GetComponent<UnitCombat>().takeDamage(amount);
        }

        Debug.Log("DEALT DAMAGE." + enemy + " REMAINING HEALTH:" + enemy.GetComponent<UnitCombat>().getHealth());
	}

    public int getHealth() {
        return health;
    }

	public void castSpellInSlot(int slot, GameObject unit){
        if (partySystem.isSelected2(gameObject)){
            spellList[slot].cast(unit);
        }
	}


	//Haetaan meleerangessa olevat viholliset ja tehd‰‰n juttuja.
	public RaycastHit2D[] getUnitsInMelee(UnitMovement.Direction dir){
        int mod = 3;

        if (dir == UnitMovement.Direction.W){
			return Physics2D.RaycastAll(new Vector3(transform.position.x - 20 * mod, transform.position.y + 10 * mod, 0),new Vector3(transform.position.x - 20 * mod, transform.position.y - 10 * mod, 0));
		}else if(dir == UnitMovement.Direction.SW){
			return Physics2D.RaycastAll(new Vector3(transform.position.x - 30 * mod, transform.position.y - 5 * mod, 0),new Vector3(transform.position.x - 5 * mod, transform.position.y - 20 * mod, 0));
		}else if(dir == UnitMovement.Direction.S){
			return Physics2D.RaycastAll(new Vector3(transform.position.x - 10 * mod, transform.position.y - 20 * mod, 0),new Vector3(transform.position.x + 10 * mod, transform.position.y - 20 * mod, 0));
		}else if(dir == UnitMovement.Direction.SE){
			return Physics2D.RaycastAll(new Vector3(transform.position.x + 30 * mod, transform.position.y - 5 * mod, 0),new Vector3(transform.position.x + 5 * mod, transform.position.y - 20 * mod, 0));
		}else if(dir == UnitMovement.Direction.E){
			return Physics2D.RaycastAll(new Vector3(transform.position.x + 20 * mod, transform.position.y + 10 * mod, 0),new Vector3(transform.position.x + 20 * mod, transform.position.y - 10 * mod, 0));
		}else if(dir == UnitMovement.Direction.NE){
			return Physics2D.RaycastAll(new Vector3(transform.position.x + 30 * mod, transform.position.y + 5 * mod, 0),new Vector3(transform.position.x + 5 * mod, transform.position.y + 20 * mod, 0));
		}else if(dir == UnitMovement.Direction.N){
			return Physics2D.RaycastAll(new Vector3(transform.position.x - 10 * mod, transform.position.y + 20 * mod, 0),new Vector3(transform.position.x + 10 * mod, transform.position.y + 20 * mod, 0));
		}else if(dir == UnitMovement.Direction.NW){
			return Physics2D.RaycastAll(new Vector3(transform.position.x - 30 * mod, transform.position.y + 5 * mod, 0),new Vector3(transform.position.x - 5 * mod, transform.position.y + 20 * mod, 0));
		}

		return null;
	}


	//Debuggaamista varten melee raycastit.
	public void DebugRay(UnitMovement.Direction dir){
        int mod = 3;
        if (dir == UnitMovement.Direction.W){
			Debug.DrawLine(new Vector3(transform.position.x - 20 * mod, transform.position.y + 10 * mod, 0),new Vector3(transform.position.x - 20 * mod, transform.position.y - 10 * mod, 0));
		}else if(dir == UnitMovement.Direction.SW){
			Debug.DrawLine(new Vector3(transform.position.x - 30 * mod, transform.position.y - 5 * mod, 0),new Vector3(transform.position.x - 5 * mod, transform.position.y - 20 * mod, 0));
		}else if(dir == UnitMovement.Direction.S){
			Debug.DrawLine(new Vector3(transform.position.x - 10 * mod, transform.position.y - 20 * mod, 0),new Vector3(transform.position.x + 10 * mod, transform.position.y - 20 * mod, 0));
		}else if(dir == UnitMovement.Direction.SE){
			Debug.DrawLine(new Vector3(transform.position.x + 30 * mod, transform.position.y - 5 * mod, 0),new Vector3(transform.position.x + 5 * mod, transform.position.y - 20 * mod, 0));
		}else if(dir == UnitMovement.Direction.E){
			Debug.DrawLine(new Vector3(transform.position.x + 20 * mod, transform.position.y + 10 * mod, 0),new Vector3(transform.position.x + 20 * mod, transform.position.y - 10 * mod, 0));
		}else if(dir == UnitMovement.Direction.NE){
			Debug.DrawLine(new Vector3(transform.position.x + 30 * mod, transform.position.y + 5 * mod, 0),new Vector3(transform.position.x + 5 * mod, transform.position.y + 20 * mod, 0));
		}else if(dir == UnitMovement.Direction.N){
			Debug.DrawLine(new Vector3(transform.position.x - 10 * mod, transform.position.y + 20 * mod, 0),new Vector3(transform.position.x + 10 * mod, transform.position.y + 20 * mod, 0));
		}else if(dir == UnitMovement.Direction.NW){
			Debug.DrawLine(new Vector3(transform.position.x - 30 * mod, transform.position.y + 5 * mod, 0),new Vector3(transform.position.x - 5 * mod, transform.position.y + 20 * mod, 0));
		}
	}


}
