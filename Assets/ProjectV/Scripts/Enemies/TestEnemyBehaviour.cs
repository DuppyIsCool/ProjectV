using UnityEngine;
using Mirror;
using Pathfinding;
public class TestEnemyBehaviour : NetworkBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private int attackDamage;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string horizontalMovementParam = "Horizontal";
    [SerializeField] private string verticalMovementParam = "Vertical";

    [Header("Movement")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;

    [Header("A* Pathfinding")]
    [SerializeField] private AIPath aiPath;
    [SerializeField] private Seeker seeker;

    [Header("Search Parameters")]
    [SerializeField] private float searchRange;

    private Health targetHealth;
    private float lastAttackTime;
    private GameObject[] players;



    private void Update()
    {
        if (!isServer) return; // Only the server should manage enemy attacks.

        players = GameObject.FindGameObjectsWithTag("Player");
        GameObject lowestHealthPlayer = null;
        float lowestHealth = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            if (gameObject.scene == player.scene)
            {
                Health playerHealth = player.GetComponent<Health>();
                if (playerHealth != null && playerHealth.currentHealth < lowestHealth)
                {
                    float distance = Vector2.Distance(transform.position, player.transform.position);
                    if (distance <= searchRange)
                    {
                        lowestHealth = playerHealth.currentHealth;
                        lowestHealthPlayer = player;
                    }
                }
            }
        }

        if (lowestHealthPlayer != null && lowestHealthPlayer.scene == gameObject.scene)
        {
            targetHealth = lowestHealthPlayer.GetComponent<Health>();

            // Set the destination for the AIPath component.
            aiPath.destination = lowestHealthPlayer.transform.position;

            // Calculate the distance between the enemy and the target.
            float distanceToTarget = Vector2.Distance(transform.position, targetHealth.transform.position);

            // Check if the enemy is ready to attack again and within the attack range.
            if (Time.time >= lastAttackTime + attackCooldown && distanceToTarget <= attackRange)
            {
                lastAttackTime = Time.time;
                AttackTarget();
            }
        }

        // Update the animator based on the enemy's movement direction.
        UpdateAnimatorParameters();
    }

    private void UpdateAnimatorParameters()
    {
        float horizontal = aiPath.desiredVelocity.x;
        float vertical = aiPath.desiredVelocity.y;

        animator.SetFloat(horizontalMovementParam, horizontal);
        animator.SetFloat(verticalMovementParam, vertical);

        float speed = aiPath.desiredVelocity.magnitude;
        animator.SetFloat("speed", speed);

        // Reverse the sprite if the enemy is moving left.
        if (horizontal < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontal > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    [Server]
    private void AttackTarget()
    {
        if (targetHealth == null) return;

        // Apply damage to the target.
        targetHealth.ApplyDamage(attackDamage);

        // Play the attack sound.
        RpcPlayAttackSound();
    }

    [ClientRpc]
    private void RpcPlayAttackSound()
    {
        audioSource.PlayOneShot(attackSound);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a circle to visualize the attack range in the editor.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
