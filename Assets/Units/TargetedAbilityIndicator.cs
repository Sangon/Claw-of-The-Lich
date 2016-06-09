using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;

public class TargetedAbilityIndicator : MonoBehaviour
{
    public List<KeyValuePair<GameObject, Ability>> indicators = new List<KeyValuePair<GameObject, Ability>>();

    public void showIndicator(GameObject player, Ability ability, Vector2 mousePoint)
    {
        switch (ability.getAbilityName())
        {
            case "ArrowRain":
                GameObject arrowIndicator = player.transform.Find("Canvas").Find("ArrowRainIndicator").gameObject;
                if (!arrowIndicator.activeSelf)
                {
                    Sprite sprite = arrowIndicator.GetComponent<SpriteRenderer>().sprite;
                    float spriteWidth = sprite.bounds.size.x * sprite.pixelsPerUnit;
                    float scale = 2 * (Tuner.BASE_BLOT_OUT_RADIUS / spriteWidth); //TODO: Fix radius
                    arrowIndicator.GetComponent<RectTransform>().localScale = new Vector2(scale, scale);
                    arrowIndicator.SetActive(true);
                    indicators.Add(new KeyValuePair<GameObject, Ability>(arrowIndicator, ability));
                }
                break;
            case "Charge":
                //Vector2 direction = mousePoint - new Vector2(player.transform.position.x, player.transform.position.y);
                GameObject chargeIndicator = player.transform.Find("Canvas").Find("ChargeIndicator").gameObject;
                if (!chargeIndicator.activeSelf)
                {
                    chargeIndicator.SetActive(true);
                    chargeIndicator.transform.LookAt(mousePoint);
                    indicators.Add(new KeyValuePair<GameObject, Ability>(chargeIndicator, ability));
                }
                break;
            case "Whirlwind":
                GameObject whirlwindIndicator = player.transform.Find("Canvas").Find("WhirlwindIndicator").gameObject;
                if (!whirlwindIndicator.activeSelf)
                {
                    Sprite sprite = whirlwindIndicator.GetComponent<SpriteRenderer>().sprite;
                    float spriteWidth = sprite.bounds.size.x * sprite.pixelsPerUnit;
                    float scale = 2 * (Tuner.BASE_WHIRLWIND_RADIUS / spriteWidth); //TODO: Fix radius
                    whirlwindIndicator.GetComponent<RectTransform>().localScale = new Vector2(scale, scale);
                    whirlwindIndicator.SetActive(true);
                    indicators.Add(new KeyValuePair<GameObject, Ability>(whirlwindIndicator, ability));
                }
                break;
        }
    }

    public void hideIndicator(GameObject player, Ability ability)
    {
        switch (ability.getAbilityName())
        {
            case "ArrowRain":
                GameObject arrowIndicator = player.transform.Find("Canvas").Find("ArrowRainIndicator").gameObject;
                if (arrowIndicator.activeSelf)
                {
                    arrowIndicator.SetActive(false);
                    indicators.Remove(new KeyValuePair<GameObject, Ability>(arrowIndicator, ability));
                }
                break;
            case "Charge":
                GameObject chargeIndicator = player.transform.Find("Canvas").Find("ChargeIndicator").gameObject;
                if (chargeIndicator.activeSelf)
                {
                    chargeIndicator.SetActive(false);
                    indicators.Remove(new KeyValuePair<GameObject, Ability>(chargeIndicator, ability));
                }
                break;
            case "Whirlwind":
                GameObject whirlwindIndicator = player.transform.Find("Canvas").Find("WhirlwindIndicator").gameObject;
                if (whirlwindIndicator.activeSelf)
                {
                    whirlwindIndicator.SetActive(false);
                    indicators.Remove(new KeyValuePair<GameObject, Ability>(whirlwindIndicator, ability));
                }
                break;
        }
    }

    private void drawIndicator(GameObject indicator, Ability ability)
    {
        Vector2 mousePosition = CameraScripts.getCurrentMousePos();

        if (indicator.name.Equals("ArrowRainIndicator") || indicator.name.Equals("WhirlwindIndicator"))
        {
            Vector2 ellipsePos = indicator.transform.position;
            float ellipseWidth = 0;
            if (indicator.name.Equals("ArrowRainIndicator"))
            {
                ellipseWidth = Tuner.BASE_BLOT_OUT_RADIUS; //TODO: Fix radius
                indicator.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
                if (ability.isInRange(mousePosition))
                    indicator.GetComponent<SpriteRenderer>().color = Tuner.TARGETED_ABILITY_INDICATOR_COLOR;
                else
                    indicator.GetComponent<SpriteRenderer>().color = Tuner.TARGETED_ABILITY_INDICATOR_COLOR_FAIL;
            }
            else if (indicator.name.Equals("WhirlwindIndicator"))
            {
                ellipseWidth = Tuner.BASE_WHIRLWIND_RADIUS; //TODO: Fix radius
                indicator.transform.position = new Vector3(indicator.transform.parent.parent.position.x, indicator.transform.parent.parent.position.y, 0);
            }
            foreach (GameObject unit in UnitList.getHostileUnitsInArea(ellipsePos, ellipseWidth))
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
            //Vector2 p4 = new Vector2(-p3.x, -p3.y);

            //Vector2 leftEnd = mousePosition;
            //Vector2 rightEnd = mousePosition;
            //Debug.DrawLine(p1, p3 + p1, Color.blue);
            //Debug.DrawLine(p1, p4 + p1, Color.blue);

            RectTransform rect = indicator.GetComponent<RectTransform>();
            rect.transform.localEulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);

            //print(angle);

            Vector2 point2 = Ellipse.getPointOnEllipsePerimeter(256f, angle);

            /*
            if (angle > 0)
                angle = angle + 0.10f + (0.28f * Mathf.Sin(angle));
            else
                angle = angle + 0.10f - (0.28f * Mathf.Sin(angle));
            */

            //Vector2 point = Ellipse.getPointOnEllipsePerimeter(p1, 256f, angle);
            //Debug.DrawLine(p1, point2 + p1, Color.red);
            rect.transform.localScale = new Vector3(point2.magnitude / 6f, p3.magnitude / 6f, 1f);
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

        foreach (GameObject unit in UnitList.getAllUnits())
        {
            if (unit.GetComponent<SpriteRenderer>().color == Color.cyan)
            {
                if (unit.name.Contains("Melee"))
                    unit.GetComponent<SpriteRenderer>().color = Tuner.ENEMY_MELEE_COLOR;
                else if (unit.name.Contains("Ranged"))
                    unit.GetComponent<SpriteRenderer>().color = Tuner.ENEMY_RANGED_COLOR;
                else
                    unit.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

        foreach (KeyValuePair<GameObject, Ability> indicator in indicators)
        {
            drawIndicator(indicator.Key, indicator.Value);
        }
    }
}