using UnityEngine;
using System.Collections;

public class UnitList : MonoBehaviour
{
    private static GameObject[] hostiles;
    private static GameObject[] players;
    private static GameObject[] dead;
    private static GameObject[] traps;
    private static GameObject[] allUnits;

    private static int unitsCreated;

    public static GameObject[] getHostiles()
    {
        return hostiles;
    }
    public static GameObject[] getPlayers()
    {
        return players;
    }
    public static GameObject[] getDead()
    {
        return dead;
    }
    public static GameObject[] getTraps()
    {
        return traps;
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

    void Awake()
    {
        createArrays();
    }

    private void createArrays()
    {
        hostiles = GameObject.FindGameObjectsWithTag("Hostile");
        players = GameObject.FindGameObjectsWithTag("Player");
        dead = GameObject.FindGameObjectsWithTag("Dead");
        traps = GameObject.FindGameObjectsWithTag("Trap");
        allUnits = CombineExtension.CreateCombinedArrayFrom(hostiles, players);
        CombineExtension.AppendSecondArrayToFirst(ref allUnits, dead);
    }

    void FixedUpdate()
    {
        createArrays();
    }
}
