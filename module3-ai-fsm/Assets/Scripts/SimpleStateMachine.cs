using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SimpleStateMachine : MonoBehaviour
{
    enum State { Idle, Wander, Graze, Alert, Flee, Cooldown }

    [Header("Scene References")]
    public Transform character;
    public Transform[] wanderWaypoints;
    public TextMeshProUGUI stateText;

    [Header("Config")]
    public float idleTimeThreshold = 2.0f;
    public float waypointThreshold = 0.6f;
    public float rotationSpeed = 1f;
    public float searchTimeThreshold = 5.0f;
    public float playerDistThreshold = 7.0f;
    public float playerCooldownThreshold = 2.0f;
    public float slowSpeed = 1.25f;
    public float normalSpeed = 3.5f;
    public float fleeSpeed = 6.0f;

    [Header("Vision Settings")]
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 60f;

    State state;
    NavMeshAgent agent;
    int wanderIndex = 0;

    public float idleTime;
    float searchTime;
    bool canSeePlayer;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        state = State.Idle;
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Wander:
                Wander();
                break;
            case State.Flee:
                Flee();
                break;
            case State.Cooldown:
                Cooldown();
                break;
            case State.Graze:
                Graze();
                break;
            case State.Alert:
                Alert();
                break;
        }

        // show state on screen
        stateText.text = $"State: {state}";
    }

    void Idle()
    {
        agent.speed = normalSpeed;

        // during idle, can see player
        canSeePlayer = IsInViewCone();

        // If you can see the player at any time, go to Alert.
        if (canSeePlayer)
        {
            Debug.Log(state);
            state = State.Alert;
        }

        float idleTimeElapsed = Time.time - idleTime;
        if (idleTimeElapsed >= idleTimeThreshold)
        {
            Debug.Log(state);
            state = State.Wander;
            idleTime = Time.time;
        }
    }

    void Wander()
    {
        agent.speed = normalSpeed;

        Transform wanderTransform = wanderWaypoints[wanderIndex];
        agent.SetDestination(wanderTransform.position);

        Vector3 positionXZ = transform.position;
        positionXZ.y = 0.0f;

        Vector3 wanderPositionXZ = wanderTransform.position;
        wanderPositionXZ.y = 0.0f;

        float distance = Vector2.Distance(positionXZ, wanderPositionXZ);
        if (distance < waypointThreshold)
        {
            IncreaseWanderIndex();
            Debug.Log(state);
            state = State.Graze;
            idleTime = Time.time;
        }

        // If you can see the player at any time, go to Alert.
        canSeePlayer = IsInViewCone();
        if (canSeePlayer)
        {
            Debug.Log(state);
            state = State.Alert;
        }
    }

    void Flee()
    {
        agent.speed = fleeSpeed;
        agent.SetDestination(transform.position - character.position);

        Vector3 toPlayer = character.position - transform.position;
        float distToPlayer = toPlayer.magnitude;

        // If the player leaves the detection radius, then return to Cooldown.
        if (distToPlayer > 10)
        {
            state = State.Cooldown;
            Debug.Log(state);
            idleTime = Time.time;
        }  
    }

    void Cooldown()
    {
        //Functionally identical to Idle.
        agent.speed = slowSpeed;
        agent.SetDestination(transform.position - character.position);
        canSeePlayer = true;


        float idleTimeElapsed = Time.time - idleTime;
        if (idleTimeElapsed >= idleTimeThreshold + 3)
        {
            Debug.Log(state);
            state = State.Idle;
            idleTime = Time.time;
        }

        // If you can see the player at any time, go to Alert.
        canSeePlayer = IsInViewCone();
        if (canSeePlayer)
        {
            Debug.Log(state);
            state = State.Alert;
        }
    }

    void Graze()
    {
        //Functionally identical to Idle, just would play a Grazing animation instead.
        agent.speed = normalSpeed;

        float idleTimeElapsed = Time.time - idleTime;
        if (idleTimeElapsed >= idleTimeThreshold)
        {
            Debug.Log(state);
            state = State.Idle;
            idleTime = Time.time;
        }

        // If it spots the player, go to Alert state
        canSeePlayer = IsInViewCone();
        if (canSeePlayer)
        {
            Debug.Log(state);
            state = State.Alert;
        }
    }

    void Alert()
    {
        // Stops them in their tracks.
        agent.speed = 0;

        // Move away from the player.
        Vector3 toPlayer = character.position - transform.position;
        float distToPlayer = toPlayer.magnitude;

        canSeePlayer = IsInViewCone();

        // If it can't see the player, go to cooldown. If it can see the player still, and they're within 5m distance, then flee.
        if (!canSeePlayer)
        {
            state = State.Cooldown;
            Debug.Log(state);
            idleTime = Time.time;
        }

        if (distToPlayer < 5 && canSeePlayer)
        {
            state = State.Flee;
            Debug.Log(state);
        }

    }



    // --- HELPER FUNCTIONS ---

    bool IsInViewCone()
    {
        Vector3 toPlayer = character.position - transform.position;
        float distToPlayer = toPlayer.magnitude;

        // 1. Distance check
        if (distToPlayer > viewRadius) return false;

        // 2. Angle check
        Vector3 dirToPlayer = toPlayer.normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle > viewAngle * 0.5f) return false;

        // 3. Raycast
        if (Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, viewRadius))
        {
            return hit.transform == character.transform;
        }
        return false;
    }

    void IncreaseWanderIndex()
    {
        wanderIndex++;
        if (wanderIndex >= wanderWaypoints.Length) wanderIndex = 0;
    }

    // --- GIZMO DRAWING FOR DEBUG ---

    private void OnDrawGizmos()
    {
        // draw the waypoints
        Gizmos.color = Color.red;
        foreach (Transform wanderTransform in wanderWaypoints)
        {
            Gizmos.DrawWireSphere(wanderTransform.position, 0.5f);
        }

        // draw the view cone (2D version)
        if (state != State.Idle)
        {
            Handles.color = new Color(0f, 1f, 1f, 0.25f);
            if (canSeePlayer) Handles.color = new Color(1f, 0f, 0f, 0.25f);

            Vector3 forward = transform.forward;
            Handles.DrawSolidArc(transform.position, Vector3.up, forward, viewAngle / 2f, viewRadius);
            Handles.DrawSolidArc(transform.position, Vector3.up, forward, -viewAngle / 2f, viewRadius);
        }
    }
}
