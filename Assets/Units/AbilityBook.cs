using System;
using UnityEngine;
using System.Collections;

public class AbilityBook
{
    public AbilityBook()
    {
    }

    public static Ability getAbility(String name, GameObject parent)
    {
        switch (name)
        {
            case "ArrowRain":
                return ScriptableObject.CreateInstance<Ability_ArrowRain>().init(parent);
            case "Charge":
                return ScriptableObject.CreateInstance<Ability_Charge>().init(parent);
            case "Whirlwind":
                return ScriptableObject.CreateInstance<Ability_Whirlwind>().init(parent);
        }

        return ScriptableObject.CreateInstance<Ability_Placeholder>().init(parent);
    }
}
