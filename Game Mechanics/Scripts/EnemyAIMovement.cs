using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyAI : MonoBehaviour
{
    private GameObject player;
    private Transform target;
    private NavMeshAgent navMeshAgent;

    [SerializeField] private float provokeRange = 30f;
    [SerializeField] private float disengageRange = 60f;
    [SerializeField] private float attackRange = 3f;
    private float distanceToTarget = Mathf.Infinity;

    [SerializeField] private float turningSpeed = 5f;
    public bool isProvoked = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = attackRange;

        player = GameObject.Find("Player");
        target = player.transform;
    }

    void Update()
    {

        float distanceToTarget = Vector3.Distance(target.position, transform.position);
        RaycastHit hit;
        Vector3 rayDirection = (target.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, rayDirection, out hit, Mathf.Infinity))
        {
            if (hit.transform.CompareTag("Player"))
            {
                if (distanceToTarget <= provokeRange)
                {
                    isProvoked = true;
                }
                else if (distanceToTarget <= disengageRange)
                {
                    isProvoked = false;
                }

                if (isProvoked)
                {
                    moveToTarget();
                }
            }
            else
            {
                Debug.Log("Raycast hit something else: " + hit.transform.name);
            }
        }
    }

    void moveToTarget()
    {
        lookAtTarget();

        if (distanceToTarget >= navMeshAgent.stoppingDistance)
        {
            navMeshAgent.SetDestination(target.position);

        }

    }

    void lookAtTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 1, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turningSpeed);
    }
}
