using UnityEngine;
using System.Collections;

public class GameHUD : MonoBehaviour
{
    private CameraScripts cameraScripts;
    private PartySystem partySystem;
    private PlayerHUD playerHUD;
    private TargetedAbilityIndicator targetedAbilityIndicator;

    private bool[] targeting = new bool[8];
    private bool[] wasTargeting = new bool[8];
    private bool[] HUDCast = new bool[8];
    private bool[] wasHUDCasting = new bool[8];
    bool shift;
    public bool test = false;
    
    public bool isTargeting()
    {
        for (int i = 0; i < 8; i++)
            if (wasTargeting[i] || targeting[i])
                return true;
        return false;
    }

    public bool isTargetingFromHUD()
    {
        for (int i = 0; i < 8; i++)
            if (wasHUDCasting[i] || HUDCast[i])
                return true;
        return false;
    }

    public void setHUDCast(int index, bool value)
    {
        wasHUDCasting[index] = HUDCast[index];
        HUDCast[index] = value;
    }

    // Use this for initialization
    void Start()
    {
        cameraScripts = Camera.main.GetComponent<CameraScripts>();
        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();
        playerHUD = GameObject.Find("HUD").GetComponent<PlayerHUD>();
        targetedAbilityIndicator = GameObject.Find("HUD").GetComponent<TargetedAbilityIndicator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            cameraScripts.toggleLock();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            HealthBar[] healthBars = FindObjectsOfType(typeof(HealthBar)) as HealthBar[];
            foreach (HealthBar bar in healthBars)
            {
                bar.toggleVisibility();
            }
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (GameObject c in partySystem.aliveCharacters)
            {
                if (c != null && c.GetComponent<UnitCombat>() != null)
                    c.GetComponent<UnitCombat>().takeDamage(-c.GetComponent<UnitCombat>().getMaxHealth(), null);
            }
        }

        //////////////////////////////////////
        /// SPELLIT
        /////////////////////////////////////
        for (int i = 0; i < 8; i++)
            wasTargeting[i] = targeting[i];

        GameObject character = null;

        if ((Input.GetKeyDown(KeyCode.Q) || HUDCast[0] == true) && !Input.GetKey(KeyCode.LeftShift) && partySystem.getCharacter(1).GetComponent<UnitCombat>().isAlive())
        {
            targeting[0] = true;
            shift = false;
        }
        if (targeting[0] && (Input.GetMouseButtonDown(1) || !Input.GetKey(KeyCode.Q)) && HUDCast[0] == false)
        {
            if (!Input.GetKey(KeyCode.Q))
            {
                character = partySystem.getCharacter(1);
                character.GetComponent<UnitCombat>().castSpellInSlot(0);
                character.GetComponent<UnitCombat>().stopAttack();
                character.GetComponent<UnitMovement>().stop();
                print("TODO: Fix whirlwind");
            }
            targeting[0] = false;
            shift = false;
        }

        if ((Input.GetKeyDown(KeyCode.W) || HUDCast[1] == true) && !Input.GetKey(KeyCode.LeftShift) && partySystem.getCharacter(1).GetComponent<UnitCombat>().isAlive())
        {
            targeting[1] = true;
            shift = false;
        }
        if (targeting[1] && (Input.GetMouseButtonDown(1) || !Input.GetKey(KeyCode.W)) && HUDCast[1] == false)
        {
            if (!Input.GetKey(KeyCode.W))
            {
                character = partySystem.getCharacter(1);
                character.GetComponent<UnitCombat>().castSpellInSlot(1);
                character.GetComponent<UnitCombat>().stopAttack();
                character.GetComponent<UnitMovement>().stop();
            }
            targeting[1] = false;
            shift = false;
        }

        if ((Input.GetKeyDown(KeyCode.E) || HUDCast[2] == true) && !Input.GetKey(KeyCode.LeftShift) && partySystem.getCharacter(2).GetComponent<UnitCombat>().isAlive())
        {
            targeting[2] = true;
            shift = false;
        }
        if (targeting[2] && (Input.GetMouseButtonDown(1) || !Input.GetKey(KeyCode.E)) && HUDCast[2] == false)
        {
            if (!Input.GetKey(KeyCode.E))
            {
                character = partySystem.getCharacter(2);
                character.GetComponent<UnitCombat>().castSpellInSlot(0);
                character.GetComponent<UnitCombat>().stopAttack();
                character.GetComponent<UnitMovement>().stop();
                print("TODO: Fix whirlwind");
            }
            targeting[2] = false;
            shift = false;
        }

        if ((Input.GetKeyDown(KeyCode.R) || HUDCast[3] == true) && !Input.GetKey(KeyCode.LeftShift) && partySystem.getCharacter(2).GetComponent<UnitCombat>().isAlive())
        {
            targeting[3] = true;
            shift = false;
        }
        if (targeting[3] && (Input.GetMouseButtonDown(1) || !Input.GetKey(KeyCode.R)) && HUDCast[3] == false)
        {
            if (!Input.GetKey(KeyCode.R))
            {
                character = partySystem.getCharacter(2);
                character.GetComponent<UnitCombat>().castSpellInSlot(1);
                character.GetComponent<UnitCombat>().stopAttack();
                character.GetComponent<UnitMovement>().stop();
            }
            targeting[3] = false;
            shift = false;
        }

        if ((Input.GetKeyDown(KeyCode.A) || HUDCast[4] == true) && partySystem.getCharacter(3).GetComponent<UnitCombat>().isAlive())
        {
            targeting[4] = true;
            shift = false;
        }
        if (Input.GetKeyDown(KeyCode.Q) && Input.GetKey(KeyCode.LeftShift) && partySystem.getCharacter(3).GetComponent<UnitCombat>().isAlive())
        {
            targeting[4] = true;
            shift = true;
        }
        if (targeting[4] && (Input.GetMouseButtonDown(1) || (!shift && !Input.GetKey(KeyCode.A)) || (shift && !Input.GetKey(KeyCode.Q))) && HUDCast[4] == false)
        {
            if (!Input.GetKey(KeyCode.A) || (shift && !Input.GetKey(KeyCode.Q)))
            {
                character = partySystem.getCharacter(3);
                character.GetComponent<UnitCombat>().castSpellInSlot(0);
                character.GetComponent<UnitCombat>().stopAttack();
                character.GetComponent<UnitMovement>().stop();
            }
            targeting[4] = false;
            shift = false;
        }

        if ((Input.GetKeyDown(KeyCode.S) || HUDCast[5] == true) && partySystem.getCharacter(3).GetComponent<UnitCombat>().isAlive())
        {
            targeting[5] = true;
            shift = false;
        }
        if (Input.GetKeyDown(KeyCode.W) && Input.GetKey(KeyCode.LeftShift) && partySystem.getCharacter(3).GetComponent<UnitCombat>().isAlive())
        {
            targeting[5] = true;
            shift = true;
        }
        if (targeting[5] && (Input.GetMouseButtonDown(1) || (!shift && !Input.GetKey(KeyCode.S)) || (shift && !Input.GetKey(KeyCode.W))) && HUDCast[5] == false)
        {
            if (!Input.GetKey(KeyCode.S) || (shift && !Input.GetKey(KeyCode.W)))
            {
                character = partySystem.getCharacter(3);
                character.GetComponent<UnitCombat>().castSpellInSlot(1);
                character.GetComponent<UnitCombat>().stopAttack();
                character.GetComponent<UnitMovement>().stop();
            }
            targeting[5] = false;
            shift = false;
        }

        if ((Input.GetKeyDown(KeyCode.D) || HUDCast[6] == true) && partySystem.getCharacter(4).GetComponent<UnitCombat>().isAlive())
        {
            targeting[6] = true;
            shift = false;
        }
        if (Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.LeftShift) && partySystem.getCharacter(4).GetComponent<UnitCombat>().isAlive())
        {
            targeting[6] = true;
            shift = true;
        }
        if (targeting[6] && (Input.GetMouseButtonDown(1) || (!shift && !Input.GetKey(KeyCode.D)) || (shift && !Input.GetKey(KeyCode.E))) && HUDCast[6] == false)
        {
            if (!Input.GetKey(KeyCode.D) || (shift && !Input.GetKey(KeyCode.E)))
            {
                character = partySystem.getCharacter(4);
                character.GetComponent<UnitCombat>().castSpellInSlot(0);
                character.GetComponent<UnitCombat>().stopAttack();
                character.GetComponent<UnitMovement>().stop();
            }
            targeting[6] = false;
            shift = false;
        }

        if ((Input.GetKeyDown(KeyCode.F) || HUDCast[7] == true) && partySystem.getCharacter(4).GetComponent<UnitCombat>().isAlive())
        {
            targeting[7] = true;
            shift = false;
        }
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftShift) && partySystem.getCharacter(4).GetComponent<UnitCombat>().isAlive())
        {
            targeting[7] = true;
            shift = true;
        }
        if (targeting[7] && (Input.GetMouseButtonDown(1) || (!shift && !Input.GetKey(KeyCode.F)) || (shift && !Input.GetKey(KeyCode.R))) && HUDCast[7] == false)
        {
            if (!Input.GetKey(KeyCode.F) || (shift && !Input.GetKey(KeyCode.R)))
            {
                character = partySystem.getCharacter(4);
                character.GetComponent<UnitCombat>().castSpellInSlot(1);
                character.GetComponent<UnitCombat>().stopAttack();
                character.GetComponent<UnitMovement>().stop();
            }
            targeting[7] = false;
            shift = false;
        }

        for (int i = 0; i < 8; i++)
        {
            int charID = Mathf.FloorToInt((i * 0.5f) + 1);
            int spellID = i % 2;
            character = partySystem.getCharacter(charID);
            string spell = character.GetComponent<UnitCombat>().getSpellList()[spellID].getSpellName();

            if (targeting[i])
            {
                if (spell.Equals("blot_out")) //Arrow rain skill
                    targetedAbilityIndicator.showIndicator(character, TargetedAbilityIndicator.Skills.arrow, PlayerMovement.getCurrentMousePos());
                else if (spell.Equals("charge")) //Charge skill
                    targetedAbilityIndicator.showIndicator(character, TargetedAbilityIndicator.Skills.charge, PlayerMovement.getCurrentMousePos());
                else if (spell.Equals("whirlwind")) //Whirlwind skill
                    targetedAbilityIndicator.showIndicator(character, TargetedAbilityIndicator.Skills.whirlwind, PlayerMovement.getCurrentMousePos());
            }
            else
            {
                if (spell.Equals("blot_out")) //Arrow rain skill
                    targetedAbilityIndicator.hideIndicator(character, TargetedAbilityIndicator.Skills.arrow);
                else if (spell.Equals("charge")) //Charge skill
                    targetedAbilityIndicator.hideIndicator(character, TargetedAbilityIndicator.Skills.charge);
                else if (spell.Equals("whirlwind")) //Whirlwind skill
                    targetedAbilityIndicator.hideIndicator(character, TargetedAbilityIndicator.Skills.whirlwind);
            }
        }
        if (isTargetingFromHUD() && !playerHUD.isMouseOverHUD())
        {
            if (Input.GetMouseButtonDown(0))
            {
                for (int i = 0; i < 8; i++)
                {
                    setHUDCast(i, false);
                }
            }
        }
    }

    void OnGUI()
    {
        int width = Screen.width;
        //int height = Screen.height;
        for (int i = 0; i < 4; i++)
            GUI.Label(new Rect(10, 100 + (i * 20), 100, 20), "Targeting: " + (targeting[i * 2] || targeting[(i * 2) + 1]));
        GUI.Label(new Rect(10, 180, 300, 20), "Press G to Toggle Camera Lock to Selection");
        GUI.Label(new Rect(10, 200, 300, 20), "Press V to Toggle Show Healthbars");
        GUI.Label(new Rect(10, 220, 300, 20), "Press (Shift +) Num to (De)select a Character");
        GUI.Label(new Rect(10, 240, 300, 20), "Press § to Select All Characters");
        GUI.Label(new Rect(10, 260, 300, 20), "Press H to Heal Player Characters");
        GUI.Label(new Rect(10, 280, 300, 20), "Press Z to Stop Moving");
        GUI.Label(new Rect(10, 300, 400, 20), "Press X to Mute/Unmute Music and Atmosphere Sounds");
        float msec = Time.deltaTime * 1000.0f;
        float fps = 1.0f / Time.deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(new Rect(width - 110, 10, 110, 20), text);
        GUI.Label(new Rect(width - 110, 30, 110, 20), "Enemies left: " + UnitList.getHostiles().Length);
    }
}
