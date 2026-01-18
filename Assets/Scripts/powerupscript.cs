using UnityEngine;

public enum PowerUpType { Speed, Damage, Health }

public class powerupscript : MonoBehaviour
{
    public float duration = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            // Apply power-up effect to the player
            Debug.Log("Power-up collected: " + gameObject.name);
            PowerUpType randomPower = (PowerUpType)Random.Range(0, 3);
            player.ActivatePowerUp(randomPower, duration);
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Destroy(gameObject,duration);
    }

    // Update is called once per frame
}
