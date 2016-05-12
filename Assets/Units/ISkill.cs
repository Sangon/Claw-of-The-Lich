using UnityEngine;

interface ISkill
{
    Texture2D getSkillIcon();
    float getCurrentCooldown();
    float getMaxCooldown();
    float getMaxRange();
}

