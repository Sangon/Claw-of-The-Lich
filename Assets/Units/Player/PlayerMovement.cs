using UnityEngine;
using System.Collections;
using System;

public class PlayerMovement : MonoBehaviour
{
    public int groupID = 0;
    private float pathfindingTimer = 0;
    private UnitMovement unitMovement;
    private UnitCombat unitCombat;
    private PartySystem partySystem;

    private int selectedSpellSlot = 0;
    private bool targeting = false;
    private bool ignoreMoving = false;
    private Action lastAction = Action.nothing;

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
        //partySystem = GetComponent<PartySystem>();
        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();
    }

    // Update is called once per frame
    void Update()
    {
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
        if (Input.GetMouseButton(1) && !unitCombat.isAttacking())
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

        if (Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !ignoreMoving && !unitCombat.isAttacking() && !targeting && pathfindingTimer <= 0)
        {
            //Liikkuu hiiren kohtaan.
            if (hit.collider != null)
            {
                //FMODUnity.RuntimeManager.PlayOneShot("event:/walk", transform.position);
                groupID = partySystem.getGroupID(this.gameObject);
                if (groupID != -1)
                {
                    unitCombat.stopAttack();
                    unitMovement.moveTo(hit.point, groupID);
                }
                pathfindingTimer = Time.fixedDeltaTime * 2.0f;
            }
        }

        pathfindingTimer -= Time.fixedDeltaTime;

        //Rullaa kameraa kauemmas.
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize += Tuner.CAMERA_ZOOM_SPEED;
        }
        //Rulla kameraa lähemmäs.
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Camera.main.orthographicSize -= Tuner.CAMERA_ZOOM_SPEED;
        }
        //Rajoittaa kameran max- ja minimietäisyydet.
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, Tuner.CAMERA_MIN_DISTANCE, Tuner.CAMERA_MAX_DISTANCE);

        //////////////////////////////////////
        /// SPELLIT
        /////////////////////////////////////
        if (Input.GetKeyDown(KeyCode.Q))
        {
            toggleTargeting();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {

        }

        if (Input.GetKeyDown(KeyCode.E)) { }

        if (Input.GetKeyDown(KeyCode.A)) { }

        if (Input.GetKeyDown(KeyCode.S)) { }

        if (Input.GetKeyDown(KeyCode.D)) { }

    }

    public static Vector2 getCurrentMousePos()
    {
        Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);
        return hit.point;
    }

    private void toggleTargeting()
    {

        if (!targeting)
        {
            targeting = true;
        }
        else {
            targeting = false;
        }

        //TODO: vaihda kursori 
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "Targeting: " + targeting);
    }
}
