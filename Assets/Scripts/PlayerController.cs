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

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();    
        focalPoint = GameObject.Find("FocalPoint");    
        powerupStrength = speed;
    }

    // Update is called once per frame
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");

        playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed);

        powerupIndicator.transform.position = transform.position;
        powerupIndicator.transform.Rotate(0, powerupRotationSpeed, 0);
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
    }

}
