using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTutorial : MonoBehaviour
{

    public Animator animator;

    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;



    //clone Ȱ��ȭ ��Ȱ��ȭ
    public GameObject[] cloneList;
    public GameObject cloudEffect;
    //patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

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

    //player can attack
    public float attackRadius; // ���� �Ÿ�
    [Range(0, 360)] //���� ���� �þ� ����
    public float attackAngle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer; //��ä�� ���� ���� �÷��̾ �ִ��� ������
    public bool canAttackPlayer;//��ä�� ���� ���� �÷��̾ �ִ��� ������ Ȯ�� �� ����

    public bool hitGrenade = false;

    private void Awake() //���� ���� �� ����
    {
        m_CurrentWaypointIndex = 0;
        player = GameObject.Find("Ber").transform; //player�� ���� ������Ʈ �ֱ�
        agent = GetComponent<NavMeshAgent>();
        CloneMaker();
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

        if (canAttackPlayer)
        {
            Invoke("AttackPlayer", 0.4f);
            Debug.Log("����");
        }
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
        Collider[] attackRangeChecks = Physics.OverlapSphere(transform.position, attackRadius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                { 
                    canSeePlayer = true;
                    ChasePlayer();
                }

                else
                {
                    canSeePlayer = false;
                    Patroling();
                }
            }
                    
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;

        if (attackRangeChecks.Length != 0)
        {
            Transform target = attackRangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canAttackPlayer = true;
                    AttackPlayer();
                }
                else
                {
                    canAttackPlayer = false;
                    Patroling();
                }
            }
            else
                canAttackPlayer = false;
        }
        else if (canAttackPlayer)
            canAttackPlayer = false;
    }

    public void CloneMaker()
    {
        Instantiate(cloudEffect, transform.position, transform.rotation);
        cloneList[1].SetActive(true);
        cloneList[2].SetActive(true);
    }

    private void Patroling()
    {
        
        if (!walkPointSet) SearchWalkPoint(); //walkPointSet �⺻�� false

        if (walkPointSet && !canAttackPlayer && !hitGrenade && !canSeePlayer)
        {
            
            agent.SetDestination(walkPoint);//������ ����
           
            animator.SetBool("isWalk", true);
        }
        

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
        hitGrenade = false;
    }

    private void AttackPlayer()
    {
        
        agent.SetDestination(transform.position);// �÷��̾ ���� ������ ������ ������ �� enemy ��ġ ����

        transform.LookAt(player);

        animator.SetBool("isWalk", false);
        
        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            animator.SetBool("isAttack", true);
            Invoke("StopAttackAnimation", 4f);
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    //4�� �� ���� �ִϸ��̼� ����
    private void StopAttackAnimation()
    {
        animator.SetBool("isAttack", false);
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
