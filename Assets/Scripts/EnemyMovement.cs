using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMovement : MonoBehaviour
{
    public float timeToMove = 0.2f;
    public float moveInterval = 0.5f;
    public LayerMask obstacleLayer;
    public float detectionRange = 10f;
    public int attackDamage = 20;
    public float attackCooldown = 1f;

    private TurnManager turnManager;
    private bool isMoving = false;
    private Transform player;
    private bool canAttack = true;
    private float lastAttackTime;

    private Vector3 lastPlayerPosition;

    private AudioSource audioSource;

    void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        audioSource = GetComponent<AudioSource>();

        //Locates Player
        if (playerObj != null)
        {
            player = playerObj.transform;
            lastPlayerPosition = player.position;
        }
        else
        {
            Debug.LogError("No GameObject with tag 'Player' found in the scene!");
        }
    }


    void Update()
    {
        if (player != null && player.position != lastPlayerPosition)
        {
            lastPlayerPosition = player.position;
        }

        if (turnManager.currentTurn == TurnManager.Turn.Enemy && !isMoving)
        {
            StartCoroutine(EnemyBehavior());
        }
    }

    IEnumerator EnemyBehavior()
    {
        if (player == null)
            yield break;

        //Checks if Enemy is next to player
        if (!IsAdjacentToPlayer())
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= detectionRange)
            {
                Vector3 moveDir = ChaseDirection();
                yield return StartCoroutine(MoveEnemy(moveDir));
            }
            else
            {
                Vector3 randomDir = RandomDirection();
                yield return StartCoroutine(MoveEnemy(randomDir));
            }
        }

        //Attacks player if possible
        if (IsAdjacentToPlayer() && canAttack && Time.time - lastAttackTime > attackCooldown)
        {
            AttackPlayer();
            lastAttackTime = Time.time;
        }

        //Ends Enemy Turn
        turnManager.EndTurn(gameObject.name);
        yield break;
    }

    void AttackPlayer()
    {
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Enemy dealt " + attackDamage + " damage to player.");
            audioSource.Play();
        }
    }

    //Moves in random direction
    Vector3 RandomDirection()
    {
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
        return directions[Random.Range(0, directions.Length)];
    }

    //Moves towards Player
    Vector3 ChaseDirection()
    {
        Vector3 diff = player.position - transform.position;
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };

        Vector3 primaryDir, secondaryDir;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            primaryDir = diff.x > 0 ? Vector3.right : Vector3.left;
            secondaryDir = diff.y > 0 ? Vector3.up : Vector3.down;
        }
        else
        {
            primaryDir = diff.y > 0 ? Vector3.up : Vector3.down;
            secondaryDir = diff.x > 0 ? Vector3.right : Vector3.left;
        }

        if (!IsDirectionBlocked(primaryDir))
        {
            return primaryDir;
        }

        if (!IsDirectionBlocked(secondaryDir))
        {
            return secondaryDir;
        }

        foreach (Vector3 dir in directions)
        {
            if (!IsDirectionBlocked(dir))
            {
                return dir;
            }
        }

        return Vector3.zero;
    }

    //Is blocked by obstacle
    bool IsDirectionBlocked(Vector3 direction)
    {
        Vector3 targetPosition = transform.position + direction;
        return Physics2D.OverlapCircle(targetPosition, 0.2f, obstacleLayer) != null;
    }

    bool IsAdjacentToPlayer()
    {
        Vector3 diff = player.position - transform.position;
        return (Mathf.Abs(diff.x) == 1f && Mathf.Approximately(diff.y, 0f)) ||
               (Mathf.Abs(diff.y) == 1f && Mathf.Approximately(diff.x, 0f));
    }

    IEnumerator MoveEnemy(Vector3 direction)
    {
        if (direction == Vector3.zero)
            yield break;

        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = originalPosition + direction;

        if (Physics2D.OverlapCircle(targetPosition, 0.2f, obstacleLayer) != null)
            yield break;

        isMoving = true;
        float elapsedTime = 0f;
        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        isMoving = false;
    }
}