using UnityEngine;
using System.Collections;

public class Ellipse : MonoBehaviour
{
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
        Vector2 point = new Vector2(0, 0);
        while (true)
        {
            point = ellipsePosition + Random.insideUnitCircle * ellipseWidth;

            if (pointInsideEllipse(point, ellipsePosition, ellipseWidth))
                break;
        }
        return point;
    }

    public static Vector2 isometricDirection(Vector2 pos1, Vector2 pos2)
    {
        Vector2 point = new Vector2(0, 0);
        float angle = Vector2.Angle(Vector3.left, pos1 - pos2) * Mathf.Deg2Rad;
        Vector2 dir = (pos1 - pos2).normalized;

        float a = 1.0f;
        float b = 1.0f * 0.5f;

        float theta = Mathf.Tan(angle);

        point.x = (a * b) / (Mathf.Sqrt(b * b + (a * a) * (theta * theta)));
        point.y = Mathf.Sqrt(1 - (point.x / a) * (point.x / a)) * b;

        if (dir.x >= 0 && point.x >= 0 || dir.x <= 0 && point.x <= 0)
            dir.x = point.x;
        else
            dir.x = -point.x;

        if (dir.y >= 0 && point.y >= 0 || dir.y <= 0 && point.y <= 0)
            dir.y = point.y;
        else
            dir.y = -point.y;

        return dir;
    }

    public static Vector2 getPointOnEllipsePerimeter(float ellipseWidth, float angleInRadians)
    {
        Vector2 point = new Vector2(0, 0);
        float a = ellipseWidth;
        float b = ellipseWidth / 2f;

        while (angleInRadians > Mathf.PI)
            angleInRadians = -Mathf.PI + (angleInRadians - Mathf.PI);

        while (angleInRadians < -Mathf.PI)
            angleInRadians = Mathf.PI + (angleInRadians + Mathf.PI);

        float theta = Mathf.Tan(angleInRadians);

        point.x = (a * b) / (Mathf.Sqrt(b * b + (a * a) * (theta * theta)));
        if (angleInRadians < -Mathf.PI / 2f || angleInRadians > Mathf.PI / 2f)
            point.x = -point.x;
        point.y = Mathf.Sqrt(1 - (point.x / a) * (point.x / a)) * b;
        if (angleInRadians < 0)
            point.y = -point.y;

        return point;
    }
}
