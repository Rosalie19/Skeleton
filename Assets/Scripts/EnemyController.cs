

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Rigidbody enemyRb;
    private NavMeshAgent agent;
    private Transform player;

    private Animator enemyAnim;
    public LayerMask whatIsGround, whatIsPlayer;

    public bool playerIsAttacking;
    public float health;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        enemyRb = GetComponent<Rigidbody>();
        playerIsAttacking = PlayerBowController.bowAttack;
        enemyAnim = GetComponent<Animator>();
        player = GameObject.Find("PlayerBow").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerIsAttacking = PlayerBowController.bowAttack;
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        
        
        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) ChasePlayer();


        if (health <= 0) {
            enemyAnim.SetTrigger("Die_Trigger");
            Invoke(nameof(DestroyEnemy), 1f);
        }

    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            enemyAnim.SetBool("Walk_Bool", true);
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        enemyAnim.SetBool("Run_Bool", true);
        enemyAnim.SetBool("Walk_Bool", false);
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            enemyAnim.SetTrigger("Attack_Trigger");
            //Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            //rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            //rb.AddForce(transform.up * 8f, ForceMode.Impulse);
            ///End of attack code

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }


    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    

    private void OnCollisionEnter(Collision other){
        if (other.gameObject.tag == "Arrow"){
            enemyAnim.SetTrigger("Hit_Trigger");
            health -= 1;
          
        }
    }

    

    
}
