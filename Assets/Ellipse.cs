using UnityEngine;
using System.Collections;

public class Ellipse : MonoBehaviour {

    public static bool pointInsideEllipse(Vector2 point, Vector2 ellipsePosition, float ellipseWidth)
    {
        Vector2 ellipseRadius = new Vector2(ellipseWidth, (ellipseWidth * 0.5f));

        float a = Mathf.Pow((point.x - ellipsePosition.x), 2);
        float b = Mathf.Pow((point.y - ellipsePosition.y), 2);
        float rX = Mathf.Pow(ellipseRadius.x, 2);
        float rY = Mathf.Pow(ellipseRadius.y, 2);

        if (((a / rX) + (b / rY)) <= 1)
        {
            return true;
        }
        return false;
    }

    public static Vector2 getRandomPointInsideEllipse(Vector2 ellipsePosition, float ellipseWidth)
    {
        Vector2 point;
        while (true)
        {
            point = ellipsePosition + Random.insideUnitCircle * ellipseWidth;

            if (pointInsideEllipse(point, ellipsePosition, ellipseWidth))
                break;
        }
        return point;
    }
}
