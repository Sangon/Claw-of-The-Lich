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

    private Vector3 clickPoint = Vector3.zero;
    private Vector3 targetPosition = Vector3.zero;

    private Vector3 oldPos = Vector3.zero;

    private int groupID = 0;

    private PartySystem partySystem = null;

    private bool ignorePath;
    private int pathNumber;

    private Vector3 currentPosition;

    private NNConstraint constraint;

    public int getPathNumber()
    {
        return pathNumber;
    }

    public void Awake()
    {
        seeker = GetComponent<Seeker>();
        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();

        oldPos = transform.position;
        currentPosition = new Vector3(transform.position.x, transform.position.y, 0);

        constraint = new NNConstraint();
        //Constrain all movement to the current area so that units can't move to "islands" and get stuck
        constraint.constrainArea = true;
        constraint.area = -1;
    }

    void Start()
    {
        GraphNode node = AstarPath.active.GetNearest(currentPosition, NNConstraint.Default).node;
        constraint.area = (int)node.Area;
    }

    public void move(Vector2 point, int groupID = 0)
    {
        ignorePath = false;
        clickPoint = point;

        //Find the closest node to the (clicked) position
        targetPosition = AstarPath.active.GetNearest(point, NNConstraint.Default).clampedPosition;
        currentPosition = new Vector3(transform.position.x, transform.position.y, 0);

        this.groupID = groupID;

        if (groupID > 0)
        {
            //The character is in a party (is selected)!
            if (groupID == 1)
            {
                //We are moving the second character in the party: clear party positions
                partySystem.resetPositions();
            }
        }
        else if (Ellipse.isometricDistance(targetPosition, currentPosition) < Tuner.PATHFINDING_MINIMUM_DISTANCE_FROM_UNIT)
        {
            //Clicked too close to the character's feet, no point moving!
            return;
        }

        ABPath p = ABPath.Construct(currentPosition, targetPosition);
        p.nnConstraint = constraint;
        //Start a new path to the targetPosition, return the result to the OnPathComplete function
        seeker.StartPath(p, onPathComplete);
    }

    public void stop()
    {
        if (path != null)
        {
            ignorePath = true;
            path = null;
            currentWaypoint = 0;
        }
    }

    public Vector2 getNextWaypoint()
    {
        if (path != null)
        {
            return path.vectorPath[currentWaypoint];
        }
        else
            return Vector2.zero;
    }

    public Vector2 getLastWaypoint()
    {
        if (path != null)
        {
            return path.vectorPath[path.vectorPath.Count - 1];
        }
        else
            return Vector2.zero;
    }

    public Vector3 getMovementDirection()
    {
        if (path != null)
        {
            Vector3 pathPoint = new Vector3(path.vectorPath[currentWaypoint].x, path.vectorPath[currentWaypoint].y, 0);
            if (pathPoint.Equals(oldPos))
                pathPoint = new Vector3(path.vectorPath[currentWaypoint + 1].x, path.vectorPath[currentWaypoint + 1].y, 0);
            return pathPoint - oldPos;
        }
        return Vector3.zero;
    }

    public void onPathComplete(Path p)
    {
        //Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        pathNumber++;
        if (ignorePath)
            stop();
        else if (!p.error)
        {
            path = p;
            Vector2 lastPoint = path.vectorPath[path.vectorPath.Count - 1];
            bool directPath = false;

            //print("last: " + lastPoint);
            //print("targ: " + targetPosition);

            RaycastHit2D hit = Physics2D.Linecast(transform.position, targetPosition, Tuner.LAYER_OBSTACLES | Tuner.LAYER_WATER);
            if (hit.collider == null)
            {
                //Debug.Log("Suora yhteys1!");
                GraphNode node = AstarPath.active.GetNearest(targetPosition).node;

                //TODO: Buy pro version and use its LineCast!

                bool freeNode = true;
                for (int i = 0; i < 8; i++)
                {
                    if (!AstarPath.active.astarData.gridGraph.HasNodeConnection((GridNode)node, i))
                    {
                        freeNode = false;
                        break;
                    }
                }
                if (freeNode)
                {
                    //print("Freenode!");
                    path.vectorPath.Clear();
                    path.vectorPath.Add(clickPoint);
                    directPath = true;
                }
            }
            if (!directPath)
            {
                if (Ellipse.isometricDistance(transform.position, lastPoint) >= Tuner.PATHFINDING_MINIMUM_DISTANCE_FROM_UNIT)
                {
                    hit = Physics2D.Linecast(transform.position, lastPoint, Tuner.LAYER_OBSTACLES | Tuner.LAYER_WATER);
                    if (hit.collider == null)
                    {
                        //Debug.Log("Yes");
                        path.vectorPath.Clear();
                        path.vectorPath.Add(currentPosition);
                        path.vectorPath.Add(lastPoint);
                        directPath = true;
                    }
                    else {
                        directPath = parsePath(true);
                        //Debug.Log("No: " + directPath);
                    }
                }
                if (path.vectorPath.Count > 1)
                {
                    Vector2 dir1 = (path.vectorPath[0] - currentPosition).normalized;
                    Vector2 dir2 = (path.vectorPath[1] - path.vectorPath[0]).normalized;
                    Vector2 sum = dir1 + dir2;

                    bool removedPath = false;

                    //Debug.Log("Dir1: " + dir1);
                    //Debug.Log("Dir2: " + dir2 + " Sum: " + sum);

                    //Remove the first waypoint if it makes the unit turn more than necessary, i.e. 180 degrees (waypoint is behind the player)
                    if (sum == Vector2.zero || dir1 == Vector2.zero || (Mathf.Abs(sum.x) <= 0.75f && Mathf.Abs(sum.y) <= 0.75f))
                    {
                        removedPath = true;
                        path.vectorPath.RemoveAt(0);
                        //Debug.Log("Angle is too steep");
                    }
                    if (!removedPath)
                    {
                        if (path.vectorPath.Count > 1)
                        {
                            hit = Physics2D.Linecast(currentPosition, path.vectorPath[1], Tuner.LAYER_OBSTACLES | Tuner.LAYER_WATER);
                            if (hit.collider == null)
                            {
                                //Debug.Log("Suora yhteys3!");
                                path.vectorPath.RemoveAt(0);
                            }
                        }
                    }
                }
            }

            if (groupID > 0)
            {
                //Moving more than one character at a time: adjust positions for the others
                if (moveToNextTile())
                {
                    //print(Ellipse.isometricDistance(targetPosition, currentPosition));
                    if (Ellipse.isometricDistance(targetPosition, currentPosition) < Tuner.PATHFINDING_MINIMUM_DISTANCE_FROM_UNIT)
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

    private bool parsePath(bool checkFromCurPos)
    {
        for (int j = path.vectorPath.Count - 1; j > currentWaypoint; j--)
        {
            Vector2 startPoint;

            if (checkFromCurPos)
                startPoint = transform.position;
            else
                startPoint = path.vectorPath[currentWaypoint];

            Vector2 endPoint = path.vectorPath[j];

            RaycastHit2D hit = Physics2D.Linecast(startPoint, endPoint, Tuner.LAYER_OBSTACLES | Tuner.LAYER_WATER);

            //Debug.Log("A: " + Ellipse.isometricDistance(hit.point, endPoint) + " -- " + endPoint);
            //Debug.Log("Checking " + currentWaypoint + " " + j);

            if (hit.collider == null)
            {
                if (!checkFromCurPos)
                {
                    //Debug.Log("Ei osumaa! " + (currentWaypoint + 1) + " " + j);
                    path.vectorPath.RemoveRange(currentWaypoint + 1, j - (currentWaypoint + 1));
                    break;
                }
                else {
                    startPoint = path.vectorPath[0];
                    hit = Physics2D.Linecast(startPoint, endPoint, Tuner.LAYER_OBSTACLES | Tuner.LAYER_WATER);
                    if (hit.collider == null)
                    {
                        //Debug.Log("Ei osumaa! " + 0 + " " + j);
                        //Debug.Log(startPoint + " " + endPoint);
                        path.vectorPath.RemoveRange(0, j - 0);
                        return true;
                    }
                }
            }
        }
        return false;
        /*
        else if (Ellipse.isometricDistance(hit.point, endPoint) < 40.0f)
        {
            Debug.Log("1: " + Ellipse.isometricDistance(hit.point, endPoint));
            path.vectorPath.RemoveRange(currentWaypoint, j - currentWaypoint);
            break;
        }
        else
        {
            hit = Physics2D.Linecast(endPoint, startPoint, Tuner.LAYER_OBSTACLES);
            //Debug.Log("B: " + Ellipse.isometricDistance(hit.point, startPoint) + " -- " + startPoint);
            if (hit.collider == null)
            {
                Debug.Log("2");
                path.vectorPath.RemoveRange(currentWaypoint, j - currentWaypoint);
                break;
            }
            else if (Ellipse.isometricDistance(hit.point, startPoint) < 40.0f)
            {
                Debug.Log("3: " + Ellipse.isometricDistance(hit.point, startPoint));
                path.vectorPath.RemoveRange(currentWaypoint, j - currentWaypoint);
                break;
            }
        }
        */
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

        //RaycastHit2D hit = Physics2D.Linecast(transform.position, targetPosition, Tuner.LAYER_OBSTACLES);
        /*
        //TODO: Optimize this!
        if (hit.collider == null)
        {
            //Debug.Log("Suora yhteys11!");
            path.vectorPath.Clear();
            path.vectorPath.Add(currentPosition);
            path.vectorPath.Add(targetPosition);
            currentWaypoint = 1;
        }
        */

        //The max distance from the AI to a waypoint for it to continue to the next waypoint
        float nextWaypointDistance = gameObject.GetComponent<UnitCombat>().getMovementSpeed() * Time.fixedDeltaTime;

        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Ellipse.isometricDistance(transform.position, path.vectorPath[currentWaypoint]) <= nextWaypointDistance)
        {
            //realPos = path.vectorPath[currentWaypoint];
            if (currentWaypoint + 2 < path.vectorPath.Count)
            {
                parsePath(false);
            }
            currentWaypoint++;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            //Debug.Log("End Of Path Reached");
            path = null;
            return;
        }

        oldPos = transform.position;

        //Direction to the next waypoint
        Vector2 pos1 = new Vector2(path.vectorPath[currentWaypoint].x, path.vectorPath[currentWaypoint].y);
        Vector2 pos2 = new Vector2(transform.position.x, transform.position.y);
        //Vector2 dir = (pos1 - pos2).normalized;

        //float angle = Vector2.Angle(Vector3.left, new Vector3(path.vectorPath[currentWaypoint].x, path.vectorPath[currentWaypoint].y) - currentPosition) * Mathf.Deg2Rad;

        Vector2 dir = Ellipse.isometricDirection(pos1, pos2);
        dir *= gameObject.GetComponent<UnitCombat>().getMovementSpeed() * Time.fixedDeltaTime;
        transform.Translate(dir);
        /*
        if (transform.name.Equals("Character#1"))
            print("angle: " + angle);
        
        if (transform.name.Equals("Character#1"))
            print("ellipse: " + ellipse);
        */
    }

    bool moveToNextTile()
    {
        Vector2 partyPoint = path.vectorPath[path.vectorPath.Count - 1];
        int offset = 0;

        for (int i = 1; i <= 8; i++)
        {
            offset = 300 - (60 * groupID) - ((i - 1) * 45);

            if (!partySystem.setPosition(groupID, offset))
                continue;

            RaycastHit2D hit;
            partyPoint.x = path.vectorPath[path.vectorPath.Count - 1].x + Tuner.PARTY_SPACING * Mathf.Sin(offset * Mathf.Deg2Rad);
            partyPoint.y = path.vectorPath[path.vectorPath.Count - 1].y + Tuner.PARTY_SPACING * Mathf.Cos(offset * Mathf.Deg2Rad);
            partyPoint += Ellipse.isometricLine(transform.position, partyPoint, Random.Range(Tuner.PARTY_SPACING * -0.4f, Tuner.PARTY_SPACING * 0.4f));
            hit = Physics2D.Linecast(path.vectorPath[path.vectorPath.Count - 1], partyPoint, Tuner.LAYER_OBSTACLES | Tuner.LAYER_WATER);

            if (hit.collider == null)
            {
                //Debug.Log("Toimii1 " + targetPosition + " " + partyPoint + " " + i);
                targetPosition = partyPoint;
                return true;
            }
            else if (Ellipse.isometricDistance(hit.point, partyPoint) < 4.0f)
            {
                //Debug.Log("Toimii2 " + targetPosition + " " + partyPoint + " " + i);
                targetPosition = hit.point;
                return true;
            }
            else
            {
                hit = Physics2D.Linecast(partyPoint, path.vectorPath[path.vectorPath.Count - 1], Tuner.LAYER_OBSTACLES | Tuner.LAYER_WATER);
                if (hit.collider == null)
                {
                    //Debug.Log("Toimii3 " + targetPosition + " " + partyPoint + " " + i);
                    targetPosition = partyPoint;
                    return true;
                }
                else if (Ellipse.isometricDistance(path.vectorPath[path.vectorPath.Count - 1], partyPoint) < 4.0f)
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
