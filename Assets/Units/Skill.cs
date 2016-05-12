using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Skill : ScriptableObject, ISkill
{
    protected float maxCooldown = Tuner.DEFAULT_SKILL_COOLDOWN;
    protected float maxRange = Tuner.DEFAULT_SPELL_RANGE;
    protected float currentCooldown = 0;
    protected float castTime = Tuner.DEFAULT_SKILL_CAST_TIME;
    protected string spellName = "";
    protected Texture2D skillIcon = null;

    //Jokanen spelli toteuttaa oman casti ja metodinsa.
    public abstract void cast(GameObject owner);
    public abstract void FixedUpdate();

    //Ei ollu mitään hajua millä iconit toimii ni laitoin Texture2D :D.
    public Texture2D getSkillIcon()
    {
        if (skillIcon != null)
        {
            return skillIcon;
        }
        else {
            //Ehkä vois palauttaa jonku default iconin jossei oo asetettu vielä.
            return null;
        }
    }

    public float getCurrentCooldown()
    {
        return currentCooldown;
    }

    public bool isOnCooldown()
    {
        if (currentCooldown > 0)
            return true;
        return false;
    }

    //Getterit muille muuttujille.
    public float getMaxCooldown()
    {
        return maxCooldown;
    }

    public float getMaxRange()
    {
        return maxRange;
    }

    public List<GameObject> getUnitsAtPoint(Vector2 point, float radius)
    {
        List<GameObject> mobs = new List<GameObject>();

        foreach (GameObject g in UnitList.getHostiles())
        {
            if (Ellipse.isometricDistance(point, g.transform.position) < radius)
            {
                mobs.Add(g);
            }
        }

        return mobs;
    }

    public Vector2 getCurrentMousePos()
    {
        Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);
        return hit.point;
    }

    public string getSpellName()
    {
        return spellName;
    }
}

