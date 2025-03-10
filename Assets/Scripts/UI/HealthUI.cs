using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public GameObject heartPrefab;
    public Transform heartContainer;
    public int maxHearts = 5;
    private Health playerHealth;
    private GameObject[] hearts;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
        }
        else
        {
            return;
        }

        hearts = new GameObject[maxHearts];

        CreateHeartIcons();

        UpdateHeartDisplay();
    }

    void CreateHeartIcons()
    {
        foreach (Transform child in heartContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < maxHearts; i++)
        {
            hearts[i] = Instantiate(heartPrefab, heartContainer);
        }
    }

    void Update()
    {
        if (playerHealth != null)
        {
            UpdateHeartDisplay();
        }
    }

    void UpdateHeartDisplay()
    {
        int heartsDisplay = Mathf.CeilToInt(playerHealth.currentHealth / (playerHealth.maxHealth / (float)maxHearts));

        for (int i = 0; i < maxHearts; i++)
        {
            hearts[i].SetActive(i < heartsDisplay);
        }
    }
}