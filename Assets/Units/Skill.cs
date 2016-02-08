using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Skill : MonoBehaviour, ISkill{

	protected int maxCooldown = Tuner.DEFAULT_SKILL_COOLDOWN;
	protected float maxRange = Tuner.DEFAULT_SPELL_RANGE;
	protected float currentCooldown;
	protected string spellName = "";
	protected Texture2D skillIcon = null;

	//Jokanen spelli toteuttaa oman casti metodinsa.
	public abstract void cast(Vector2 point,GameObject unit);

	//Ei ollu mitään hajua millä iconit toimii ni laitoin Texture2D :D.
	public Texture2D getSkillIcon (){
		if (skillIcon != null) {
			return skillIcon;
		} else {
			//Ehkä vois palauttaa jonku default iconin jossei oo asetettu vielä.
			return null;
		}
	}

	//Cooldowni toimii niin että lasketaan ylöspäin niin pitkään kunnes tulee max cooldown vastaan.
	//Jos halutaan nähdä montako sekuntia on jäljellä niin käytetään tätä functiota.
	public float getCurrentCooldown (){
		return (maxCooldown - currentCooldown);
	}

	//Getterit muille muuttujille.
	public int getMaxCooldown (){
		return maxCooldown;
	}

	public float getMaxRange (){
		return maxRange;
	}

}

