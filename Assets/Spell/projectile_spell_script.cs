using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class projectile_spell_script : Spell {
	
	public float velocity;
	public int damage;
	public float blastRadius;
	public Vector2 dir;

	void Start () {
		spellID = 0;
		velocity = Tuner.DEFAULT_PROJECTILE_VELOCITY;
		blastRadius = Tuner.DEFAULT_PROJECTILE_BLAST_RADIUS;
		damage = Tuner.DEFAULT_PROJECTILE_DAMAGE;
		castLocation = getCurrentMousePos();
		dir = new Vector2 (castLocation.x - transform.position.x, castLocation.y - transform.position.y);
		Destroy (gameObject,2f);
	}
	

	void FixedUpdate () {
		//Liikuttaa projektiiliä kohteen suuntaan
		transform.Translate(dir.normalized*velocity);
	}

	void OnTriggerEnter2D(Collider2D coll){
		//explode();

		print ("ALLAHU AKBAR " + coll.name);
	}

	public void explode(){
		//Instansioi räjähdys


		//Tee damagea ympärille
		GameObject[] hostileList = GameObject.FindGameObjectsWithTag("Hostile");
		List<GameObject> targetList = new List<GameObject>();

		//GameObject[] neutralList = GameObject.FindGameObjectsWithTag("Neutral");
		//targetList.AddRange (neutralList);

		targetList.AddRange (hostileList);


		foreach(GameObject g in targetList){
			if(Vector2.Distance(g.transform.position , gameObject.transform.position) < blastRadius){
				g.GetComponent<UnitCombat> ().takeDamage (damage);
			}
		}

		destroy();
	}

	public bool colliding(){
		return true;
	}
}