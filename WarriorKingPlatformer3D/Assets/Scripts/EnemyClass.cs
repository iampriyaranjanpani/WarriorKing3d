using System.Collections;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] int enemyMaxHealth;
    [SerializeField] int enemyKillValue = 300;
    //enemy will do damage to player of this amount
    public float enemyDamageValue = 10;
    private int enemyCurHealth;
    //damage will taken by enemy when user hits it
    private int damageAmount = 10;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    [SerializeField] float movementRange;
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] bool canDoRandomMovement;
    private bool useFirstPosition = true;

    private Animator animator;
    private bool isHit;
    private int isHitID;
    private int isDeadID;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        enemyCurHealth = enemyMaxHealth;
        startPosition = transform.position;
        targetPosition = GetRandomTargetPosition();

        isHitID = Animator.StringToHash("isGetHit");
        isDeadID = Animator.StringToHash("isDead");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sword"))
        {
            animator.SetBool(isHitID, true);
            isHit = true;
            DamageTaken();
        }
    }
    private void Update()
    {
        MoveWithinArea();
    }
    private void MoveWithinArea()
    {
        if (!isHit)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        }        

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetPosition = GetRandomTargetPosition();
        }

        // Calculate the direction of movement
        Vector3 movementDirection = targetPosition - transform.position;

        // Rotate the enemy based on the movement direction
        if (movementDirection.x < 0)
        {
            transform.rotation = Quaternion.Euler(0f, -90f, 0f); // Rotate -90 degrees in y-axis
        }
        else if (movementDirection.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 90f, 0f); // Rotate 90 degrees in y-axis
        }
    }
    private Vector3 GetRandomTargetPosition()
    {
        /*The expression useFirstPosition ? (startPosition.x - movementRange) : (startPosition.x + movementRange) is a ternary operator. It acts as a shorthand if-else statement.
         If useFirstPosition is true, the expression(startPosition.x -movementRange) is evaluated and assigned to targetPosition.
         This means the target position will be calculated by subtracting movementRange from startPosition.x.or vice versa*/
        float targetPosition = useFirstPosition ? (startPosition.x - movementRange) : (startPosition.x + movementRange);
        /*After determining the target position, useFirstPosition is negated using the !(logical NOT) operator. 
         This means if it was true, it will become false, and vice versa.This is done to toggle the value of useFirstPosition for the next iteration or usage.*/
        useFirstPosition = !useFirstPosition;
        float constantX = targetPosition;
        float randomX = Random.Range(startPosition.x - movementRange, startPosition.x + movementRange);
        if (canDoRandomMovement)
        {
            return new Vector3(randomX, transform.position.y, 0f);
        }
        else
        {       
            return new Vector3(constantX, transform.position.y, 0f);
        }
    }

    private void DamageTaken()
    {
        enemyCurHealth -= damageAmount;

        if(enemyCurHealth <= 0)
        {
            gameObject.GetComponent<Collider>().enabled = false;
            animator.SetBool(isDeadID, true);
            gameManager.CoinCollected(enemyKillValue);
            StartCoroutine(DestroyEnemy());
        }
        else
        {
            StartCoroutine(StartMovingAgain());
        }
    }
    IEnumerator StartMovingAgain()
    {
        yield return new WaitForSeconds(2f);
        animator.SetBool(isHitID, false);
        isHit = false;
    }
    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
}
