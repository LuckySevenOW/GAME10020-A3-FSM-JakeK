using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class StateMachine : MonoBehaviour
{
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

    [HideInInspector]
    public NavMeshAgent agent;
    
    [HideInInspector]
    public int patrolIndex = 0;

    [HideInInspector]
    public float idleTime;

    [HideInInspector]
    public float searchTime;

    [HideInInspector]
    public bool canSeePlayer;
    
    private State currentState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // start in the Patrol state
        currentState = new PatrolState(this);
    }

    // --- STATE MACHINE ---
    public void ChangeState(State newState)
    {
        if (currentState != null)
        {
            Debug.Log($"Old state: {currentState}");
            currentState.Exit();
        }
        currentState = newState;
        Debug.Log($"New state: {currentState}");

        currentState.Enter();
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.Execute();
            stateText.text = $"State: {currentState}";
        }

        // if NPC ever gets close to player, end 
        Vector3 toPlayer = character.position - transform.position;
        float distToPlayer = toPlayer.magnitude;
        if (distToPlayer < playerDistThreshold && canSeePlayer) SceneManager.LoadScene("END");
    }

    // --- HELPER FUNCTIONS ---
    public bool IsInViewCone()
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


    private void OnDrawGizmos()
    {
        // draw the waypoints
        Gizmos.color = Color.red;
        foreach (Transform patrolTransform in patrolWaypoints)
        {
            Gizmos.DrawWireSphere(patrolTransform.position, 0.5f);
        }

        // draw the view cone (2D version)
        if (!(currentState is IdleState))
        {
            Handles.color = new Color(0f, 1f, 1f, 0.25f);
            if (canSeePlayer) Handles.color = new Color(1f, 0f, 0f, 0.25f);

            Vector3 forward = transform.forward;
            Handles.DrawSolidArc(transform.position, Vector3.up, forward, viewAngle / 2f, viewRadius);
            Handles.DrawSolidArc(transform.position, Vector3.up, forward, -viewAngle / 2f, viewRadius);
        }
    }
}
