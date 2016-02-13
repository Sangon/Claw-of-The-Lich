using UnityEngine;
using System.Collections;
//Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
//This line should always be present at the top of scripts which use pathfinding
using Pathfinding;

public class AstarAI : MonoBehaviour
{
    //The point to move to
    public Transform target;

    private Seeker seeker;

    //The calculated path
    public Path path;

    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;

    private Vector3 newPoint = Vector3.zero;

    private uint groupID = 0;

    private bool firstPath = true;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        //Start a new path to the targetPosition, return the result to the OnPathComplete function
        //seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    public void move(Vector2 point, uint groupID = 0)
    {
        newPoint = new Vector3(point.x, point.y, 0);
        firstPath = true;
        this.groupID = groupID;
        seeker.StartPath(transform.position, newPoint, OnPathComplete);
    }

    public void stop()
    {
        if (path != null)
        {
            path = null;
            currentWaypoint = 0;
        }
    }

    public void OnPathComplete(Path p)
    {
        //Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        if (!p.error)
        {
            path = p;
            Vector2 lastPoint = path.vectorPath[path.vectorPath.Count - 1];

            float minDistance = 3.0f;
            if (Vector2.Distance(transform.position, newPoint) < minDistance || Vector2.Distance(transform.position, lastPoint) < minDistance)
            {
                path = null;
                currentWaypoint = 0;
                return;
            }

            bool directPath = false;

            RaycastHit2D hit = Physics2D.Linecast(transform.position, newPoint, 1 << 9);
            if (hit.collider == null)
            {
                //Debug.Log("Suora yhteys1!");
                path.vectorPath.Clear();
                path.vectorPath.Add(transform.position);
                path.vectorPath.Add(newPoint);
                directPath = true;
            }
            else {
                hit = Physics2D.Linecast(transform.position, lastPoint, 1 << 9);
                if (hit.collider == null)
                {
                    //Debug.Log("Suora yhteys2!");
                    path.vectorPath.Clear();
                    path.vectorPath.Add(transform.position);
                    path.vectorPath.Add(lastPoint);
                    directPath = true;
                }
                else {
                    for (int j = path.vectorPath.Count - 1; j > 1; j--)
                    {
                        Vector2 startPoint = transform.position;
                        Vector2 endPoint = path.vectorPath[j];
                        hit = Physics2D.Linecast(startPoint, endPoint, 1 << 9);
                        //Debug.Log("A: " + Vector2.Distance(hit.point, endPoint) + " -- " + endPoint);
                        //Debug.Log("Checking " + currentWaypoint + " " + j);
                        if (hit.collider == null)
                        {
                            startPoint = path.vectorPath[0];
                            hit = Physics2D.Linecast(startPoint, endPoint, 1 << 9);
                            if (hit.collider == null)
                            {
                                //Debug.Log("Ei osumaa! " + 0 + " " + j);
                                //Debug.Log(startPoint + " " + endPoint);
                                path.vectorPath.RemoveRange(0, j - 0);
                                directPath = true;
                                break;
                            }
                            else if (Vector2.Distance(hit.point, endPoint) < 4.0f)
                            {
                                //Debug.Log("1: " + Vector2.Distance(hit.point, endPoint));
                                path.vectorPath.RemoveRange(0, j - 0);
                                directPath = true;
                                break;
                            }
                            else
                            {
                                hit = Physics2D.Linecast(endPoint, startPoint, 1 << 9);
                                //Debug.Log("B: " + Vector2.Distance(hit.point, startPoint) + " -- " + startPoint);
                                if (hit.collider == null)
                                {
                                    //Debug.Log("2");
                                    path.vectorPath.RemoveRange(currentWaypoint, j - currentWaypoint);
                                    directPath = true;
                                    break;
                                }
                                else if (Vector2.Distance(hit.point, startPoint) < 4.0f)
                                {
                                    //Debug.Log("3: " + Vector2.Distance(hit.point, startPoint));
                                    path.vectorPath.RemoveRange(currentWaypoint, j - currentWaypoint);
                                    directPath = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (!directPath && path.vectorPath.Count > 1)
            {
                Vector2 dir1 = (path.vectorPath[0] - transform.position).normalized;
                Vector2 dir2 = (path.vectorPath[1] - path.vectorPath[0]).normalized;
                Vector2 sum = dir1 + dir2;

                bool removedPath = false;

                //Debug.Log("Dir1: " + dir1);
                //Debug.Log("Dir2: " + dir2 + " Sum: " + sum);

                if (sum == Vector2.zero || dir1 == Vector2.zero || (Mathf.Abs(sum.x) <= 0.85f && Mathf.Abs(sum.y) <= 0.85f))
                {
                    removedPath = true;
                    path.vectorPath.RemoveAt(0);
                }
                if (!removedPath)
                {
                    if (path.vectorPath.Count > 1)
                    {
                        hit = Physics2D.Linecast(transform.position, path.vectorPath[1], 1 << 9);
                        if (hit.collider == null)
                        {
                            //Debug.Log("Suora yhteys3!");
                            path.vectorPath.RemoveAt(0);
                        }
                    }
                }
            }

            if (groupID > 0 && firstPath)
            {
                moveToNextTile();
            }

            //Reset the waypoint counter
            currentWaypoint = 0;
        }
    }

    public void FixedUpdate()
    {
        if (path == null)
        {
            //We have no path to move after yet
            //Debug.Log("1111");
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            //Needed for unitMovement.stop()
            path = null;
            return;
        }

        //TODO: Optimize this!
        RaycastHit2D hit = Physics2D.Linecast(transform.position, newPoint, 1 << 9);
        if (hit.collider == null)
        {
            //Debug.Log("Suora yhteys1!");
            path.vectorPath.Clear();
            path.vectorPath.Add(transform.position);
            path.vectorPath.Add(newPoint);
            currentWaypoint = 1;
        }

        //The max distance from the AI to a waypoint for it to continue to the next waypoint
        float nextWaypointDistance = Tuner.UNIT_BASE_SPEED * Time.fixedDeltaTime;

        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
            if (currentWaypoint + 1 < path.vectorPath.Count)
            {
                for (int j = path.vectorPath.Count - 1; j > currentWaypoint; j--)
                {
                    Vector2 startPoint = path.vectorPath[currentWaypoint - 1];
                    Vector2 endPoint = path.vectorPath[j];
                    hit = Physics2D.Linecast(startPoint, endPoint, 1 << 9);
                    //Debug.Log("A: " + Vector2.Distance(hit.point, endPoint) + " -- " + endPoint);
                    //Debug.Log("Checking " + currentWaypoint + " " + j);
                    if (hit.collider == null)
                    {
                        //Debug.Log("Ei osumaa! " + currentWaypoint + " " + j);
                        path.vectorPath.RemoveRange(currentWaypoint, j - currentWaypoint);
                        break;
                    }
                    else if (Vector2.Distance(hit.point, endPoint) < 4.0f)
                    {
                        //Debug.Log("1: " + Vector2.Distance(hit.point, endPoint));
                        path.vectorPath.RemoveRange(currentWaypoint, j - currentWaypoint);
                        break;
                    }
                    else
                    {
                        hit = Physics2D.Linecast(endPoint, startPoint, 1 << 9);
                        //Debug.Log("B: " + Vector2.Distance(hit.point, startPoint) + " -- " + startPoint);
                        if (hit.collider == null)
                        {
                            //Debug.Log("2");
                            path.vectorPath.RemoveRange(currentWaypoint, j - currentWaypoint);
                            break;
                        }
                        else if (Vector2.Distance(hit.point, startPoint) < 4.0f)
                        {
                            //Debug.Log("3: " + Vector2.Distance(hit.point, startPoint));
                            path.vectorPath.RemoveRange(currentWaypoint, j - currentWaypoint);
                            break;
                        }
                    }
                }
            }
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            //Debug.Log("End Of Path Reached");
            path = null;
            return;
        }

        //Direction to the next waypoint
        Vector2 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= Tuner.UNIT_BASE_SPEED * Time.fixedDeltaTime;
        this.gameObject.transform.Translate(dir);

        //Debug.Log("Moving: " + dir + " - " + path.vectorPath[currentWaypoint] + " - " + transform.position);
    }

    bool moveToNextTile()
    {
        firstPath = false;
        Vector2 closePoint = path.vectorPath[path.vectorPath.Count - 1];
        for (int i = 1; i <= 8; i++)
        {
            closePoint.x = path.vectorPath[path.vectorPath.Count - 1].x + 15.0f * Mathf.Sin((360 - ((120 / i) * groupID)) * Mathf.Deg2Rad);
            closePoint.y = path.vectorPath[path.vectorPath.Count - 1].y + 15.0f * Mathf.Cos((360 - ((120 / i) * groupID)) * Mathf.Deg2Rad);

            RaycastHit2D hit = Physics2D.Linecast(path.vectorPath[path.vectorPath.Count - 1], closePoint, 1 << 9);
            if (hit.collider == null)
            {
                //Debug.Log("Toimii1 " + newPoint + " " + closePoint + " " + i);
                newPoint = closePoint;
                return true;
            }
            else if (Vector2.Distance(hit.point, closePoint) < 4.0f)
            {
                //Debug.Log("Toimii2 " + newPoint + " " + closePoint + " " + i);
                newPoint = hit.point;
                return true;
            }
            else
            {
                hit = Physics2D.Linecast(closePoint, path.vectorPath[path.vectorPath.Count - 1], 1 << 9);
                if (hit.collider == null)
                {
                    //Debug.Log("Toimii3 " + newPoint + " " + closePoint + " " + i);
                    newPoint = closePoint;
                    return true;
                }
                else if (Vector2.Distance(path.vectorPath[path.vectorPath.Count - 1], closePoint) < 4.0f)
                {
                    //Debug.Log("Toimii4 " + newPoint + " " + closePoint + " " + i);
                    newPoint = closePoint;
                    return true;
                }
            }
        }
        return false;
    }
}
