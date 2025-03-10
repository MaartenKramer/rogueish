using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth { get; private set; }

    private UIManager uiManager;

    void Start()
    {
        currentHealth = maxHealth;
        uiManager = FindObjectOfType<UIManager>();
    }

    //Calculates new health after taking damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Damage taken: " + damage + ". Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    //Entity Dies
    private void Die()
    {
        Debug.Log(gameObject.name + " has died!");

        //Player Death
        if (gameObject.CompareTag("Player"))
        {
            TriggerGameOver();
        }

        //NPC Death
        else
        {
            gameObject.SetActive(false);
        }

    }

    private void TriggerGameOver()
    {
        uiManager.ShowGameOverScreen();
    }
}
