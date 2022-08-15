using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10;
    private Rigidbody playerRb;
    private GameObject focalPoint;
    private float powerupStrength;
    public bool hasPowerup = false;
    public GameObject powerupIndicator;
    public float powerupRotationSpeed = 2.0f;
    public float powerupTimer = 10;
    public int missleCount;
    private Vector3 missleFireOffset = new Vector3(0f, 1.5f, 0f);
    public int breakFriction = 2;
    public bool isOnGround = false;

    public GameObject misslePrefab;
    public Rigidbody missleRb;
    public float fallingVelocity; 
    private Vector3 lastposition;

    // Homing variables
    public GameObject[] enemiesInScene;
    public float missleSpeed = 10f;


    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();    
        focalPoint = GameObject.Find("FocalPoint");    
        powerupStrength = speed;
        lastposition = transform.position;  // sets lastPosition for DetectFalling()
        missleRb = misslePrefab.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");

        playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed);

        powerupIndicator.transform.position = transform.position;
        powerupIndicator.transform.Rotate(0, powerupRotationSpeed, 0);

        FireMissle();
        DetectAirborne();
        SlowToStop();

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Powerup") && !hasPowerup)
        {
            hasPowerup = true;
            powerupIndicator.gameObject.SetActive(true);
            Destroy(other.gameObject);
            StartCoroutine(PowerupCountDownRoutine());
        }
    }

    IEnumerator PowerupCountDownRoutine()
    {
        yield return new WaitForSeconds(powerupTimer);
        hasPowerup = false;
        powerupIndicator.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy") && hasPowerup)
        {
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;
            enemyRb.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            Debug.Log("Collided with : " + collision.gameObject.name + " and powerup set to " + hasPowerup);
        }


        if(collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
        // playerRB moves up and down during horizontal movement
        // setting the velocity trigger to 4 ensure it occurs with large movements.

    }

    // The script below should calculate the rotation between the player and the enemy
    // and store the result in missleAngle
    // and Instantiate the missle pointing in it's direction.
    void FireMissle()
    {        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            missleCount++;
            Quaternion missleAngle;

            enemiesInScene = GameObject.FindGameObjectsWithTag("Enemy");
            for(int i = 0; i < 1; i++)
            {
                Vector3 enemyDirection = (enemiesInScene[i].transform.position - transform.position).normalized;
                Vector3 rotateAmount = Vector3.Cross(enemyDirection, transform.up);
                Vector3 missleDirection = (enemyDirection - playerRb.position);

                missleAngle = new Quaternion(rotateAmount.x, rotateAmount.y, rotateAmount.z, 0);

                Instantiate(misslePrefab, transform.position + missleFireOffset, missleAngle);
            }
        }
    }

    void SlowToStop()
    {
        if(Input.GetKey(KeyCode.E) && isOnGround)
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x / breakFriction, playerRb.velocity.y / breakFriction, playerRb.velocity.z / breakFriction);
        }
    }

    void DetectAirborne()
    {
        fallingVelocity = (transform.position.y - lastposition.y);
        lastposition = transform.position;

        if(fallingVelocity > .1f || fallingVelocity < -.1f)
        {
            isOnGround = false;
        }

        // // The below script caluclates the speed between current and last position.
        // fallingVelocity = (transform.position.y - lastposition.y);
        // lastposition = transform.position;

        // if(fallingVelocity != 0)
        // {
        //     isOnGround = false;
        // }
    }


    // // Identifies enemies in scene
    // // Compares their distance to the player
    // // Returns the index of the shortest distance enemy
    // // Sorting to lowest could use improvement
    // public int Proximity(int closestEnemy, GameObject[] enemiesInScene)
    // {
    //     closestEnemy = 0;
    //     for(int i = 0; i < enemiesInScene.Length; i++)
    //     {
    //         Vector3 enemyDistance = (enemiesInScene[i].transform.position - transform.position);

    //         if(enemyDistance[i] < enemyDistance[i+1])
    //         {
    //             closestEnemy = i;
    //         }
    //     }

    //     return closestEnemy;
    // }


}
