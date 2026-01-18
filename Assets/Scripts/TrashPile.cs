using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrashPile : MonoBehaviour
{
    
    public int trashAmount { get; private set; } = 100;

    public void StealTrash(int amount)
    {
        trashAmount -= amount;
        if (trashAmount <= 0)
        {
            trashAmount = 0;
            SceneManager.LoadSceneAsync("GameOver");
        }
    }

    public void ReturnTrash(int amount)
    {
        trashAmount += amount;
    }
}
