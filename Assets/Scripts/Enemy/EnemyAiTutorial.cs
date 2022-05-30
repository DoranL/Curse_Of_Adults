using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTutorial : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    //patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    //patroling path
    public Transform[] waypoints;
    int m_CurrentWaypointIndex;

    //player can See 
    public float radius;
    [Range(0,360)]
    public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;
    
    private void Awake() //���� ���� �� ����
    {
        m_CurrentWaypointIndex = 0;
        player = GameObject.Find("Ber").transform; //player�� ���� ������Ʈ �ֱ�
        agent = GetComponent<NavMeshAgent>();
    }

    public void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    private void Update()
    {
        //check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer); //�� �ȿ� �þ� ���� ���� �÷��̾ ������ true ������ false
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);//�� �ȿ� ���� ���� ���� �÷��̾ ������ true ������ false
        
        if (!playerInSightRange && !playerInAttackRange) Patroling(); //�þ� & ���ݹ��� ���� �÷��̾ ������ ����
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();//�þ߳��� �÷��̾ ������ ���� ���� ���� ������ �߰�
        if (playerInAttackRange && playerInSightRange) AttackPlayer();//�þ� & ���ݹ��� ���� ������ �÷��̾� ����
    }

    private IEnumerator FOVRoutine() //���� �þ߿� ���� �ִ��� Ž���ϸ� ������ �������� ������ �ִ� �Լ�
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
    private void Patroling()
    {
        //m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        //agent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        if (!walkPointSet) SearchWalkPoint(); //walkPointSet �⺻�� false

        if (walkPointSet)
            agent.SetDestination(walkPoint);//������ ����

        Vector3 distanceToWalkPoint = transform.position - walkPoint; //�������� �̵��� ��ġ������ �Ÿ�

        //WalkPoint reached
        if (distanceToWalkPoint.magnitude < 1f) //�������� ��������� walkPointSet�� false�� ��
        {
            if (m_CurrentWaypointIndex != waypoints.Length)
            {
                m_CurrentWaypointIndex++;//�ε��� ����
            }
            if (m_CurrentWaypointIndex == waypoints.Length)
            {
                m_CurrentWaypointIndex = 0;
            }
            
            walkPointSet = false; //SearchWalkPoint�� �̵�

        }
    }

    private void SearchWalkPoint()
    {
        
        walkPoint = new Vector3(waypoints[m_CurrentWaypointIndex].position.x, waypoints[m_CurrentWaypointIndex].position.y, waypoints[m_CurrentWaypointIndex].position.z);
        walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    public void EnemySlow(float speed)
    {
        
        if(agent.speed > 0)
        {
            agent.speed -= speed;
        }
        
        Invoke("ReturnSpeed", 4f);
    }

    void ReturnSpeed()
    {
        agent.speed = 9.0f;
    }

    private void AttackPlayer()
    {
        
        agent.SetDestination(transform.position);// �÷��̾ ���� ������ ������ ������ �� enemy ��ġ ����

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            //Attack code here
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 15f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
