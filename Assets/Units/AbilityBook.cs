using System;
using UnityEngine;
using System.Collections;

public class AbilityBook
{
    public AbilityBook()
    {
    }

    public static Ability getAbility(String name)
    {
        switch (name)
        {
            case "ArrowRain":
                return ScriptableObject.CreateInstance<Ability_ArrowRain>();
            case "Charge":
                return ScriptableObject.CreateInstance<Ability_Charge>();
            case "Whirlwind":
                return ScriptableObject.CreateInstance<Ability_Whirlwind>();
        }

        return ScriptableObject.CreateInstance<Ability_Placeholder>();
    }
}
