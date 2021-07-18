using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using allgameColor;
using Photon.Pun;
using Photon.Realtime;
public class TestEnemyPathfinding : MonoBehaviour
{
    [SerializeField]
    NavMeshAgent agent;

    //Transform player;
    GameObject player;
    public GameObject bulletPrefab;
    public LayerMask l_ground, l_player;
    private int hp = 1;
    //Patrolling

    // Attacking
    public float timebetweenAttacks = 1f;
    bool isAttacked;

    public float sightRange, attackRange;
    public bool playerinSightRange, playerinAttackRange;

    [SerializeField]
    bool _patrolWaiting;

    float waitTime = 3f;

    float switchDirection = 0.2f;

    [SerializeField]
    public List<Transform> _patrolPoints;

//    PatrolPoints patrolPoints = new PatrolPoints();

    int currentPatrolIndex = 0;
    bool isMoving = true;
    bool isWaiting = false;
    bool isMovingForward = true;
    float waitTimer;

    private PhotonView photonView;
    int currentPlayer = 0;
    public void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        photonView = GetComponent<PhotonView>();
        player = GameObject.Find("Player");
        //if(agent == null)
        //{
        //    Debug.Log("NO NavAgent");
        //}
        //else
        //{
        //    if(_patrolPoints != null && _patrolPoints.Count >= 2)
        //    {
        //        currentPatrolIndex = 0;
        //    }
        //    else if(_patrolPoints.Count < 2)
        //    {
        //        Debug.Log("NOT ENOUGH PATROL POINTS");
        //    }
        //}
    }
    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (_patrolPoints == null)
        {
            return;
        }
        if(_patrolPoints.Count == 0)
        {
            Debug.Log("NO WAYPOINTS");
            return;
        }
        if(hp < 1)
        {
            PhotonNetwork.Destroy(gameObject);
            return;
        }
        // Check for whether player is in range
        playerinSightRange = Physics.CheckSphere(transform.position, sightRange, l_player);
        playerinAttackRange = Physics.CheckSphere(transform.position, attackRange, l_player);
        agent.isStopped = false;
        if (!playerinSightRange && !playerinAttackRange)
        {
            Patrolling();
        }
        if (playerinSightRange && !playerinAttackRange)
        {
            ChasePlayer();
        }
        if (playerinSightRange && playerinAttackRange)
        {
            AttackPlayer();
        }
        //Debug.Log("Player Count: " + arr.Length);
    }

    public void StartAI()
    {
        SetDestination();
    }

    private void SetDestination()
    {
        if(_patrolPoints != null)
        {
            Vector3 targetVector = _patrolPoints[currentPatrolIndex].transform.position;
            agent.SetDestination(targetVector);
            isMoving = true;
        }
    }

    private void ChangePatrolPoint()
    {
        if(UnityEngine.Random.Range(0f, 1f) <= switchDirection)
        {
            isMovingForward = !isMovingForward;
        }

        if(isMovingForward)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % _patrolPoints.Count;
        }
        else
        {
            if(--currentPatrolIndex < 0)
            {
                currentPatrolIndex = _patrolPoints.Count - 1;
            }
        }
    }

    private void Patrolling()
    {  
        if(!isAttacked)
        {
            SearchWalkPoint();
        }
    }

    private void ChasePlayer()
    { 
        float shortestDistance = float.MaxValue;

        for (int i = 0; i < player.transform.childCount; ++i)
        {
            float fdistance = Vector3.Distance(player.transform.GetChild(i).position, this.transform.position);
            if (fdistance < shortestDistance)
            {
                shortestDistance = fdistance;
                currentPlayer = i;
            }
        }
        agent.SetDestination(player.transform.GetChild(currentPlayer).position);
    }

    private void AttackPlayer()
    {
        //Enemy stop moving
        agent.isStopped = true;

        transform.LookAt(player.transform.GetChild(currentPlayer).transform.position);

        if(!isAttacked)
        {
            // Spawn projectiles here 
            photonView.RPC("SpawnBullet", RpcTarget.All);
            isAttacked = true;
            Invoke(nameof(ResetAttack), timebetweenAttacks);
        }

        ChasePlayer();
    }

    [PunRPC]
    public void SpawnBullet()
    {
        Rigidbody rb = Instantiate(bulletPrefab, transform.position + transform.forward, transform.rotation).GetComponent<Rigidbody>();
        rb.velocity = transform.forward * 30f;
    }

    private void ResetAttack()
    {
        isAttacked = false;
    }

    private void SearchWalkPoint()
    {
        if (isMoving && agent.remainingDistance <= 1.0f)
        {
            isMoving = false;

            if (_patrolWaiting)
            {
                isWaiting = true;
                waitTimer = 0f;
            }
            else
            {
                ChangePatrolPoint();
                SetDestination();
            }
        }
        
        //if (isWaiting)
        //{
        //    waitTimer += Time.deltaTime;
        //    if (waitTimer >= waitTime)
        //    {
        //        isWaiting = false;

        //        ChangePatrolPoint();
        //        SetDestination();
        //    }
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "LaserCollider")
        {
             CGameColors.GameColors lasercolor = other.GetComponentInParent<Laser>().color;
            if(lasercolor == this.GetComponentInChildren<EnemyColor>().color)
            {
                OnHit(1);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    public void OnHit(int damage)
    {
        hp -= damage;
    }
}
