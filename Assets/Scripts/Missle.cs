using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missle : MonoBehaviour
{
    private Rigidbody missleRb;
    public GameObject enemyInScene;
    public GameObject[] enemiesInScene;
    public float missleSpeed = 50f;
    public float misslepower = 50.0f;
    public PlayerController playerControllerScript;
    public float timer;
    public float rotateSpeed = 200;


    // Start is called before the first frame update
    void Start()
    {
        missleRb = GetComponent<Rigidbody>();
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HomingFunction();
        SelfDestruct();

    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            Vector3 collisionPoint = collision.gameObject.transform.position;
            Vector3 collisionDirection = (collisionPoint - transform.position).normalized;
            collision.gameObject.GetComponent<Rigidbody>().AddForce(collisionDirection * misslepower);
            playerControllerScript.missleCount -= 1;

            Destroy(gameObject);
        }
    }

    void HomingFunction()
    {
        enemyInScene = GameObject.FindGameObjectWithTag("Enemy");

        {
            Vector3 enemyDirection = (enemyInScene.transform.position - missleRb.position).normalized;
            Vector3 rotateAmount = Vector3.Cross(enemyDirection, transform.up);
            Vector3 missleDirection = (enemyDirection - missleRb.position);
            
            missleRb.AddForce(enemyDirection * missleSpeed, ForceMode.Force);
            missleRb.angularVelocity = rotateAmount * rotateSpeed;
        }
    }


    void SelfDestruct()
    {
        timer += Time.deltaTime;

        if(timer >= 5)
        {
            Destroy(gameObject);
        }
    }

}

