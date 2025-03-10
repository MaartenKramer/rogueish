using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private bool isMoving;
    private Vector3 originalPosition, targetPosition;
    private float timeToMove = 0.2f;
    public LayerMask obstacleLayer;
    public TurnManager turnManager;

    void Update()
    {
        //Checks if it's the player's turn
        if (turnManager.currentTurn != TurnManager.Turn.Player) return;

        //Checks if the player isn't already moving
        if (!isMoving)
        {
            //Checks input to give appropiate direction
            if (Input.GetKeyDown(KeyCode.W))
                StartCoroutine(MovePlayer(Vector3.up));
            else if (Input.GetKeyDown(KeyCode.A))
                StartCoroutine(MovePlayer(Vector3.left));
            else if (Input.GetKeyDown(KeyCode.D))
                StartCoroutine(MovePlayer(Vector3.right));
            else if (Input.GetKeyDown(KeyCode.S))
                StartCoroutine(MovePlayer(Vector3.down));
        }
    }

    //Moves player
    private IEnumerator MovePlayer(Vector3 direction)
    {
        originalPosition = transform.position;
        targetPosition = originalPosition + direction;

        if (Physics2D.OverlapCircle(targetPosition, 0.2f, obstacleLayer) != null)
        {
            yield break;
        }

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

        //Ends player turn
        turnManager.EndTurn(gameObject.name);
    }
}
