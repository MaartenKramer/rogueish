using System.Collections;
using UnityEngine;
public class TurnManager : MonoBehaviour
{
    public enum Turn { Player, Enemy }
    public Turn currentTurn = Turn.Player;
    private bool isPlayerTurn = true;
    private bool isEnemyTurn = false;
    private bool isTurnLocked = false;
    void Update()
    {
        //Checks whose turn it is
        if (currentTurn == Turn.Player && !isPlayerTurn)
        {
            return;
        }
        if (currentTurn == Turn.Enemy && !isEnemyTurn)
        {
            return;
        }
        if (currentTurn == Turn.Player)
        {
            isPlayerTurn = true;
        }
        else if (currentTurn == Turn.Enemy)
        {
            isEnemyTurn = true;
            if (!EnemiesRemaining())
            {
                EndTurn("AutoEndEnemyTurn");
            }
        }
    }

    //Checks if there are enemies remaining
    private bool EnemiesRemaining()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies.Length > 0;
    }

    //Ends turn of Entity
    public void EndTurn(string turnObject)
    {
        if (isTurnLocked)
            return;
        isTurnLocked = true;
        Debug.Log(currentTurn);
        if (currentTurn == Turn.Player)
        {
            currentTurn = Turn.Enemy;
            isPlayerTurn = false;
        }
        else
        {
            currentTurn = Turn.Player;
            isEnemyTurn = false;
        }
        StartCoroutine(UnlockTurnTransition());
    }
    private IEnumerator UnlockTurnTransition()
    {
        yield return new WaitForSeconds(0.2f);
        isTurnLocked = false;
    }
}