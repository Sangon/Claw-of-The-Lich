using UnityEngine;
using System.Collections;

public class empty_skill : Skill{

    public empty_skill()
    {
        spellName = "empty_skill";
        skillIcon = null;
        maxCooldown = 0;
    }
    public override void cast(GameObject owner){}

    public override void FixedUpdate(){}
}
