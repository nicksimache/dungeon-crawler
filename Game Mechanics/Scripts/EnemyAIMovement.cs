using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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

        if (distanceToTarget <= provokeRange)
        {
            isProvoked = true;
        }
        else if(distanceToTarget <= disengageRange)
        {
            isProvoked = false;
        }

        if (isProvoked)
        {
            RaycastHit hit;
            Vector3 rayDirection = (target.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, rayDirection, out hit, Mathf.Infinity))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    moveToTarget();
                }
                else
                {
                    AStar aStar = new AStar(new Vector2Int(1000, 1000));

                    var startPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
                    var endPos = new Vector2Int((int)target.position.x, (int)target.position.y);

                    var path = aStar.FindPath(startPos, endPos, (AStar.Node a, AStar.Node b) =>
                    {

                        var pathCost = new AStar.PathCost();

                        pathCost.cost = Vector2Int.Distance(b.Position, endPos);

                        if (Begin.grid[b.Position] == Begin.type.block)
                        {
                            pathCost.cost += 10;
                        }
                        else if (Begin.grid[b.Position] == Begin.type.empty)
                        {
                            pathCost.cost += 1;
                        }

                        pathCost.traversable = true;

                        return pathCost;


                    });

                    if (path != null)
                    {
                        for (int i = 0; i < path.Count; i++)
                        {
                            var current = path[i];

                            moveToLocation(new Vector3Int(current.x, 0, current.y));
                        }
                    }

                }
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

    void moveToLocation(Vector3Int pos)
    {
        navMeshAgent.SetDestination(pos);
    }

    void lookAtTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 1, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turningSpeed);
    }
}
