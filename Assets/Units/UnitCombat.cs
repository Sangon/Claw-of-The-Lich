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
	private int attackTimer = 60;
	private int maxAttackTimer = 60;
	private int attackPoint = 30;

	//TODO: Parempi spellilista
	private Skill[] spellList = new Skill[2];

	void Start () {
		health = Tuner.UNIT_BASE_HEALTH;
		attackRange = Tuner.UNIT_BASE_MELEE_RANGE;

		//TODO: Parempi spellien initialisointi.
		spellList [0] = new projectile_skill();
		spellList [1] = new projectile_skill();

	}

	RaycastHit2D[] hits;

	void FixedUpdate () {

		if(health <= 0){
			Destroy(gameObject);
		}

		if(lockedTarget != null){
			
			if (!inRange(lockedTarget)) {
				GetComponent<UnitMovement>().moveTo(lockedTarget.transform.position);
			}else{
				GetComponent<UnitMovement>().stop();
				startAttack();
			}

		}

		//Debug.Log (attackAngle);
		//Debug.DrawLine(transform.position, new Vector3(transform.position.x + GetComponent<UnitMovement>().getMovementDelta().x*10,transform.position.y + GetComponent<UnitMovement>().getMovementDelta().y*10,0));
		//Debug.Log("DIR: " + GetComponent<UnitMovement>().getDirection());

		if(attacking){

			//Nappaa targetit vähän ennen kuin tekee damage, estää sitä että targetit kerkeää juosta rangesta pois joka kerta jos ne juoksee karkuun.

			if(attackTimer == Mathf.Floor(maxAttackTimer*0.9f)){
				hits = getUnitsInMelee(GetComponent<UnitMovement>().direction);

			}


			attackTimer--;


			if(attackTimer >= maxAttackTimer){
				stopAttack();
			}

			//Tehdään damage, otetaan kaikki targetit jotka olivat rangessa ja niihin damage.
			if(attackTimer == attackPoint){
				
				if (hits != null) {
					foreach (RaycastHit2D hit in hits) {
						hit.collider.GetComponent<UnitCombat>().takeDamage(10);
					}
				}

			}

		}
		DebugRay(GetComponent<UnitMovement>().direction);

		//Päivitetään spellien logiikka.
		foreach(Skill s in spellList){
			s.FixedUpdate();
		}


	}

	public void attackClosestTargetToPoint(Vector2 hit){
		
		List<GameObject> targetList = new List<GameObject>();

		//Hakee kaikki mobit Hostile tagillä ja lisää ne potentiaalisten vihollisten listaan.
		GameObject[] hostileList = GameObject.FindGameObjectsWithTag("Hostile");
		targetList.AddRange (hostileList);

		//NYI: bugaa ihan huolella jostaki syystä
		//GameObject[] neutralList = GameObject.FindGameObjectsWithTag("Neutral");
		//targetList.AddRange (neutralList);

		//Laskee kuka potentiaalisten vihollisten listasta on lähimpänä ja lockinnaa siihen.
		float distance = Mathf.Infinity;
		foreach(GameObject g in targetList){
			float currentDistance = Vector3.Distance(g.transform.position,hit);

			if(currentDistance < distance){
				lockedTarget = g;
				distance = currentDistance;
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
		attacking = true;
	}

	public void stopAttack(){
		attacking = false;
		attackTimer = 60;
	}

	//Hakee etäisyyden tämän ja parametrinä annetun unitin välillä.
	public float getRange(GameObject target){
		return Vector2.Distance(transform.position, lockedTarget.transform.position);
	}

	//Tarkastaa nyt vain jos kyseinen kohde on attack rangen sisällä. Tarkistukset siitä jos vihollinen on liian kaukana enään seuraamiseen pitää tehdä itse.
	public bool inRange(GameObject target){
		//Asettaa vaan ranged unitille pidemmän rangen, LOS tarkistukset voi tehdä vaikka jokasen mobin omassa scriptissä, tai luoda uuden function tänne tarkistusta varten.
		if (target != null) {
			if (Vector2.Distance (transform.position, lockedTarget.transform.position) < attackRange) {
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
		enemy.GetComponent<UnitCombat>().takeDamage(amount);
	}

	public void castSpellInSlot(int slot, GameObject unit){
		spellList[slot].cast(unit);
	}


	//Haetaan meleerangessa olevat viholliset ja tehdään juttuja.
	public RaycastHit2D[] getUnitsInMelee(int dir){

		if(dir == 1){
			return Physics2D.RaycastAll(new Vector3(transform.position.x - 20, transform.position.y + 10,0),new Vector3(transform.position.x - 20, transform.position.y - 10,0));
		}else if(dir == 2){
			return Physics2D.RaycastAll(new Vector3(transform.position.x - 30, transform.position.y - 5,0),new Vector3(transform.position.x - 5, transform.position.y - 20,0));
		}else if(dir == 3){
			return Physics2D.RaycastAll(new Vector3(transform.position.x - 10, transform.position.y - 20,0),new Vector3(transform.position.x + 10, transform.position.y - 20,0));
		}else if(dir == 4){
			return Physics2D.RaycastAll(new Vector3(transform.position.x + 30, transform.position.y - 5,0),new Vector3(transform.position.x + 5, transform.position.y - 20,0));
		}else if(dir == 5){
			return Physics2D.RaycastAll(new Vector3(transform.position.x + 20, transform.position.y + 10,0),new Vector3(transform.position.x + 20, transform.position.y - 10,0));
		}else if(dir == 6){
			return Physics2D.RaycastAll(new Vector3(transform.position.x + 30, transform.position.y + 5,0),new Vector3(transform.position.x + 5, transform.position.y + 20,0));
		}else if(dir == 7){
			return Physics2D.RaycastAll(new Vector3(transform.position.x - 10, transform.position.y + 20,0),new Vector3(transform.position.x + 10, transform.position.y + 20,0));
		}else if(dir == 8){
			return Physics2D.RaycastAll(new Vector3(transform.position.x - 30, transform.position.y + 5,0),new Vector3(transform.position.x - 5, transform.position.y + 20,0));
		}

		return null;
	}


	//Debuggaamista varten melee raycastit.
	public void DebugRay(int dir){

		if(dir == 1){
			Debug.DrawLine(new Vector3(transform.position.x - 20, transform.position.y + 10,0),new Vector3(transform.position.x - 20, transform.position.y - 10,0));
		}else if(dir == 2){
			Debug.DrawLine(new Vector3(transform.position.x - 30, transform.position.y - 5,0),new Vector3(transform.position.x - 5, transform.position.y - 20,0));
		}else if(dir == 3){
			Debug.DrawLine(new Vector3(transform.position.x - 10, transform.position.y - 20,0),new Vector3(transform.position.x + 10, transform.position.y - 20,0));
		}else if(dir == 4){
			Debug.DrawLine(new Vector3(transform.position.x + 30, transform.position.y - 5,0),new Vector3(transform.position.x + 5, transform.position.y - 20,0));
		}else if(dir == 5){
			Debug.DrawLine(new Vector3(transform.position.x + 20, transform.position.y + 10,0),new Vector3(transform.position.x + 20, transform.position.y - 10,0));
		}else if(dir == 6){
			Debug.DrawLine(new Vector3(transform.position.x + 30, transform.position.y + 5,0),new Vector3(transform.position.x + 5, transform.position.y + 20,0));
		}else if(dir == 7){
			Debug.DrawLine(new Vector3(transform.position.x - 10, transform.position.y + 20,0),new Vector3(transform.position.x + 10, transform.position.y + 20,0));
		}else if(dir == 8){
			Debug.DrawLine(new Vector3(transform.position.x - 30, transform.position.y + 5,0),new Vector3(transform.position.x - 5, transform.position.y + 20,0));
		}
	}

}
