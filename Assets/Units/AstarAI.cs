using UnityEngine;
using System.Collections;
//Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
//This line should always be present at the top of scripts which use pathfinding
using Pathfinding;

public class AstarAI : MonoBehaviour
{
    private Seeker seeker;

    //The calculated path
    public Path path;

    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;

    private Vector3 targetPosition = Vector3.zero;

    private int groupID = 0;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
    }

    public void move(Vector2 point, int groupID = 0)
    {
        targetPosition = new Vector3(point.x, point.y, 0);
        this.groupID = groupID;

        if (Vector2.Distance(targetPosition, new Vector3(transform.position.x, transform.position.y, 0)) < Tuner.PATHFINDING_MINIMUM_DISTANCE)
            return;

        //Start a new path to the targetPosition, return the result to the OnPathComplete function
        seeker.StartPath(new Vector3(transform.position.x, transform.position.y, 0), targetPosition, onPathComplete);
    }

    public void stop()
    {
        if (path != null)
        {
            path = null;
            currentWaypoint = 0;
        }
    }

    public Vector2 getNextPathPoint()
    {
        if (path != null)
        {
            Vector2 pathPoint = new Vector2(path.vectorPath[currentWaypoint].x, path.vectorPath[currentWaypoint].y);
            Vector2 curPos = new Vector2(transform.position.x, transform.position.y);
            if (pathPoint.Equals(curPos))
                return new Vector2(path.vectorPath[currentWaypoint+1].x, path.vectorPath[currentWaypoint+1].y);
            return pathPoint;
        }
        return Vector3.zero;
    }

    public void onPathComplete(Path p)
    {
        //Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        if (!p.error)
        {
            path = p;
            Vector2 lastPoint = path.vectorPath[path.vectorPath.Count - 1];
            /*
            float minDistance = 3.0f;
            if (Vector2.Distance(new Vector3(transform.position.x, transform.position.y, 0), targetPosition) < minDistance || Vector2.Distance(new Vector3(transform.position.x, transform.position.y, 0), lastPoint) < minDistance)
            {
                path = null;
                currentWaypoint = 0;
                return;
            }
            */

            bool directPath = false;

            RaycastHit2D hit = Physics2D.Linecast(new Vector3(transform.position.x, transform.position.y, 0), targetPosition, Tuner.LAYER_OBSTACLES);
            if (hit.collider == null)
            {
                //Debug.Log("Suora yhteys1!");
                path.vectorPath.Clear();
                path.vectorPath.Add(new Vector3(transform.position.x, transform.position.y, 0));
                path.vectorPath.Add(targetPosition);
                directPath = true;
            }
            else {
                hit = Physics2D.Linecast(new Vector3(transform.position.x, transform.position.y, 0), lastPoint, Tuner.LAYER_OBSTACLES);
                if (hit.collider == null)
                {
                    //Debug.Log("Suora yhteys2!");
                    path.vectorPath.Clear();
                    path.vectorPath.Add(new Vector3(transform.position.x, transform.position.y, 0));
                    path.vectorPath.Add(lastPoint);
                    directPath = true;
                }
                else {
                    for (int j = path.vectorPath.Count - 1; j > 1; j--)
                    {
                        Vector2 startPoint = new Vector3(transform.position.x, transform.position.y, 0);
                        Vector2 endPoint = path.vectorPath[j];
                        hit = Physics2D.Linecast(startPoint, endPoint, Tuner.LAYER_OBSTACLES);
                        //Debug.Log("A: " + Vector2.Distance(hit.point, endPoint) + " -- " + endPoint);
                        //Debug.Log("Checking " + currentWaypoint + " " + j);
                        if (hit.collider == null)
                        {
                            startPoint = path.vectorPath[0];
                            hit = Physics2D.Linecast(startPoint, endPoint, Tuner.LAYER_OBSTACLES);
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
                                hit = Physics2D.Linecast(endPoint, startPoint, Tuner.LAYER_OBSTACLES);
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
                Vector2 dir1 = (path.vectorPath[0] - new Vector3(transform.position.x, transform.position.y, 0)).normalized;
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
                        hit = Physics2D.Linecast(new Vector3(transform.position.x, transform.position.y, 0), path.vectorPath[1], Tuner.LAYER_OBSTACLES);
                        if (hit.collider == null)
                        {
                            //Debug.Log("Suora yhteys3!");
                            path.vectorPath.RemoveAt(0);
                        }
                    }
                }
            }

            if (groupID > 0)
            {
                if (moveToNextTile())
                {
                    if (Vector2.Distance(targetPosition, new Vector3(transform.position.x, transform.position.y, 0)) < Tuner.PATHFINDING_MINIMUM_DISTANCE)
                    {
                        path = null;
                        currentWaypoint = 0;
                        return;
                    }
                    path.vectorPath.RemoveAt(path.vectorPath.Count - 1);
                    path.vectorPath.Add(targetPosition);
                }
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
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            //Needed for unitMovement.stop()
            path = null;
            return;
        }

        RaycastHit2D hit = Physics2D.Linecast(new Vector3(transform.position.x, transform.position.y, 0), targetPosition, Tuner.LAYER_OBSTACLES);
        /*
        //TODO: Optimize this!
        if (hit.collider == null)
        {
            //Debug.Log("Suora yhteys11!");
            path.vectorPath.Clear();
            path.vectorPath.Add(new Vector3(transform.position.x, transform.position.y, 0));
            path.vectorPath.Add(targetPosition);
            currentWaypoint = 1;
        }
        */

        //The max distance from the AI to a waypoint for it to continue to the next waypoint
        float nextWaypointDistance = Tuner.UNIT_BASE_SPEED * Time.fixedDeltaTime;

        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector2.Distance(new Vector3(transform.position.x, transform.position.y, 0), path.vectorPath[currentWaypoint]) <= nextWaypointDistance)
        {
            currentWaypoint++;
            if (currentWaypoint + 1 < path.vectorPath.Count)
            {
                for (int j = path.vectorPath.Count - 1; j > currentWaypoint; j--)
                {
                    Vector2 startPoint = path.vectorPath[currentWaypoint - 1];
                    Vector2 endPoint = path.vectorPath[j];
                    hit = Physics2D.Linecast(startPoint, endPoint, Tuner.LAYER_OBSTACLES);
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
                        hit = Physics2D.Linecast(endPoint, startPoint, Tuner.LAYER_OBSTACLES);
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
        Vector2 dir = (path.vectorPath[currentWaypoint] - new Vector3(transform.position.x, transform.position.y, 0)).normalized;
        dir *= Tuner.UNIT_BASE_SPEED * Time.fixedDeltaTime;
        transform.Translate(dir);
    }

    bool moveToNextTile()
    {
        Vector2 partyPoint = path.vectorPath[path.vectorPath.Count - 1];
        for (int i = 1; i <= 8; i++)
        {
            partyPoint.x = path.vectorPath[path.vectorPath.Count - 1].x + Tuner.PARTY_SPACING * Mathf.Sin((360 - ((120 / i) * groupID)) * Mathf.Deg2Rad);
            partyPoint.y = path.vectorPath[path.vectorPath.Count - 1].y + Tuner.PARTY_SPACING * Mathf.Cos((360 - ((120 / i) * groupID)) * Mathf.Deg2Rad);

            RaycastHit2D hit = Physics2D.Linecast(path.vectorPath[path.vectorPath.Count - 1], partyPoint, Tuner.LAYER_OBSTACLES);
            if (hit.collider == null)
            {
                //Debug.Log("Toimii1 " + targetPosition + " " + partyPoint + " " + i);
                targetPosition = partyPoint;
                return true;
            }
            else if (Vector2.Distance(hit.point, partyPoint) < 4.0f)
            {
                //Debug.Log("Toimii2 " + targetPosition + " " + partyPoint + " " + i);
                targetPosition = hit.point;
                return true;
            }
            else
            {
                hit = Physics2D.Linecast(partyPoint, path.vectorPath[path.vectorPath.Count - 1], Tuner.LAYER_OBSTACLES);
                if (hit.collider == null)
                {
                    //Debug.Log("Toimii3 " + targetPosition + " " + partyPoint + " " + i);
                    targetPosition = partyPoint;
                    return true;
                }
                else if (Vector2.Distance(path.vectorPath[path.vectorPath.Count - 1], partyPoint) < 4.0f)
                {
                    //Debug.Log("Toimii4 " + targetPosition + " " + partyPoint + " " + i);
                    targetPosition = partyPoint;
                    return true;
                }
            }
        }
        return false;
    }
}
