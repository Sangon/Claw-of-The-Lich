using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    private Transform healthBarIndicator = null;
    private Transform staminaBarIndicator = null;
    private GameObject selectionIndicator = null;
    private GameObject weaponIndicator = null;
    private GameObject bar = null;
    private Sprite meleeSprite = null;
    private Sprite rangedSprite = null;
    private float healthBarWidth = 0;
    private float staminaBarWidth = 0;

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
                child.transform.position += new Vector3(350f, 0, 0);
            }
        }
        if (gameObject.name.Equals("Character#3"))
        {
            bar = GameObject.Find("HUDBars").transform.Find("Bar3").gameObject;
            foreach (Transform child in bar.transform)
            {
                child.transform.position += new Vector3(700f, 0, 0);
            }
        }
        if (gameObject.name.Equals("Character#4"))
        {
            bar = GameObject.Find("HUDBars").transform.Find("Bar4").gameObject;
            foreach (Transform child in bar.transform)
            {
                child.transform.position += new Vector3(1050f, 0, 0);
            }
        }
        healthBarIndicator = bar.transform.Find("Bar_HP");
        staminaBarIndicator = bar.transform.Find("Bar_Stamina");
        selectionIndicator = bar.transform.Find("Selection").gameObject;
        weaponIndicator = bar.transform.Find("Weapon").gameObject;

        meleeSprite = Resources.Load<Sprite>("HUD_Weapon_Melee");
        rangedSprite = Resources.Load<Sprite>("HUD_Weapon_Ranged");

        healthBarWidth = healthBarIndicator.localScale.x;
        staminaBarWidth = staminaBarIndicator.localScale.x;
    }

    public void Update()
    {
        if (partySystem.getGroupID(gameObject) >= 0)
            selectionIndicator.GetComponent<Image>().color = Color.green;
        else
            selectionIndicator.GetComponent<Image>().color = Color.red;
        if (unitCombat.isMelee && weaponIndicator.GetComponent<Image>().sprite == rangedSprite)
            weaponIndicator.GetComponent<Image>().sprite = meleeSprite;
        else if (!unitCombat.isMelee && weaponIndicator.GetComponent<Image>().sprite == meleeSprite)
            weaponIndicator.GetComponent<Image>().sprite = rangedSprite;
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
}
