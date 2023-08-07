using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private PlayerClass playerClass;
    [SerializeField] Image healthBar;
    private float fillSpeed = 0.5f;
    private bool isPlayerDead;
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [SerializeField] AudioSource hitAudio;
    private void Awake()
    {
        playerClass = GetComponent<PlayerClass>();
    }
    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.fillAmount = currentHealth;
    }
    private void Update()
    {
        if(!isPlayerDead && gameObject.transform.position.y <= -10f)
        {
            TakeDamage(100);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        StartCoroutine(UpdateHealthBar());
        //Debug.Log("Player took " + damageAmount + " damage. Current health: " + currentHealth);
        hitAudio.Play();
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void AddHealth(float healthAmount)
    {
        currentHealth += healthAmount;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        StartCoroutine(UpdateHealthBar());
    }
    private IEnumerator UpdateHealthBar()
    {
        float elapsedTime = 0f;
        float startFillAmount = healthBar.fillAmount;
        float targetFillAmount = currentHealth / maxHealth;

        while (elapsedTime < fillSpeed)
        {
            elapsedTime += Time.deltaTime;
            float curFillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / fillSpeed);
            healthBar.fillAmount = curFillAmount;
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isPlayerDead)
        {
            float damageValue = collision.gameObject.GetComponent<EnemyClass>().enemyDamageValue;
            TakeDamage(damageValue);
            // Get the direction from the collision point to the player
            Vector3 collisionDirection = transform.position - collision.contacts[0].point;
            collisionDirection.y = 0f; // Set the Y component to zero
            collisionDirection.z = 0f; // Set the Z component to zero
            collisionDirection.Normalize();

            // Calculate the impulse force magnitude
            float impulseMagnitude = 30f; // Adjust the magnitude as needed

            // Apply the impulse force in the opposite direction of the collision
            GetComponent<Rigidbody>().AddForce(collisionDirection * impulseMagnitude, ForceMode.Impulse);
        }    
        if (collision.gameObject.CompareTag("Danger") && !isPlayerDead)
        {
            TakeDamage(100);
        }
    }

    private void Die()
    {
        // Handle player death
        isPlayerDead = true;
        playerClass.PlayerDeathCalled();
        Debug.Log("Player has died.");
        // Perform necessary actions, such as game over, respawn, etc.
    }
}
