using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PlayerHUD : MonoBehaviour
{
    private Sprite meleeSprite;
    private Sprite rangedSprite;
    private float healthBarWidth = 0;
    private float staminaBarWidth = 0;

    private GameObject mouseOverTarget;
    private bool mouseOverHUD = false;

    private PartySystem partySystem;
    private GameHUD gameHUD;

    public bool isMouseOverHUD()
    {
        return mouseOverHUD;
    }

    // Use this for initialization
    void Start()
    {
        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();
        gameHUD = GameObject.Find("HUD").GetComponent<GameHUD>();

        GameObject bar = transform.Find("HUDBars").Find("Bar1").gameObject;
        healthBarWidth = bar.transform.Find("Bar_HP").localScale.x;
        staminaBarWidth = bar.transform.Find("Bar_Stamina").localScale.x;

        StartCoroutine(LateStart(0.01f));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        GameObject bar = null;
        for (int i = 1; i <= 4; i++)
        {
            bar = transform.Find("HUDBars").Find("Bar" + i).gameObject;
            foreach (Transform child in bar.transform)
            {
                child.transform.position += new Vector3((i - 1) * 300f, 0, 0);
            }
        }

        meleeSprite = Resources.Load<Sprite>("HUD_Weapon_Melee");
        rangedSprite = Resources.Load<Sprite>("HUD_Weapon_Ranged");
    }

    public void LateUpdate()
    {
        resetTarget();
        if (!gameHUD.isTargeting())
            mouseOver(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (mouseOverTarget != null)
            mouseOverHUD = true;

        if (mouseOverTarget != null && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !gameHUD.isTargeting() && !partySystem.isMouseOverCharacter())
        {
            GameObject character = getOwner(mouseOverTarget);
            if (character != null)
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

        for (int i = 1; i <= 4; i++)
        {
            GameObject character = partySystem.getCharacter(i);
            UnitCombat unitCombat = character.GetComponent<UnitCombat>();
            GameObject bar = transform.Find("HUDBars").Find("Bar" + i).gameObject;

            Transform healthBarIndicator = bar.transform.Find("Bar_HP");
            Transform staminaBarIndicator = bar.transform.Find("Bar_Stamina");
            GameObject selectionIndicator = bar.transform.Find("Selection").gameObject;
            GameObject weaponPrimaryIndicator = bar.transform.Find("Weapon_Primary").gameObject;
            GameObject weaponSecondaryIndicator = bar.transform.Find("Weapon_Secondary").gameObject;

            float healthScale = (unitCombat.getHealth() / unitCombat.getMaxHealth());
            float staminaScale = (unitCombat.getHealth() / unitCombat.getMaxHealth());

            Mathf.Clamp(healthScale, 0, 1f);
            Mathf.Clamp(staminaScale, 0, 1f);

            // Set Stamina Bar width to 0 if the character is dead
            if (!unitCombat.isAlive())
                staminaScale = 0f;

            healthBarIndicator.localScale = new Vector3(healthBarWidth * healthScale, healthBarIndicator.localScale.y, healthBarIndicator.localScale.z);
            staminaBarIndicator.localScale = new Vector3(staminaBarWidth * staminaScale, staminaBarIndicator.localScale.y, staminaBarIndicator.localScale.z);

            if (unitCombat.isAlive())
            {
                if (partySystem.getGroupID(character) >= 0)
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
    }

    private void resetTarget()
    {
        if (mouseOverTarget != null)
        {
            mouseOverTarget.GetComponent<Image>().color = Color.white;
            mouseOverTarget = null;
        }
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
