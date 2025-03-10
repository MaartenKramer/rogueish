using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public LayerMask enemyLayer;
    public LayerMask obstacleLayer;
    public int attackDamage = 50;
    public float attackRange = 10f;
    public TurnManager turnManager;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float beamDuration = 0.1f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //Checks if It's the player's turn
        if (turnManager.currentTurn != TurnManager.Turn.Player) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            BeamAttack(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            BeamAttack(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            BeamAttack(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            BeamAttack(Vector2.right);
        }
    }

    //Shoots Beam in given direction
    void BeamAttack(Vector2 direction)
    {
        audioSource.Play();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, attackRange, enemyLayer | obstacleLayer);

        Vector3 endPoint = hit.collider ? hit.point : (Vector3)(transform.position + (Vector3)(direction * attackRange));

        if (hit.collider)
        {
            if (((1 << hit.collider.gameObject.layer) & enemyLayer) != 0)
            {
                Health enemyHealth = hit.collider.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                    Debug.Log("Beam hit enemy for " + attackDamage + " damage.");
                }
            }
            else
            {
                Debug.Log("Beam hit an obstacle. Attack stopped.");
            }
        }

        StartCoroutine(ShowBeamEffect(transform.position, endPoint));
        turnManager.EndTurn(gameObject.name);
    }

    //Visualizes beam through line renderer
    IEnumerator ShowBeamEffect(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(beamDuration);

        lineRenderer.enabled = false;
    }
}
