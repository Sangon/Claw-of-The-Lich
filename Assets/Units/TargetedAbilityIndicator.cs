using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;

public class TargetedAbilityIndicator : MonoBehaviour
{
    bool index = false;
    Vector3 newPosition;
    public GameObject circle;
    public GameObject rect;
    //public GameObject myNewInstance;

    public List<GameObject> indicators;

    public enum Skills
    {
        charge,
        arrow
    }

    public void showIndicator(GameObject player, Skills skill, Vector2 mousePoint)
    {
        switch (skill)
        {
            case Skills.charge:
                //Vector2 direction = mousePoint - new Vector2(player.transform.position.x, player.transform.position.y);
                GameObject chargeIndicator = player.transform.Find("Canvas").Find("ChargeIndicator").gameObject;
                chargeIndicator.SetActive(true);
                indicators.Add(chargeIndicator);
                chargeIndicator.transform.LookAt(mousePoint);
                break;
            case Skills.arrow:
                GameObject arrowIndicator = player.transform.Find("Canvas").Find("ArrowIndicator").gameObject;
                arrowIndicator.SetActive(true);
                indicators.Add(arrowIndicator);
                break;
        }
    }

    public void hideIndicator(GameObject player, Skills skill)
    {
        switch (skill)
        {
            case Skills.charge:
                GameObject chargeIndicator = player.transform.Find("Canvas").Find("ChargeIndicator").gameObject;
                chargeIndicator.SetActive(false);
                indicators.Remove(chargeIndicator);
                break;
            case Skills.arrow:
                GameObject arrowIndicator = player.transform.Find("Canvas").Find("ArrowIndicator").gameObject;
                arrowIndicator.SetActive(false);
                indicators.Remove(arrowIndicator);
                break;
        }
    }

    void Update()
    {
        if (indicators.Count == 0)
            Cursor.visible = true;
        else
            Cursor.visible = true;

        newPosition = PlayerMovement.getCurrentMousePos();

        foreach (GameObject i in indicators)
        {
            if (i.name.Equals("ArrowIndicator"))
            {
                i.transform.position = new Vector3(newPosition.x, newPosition.y, 0);

                GameObject[] hostileList = GameObject.FindGameObjectsWithTag("Hostile");

                foreach (GameObject h in hostileList)
                {
                    Vector2 enemyPos = h.transform.position;
                    Vector2 ellipsePos = i.transform.position;
                    Vector2 ellipseRadius = new Vector2(384.0f, 192.0f);

                    float a = Mathf.Pow((enemyPos.x - ellipsePos.x), 2);
                    float b = Mathf.Pow((enemyPos.y - ellipsePos.y), 2);
                    float rX = Mathf.Pow(ellipseRadius.x, 2);
                    float rY = Mathf.Pow(ellipseRadius.y, 2);

                    if (((a / rX) + (b / rY)) <= 1)
                    {
                        h.GetComponent<SpriteRenderer>().color = Color.cyan;
                    } else if (h.GetComponent<SpriteRenderer>().color == Color.cyan) {
                        if (h.name.Contains("Melee"))
                            h.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                        else if (h.name.Contains("Ranged"))
                            h.GetComponent<SpriteRenderer>().color = new Color(199, 255, 0);
                    }
                }
            }
        }
        /*
        if (Input.GetKeyDown("0"))
        {
            if (index == false)
            {

                index = true;
                myNewInstance =
                    Instantiate(circle, new Vector3(newPosition.x, newPosition.y, 0), Quaternion.identity) as GameObject;
                //myNewInstance.transform.parent = transform;
                Cursor.visible = false;
            }
            else
            {
                index = false;
                Destroy(myNewInstance);
                Cursor.visible = true;
            }
        }
        else if (Input.GetKeyDown("9"))
        {
            if (index == false)
            {

                index = true;
                myNewInstance =
                    Instantiate(rect, new Vector3(newPosition.x, newPosition.y, 0), Quaternion.identity) as GameObject;
                //wqrdsafdmyNewInstance.transform.parent = transform;
                Cursor.visible = false;

            }
            else
            {
                index = false;
                Destroy(myNewInstance);
                Cursor.visible = true;
            }

        }

        if (index == true)
        {

            myNewInstance.transform.localPosition = new Vector3(newPosition.x, newPosition.y, 0);



            GameObject[] hostileList = GameObject.FindGameObjectsWithTag("Hostile");

            foreach (GameObject o in hostileList)
            {
                Vector2 enemyPos = o.transform.position;
                Vector2 ellipsePos = myNewInstance.transform.position;
                Vector2 ellipseRadius = new Vector2(100.0f, 50.0f);

                float a = Mathf.Pow((enemyPos.x - ellipsePos.x), 2);
                float b = Mathf.Pow((enemyPos.y - ellipsePos.y), 2);
                float rX = Mathf.Pow(ellipseRadius.x, 2);
                float rY = Mathf.Pow(ellipseRadius.y, 2);

                if (((a / rX) + (b / rY)) <= 1)
                {
                    print("Osuma");
                }
            }
        }
        */

    }



}