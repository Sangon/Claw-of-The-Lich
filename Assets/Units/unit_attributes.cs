using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

[XmlRoot("attribute")]
public class unit_attributes{

    [XmlElement("health")]
    public float health;

    [XmlElement("movementspeed")]
    public float speed;

    [XmlElement("ismelee")]
    public Boolean isMelee;

    [XmlElement("skillslot1")]
    public String skillslot1;

    [XmlElement("skillslot2")]
    public String skillslot2;

    public Skill skill1;
    public Skill skill2;



    public unit_attributes(){}

    public unit_attributes(String uname)
    {

        Regex regex = new Regex(@"^(Character)#[1-4]");
        Match match = regex.Match(uname);

        String unit_name;

        if (match.Success){
            Debug.Log("FOUND CHARACTER: " + uname);
            unit_name = uname;
        }else{
            unit_name = uname.Split((char) 32)[0];
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
        skill1 = SkillBook.getSkill(items.skillslot1);
        skill2 = SkillBook.getSkill(items.skillslot2);
        isMelee = items.isMelee;
        speed = items.speed;
        reader.Close();

    }

    public void loadDefaultValues(){

        health = Tuner.UNIT_BASE_HEALTH;
        isMelee = true;
        skill1 = SkillBook.getSkill("empty_skill");
        skill2 = SkillBook.getSkill("empty_skill");

    }
}
