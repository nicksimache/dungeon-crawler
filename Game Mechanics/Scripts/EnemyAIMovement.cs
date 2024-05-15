using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyAI : MonoBehaviour
{
    private Transform target;
    private NavMeshAgent navMeshAgent;

    [SerializeField] private float range = 10f;
    private float distanceToTarget = Mathf.Infinity;

    [SerializeField] private float turningSpeed = 5f;
    public bool isProvoked = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        GameObject playerObject = GameObject.Find("Player");
        target = playerObject.transform;
    }

    void Update()
    {
        distanceToTarget = Vector3.Distance(target.position, transform.position);

        if (distanceToTarget <= range)
        {
            isProvoked = true;
        }

        if (isProvoked)
        {
            moveToTarget();
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
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turningSpeed);
    }
}
