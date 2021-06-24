using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _firerate = 0.15f;
    private float nextFire = 0.0f;
    [SerializeField]
    private int _lives = 3;
    private GameObject _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.FindGameObjectWithTag("SpawnManager");
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();  

        FireLaser();
    }

    void FireLaser(){
        Vector3 offset = transform.position + new Vector3(0, 0.8f, 0);
        if(Input.GetKeyDown(KeyCode.Space) && Time.time > nextFire){
            nextFire = Time.time + _firerate;
            Instantiate(_laserPrefab, offset, Quaternion.identity);
        }  
    }

    void CalculateMovement(){
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float upperBounds = 0f;
        float lowerBounds = -3.8f;
        float rightBounds = 11.3f;
        float leftBounds = -11.3f;

        // move player
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);

        //restrict bounds
        if(transform.position.y >= upperBounds){
            transform.position = new Vector3(transform.position.x, upperBounds, transform.position.z);
        }else if(transform.position.y <= lowerBounds){
            transform.position = new Vector3(transform.position.x, lowerBounds, transform.position.z);
        }

        if(transform.position.x >= rightBounds){
            transform.position = new Vector3(-rightBounds, transform.position.y, transform.position.z);
        }else if(transform.position.x <= leftBounds){
            transform.position = new Vector3(-leftBounds, transform.position.y, transform.position.z);
        }
    }


    public void Damage(){
        _lives -= 1;
        if(_lives == 0){
            Debug.Log("You died!");
            Destroy(this.gameObject);
            // when player dies, stop spawing...
            if(_spawnManager){
                _spawnManager.transform.GetComponent<SpawnManager>().OnPlayerDeath();
            }
        }
    }
}
