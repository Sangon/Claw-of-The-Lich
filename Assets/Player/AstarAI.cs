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

    //The AI's speed per second
    public float speed = 0.0f;

    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 0.0f;

    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;

    private Vector3 newPoint = Vector3.zero;

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
            if (!newPath && path.vectorPath.Count > 1)
            {
                float dis = Vector2.Distance(path.vectorPath[0], transform.position);
                //Debug.Log("Dis: " + dis);
            }

            RaycastHit2D hit = Physics2D.Linecast(transform.position, newPoint, 1 << 9);
            if (hit.collider == null)
            {
                //Debug.Log("Suora yhteys1!");
                path.vectorPath.Clear();
                path.vectorPath.Add(transform.position);
                path.vectorPath.Add(newPoint);
            }
            else {
                Vector3 pos = path.vectorPath[path.vectorPath.Count - 1];
                hit = Physics2D.Linecast(transform.position, pos, 1 << 9);
                if (hit.collider == null)
                {
                    //Debug.Log("Suora yhteys2!");
                    path.vectorPath.Clear();
                    path.vectorPath.Add(transform.position);
                    path.vectorPath.Add(pos);
                }
                else {
                    if (path.vectorPath.Count > 1)
                    {
                        hit = Physics2D.Linecast(path.vectorPath[0], path.vectorPath[1], 1 << 9);
                        Debug.DrawLine(path.vectorPath[0], path.vectorPath[1], Color.red);
                        if (hit.collider == null)
                        {
                            //Debug.Log("Suora yhteys3! " + path.vectorPath.Count);
                            path.vectorPath.RemoveAt(0);
                        }
                        else {
                            hit = Physics2D.Linecast(transform.position, path.vectorPath[1], 1 << 9);
                            Debug.DrawLine(transform.position, path.vectorPath[1], Color.red);
                            if (hit.collider == null)
                            {
                                //Debug.Log("Suora yhteys4!");
                                path.vectorPath.RemoveAt(0);
                            }
                        }
                    }
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

        //Debug.Log(currentWaypoint);

        //Direction to the next waypoint
        Vector2 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= Tuner.playerSpeed * Time.fixedDeltaTime;
        this.gameObject.transform.Translate(dir);


        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
            //Debug.Log("hit");
            return;
        }
    }
}