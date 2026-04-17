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
    enum State { Idle, Patrol, Chase, Search }

    [Header("Scene References")]
    public Transform character;
    public Transform[] patrolWaypoints;
    public TextMeshProUGUI stateText;

    [Header("Config")]
    public float idleTimeThreshold = 2.0f;
    public float waypointThreshold = 0.6f;
    public float rotationSpeed = 1f;
    public float searchTimeThreshold = 5.0f;
    public float playerDistThreshold = 2.0f;
    public float normalSpeed = 3.5f;
    public float chaseSpeed = 5.0f;

    [Header("Vision Settings")]
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 60f;

    State state;
    NavMeshAgent agent;
    int patrolIndex = 0;

    float idleTime;
    float searchTime;
    bool canSeePlayer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        state = State.Patrol;
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                Idle();
                break;

            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Search:
                Search();
                break;
        }

        /*
        // regardless of state, NPC always looks in the direction they are moving
        if (agent.velocity.magnitude > 0.1f)
        {
            Vector3 direction = agent.velocity.normalized;
            Quaternion lookDirection = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, Time.deltaTime * rotationSpeed);
        }
        */

        // show state on screen
        stateText.text = $"State: {state}";

        // if NPC ever gets close to player, end 
        Vector3 toPlayer = character.position - transform.position;
        float distToPlayer = toPlayer.magnitude;
        if (distToPlayer < playerDistThreshold && canSeePlayer) SceneManager.LoadScene("END");
    }

    void Idle()
    {
        agent.speed = normalSpeed;

        // during idle, can never see player
        canSeePlayer = false;

        float idleTimeElapsed = Time.time - idleTime;
        if (idleTimeElapsed >= idleTimeThreshold)
        {
            Debug.Log(state);
            state = State.Patrol;
        }
    }

    void Patrol()
    {
        agent.speed = normalSpeed;

        Transform patrolTransform = patrolWaypoints[patrolIndex];
        agent.SetDestination(patrolTransform.position);

        Vector3 positionXZ = transform.position;
        positionXZ.y = 0.0f;

        Vector3 patrolPositionXZ = patrolTransform.position;
        patrolPositionXZ.y = 0.0f;

        float distance = Vector2.Distance(positionXZ, patrolPositionXZ);
        if (distance < waypointThreshold)
        {
            IncreasePatrolIndex();
            Debug.Log(state);
            state = State.Idle;
            idleTime = Time.time;
        }

        canSeePlayer = IsInViewCone();
        if (canSeePlayer)
        {
            Debug.Log(state);
            state = State.Chase;
        }
    }

    void Chase()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(character.position);

        canSeePlayer = IsInViewCone();
        if (!canSeePlayer)
        {
            state = State.Search;
            Debug.Log(state);
            searchTime = Time.time;
        }
    }

    void Search()
    {
        agent.speed = chaseSpeed;

        float searchTimeElapsed = Time.time - searchTime;

        agent.SetDestination(transform.forward + transform.right);
        canSeePlayer = IsInViewCone();

        if (canSeePlayer)
        {
            state = State.Chase;
            Debug.Log(state);
        }

        if (searchTimeElapsed >= searchTimeThreshold)
        {
            state = State.Patrol;
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

    void IncreasePatrolIndex()
    {
        patrolIndex++;
        if (patrolIndex >= patrolWaypoints.Length) patrolIndex = 0;
    }

    // --- GIZMO DRAWING FOR DEBUG ---

    private void OnDrawGizmos()
    {
        // draw the waypoints
        Gizmos.color = Color.red;
        foreach (Transform patrolTransform in patrolWaypoints)
        {
            Gizmos.DrawWireSphere(patrolTransform.position, 0.5f);
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
