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
            lastAction = Action.nothing;

        //Hiiren oikea nappi.
        if (Input.GetMouseButton(1) && !unitCombat.isAttacking() && !playerHUD.isMouseOverHUD() && !gameHUD.isTargeting())
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

        if ((partySystem.isMouseOverCharacter() || playerHUD.isMouseOverHUD() || gameHUD.isTargeting()) && Input.GetMouseButtonDown(0))
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


    void OnGUI()
    {
        int offset = (partySystem.getGroupID(gameObject) * 20);
        if (offset >= 0)
            GUI.Label(new Rect(10, 100 + offset, 100, 20), "Targeting: " + targeting);
    }
}
