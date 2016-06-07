using System;
using UnityEngine;
using System.Collections;

public class ArrowIndicator : MonoBehaviour
{
    private Texture Symbol;
    private Vector3 dir;
    private Vector2 pivotPoint;

    void Awake()
    {
        Symbol = Resources.Load<Texture>("HUD/HUD_ArrowIndicator");
    }

    void OnGUI()
    {
        Vector3 v3Screen = Camera.main.WorldToViewportPoint(transform.position);
        //rotation = Math.atan2(centerMouse.top, centerMouse.left);

        dir = Vector3.Normalize(Camera.main.transform.position - transform.position);
        //Screen.height * dir.y
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (v3Screen.x > 1)
            {
                float a = Screen.height - Camera.main.WorldToScreenPoint(transform.position).y;
                if (a > Screen.height)
                    a = Screen.height;
                if (a < 30)
                    a = 30;
                GUIUtility.RotateAroundPivot(270, new Vector2(Screen.width - 30, a));
                GUI.DrawTexture(new Rect(Screen.width - 30, a, 30, 30), Symbol);
            }
            if (v3Screen.x < 0)
            {
                float b = Screen.height - Camera.main.WorldToScreenPoint(transform.position).y;
                if (b > Screen.height)
                    b = Screen.height;
                if (b < 30)
                    b = 30;
                GUIUtility.RotateAroundPivot(90, new Vector2(30, b));
                GUI.DrawTexture(new Rect(0, b, 30, 30), Symbol);
            }
        }
        if (Mathf.Abs(dir.x) <= Mathf.Abs(dir.y))
        {
            if (v3Screen.y > 1)
            {
                float c = Camera.main.WorldToScreenPoint(transform.position).x;
                if (c > Screen.width)
                    c = Screen.width;
                if (c < 30)
                    c = 30;
                GUIUtility.RotateAroundPivot(180, new Vector2(c, 10));
                GUI.DrawTexture(new Rect(c, -10, 30, 30), Symbol);
            }
            if (v3Screen.y < 0)
            {
                float d = Camera.main.WorldToScreenPoint(transform.position).x;
                if (d > Screen.width - 30)
                    d = Screen.width - 30;
                if (d < 0)
                    d = 0;
                GUI.DrawTexture(new Rect(d, Screen.height - 30, 30, 30), Symbol);
            }
        }
    }

}


