using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

[XmlRoot("attribute")]
public class unit_attributes
{
    [XmlElement("health")]
    public float health;

    [XmlElement("meleedamage")]
    public float meleedamage;

    [XmlElement("meleeattackspeed")]
    public float meleeattackspeed;

    [XmlElement("rangeddamage")]
    public float rangeddamage;

    [XmlElement("rangedattackspeed")]
    public float rangedattackspeed;

    [XmlElement("movementspeed")]
    public float movementspeed;

    [XmlElement("ismelee")]
    public Boolean isMelee;

    [XmlElement("skillslot1")]
    public String skillslot1;

    [XmlElement("skillslot2")]
    public String skillslot2;

    public Skill skill1;
    public Skill skill2;

    public unit_attributes() { }

    public unit_attributes(String uname)
    {
        Regex regex = new Regex(@"^(Character)#[1-4]");
        Match match = regex.Match(uname);

        String unit_name;

        if (match.Success)
        {
            Debug.Log("FOUND CHARACTER: " + uname);
            unit_name = uname;
        }
        else {
            unit_name = uname.Split((char)32)[0];
            unit_name = unit_name.Replace("(Clone)", "");
        }

        TextAsset _xml = Resources.Load<TextAsset>(unit_name);

        if (_xml == null)
        {
            Debug.LogError("COULD NOT FIND UNIT ATTRIBUTES: " + unit_name);
            loadDefaultValues();
            return;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(unit_attributes));

        StringReader reader = new StringReader(_xml.text);

        unit_attributes items = serializer.Deserialize(reader) as unit_attributes;

        health = items.health;
        meleedamage = items.meleedamage;
        meleeattackspeed = items.meleeattackspeed;
        rangeddamage = items.rangeddamage;
        rangedattackspeed = items.rangedattackspeed;
        skill1 = SkillBook.getSkill(items.skillslot1);
        skill2 = SkillBook.getSkill(items.skillslot2);
        isMelee = items.isMelee;
        movementspeed = items.movementspeed;
        reader.Close();
    }

    public void loadDefaultValues()
    {
        health = Tuner.UNIT_BASE_HEALTH;
        meleedamage = Tuner.UNIT_BASE_MELEE_DAMAGE;
        rangeddamage = Tuner.UNIT_BASE_RANGED_DAMAGE;
        meleeattackspeed = Tuner.UNIT_BASE_MELEE_ATTACK_SPEED;
        rangedattackspeed = Tuner.UNIT_BASE_RANGED_ATTACK_SPEED;
        isMelee = true;
        skill1 = SkillBook.getSkill("empty_skill");
        skill2 = SkillBook.getSkill("empty_skill");
    }
}
