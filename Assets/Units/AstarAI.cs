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
    private Vector3 oldPoint = Vector3.zero;

    private bool newPath = true;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        //Start a new path to the targetPosition, return the result to the OnPathComplete function
        //seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    public void move(Vector2 point)
    {
        newPoint = new Vector3(point.x, point.y, 0);

        if (path != null && currentWaypoint != path.vectorPath.Count)
        {
            /*if (currentWaypoint == 0)
            {
                currentWaypoint = 1;
                Debug.Log("Num: " + path.vectorPath.Count);
            }*/
            newPath = false;
            //oldPoint = path.vectorPath[currentWaypoint];
            //Debug.Log("Num: " + currentWaypoint);
            //path.vectorPath.Clear();
            seeker.StartPath(transform.position, newPoint, OnPathComplete);
            //path.vectorPath.RemoveRange(1, path.vectorPath.Count - 1);
            //path.vectorPath.RemoveAt(0);
            //path.vectorPath.RemoveAt(1);
            //path.vectorPath.Insert(0, oldPoint);
        }
        else {
            newPath = true;
            seeker.StartPath(transform.position, newPoint, OnPathComplete);
        }
    }

    public void OnPathComplete2(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public void OnPathComplete(Path p)
    {
        //Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        if (!p.error)
        {
            path = p;
            /*
            if (!newPath)
            {
                path.vectorPath.Insert(0, oldPoint);
                //Debug.Log("Inserted");
            } else
            {
                //Debug.Log("pos: " + transform.position);
                //Debug.Log("pat: " + path.vectorPath[0]);
            }
            */
            /*
            if (!newPath && path.vectorPath.Count > 1)
            {
                float dis = Vector2.Distance(path.vectorPath[0], transform.position);
                Debug.Log("Dis: " + dis);
            }
            */
            Vector2 lastPoint = path.vectorPath[path.vectorPath.Count - 1];

            if (Vector2.Distance(transform.position, newPoint) < 3.0f || Vector2.Distance(transform.position, lastPoint) < 3.0f)
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
                    /*
                    for (int i = 0; i < path.vectorPath.Count - 1; i++)
                    {
                        for (int j = path.vectorPath.Count - 1; j > i + 1; j--)
                        {
                            hit = Physics2D.Linecast(path.vectorPath[i], path.vectorPath[j], 1 << 9);
                            //Debug.Log("Checking " + i + " " + j);
                            if (hit.collider == null)
                            {
                                //Debug.Log("Ei osumaa! " + i + " " + j);
                                path.vectorPath.RemoveRange(i + 1, j - i - 1);
                                i = 1;
                                j = path.vectorPath.Count - 1;
                            }
                        }
                    }
                    */
                    for (int j = path.vectorPath.Count - 1; j > 1; j--)
                    {
                        Vector2 startPoint = path.vectorPath[0];
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
                        else if (Vector2.Distance(hit.point, endPoint) < 12.0f)
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
                            else if (Vector2.Distance(hit.point, startPoint) < 12.0f)
                            {
                                //Debug.Log("3: " + Vector2.Distance(hit.point, startPoint));
                                path.vectorPath.RemoveRange(currentWaypoint, j - currentWaypoint);
                                break;
                            }
                        }



                    }
                }
            }
            if (!directPath)
            {
                Vector2 dir1 = (path.vectorPath[0] - transform.position).normalized;
                Vector2 dir2 = (path.vectorPath[1] - path.vectorPath[0]).normalized;
                Vector2 sum = dir1 + dir2;

                bool removedPath = false;

                Debug.Log("Dir1: " + dir1);
                Debug.Log("Dir2: " + dir2 + " Sum: " + sum);

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
            //Debug.Log("End Of Path Reached");
            path = null;
            newPath = true;
            return;
        }

        //Direction to the next waypoint
        Vector2 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= Tuner.UNIT_BASE_SPEED * Time.fixedDeltaTime;
        this.gameObject.transform.Translate(dir);

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
                    RaycastHit2D hit = Physics2D.Linecast(startPoint, endPoint, 1 << 9);
                    //Debug.Log("A: " + Vector2.Distance(hit.point, endPoint) + " -- " + endPoint);
                    //Debug.Log("Checking " + currentWaypoint + " " + j);
                    if (hit.collider == null)
                    {
                        //Debug.Log("Ei osumaa! " + currentWaypoint + " " + j);
                        path.vectorPath.RemoveRange(currentWaypoint, j - currentWaypoint);
                        break;
                    }
                    else if (Vector2.Distance(hit.point, endPoint) < 12.0f)
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
                        else if (Vector2.Distance(hit.point, startPoint) < 12.0f)
                        {
                            //Debug.Log("3: " + Vector2.Distance(hit.point, startPoint));
                            path.vectorPath.RemoveRange(currentWaypoint, j - currentWaypoint);
                            break;
                        }
                    }
                }
            }
            return;
        }
    }
}