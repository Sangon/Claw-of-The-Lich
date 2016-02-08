using UnityEngine;

interface ISkill{
	
	Texture2D getSkillIcon();
	float getCurrentCooldown();
	int getMaxCooldown();
	float getMaxRange();

}

