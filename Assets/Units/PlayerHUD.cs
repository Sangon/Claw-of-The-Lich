using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayerHUD : MonoBehaviour
{
    private Transform healthBarIndicator = null;
    private Transform staminaBarIndicator = null;
    private GameObject selectionIndicator = null;
    private GameObject weaponPrimaryIndicator = null;
    private GameObject weaponSecondaryIndicator = null;
    private GameObject bar = null;
    private Sprite meleeSprite = null;
    private Sprite rangedSprite = null;
    private float healthBarWidth = 0;
    private float staminaBarWidth = 0;

    private GameObject mouseOverTarget = null;
    public bool mouseOverHUD = false;

    private PartySystem partySystem;
    private UnitCombat unitCombat;

    // Use this for initialization
    void Start()
    {
        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();
        unitCombat = GetComponent<UnitCombat>();

        if (gameObject.name.Equals("Character#1"))
        {
            bar = GameObject.Find("HUDBars").transform.Find("Bar1").gameObject;
            foreach (Transform child in bar.transform)
            {
                child.transform.position += new Vector3(0, 0, 0);
            }
        }
        if (gameObject.name.Equals("Character#2"))
        {
            bar = GameObject.Find("HUDBars").transform.Find("Bar2").gameObject;
            foreach (Transform child in bar.transform)
            {
                child.transform.position += new Vector3(300f, 0, 0);
            }
        }
        if (gameObject.name.Equals("Character#3"))
        {
            bar = GameObject.Find("HUDBars").transform.Find("Bar3").gameObject;
            foreach (Transform child in bar.transform)
            {
                child.transform.position += new Vector3(600f, 0, 0);
            }
        }
        if (gameObject.name.Equals("Character#4"))
        {
            bar = GameObject.Find("HUDBars").transform.Find("Bar4").gameObject;
            foreach (Transform child in bar.transform)
            {
                child.transform.position += new Vector3(900f, 0, 0);
            }
        }
        healthBarIndicator = bar.transform.Find("Bar_HP");
        staminaBarIndicator = bar.transform.Find("Bar_Stamina");
        selectionIndicator = bar.transform.Find("Selection").gameObject;
        weaponPrimaryIndicator = bar.transform.Find("Weapon_Primary").gameObject;
        weaponSecondaryIndicator = bar.transform.Find("Weapon_Secondary").gameObject;

        meleeSprite = Resources.Load<Sprite>("HUD_Weapon_Melee");
        rangedSprite = Resources.Load<Sprite>("HUD_Weapon_Ranged");

        healthBarWidth = healthBarIndicator.localScale.x;
        staminaBarWidth = staminaBarIndicator.localScale.x;
    }

    public void Update()
    {
        mouseOver(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (mouseOverTarget != null && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            mouseOverHUD = true;
            GameObject character = getOwner(mouseOverTarget);
            if (character != null && character == gameObject)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (mouseOverTarget.name.Equals("Weapon_Primary"))
                    {
                        character.GetComponent<UnitCombat>().changeWeapon();
                    }
                    else if (mouseOverTarget.name.Equals("Portrait"))
                    {
                        partySystem.selectCharacter(character, Input.GetKey(KeyCode.LeftShift));
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (mouseOverTarget.name.Equals("Weapon_Primary"))
                    {
                        foreach (GameObject c in partySystem.aliveCharacters)
                        {
                            if (character.GetComponent<UnitCombat>().isMelee() != c.GetComponent<UnitCombat>().isMelee())
                                c.GetComponent<UnitCombat>().changeWeapon();
                        }
                    }
                }
            }
        }
        else if (mouseOverTarget == null)
            mouseOverHUD = false;
        if (unitCombat.isAlive())
        {
            if (partySystem.getGroupID(gameObject) >= 0)
                selectionIndicator.GetComponent<Image>().color = Color.green;
            else
                selectionIndicator.GetComponent<Image>().color = Color.red;
        }
        else
            selectionIndicator.GetComponent<Image>().color = Color.black;

        if (unitCombat.isMelee() && weaponPrimaryIndicator.GetComponent<Image>().sprite == rangedSprite)
            weaponPrimaryIndicator.GetComponent<Image>().sprite = meleeSprite;
        else if (!unitCombat.isMelee() && weaponPrimaryIndicator.GetComponent<Image>().sprite == meleeSprite)
            weaponPrimaryIndicator.GetComponent<Image>().sprite = rangedSprite;
        if (unitCombat.isMelee() && weaponSecondaryIndicator.GetComponent<Image>().sprite == meleeSprite)
            weaponSecondaryIndicator.GetComponent<Image>().sprite = rangedSprite;
        else if (!unitCombat.isMelee() && weaponSecondaryIndicator.GetComponent<Image>().sprite == rangedSprite)
            weaponSecondaryIndicator.GetComponent<Image>().sprite = meleeSprite;
    }

    // Update is called once per frame
    public void updateStats(float healthScale, float staminaScale)
    {
        if (healthBarIndicator == null || staminaBarIndicator == null)
            return;

        Mathf.Clamp(healthScale, 0, 1f);
        Mathf.Clamp(staminaScale, 0, 1f);

        // Set Stamina Bar width to 0 if the character is dead
        if (unitCombat.getHealth() <= 0f)
            staminaScale = 0f;

        healthBarIndicator.localScale = new Vector3(healthBarWidth * healthScale, healthBarIndicator.localScale.y, healthBarIndicator.localScale.z);
        staminaBarIndicator.localScale = new Vector3(staminaBarWidth * staminaScale, staminaBarIndicator.localScale.y, staminaBarIndicator.localScale.z);
    }

    private GameObject getOwner(GameObject HUDElement)
    {
        if (HUDElement == null)
            return null;
        if (HUDElement.transform.parent.name.Equals("Bar1"))
            return partySystem.getCharacter(1);
        else if (HUDElement.transform.parent.name.Equals("Bar2"))
            return partySystem.getCharacter(2);
        else if (HUDElement.transform.parent.name.Equals("Bar3"))
            return partySystem.getCharacter(3);
        else if (HUDElement.transform.parent.name.Equals("Bar4"))
            return partySystem.getCharacter(4);
        return null;
    }

    public void mouseOver(Vector2 ray)
    {
        UnityEngine.EventSystems.PointerEventData pe = new PointerEventData(EventSystem.current);
        pe.position = Input.mousePosition;

        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pe, hits);

        if (mouseOverTarget != null)
        {
            mouseOverTarget.GetComponent<Image>().color = Color.white;
            mouseOverTarget = null;
        }
        if (hits.Count > 0) //if no object was found there is no minimum
        {
            float min = hits[0].distance; //lets assume that the minimum is at the 0th place
            int minIndex = 0; //store the index of the minimum because thats hoow we can find our object

            for (int i = 1; i < hits.Count; ++i)// iterate from the 1st element to the last.(Note that we ignore the 0th element)
            {
                if (hits[i].distance < min) //if we found smaller distance and its not the player we got a new minimum
                {
                    min = hits[i].distance; //refresh the minimum distance value
                    minIndex = i; //refresh the distance
                }
            }
            if (hits[minIndex].gameObject.tag.Equals("UI"))
            {
                mouseOverTarget = hits[minIndex].gameObject;
                GameObject character = getOwner(mouseOverTarget);

                if (character != null && character.GetComponent<UnitCombat>().isAlive())
                {
                    // Highlight the icon
                    mouseOverTarget.GetComponent<Image>().color = Color.green;
                }
                else
                {
                    mouseOverTarget = null;
                }
            }
        }
    }
}
