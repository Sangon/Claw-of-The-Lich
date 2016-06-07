using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitList : MonoBehaviour
{
    private static GameObject[] allUnits; //All dead and alive units
    private static GameObject[] aliveUnits; //All alive units
    private static GameObject[] deadUnits; //All dead units
    private static GameObject[] hostiles;
    private static GameObject[] players;
    //private static GameObject[] traps;

    public enum searchType
    {
        all,
        alive,
        dead,
        hostiles,
        players
    }

    //All units that are dear or alive and inside the rects
    private static List<GameObject>[] dividedAllUnits = new List<GameObject>[Tuner.LEVEL_AREA_DIVISIONS];
    //All units that are alive and inside the rects
    private static List<GameObject>[] dividedAliveUnits = new List<GameObject>[Tuner.LEVEL_AREA_DIVISIONS];
    //All units that are dead and inside the rects
    private static List<GameObject>[] dividedDeadUnits = new List<GameObject>[Tuner.LEVEL_AREA_DIVISIONS];
    //Hostile units that are alive and inside the rects
    private static List<GameObject>[] dividedHostiles = new List<GameObject>[Tuner.LEVEL_AREA_DIVISIONS];
    //Player units that are alive and inside the rects
    private static List<GameObject>[] dividedPlayers = new List<GameObject>[Tuner.LEVEL_AREA_DIVISIONS];
    //Triggers that are inside the rects
    private static List<GameObject>[] dividedTriggers = new List<GameObject>[Tuner.LEVEL_AREA_DIVISIONS];
    //Traps that are inside the rects
    private static List<GameObject>[] dividedTraps = new List<GameObject>[Tuner.LEVEL_AREA_DIVISIONS];

    private static Color[] colors = new Color[Tuner.LEVEL_AREA_DIVISIONS];

    private static Rect[] rects = new Rect[Tuner.LEVEL_AREA_DIVISIONS];

    private static int unitsCreated;

    public static GameObject[] getHostiles()
    {
        return hostiles;
    }

    public static GameObject[] getPlayers()
    {
        return players;
    }

    public static GameObject[] getDeadUnits()
    {
        return deadUnits;
    }

    public static GameObject[] getAliveUnits()
    {
        return aliveUnits;
    }
    public static GameObject[] getAllUnits()
    {
        return allUnits;
    }

    public static int getUnitsCreated()
    {
        return unitsCreated;
    }

    public static int createUnit()
    {
        unitsCreated++;
        return unitsCreated;
    }

    public static List<GameObject> getAllUnitsInArea(Vector2 point, float radius)
    {
        return getUnitsInArea(point, radius, searchType.all);
    }

    public static List<GameObject> getAliveUnitsInArea(Vector2 point, float radius)
    {
        return getUnitsInArea(point, radius, searchType.alive);
    }

    public static List<GameObject> getDeadUnitsInArea(Vector2 point, float radius)
    {
        return getUnitsInArea(point, radius, searchType.dead);
    }

    public static List<GameObject> getHostileUnitsInArea(Vector2 point, float radius)
    {
        return getUnitsInArea(point, radius, searchType.hostiles);
    }

    public static List<GameObject> getPlayerUnitsInArea(Vector2 point, float radius)
    {
        return getUnitsInArea(point, radius, searchType.players);
    }


    private static List<GameObject> getUnitsInArea(Vector2 point, float radius, searchType types)
    {
        List<GameObject> units = new List<GameObject>();
        List<GameObject> possibleUnits = new List<GameObject>();
        List<GameObject>[] division = null;
        switch (types)
        {
            case searchType.all:
                division = dividedAllUnits;
                break;
            case searchType.alive:
                division = dividedAliveUnits;
                break;
            case searchType.dead:
                division = dividedDeadUnits;
                break;
            case searchType.hostiles:
                division = dividedHostiles;
                break;
            case searchType.players:
                division = dividedPlayers;
                break;
        }

        for (int i = 0; i < Tuner.LEVEL_AREA_DIVISIONS; i++)
        {
            Rect ellipseRect = new Rect(point.x - radius, point.y + (radius * 0.5f), (radius * 2f), -radius);
            if (rects[i].Overlaps(ellipseRect, true))
            {
                possibleUnits.AddRange(division[i]);
            }
        }

        //print("Possibleunits: " + possibleUnits.Count);

        foreach (GameObject unit in possibleUnits)
        {
            if (Ellipse.isometricDistance(unit.transform.position, point) <= radius)
            {
                units.Add(unit);
            }
        }

        return units;
    }

    void Awake()
    {
        colors[0] = Color.black;
        colors[1] = Color.blue;
        colors[2] = Color.cyan;
        colors[3] = Color.gray;
        colors[4] = Color.green;
        colors[5] = Color.magenta;
        colors[6] = Color.red;
        colors[7] = Color.white;
        colors[8] = Color.yellow;
        for (int i = 0; i < Tuner.LEVEL_AREA_DIVISIONS; i++)
        {
            dividedAllUnits[i] = new List<GameObject>();
            dividedAliveUnits[i] = new List<GameObject>();
            dividedDeadUnits[i] = new List<GameObject>();
            dividedHostiles[i] = new List<GameObject>();
            dividedPlayers[i] = new List<GameObject>();
            dividedTriggers[i] = new List<GameObject>();
            dividedTraps[i] = new List<GameObject>();
            if (i > 8)
            {
                colors[i] = colors[i - 9];
            }
        }

        GameObject[] traps = GameObject.FindGameObjectsWithTag("Trap");

        GameObject[] triggers = GameObject.FindGameObjectsWithTag("Trigger");

        for (int i = 0; i < Tuner.LEVEL_AREA_DIVISIONS; i++)
        {
            float x = (Tuner.LEVEL_WIDTH_IN_WORLD_UNITS / Tuner.LEVEL_AREA_DIVISIONS_WIDTH);
            float y = (Tuner.LEVEL_HEIGHT_IN_WORLD_UNITS / Tuner.LEVEL_AREA_DIVISIONS_HEIGHT);
            int xTimes = i % Tuner.LEVEL_AREA_DIVISIONS_WIDTH;
            int yTimes = i / Tuner.LEVEL_AREA_DIVISIONS_HEIGHT;

            rects[i] = new Rect((xTimes * x), (yTimes * -y), x, -y);
            Debug.DrawLine(new Vector3(rects[i].x, rects[i].y), new Vector3(rects[i].x + x, rects[i].y), colors[i], 3600f);
            Debug.DrawLine(new Vector3(rects[i].x, rects[i].y), new Vector3(rects[i].x, rects[i].y - y), colors[i], 3600f);
            Debug.DrawLine(new Vector3(rects[i].x + x, rects[i].y), new Vector3(rects[i].x + x, rects[i].y - y), colors[i], 3600f);
            Debug.DrawLine(new Vector3(rects[i].x, rects[i].y - y), new Vector3(rects[i].x + x, rects[i].y - y), colors[i], 3600f);

            foreach (GameObject trigger in triggers)
            {
                Rect triggerRect = new Rect();
                Bounds triggerBounds = trigger.GetComponent<PolygonCollider2D>().bounds;

                triggerRect.xMax = triggerBounds.max.x;
                triggerRect.yMax = triggerBounds.max.y;
                triggerRect.xMin = triggerBounds.min.x;
                triggerRect.yMin = triggerBounds.min.y;

                if (rects[i].Overlaps(triggerRect, true))
                {
                    //Trigger is inside a rect
                    dividedTriggers[i].Add(trigger);
                }
            }
            foreach (GameObject trap in traps)
            {
                Rect triggerRect = new Rect();
                Trap trapScript = trap.GetComponent<Trap>();

                triggerRect.xMax = trap.transform.position.x + trapScript.triggerDistance * 0.5f;
                triggerRect.yMax = trap.transform.position.y - trapScript.triggerDistance * 0.25f;
                triggerRect.xMin = trap.transform.position.x - trapScript.triggerDistance * 0.5f;
                triggerRect.yMin = trap.transform.position.y + trapScript.triggerDistance * 0.25f;

                if (rects[i].Overlaps(triggerRect, true))
                {
                    //Trap is inside a rect
                    dividedTraps[i].Add(trap);
                }
            }
        }
        updateArrays();
        updateRectLists();
    }

    public static void updateArrays()
    {
        hostiles = GameObject.FindGameObjectsWithTag("Hostile");
        players = GameObject.FindGameObjectsWithTag("Player");
        aliveUnits = CombineExtension.CreateCombinedArrayFrom(hostiles, players);
        deadUnits = GameObject.FindGameObjectsWithTag("Dead");
        allUnits = CombineExtension.CreateCombinedArrayFrom(aliveUnits, deadUnits);
        /*
        print("AllUnits: " + allUnits.Length);
        print("AliveUnits: " + aliveUnits.Length);
        print("DeadUnits: " + deadUnits.Length);
        print("Hostiles: " + hostiles.Length);
        print("Players: " + players.Length);
        */
    }

    private void updateRectLists()
    {
        List<GameObject> allUnitsCopy = new List<GameObject>(allUnits);

        //Loop through all units and put them into divisions
        for (int i = 0; i < Tuner.LEVEL_AREA_DIVISIONS; i++)
        {
            dividedAllUnits[i].Clear();
            dividedAliveUnits[i].Clear();
            dividedDeadUnits[i].Clear();
            dividedHostiles[i].Clear();
            dividedPlayers[i].Clear();

            for (int j = allUnitsCopy.Count - 1; j >= 0; j--)
            {
                if (rects[i].Contains(new Vector2(allUnitsCopy[j].transform.position.x, allUnitsCopy[j].transform.position.y), true))
                {
                    //Unit is inside a rect
                    bool alive = true;
                    dividedAllUnits[i].Add(allUnitsCopy[j]);
                    if (allUnitsCopy[j].tag.Equals("Hostile"))
                    {
                        //Unit is a hostile
                        dividedHostiles[i].Add(allUnitsCopy[j]);
                    }
                    else if (allUnitsCopy[j].tag.Equals("Player"))
                    {
                        //Unit is a player
                        dividedPlayers[i].Add(allUnitsCopy[j]);
                    }
                    else if (allUnitsCopy[j].tag.Equals("Dead"))
                    {
                        //Unit is dead
                        dividedDeadUnits[i].Add(allUnitsCopy[j]);
                        alive = false;
                    }

                    if (alive) //Unit is alive
                        dividedAliveUnits[i].Add(allUnitsCopy[j]);

                    //A unit can't be in two divisions at once: exclude it from future iterations
                    allUnitsCopy.RemoveAt(j);
                }
                /*
                else if (dividedUnits[i].Contains(aliveUnits[j]))
                {
                    //Unit is no longer in this division: remove it
                    dividedUnits[i].Remove(aliveUnits[j]);
                }
                else if (aliveUnits[j].tag.Equals("Hostile") && dividedHostiles[i].Contains(aliveUnits[j]))
                {
                    //Unit is a hostile
                    dividedHostiles[i].Remove(aliveUnits[j]);
                }
                else if (aliveUnits[j].tag.Equals("Player") && dividedPlayers[i].Contains(aliveUnits[j]))
                {
                    //Unit is a player
                    dividedPlayers[i].Remove(aliveUnits[j]);
                }
                */
            }
        }
    }

    void checkTriggerCollisions()
    {
        //traps.checkTrigger(transform.position, transform.tag);
        for (int i = 0; i < Tuner.LEVEL_AREA_DIVISIONS; i++)
        {
            foreach (GameObject trigger in dividedTriggers[i])
            {
                foreach (GameObject unit in dividedAllUnits[i])
                {
                    if (trigger.GetComponent<PolygonCollider2D>().OverlapPoint(unit.transform.position))
                    {
                        trigger.GetComponent<HouseTransparency>().trigger(unit);
                    }
                }
            }
            foreach (GameObject trap in dividedTraps[i])
            {
                foreach (GameObject unit in dividedAliveUnits[i])
                {
                    trap.GetComponent<Trap>().checkTrigger(unit);
                }
            }
        }
    }

    void FixedUpdate()
    {
        updateRectLists();
        checkTriggerCollisions();
    }
}
