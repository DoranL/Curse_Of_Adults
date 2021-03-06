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

    //projectile 공격 설정
    public GameObject projectile;
    public Transform projectilePoint;

    //clone 활성화 비활성화
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
    public float attackRadius; // 공격 거리
    [Range(0, 360)] //공격 가능 시야 범위
    public float attackAngle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer; //부채골 범위 내에 플레이어가 있는지 없는지
    public bool canAttackPlayer;//부채골 범위 내에 플레이어가 있는지 없는지 확인 후 공격

    public bool hitGrenade = false;


    public GameObject canThrow;

    private void Awake() //게임 실행 전 수행
    {
        m_CurrentWaypointIndex = 0;
        player = GameObject.Find("Ber").transform; //player에 베르 오브젝트 넣기
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
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer); //구 안에 시야 범위 내에 플레이어가 있으면 true 없으면 false
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);//구 안에 공격 범위 내에 플레이어가 있으면 true 없으면 false
        
        if (!playerInSightRange && !playerInAttackRange) Patroling(); //시야 & 공격범위 내에 플레이어가 없으면 순찰
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();//시야내에 플레이어가 있지만 공격 범위 내에 없으면 추격
        if (playerInAttackRange && playerInSightRange) AttackPlayer();//시야 & 공격범위 내에 있으면 플레이어 공격

        if (canAttackPlayer)
        {
            Invoke("AttackPlayer", 0.1f);
            
        }
    }

    private IEnumerator FOVRoutine() //매초 시야에 적이 있는지 탐색하면 성능이 떨어져서 지연을 주는 함수
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

    //projectile
    //public void Shoot()
    //{
    //    Rigidbody rb = Instantiate(projectile, projectilePoint.position, Quaternion.identity).GetComponent<Rigidbody>();
    //    rb.AddForce(transform.forward * 30f, ForceMode.Impulse);
    //    rb.AddForce(transform.up * 7, ForceMode.Impulse);
    //}
    public void CloneMaker()
    {
        if (!(gameObject.name.Contains("Feathers")))
        {
            Instantiate(cloudEffect, transform.position, transform.rotation);
            cloneList[1].SetActive(true);
            cloneList[2].SetActive(true);
        }
        
    }

    private void Patroling()
    {
        
        if (!walkPointSet) SearchWalkPoint(); //walkPointSet 기본값 false

        if (walkPointSet && !canAttackPlayer && !hitGrenade && !canSeePlayer)
        {
            
            agent.SetDestination(walkPoint);//목적지 지정
           
            animator.SetBool("isWalk", true);
        }
        

        Vector3 distanceToWalkPoint = transform.position - walkPoint; //다음으로 이동한 위치까지의 거리

        //WalkPoint reached
        if (distanceToWalkPoint.magnitude < 1f) //목적지가 가까워지면 walkPointSet에 false를 줌
        {

            if (m_CurrentWaypointIndex != waypoints.Length)
            {
                m_CurrentWaypointIndex++;//인덱스 증가
            }
            if (m_CurrentWaypointIndex == waypoints.Length)
            {
                m_CurrentWaypointIndex = 0;
            }
            
            walkPointSet = false; //SearchWalkPoint로 이동
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
        agent.SetDestination(transform.position);// 플레이어가 공격 범위에 들어오고 공격할 때 enemy 위치 고정

        transform.LookAt(player);

        animator.SetBool("isWalk", false);
 
        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            animator.SetBool("isAttack", true);

            canThrow.GetComponent<EnemyThrower>().Invoke("EnemyThrowGrenade", 0.5f);

            Invoke("StopAttackAnimation", 3f);
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    //4초 후 공격 애니메이션 종료
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
