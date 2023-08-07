using UnityEngine;

public class Collectables : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] AudioSource collectionAudio;
    private PlayerHealth playerHealth;

    private int coinValue = 100;

    private float healthValue = 10f;
    // Start is called before the first frame update
    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            collectionAudio.Play();
            gameManager.CoinCollected(coinValue);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Life"))
        {
            collectionAudio.Play();
            playerHealth.AddHealth(healthValue);
            Destroy(other.gameObject);
        }
    }
}
