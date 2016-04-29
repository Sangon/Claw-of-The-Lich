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

    public Vector2 getCurrentMousePos()
    {
        Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);
        return hit.point;
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
}
