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
    private GameHUD gameHUD;

    private bool ignoreRightClick = false;
    private bool ignoreLeftClick = false;
    private Action lastAction = Action.nothing;

    private Vector2 movePoint;
    private bool moveAfterAttack = false;

    private Vector2 clickPosition;

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
        playerHUD = GameObject.Find("HUD").GetComponent<PlayerHUD>();
        gameHUD = GameObject.Find("HUD").GetComponent<GameHUD>();
    }

    void FixedUpdate()
    {
        //groupID = partySystem.getGroupID(this.gameObject);
        if (!unitCombat.isAttacking() && moveAfterAttack)
        {
            //if (groupID != -1)
            //{
            //Character is no longer attacking and the player issued a move order
            unitCombat.stopAttack();
            unitMovement.moveTo(movePoint, lastGroupID);
            //}
            moveAfterAttack = false;
        }
    }

    public Vector2 getClickPosition()
    {
        return clickPosition;
    }

    void LateUpdate()
    {
        groupID = partySystem.getGroupID(this.gameObject);
        //Hakee hiiren kohdan world spacessa.
        Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            unitMovement.stop();
        }

        if (Input.GetMouseButtonUp(1))
        {
            lastAction = Action.nothing;
        }
        if (gameHUD.isTargeting() || gameHUD.isTargetingFromHUD() || playerHUD.isMouseOverHUD())
            ignoreRightClick = true;
        else
            ignoreRightClick = false;

        //Hiiren oikea nappi.
        if (Input.GetMouseButton(1) && !unitCombat.isAttacking() && !ignoreRightClick)
        {
            //Pysäyttää hahmon ja lyö ilmaa jos vasen shift on pohjassa
            if (Input.GetKey(KeyCode.LeftShift) && lastAction != Action.attackMove)
            {
                if (partySystem.getGroupID(gameObject) != -1)
                {
                    lastAction = Action.pointAttack;
                    clickPosition = getCurrentMousePos();
                    unitCombat.startAttack();
                }
            }
            else if (lastAction != Action.pointAttack)
            {
                lastAction = Action.attackMove;
                unitCombat.attackClosestTargetToPoint(hit.point);
            }
        }

        if (partySystem.isMouseOverCharacter() || playerHUD.isMouseOverHUD() || gameHUD.isTargetingFromHUD())
        {
            ignoreLeftClick = true;
        }
        else
            ignoreLeftClick = false;

        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftShift) && !ignoreLeftClick && pathfindingTimer <= 0)
        {
            //Liikkuu hiiren kohtaan.
            if (hit.collider != null)
            {
                if (groupID != -1)
                {
                    movePoint = hit.point;
                    if (unitCombat.isAttacking())
                    {
                        //Trying to move, but the character is attacking. Move after the attack has finished
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
    }

    public static Vector2 getCurrentMousePos()
    {
        Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);
        return hit.point;
    }
}
