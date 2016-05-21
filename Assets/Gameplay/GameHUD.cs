using UnityEngine;
using System.Collections;

public class GameHUD : MonoBehaviour
{
    private CameraScripts cameraScripts;
    private PartySystem partySystem;
    private PlayerHUD playerHUD;
    private TargetedAbilityIndicator targetedAbilityIndicator;
    private GameObject keyboardHelp;

    private bool[] targeting = new bool[8];
    private bool[] wasTargeting = new bool[8];
    private bool[] HUDCast = new bool[8];
    private bool[] wasHUDCasting = new bool[8];
    //bool shift;

    private float lichMana = 100f;
    private RectTransform lichManaBar;
    private float lichManaBarScaleX;

    private int frames;
    private float frameTime;
    private int avgFPS;

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

    public void addMana(float value)
    {
        lichMana += value;
        lichMana = Mathf.Clamp(lichMana, 0, 100f);
    }

    // Use this for initialization
    void Start()
    {
        cameraScripts = Camera.main.GetComponent<CameraScripts>();
        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();
        playerHUD = GameObject.Find("HUD").GetComponent<PlayerHUD>();
        targetedAbilityIndicator = GameObject.Find("HUD").GetComponent<TargetedAbilityIndicator>();
        keyboardHelp = GameObject.Find("KeyboardHelp").gameObject;
        keyboardHelp.SetActive(false);
        lichManaBar = GameObject.Find("Bar_Mana").gameObject.GetComponent<RectTransform>();
        lichManaBarScaleX = lichManaBar.localScale.x;
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
            if (lichMana >= 25f)
            {
                bool healed = false;
                foreach (GameObject c in partySystem.aliveCharacters)
                {
                    if (c != null && c.GetComponent<UnitCombat>() != null)
                    {
                        if (c.GetComponent<UnitCombat>().getHealth() != c.GetComponent<UnitCombat>().getMaxHealth())
                        {
                            foreach (GameObject b in partySystem.aliveCharacters)
                            {
                                if (b != null && b.GetComponent<UnitCombat>() != null)
                                {
                                    b.GetComponent<UnitCombat>().takeDamage(-(b.GetComponent<UnitCombat>().getMaxHealth() * 0.25f), null);
                                }
                            }
                            healed = true;
                            break;
                        }
                    }
                }
                if (healed)
                    lichMana -= 25f;
            }
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            keyboardHelp.SetActive(!keyboardHelp.activeSelf);
        }

        /////////////////////////////////////
        /// LICHIN SPELLIT
        /////////////////////////////////////
        float manaScale = lichMana / 100f;
        manaScale = Mathf.Clamp(manaScale, 0, 1f);
        lichManaBar.localScale = new Vector3(lichManaBarScaleX * manaScale, lichManaBar.localScale.y, lichManaBar.localScale.z);
        /////////////////////////////////////
        /// SPELLIT
        /////////////////////////////////////
        for (int i = 0; i < 8; i++)
            wasTargeting[i] = targeting[i];

        GameObject character = null;

        for (int i = 0; i < 8; i++)
        {
            int characterID = Mathf.FloorToInt((i * 0.5f) + 1);
            int spellID = (i % 2);
            UnitCombat unitCombat = partySystem.getCharacter(characterID).GetComponent<UnitCombat>();

            if ((Input.GetKeyDown(Tuner.KEYS_CHARACTER_ABILITY[i]) || HUDCast[i] == true) && !Input.GetKey(KeyCode.LeftShift) && unitCombat.isAlive())
            {
                if (unitCombat.canCastSpell(spellID))
                {
                    targeting[i] = true;
                    //shift = false;
                }
            }
            if (targeting[i] && (Input.GetMouseButtonDown(1) || !Input.GetKey(Tuner.KEYS_CHARACTER_ABILITY[i])) && HUDCast[i] == false)
            {
                if (!Input.GetKey(Tuner.KEYS_CHARACTER_ABILITY[i]))
                {
                    unitCombat.castSpellInSlot(spellID);
                }
                targeting[i] = false;
                //shift = false;
            }
        }
        /*
            if (i < 4)
            {
                if (targeting[i] && (Input.GetMouseButtonDown(1) || !Input.GetKey(Tuner.KEYS_CHARACTER_ABILITY[i])) && HUDCast[i] == false)
                {
                    if (!Input.GetKey(Tuner.KEYS_CHARACTER_ABILITY[i]))
                    {
                        unitCombat.castSpellInSlot(spellID);
                    }
                    targeting[i] = false;
                    shift = false;
                }
            }
            else
            {
                if (Input.GetKeyDown(Tuner.KEYS_CHARACTER_ABILITY[i - 4]) && Input.GetKey(KeyCode.LeftShift) && unitCombat.isAlive())
                {
                    if (unitCombat.canCastSpell(spellID))
                    {
                        targeting[i] = true;
                        shift = true;
                    }
                }
                if (targeting[i] && (Input.GetMouseButtonDown(1) || (!shift && !Input.GetKey(Tuner.KEYS_CHARACTER_ABILITY[i])) || (shift && !Input.GetKey(Tuner.KEYS_CHARACTER_ABILITY[i - 4]))) && HUDCast[i] == false)
                {
                    if (!Input.GetKey(Tuner.KEYS_CHARACTER_ABILITY[i]) || (shift && !Input.GetKey(Tuner.KEYS_CHARACTER_ABILITY[i - 4])))
                    {
                        unitCombat.castSpellInSlot(spellID);
                    }
                    targeting[i] = false;
                    shift = false;
                }
            }
        */
        for (int i = 0; i < 8; i++)
        {
            int characterID = Mathf.FloorToInt((i * 0.5f) + 1);
            int spellID = (i % 2);
            character = partySystem.getCharacter(characterID);
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
        /*
        for (int i = 0; i < 4; i++)
            GUI.Label(new Rect(10, 100 + (i * 20), 100, 20), "Targeting: " + (targeting[i * 2] || targeting[(i * 2) + 1]));
        GUI.Label(new Rect(10, 180, 300, 20), "Press G to Toggle Camera Lock to Selection");
        GUI.Label(new Rect(10, 200, 300, 20), "Press V to Toggle Show Healthbars");
        GUI.Label(new Rect(10, 220, 300, 20), "Press (Shift +) Num to (De)select a Character");
        GUI.Label(new Rect(10, 240, 300, 20), "Press § to Select All Characters");
        GUI.Label(new Rect(10, 260, 300, 20), "Press H to Heal Player Characters");
        GUI.Label(new Rect(10, 280, 300, 20), "Press Z to Stop Moving");
        GUI.Label(new Rect(10, 300, 400, 20), "Press X to Mute/Unmute Music and Atmosphere Sounds");
        */
        GUI.Label(new Rect(10, 120, 300, 20), "Press F1 to Show/Hide Control Scheme");
        float msec = Time.deltaTime * 1000.0f;
        float fps = 1.0f / Time.deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(new Rect(width - 110, 10, 110, 20), text);
        frames++;
        frameTime += Time.deltaTime;
        if (frameTime >= 3.0f)
        {
            avgFPS = (int)(frames / frameTime);
            frameTime -= 3.0f;
            frames = 0;
        }
        text = string.Format("Avg. {0} fps", avgFPS);
        GUI.Label(new Rect(width - 110, 30, 110, 20), text);
        GUI.Label(new Rect(width - 110, 50, 110, 20), "Enemies left: " + UnitList.getHostiles().Length);
    }
}
