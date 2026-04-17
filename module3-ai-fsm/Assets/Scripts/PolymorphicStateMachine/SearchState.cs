using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SearchState : State
{
    public SearchState(StateMachine _stateMachine) : base(_stateMachine) { }

    public override void Enter()
    {
        stateMachine.searchTime = Time.time;
    }

    public override void Execute()
    {
        stateMachine.agent.speed = stateMachine.chaseSpeed;

        float searchTimeElapsed = Time.time - stateMachine.searchTime;

        stateMachine.agent.SetDestination(stateMachine.transform.forward + stateMachine.transform.right);
        stateMachine.canSeePlayer = stateMachine.IsInViewCone();

        if (stateMachine.canSeePlayer)
        {
            stateMachine.ChangeState(new ChaseState(stateMachine));
        }

        if (searchTimeElapsed >= stateMachine.searchTimeThreshold)
        {
            stateMachine.ChangeState(new PatrolState(stateMachine));
        }
    }

    public override void Exit()
    {
    }

}
