using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Skill : ScriptableObject, ISkill
{

    protected int maxCooldown = Tuner.DEFAULT_SKILL_COOLDOWN;
    protected float maxRange = Tuner.DEFAULT_SPELL_RANGE;
    protected float currentCooldown = Tuner.DEFAULT_SKILL_COOLDOWN;
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

    //Cooldowni toimii niin että lasketaan ylöspäin niin pitkään kunnes tulee max cooldown vastaan.
    //Jos halutaan nähdä montako sekuntia on jäljellä niin käytetään tätä functiota.
    public float getCurrentCooldown()
    {
        return (maxCooldown - currentCooldown);
    }

    //Getterit muille muuttujille.
    public int getMaxCooldown()
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
        GameObject[] hostileList = GameObject.FindGameObjectsWithTag("Hostile");

        foreach (GameObject g in hostileList)
        {

            Vector2 enemyPos = g.transform.position;
            Vector2 ellipsePos = point;
            Vector2 ellipseRadius = new Vector2(radius, (radius * 0.5f));

            float a = Mathf.Pow((enemyPos.x - ellipsePos.x), 2);
            float b = Mathf.Pow((enemyPos.y - ellipsePos.y), 2);
            float rX = Mathf.Pow(ellipseRadius.x, 2);
            float rY = Mathf.Pow(ellipseRadius.y, 2);

            if (((a / rX) + (b / rY)) <= 1)
            {
                mobs.Add(g);
            }
            /*
            if (Vector2.Distance(point, g.transform.position) < radius)
            {
                mobs.Add(g);
            }
            */
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

