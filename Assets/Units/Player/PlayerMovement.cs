using UnityEngine;
using System.Collections;
using System;

public class PlayerMovement : MonoBehaviour
{
    public int groupID = 0;
    private int lastGroupID = 0;
    private float pathfindingTimer = 0;
    private UnitMovement unitMovement;
    private UnitCombat unitCombat;
    private PartySystem partySystem;
    private PlayerHUD playerHUD;
    private TargetedAbilityIndicator targetedAbilityIndicator;

    private int selectedSpellSlot = 0;
    private bool targeting = false;
    private bool ignoreMoving = false;
    private Action lastAction = Action.nothing;

    private Vector2 movePoint;
    private bool moveAfterAttack = false;

    private enum Action
    {
        nothing,
        attackMove,
        pointAttack
    };

    void Start()
    {
        unitMovement = GetComponent<UnitMovement>();
        unitCombat = GetComponent<UnitCombat>();
        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();
        playerHUD = GetComponent<PlayerHUD>();
        targetedAbilityIndicator = GameObject.Find("HUD").GetComponent<TargetedAbilityIndicator>();
    }

    void FixedUpdate()
    {
        //groupID = partySystem.getGroupID(this.gameObject);
        if (!unitCombat.isAttacking() && moveAfterAttack)
        {
            //if (groupID != -1)
            //{
            // Character is no longer attacking and the player issued a move order
            unitCombat.stopAttack();
            unitMovement.moveTo(movePoint, lastGroupID);
            //}
            moveAfterAttack = false;
        }
    }

    void LateUpdate()
    {
        groupID = partySystem.getGroupID(this.gameObject);
        //Hakee hiiren kohdan world spacessa.
        Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);

        if (Input.GetKeyDown(KeyCode.S))
        {
            unitMovement.stop();
        }

        if (Input.GetMouseButtonUp(1))
            lastAction = Action.nothing;

        //Hiiren oikea nappi.
        if (Input.GetMouseButton(1) && !unitCombat.isAttacking() && !playerHUD.mouseOverHUD)
        {
            //Pysäyttää hahmon ja lyö ilmaa jos vasen shift on pohjassa, muuten liikkuu kohteeseen.
            if (Input.GetKey(KeyCode.LeftShift) && lastAction != Action.attackMove)
            {
                if (partySystem.getGroupID(gameObject) != -1)
                {
                    lastAction = Action.pointAttack;
                    unitCombat.startAttack();
                }
            }
            else if (lastAction != Action.pointAttack)
            {
                lastAction = Action.attackMove;
                unitCombat.attackClosestTargetToPoint(hit.point);
            }
        }

        if (Input.GetMouseButtonUp(0))
            ignoreMoving = false;
        else if (Input.GetMouseButton(0) && !Input.GetMouseButton(1) && targeting)
        {
            unitCombat.castSpellInSlot(selectedSpellSlot, gameObject);
            toggleTargeting();
            unitMovement.stop();
            ignoreMoving = true;
            pathfindingTimer = Time.fixedDeltaTime * 2.0f;
        }

        if ((partySystem.mouseOverCharacter || playerHUD.mouseOverHUD) && Input.GetMouseButtonDown(0))
            ignoreMoving = true;

        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftShift) && !ignoreMoving && !targeting && pathfindingTimer <= 0)
        {
            //Liikkuu hiiren kohtaan.
            if (hit.collider != null)
            {
                if (groupID != -1)
                {
                    movePoint = hit.point;
                    if (unitCombat.isAttacking())
                    {
                        // Trying to move, but the character is attacking. Move after the attack has finished
                        moveAfterAttack = true;
                        lastGroupID = groupID;
                    }
                    else {
                        unitCombat.stopAttack();
                        unitMovement.moveTo(movePoint, groupID);
                    }
                }
                pathfindingTimer = Time.fixedDeltaTime * 2.0f;
            }
        }

        pathfindingTimer -= Time.fixedDeltaTime;

        //////////////////////////////////////
        /// SPELLIT
        /////////////////////////////////////
        if (Input.GetKeyDown(KeyCode.Q) && gameObject.name.Equals("Character#1"))
        {
            selectedSpellSlot = 0;
            toggleTargeting();
        }

        if (Input.GetKeyDown(KeyCode.W) && gameObject.name.Equals("Character#1"))
        {
            selectedSpellSlot = 1;
            toggleTargeting();
        }

        if (Input.GetKeyDown(KeyCode.E) && gameObject.name.Equals("Character#2")) {
            selectedSpellSlot = 0;
            toggleTargeting();
        }

        if (Input.GetKeyDown(KeyCode.A) && gameObject.name.Equals("Character#2")) {
            selectedSpellSlot = 1;
            toggleTargeting();
        }

        if (Input.GetKeyDown(KeyCode.S) && gameObject.name.Equals("Character#3")) {
            selectedSpellSlot = 0;
            toggleTargeting();
        }

        if (Input.GetKeyDown(KeyCode.D) && gameObject.name.Equals("Character#3")) {
            selectedSpellSlot = 1;
            toggleTargeting();
        }

    }

    public static Vector2 getCurrentMousePos()
    {
        Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);
        return hit.point;
    }

    private void toggleTargeting()
    {
        targeting = !targeting;
        string spell = unitCombat.getSpellList()[selectedSpellSlot].getSpellName();
        if (targeting && gameObject.name.Equals("Character#3"))
        {
            if (spell.Equals("blot_out")) //Arrow rain skill
                targetedAbilityIndicator.showIndicator(gameObject, TargetedAbilityIndicator.Skills.arrow, getCurrentMousePos());
            else if (spell.Equals("charge")) //Charge skill
                targetedAbilityIndicator.showIndicator(gameObject, TargetedAbilityIndicator.Skills.charge, getCurrentMousePos());
        }
        else if (gameObject.name.Equals("Character#3"))
        {
            if (spell.Equals("blot_out")) //Arrow rain skill
                targetedAbilityIndicator.hideIndicator(gameObject, TargetedAbilityIndicator.Skills.arrow);
            else if (spell.Equals("charge")) //Charge skill
                targetedAbilityIndicator.hideIndicator(gameObject, TargetedAbilityIndicator.Skills.charge);
        }

        //TODO: vaihda kursori 
    }

    void OnGUI()
    {
        int offset = (partySystem.getGroupID(gameObject) * 20);
        if (offset >= 0)
            GUI.Label(new Rect(10, 100 + offset, 100, 20), "Targeting: " + targeting);
    }
}
