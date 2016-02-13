using UnityEngine;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

public class PlayerMovement : MonoBehaviour
{
    public uint groupID = 0;
    private float pathfindingTimer = 0;
    private UnitMovement unitMovement = null;
    private UnitCombat unitCombat = null;

    void Start()
    {
        unitMovement = GetComponent<UnitMovement>();
        unitCombat = GetComponent<UnitCombat>();
    }

    // Update is called once per frame
    void Update()
    {

        //Hakee hiiren kohdan world spacessa.
        Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            unitMovement.stop();
        }

            //Hiiren vasen nappi.
        if (pathfindingTimer <= 0 && Input.GetMouseButton(0))
        {
            //Pysäyttää hahmon ja lyö ilmaa jos vasen shift on pohjassa, muuten liikkuu kohteeseen.
            if (Input.GetKey(KeyCode.LeftShift))
            {
                unitCombat.startAttack(hit.point);
            }
            else {
                //Liikkuu hiiren kohtaan.
                if (hit.collider != null)
                {
                    //FMODUnity.RuntimeManager.PlayOneShot("event:/walk", transform.position);
                    unitMovement.moveTo(hit.point, groupID);
                    pathfindingTimer = 0.05f;
                }
            }
        }


        //Hiiren oikea nappi.
        if (pathfindingTimer <= 0 && Input.GetMouseButtonDown(1))
        {
            //unitMovement.stop();

            //TODO: Etsii lähimmän kohteen ja lockkaa siihen.

            unitCombat.attackClosestTargetToPoint(hit.point);
        }
        pathfindingTimer -= Time.fixedDeltaTime;


        //Rullaa kameraa kauemmas.
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize += 10;
        }
        //Rulla kameraa lähemmäs.
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Camera.main.orthographicSize -= 10;
        }
        //Rajoittaa kameran max- ja minimietäisyydet.
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, Tuner.CAMERA_MIN_DISTANCE, Tuner.CAMERA_MAX_DISTANCE);


        //////////////////////////////////////
        /// SPELLIT
        /////////////////////////////////////

        if (Input.GetKeyDown(KeyCode.Q)) { }

        if (Input.GetKeyDown(KeyCode.W)) { }

        if (Input.GetKeyDown(KeyCode.E)) { }

        if (Input.GetKeyDown(KeyCode.A)) { }

        if (Input.GetKeyDown(KeyCode.S)) { }

        if (Input.GetKeyDown(KeyCode.D)) { }

    }

}
