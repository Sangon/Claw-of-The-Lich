using System;
using UnityEngine;
using System.Collections;

public class SkillBook{

    public SkillBook(){
    }

    public static Skill getSkill(String name){

        switch (name){
            case "whirlwind_skill":
               return ScriptableObject.CreateInstance("whirlwind_skill") as whirlwind_skill;
            case "charge_skill":
                return ScriptableObject.CreateInstance("charge_skill") as charge_skill;
            case "blot_out_skill":
                return ScriptableObject.CreateInstance("blot_out_skill") as blot_out_skill;
        }

        return ScriptableObject.CreateInstance("empty_skill") as empty_skill;

    }
}
