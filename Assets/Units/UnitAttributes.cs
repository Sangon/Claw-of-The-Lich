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
public class UnitAttributes
{
    [XmlElement("health")]
    public float health;

    [XmlElement("damage_melee_dices")]
    public int damage_melee_dices;

    [XmlElement("damage_melee_sides")]
    public int damage_melee_sides;

    [XmlElement("damage_ranged_dices")]
    public int damage_ranged_dices;

    [XmlElement("damage_ranged_sides")]
    public int damage_ranged_sides;

    [XmlElement("attackspeed_melee")]
    public float attackspeed_melee;

    [XmlElement("attackspeed_ranged")]
    public float attackspeed_ranged;

    [XmlElement("ranged_projectile")]
    public string ranged_projectile;

    [XmlElement("ability1")]
    public String ability1;

    [XmlElement("ability2")]
    public String ability2;

    [XmlElement("ismelee")]
    public Boolean ismelee;

    [XmlElement("movementspeed")]
    public float movementspeed;

    public Ability abilitySlot1;
    public Ability abilitySlot2;

    public UnitAttributes() { }

    public UnitAttributes(String uname)
    {
        Regex regex = new Regex(@"^(Character)#[1-4]");
        Match match = regex.Match(uname);

        String unit_name;

        if (match.Success)
        {
            Debug.Log("FOUND CHARACTER: " + uname);
            unit_name = uname;
        }
        else
        {
            regex = new Regex("[^a-zA-Z_]+|(Enemy_)|(Clone)");
            unit_name = regex.Replace(uname, string.Empty);
        }

        TextAsset _xml = Resources.Load<TextAsset>(unit_name);

        if (_xml == null)
        {
            Debug.LogError("COULD NOT FIND UNIT ATTRIBUTES: " + unit_name);
            loadDefaultValues();
            return;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(UnitAttributes));

        StringReader reader = new StringReader(_xml.text);

        UnitAttributes unitAttributes = serializer.Deserialize(reader) as UnitAttributes;

        health = unitAttributes.health;
        damage_melee_dices = unitAttributes.damage_melee_dices;
        damage_melee_sides = unitAttributes.damage_melee_sides;
        damage_ranged_dices = unitAttributes.damage_ranged_dices;
        damage_ranged_sides = unitAttributes.damage_ranged_sides;
        attackspeed_melee = unitAttributes.attackspeed_melee;
        attackspeed_ranged = unitAttributes.attackspeed_ranged;
        ranged_projectile = unitAttributes.ranged_projectile;
        abilitySlot1 = AbilityBook.getAbility(unitAttributes.ability1);
        abilitySlot2 = AbilityBook.getAbility(unitAttributes.ability2);
        ismelee = unitAttributes.ismelee;
        movementspeed = unitAttributes.movementspeed;
        reader.Close();
    }

    public void loadDefaultValues()
    {
        health = Tuner.UNIT_BASE_HEALTH;
        damage_melee_dices = Tuner.UNIT_BASE_DAMAGE_MELEE_DICES;
        damage_melee_sides = Tuner.UNIT_BASE_DAMAGE_MELEE_SIDES;
        damage_ranged_dices = Tuner.UNIT_BASE_DAMAGE_RANGED_DICES;
        damage_ranged_sides = Tuner.UNIT_BASE_DAMAGE_RANGED_SIDES;
        attackspeed_melee = Tuner.UNIT_BASE_ATTACK_SPEED_MELEE;
        attackspeed_ranged = Tuner.UNIT_BASE_ATTACK_SPEED_RANGED;
        ranged_projectile = "Arrow";
        ismelee = true;
        abilitySlot1 = AbilityBook.getAbility("Placeholder");
        abilitySlot2 = AbilityBook.getAbility("Placeholder");
    }
}
