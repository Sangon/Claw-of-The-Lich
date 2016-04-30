using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;

public class TargetedAbilityIndicator : MonoBehaviour
{
    public List<GameObject> indicators;

    public enum Skills
    {
        charge,
        arrow,
        whirlwind
    }

    public void showIndicator(GameObject player, Skills skill, Vector2 mousePoint)
    {
        switch (skill)
        {
            case Skills.charge:
                //Vector2 direction = mousePoint - new Vector2(player.transform.position.x, player.transform.position.y);
                GameObject chargeIndicator = player.transform.Find("Canvas").Find("ChargeIndicator").gameObject;
                if (!chargeIndicator.activeSelf)
                {
                    chargeIndicator.SetActive(true);
                    indicators.Add(chargeIndicator);
                    chargeIndicator.transform.LookAt(mousePoint);
                }
                break;
            case Skills.arrow:
                GameObject arrowIndicator = player.transform.Find("Canvas").Find("ArrowIndicator").gameObject;
                if (!arrowIndicator.activeSelf)
                {
                    Sprite sprite = arrowIndicator.GetComponent<SpriteRenderer>().sprite;
                    float spriteWidth = sprite.bounds.size.x * sprite.pixelsPerUnit;
                    float scale = 2 * (Tuner.DEFAULT_BLOT_OUT_RADIUS / spriteWidth);
                    arrowIndicator.GetComponent<RectTransform>().localScale = new Vector2(scale, scale);
                    arrowIndicator.SetActive(true);
                    indicators.Add(arrowIndicator);
                }
                break;
            case Skills.whirlwind:
                GameObject whirlwindIndicator = player.transform.Find("Canvas").Find("WhirlwindIndicator").gameObject;
                if (!whirlwindIndicator.activeSelf)
                {
                    Sprite sprite = whirlwindIndicator.GetComponent<SpriteRenderer>().sprite;
                    float spriteWidth = sprite.bounds.size.x * sprite.pixelsPerUnit;
                    float scale = 2 * (Tuner.DEFAULT_WHIRLWIND_RADIUS / spriteWidth);
                    whirlwindIndicator.GetComponent<RectTransform>().localScale = new Vector2(scale, scale);
                    whirlwindIndicator.SetActive(true);
                    indicators.Add(whirlwindIndicator);
                }
                break;
        }
    }

    public void hideIndicator(GameObject player, Skills skill)
    {
        switch (skill)
        {
            case Skills.charge:
                GameObject chargeIndicator = player.transform.Find("Canvas").Find("ChargeIndicator").gameObject;
                if (chargeIndicator.activeSelf)
                {
                    chargeIndicator.SetActive(false);
                    indicators.Remove(chargeIndicator);
                }
                break;
            case Skills.arrow:
                GameObject arrowIndicator = player.transform.Find("Canvas").Find("ArrowIndicator").gameObject;
                if (arrowIndicator.activeSelf)
                {
                    arrowIndicator.SetActive(false);
                    indicators.Remove(arrowIndicator);
                }
                break;
            case Skills.whirlwind:
                GameObject whirlwindIndicator = player.transform.Find("Canvas").Find("WhirlwindIndicator").gameObject;
                if (whirlwindIndicator.activeSelf)
                {
                    whirlwindIndicator.SetActive(false);
                    indicators.Remove(whirlwindIndicator);
                }
                break;
        }
    }

    private void drawIndicator(GameObject indicator, GameObject unit)
    {
        Vector2 mousePosition = PlayerMovement.getCurrentMousePos();
        Vector2 unitPos = Vector2.zero;
        if (unit != null)
            unitPos = unit.transform.position;

        if (indicator.name.Equals("ArrowIndicator") || indicator.name.Equals("WhirlwindIndicator"))
        {
            Vector2 ellipsePos = indicator.transform.position;
            float ellipseWidth = 0;
            if (indicator.name.Equals("ArrowIndicator"))
            {
                ellipseWidth = Tuner.DEFAULT_BLOT_OUT_RADIUS;
                indicator.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
            }
            else if (indicator.name.Equals("WhirlwindIndicator"))
            {
                ellipseWidth = Tuner.DEFAULT_WHIRLWIND_RADIUS;
                indicator.transform.position = new Vector3(indicator.transform.parent.parent.position.x, indicator.transform.parent.parent.position.y, 0);
            }
            if (unit != null && Ellipse.pointInsideEllipse(unitPos, ellipsePos, ellipseWidth))
            {
                unit.GetComponent<SpriteRenderer>().color = Color.cyan;
            }
        }
        else if (indicator.name.Equals("ChargeIndicator"))
        {
            /*
                                P3 = (-v.y, v.x) / Sqrt(v.x^2 + v.y^2) * h
                                P4 = (-v.y, v.x) / Sqrt(v.x^2 + v.y^2) * -h
            */
            Vector2 p1 = indicator.transform.parent.parent.position;
            Vector2 p2 = mousePosition;
            Vector2 v = p2 - p1;

            float angle = Mathf.Atan2(p2.y - p1.y, p2.x - p1.x);

            Vector2 p3;
            if (angle > 0)
                p3 = new Vector2(-v.y, v.x) / Mathf.Sqrt(v.x * v.x + v.y * v.y) * (25f + (25f * Mathf.Sin(angle)));
            else
                p3 = new Vector2(-v.y, v.x) / Mathf.Sqrt(v.x * v.x + v.y * v.y) * (25f + (25f * Mathf.Sin(-angle)));
            Vector2 p4 = new Vector2(-p3.x, -p3.y);

            //Vector2 leftEnd = mousePosition;
            //Vector2 rightEnd = mousePosition;
            Debug.DrawLine(p1, p3 + p1, Color.blue);
            Debug.DrawLine(p1, p4 + p1, Color.blue);

            RectTransform rect = indicator.GetComponent<RectTransform>();
            rect.transform.localEulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);

            //print(angle);

            Vector2 point2 = Ellipse.getPointOnEllipsePerimeter(p1, 256f, angle);

            /*
            if (angle > 0)
                angle = angle + 0.10f + (0.28f * Mathf.Sin(angle));
            else
                angle = angle + 0.10f - (0.28f * Mathf.Sin(angle));
            */

            //Vector2 point = Ellipse.getPointOnEllipsePerimeter(p1, 256f, angle);
            Debug.DrawLine(p1, point2 + p1, Color.red);
            rect.transform.localScale = new Vector3(point2.magnitude / 5f, p3.magnitude / 5f, 1f);
            Vector3[] fourCornersArray = new Vector3[4];
            rect.GetWorldCorners(fourCornersArray);
            float height = Vector3.Distance(fourCornersArray[0], fourCornersArray[2]);
            //float width = Vector3.Distance(fourCornersArray[0], fourCornersArray[1]);
            //print("h: " + height + " w: " + width);
            //float spriteHeight = i.GetComponent<SpriteRenderer>().sprite.rect.height;
            //float scaleY = rect.transform.localScale.y;
            rect.transform.position = new Vector2(p1.x, p1.y) + (point2.normalized * height / 6f);
            //print(i.GetComponent<SpriteRenderer>().sprite.rect.height);
            //print(rect.transform.localScale.y);
        }
    }

    void Update()
    {
        if (indicators.Count == 0)
            Cursor.visible = true;
        else
            Cursor.visible = true;

        GameObject[] hostileList = GameObject.FindGameObjectsWithTag("Hostile");

        foreach (GameObject h in hostileList)
        {
            if (h.GetComponent<SpriteRenderer>().color == Color.cyan)
            {
                if (h.name.Contains("Melee"))
                    h.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                else if (h.name.Contains("Ranged"))
                    h.GetComponent<SpriteRenderer>().color = new Color(199, 255, 0);
            }
            foreach (GameObject i in indicators)
            {
                drawIndicator(i, h);
            }
        }
        if (hostileList.Length == 0)
        {
            print("toimiiko");
            foreach (GameObject i in indicators)
            {
                drawIndicator(i, null);
            }
        }

    }
}