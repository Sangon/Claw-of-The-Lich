using UnityEngine;
using System.Collections;

public class AIStates : MonoBehaviour
{
    enum State
    {
        Idle,
        Wander,
        Chase
    };

    private State currentState = State.Idle;
    private UnitMovement unitMovement = null;
    private UnitCombat unitCombat = null;



    void Start()
    {
        unitMovement = GetComponent<UnitMovement>();
        unitCombat = GetComponent<UnitCombat>();
    }

    void UpdateState()
    {
        switch (currentState)
        {
            case State.Idle:
                //if (UnitCombat.attackClosestTargetToPoint(transform.position) != null)
                    //ChangeState(State.Chase);
                break;
            case State.Wander:
                //TODO: jotain
                break;
            case State.Chase:
                //if (UnitCombat.lockedTarget == null)
                    //ChangeState(State.Idle);
                break;
        }
    }

    void ChangeState(State state)
    {
        currentState = state;
    }
}
