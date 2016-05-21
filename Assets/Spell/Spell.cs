using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spell : MonoBehaviour
{
    public Vector2 castLocation;
    public int spellID;
    public string spellName;
    private GameObject parent;

    public void destroy()
    {
        Destroy(gameObject);
    }

    public void setParent(GameObject g)
    {
        parent = g;
    }

    public GameObject getParent()
    {
        return parent;
    }

    public List<GameObject> getUnitsAtPoint(Vector2 point, float radius)
    {
        List<GameObject> mobs = null;

        if (parent.tag.Equals("Player"))
        {
            mobs = UnitList.getHostileUnitsInArea(point, radius);
        } else
        {
            mobs = UnitList.getPlayerUnitsInArea(point, radius);
        }

        return mobs;
    }
}
