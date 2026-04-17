using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    public PatrolState(StateMachine _stateMachine) : base(_stateMachine) { }

    public override void Enter()
    {
    }

    public override void Execute()
    {
        stateMachine.agent.speed = stateMachine.normalSpeed;

        Transform patrolTransform = stateMachine.patrolWaypoints[stateMachine.patrolIndex];
        stateMachine.agent.SetDestination(patrolTransform.position);

        Vector3 positionXZ = stateMachine.transform.position;
        positionXZ.y = 0.0f;

        Vector3 patrolPositionXZ = patrolTransform.position;
        patrolPositionXZ.y = 0.0f;

        float distance = Vector2.Distance(positionXZ, patrolPositionXZ);
        if (distance < stateMachine.waypointThreshold)
        {
            stateMachine.ChangeState(new IdleState(stateMachine));
        }

        stateMachine.canSeePlayer = stateMachine.IsInViewCone();
        if (stateMachine.canSeePlayer)
        {
            stateMachine.ChangeState(new ChaseState(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.patrolIndex++;
        if (stateMachine.patrolIndex >= stateMachine.patrolWaypoints.Length)
            stateMachine.patrolIndex = 0;
    }

}
