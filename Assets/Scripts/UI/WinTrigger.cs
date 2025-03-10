using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasTriggered && collision.CompareTag("Player"))
        {
            hasTriggered = true;
            UIManager.Instance.ShowWinScreen();
        }
    }
}
