using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Managers")]
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private int _score;
    private AudioManager _audioManager;

    [Header("Health")]
    [SerializeField] private int _lives = 3;
    private int _numHits = 0;

    [Header("Movement")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private GameObject _thruster;
    [SerializeField] private GameObject _rightEngine, _leftEngine;

    [Header("Power")]
    private float _powerupDuration = 5.0f;
    [SerializeField]
    private int _shotsRemaining = 15;
    [SerializeField] private float _fireRate = 0.15f;
    private float nextFire = 0.0f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _trippleShotPrefab;
    private bool _hasTrippleShot = false;
    [SerializeField] private GameObject _shield;
    private bool _hasShield = false;

    
    [Header("Audio")]
    [SerializeField] private AudioClip _powerupSound;
    [SerializeField] private AudioClip _laserShotSound;
    [SerializeField] private AudioClip _explosionSound;

    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if(!_spawnManager){
            Debug.LogError("Spawn Manager is NULL.");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(!_uiManager){
            Debug.LogError("UI Manager is NULL.");
        }

        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if(!_audioManager){
            Debug.LogError("Audio Manager is NULL.");
        }

        transform.position = new Vector3(0, -3.8f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();  

        FireLaser();

    }

    void FireLaser(){
        if(_shotsRemaining > 0){
            Vector3 offset = transform.position + new Vector3(0, 1.1f, 0);
            if(Input.GetKeyDown(KeyCode.Space) && Time.time > nextFire){
                nextFire = Time.time + _fireRate;
                if(_hasTrippleShot){
                    // fire 3 lasers
                    Instantiate(_trippleShotPrefab, offset, Quaternion.identity);  
                }else{
                    // fire 1 laser
                    Instantiate(_laserPrefab, offset, Quaternion.identity);
                }
                _audioManager.PlayAudio(_laserShotSound);
                ManageAmmo();
            }
        }else{
            Debug.Log("Out of AMMO!");
        }
        
    }

    void CalculateMovement(){
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float upperBounds = 0f;
        float lowerBounds = -3.8f;
        float rightBounds = 11.3f;
        float leftBounds = -11.3f;

        ManageSpeed();

        // move player
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);

        //restrict bounds
        if(transform.position.y >= upperBounds){
            transform.position = new Vector3(transform.position.x, upperBounds, 0);
        }else if(transform.position.y <= lowerBounds){
            transform.position = new Vector3(transform.position.x, lowerBounds, 0);
        }

        if(transform.position.x >= rightBounds){
            transform.position = new Vector3(-rightBounds, transform.position.y, 0);
        }else if(transform.position.x <= leftBounds){
            transform.position = new Vector3(-leftBounds, transform.position.y, 0);
        }
    }

    void ManageSpeed(){
        float thrustSpeed = 4.0f;

        if(Input.GetKeyDown(KeyCode.LeftShift)){
            _speed += thrustSpeed;
            _thruster.SetActive(true);
        }

        if(Input.GetKeyUp(KeyCode.LeftShift)){
            _speed -= thrustSpeed;
            _thruster.SetActive(false);
        }
    }

    public void Damage(){
        _numHits++;
        Debug.Log("numHits = " + _numHits);
        if(_hasShield){
            switch(_numHits){
                case 2: //1st hit, change color to #096cf7
                    _shield.GetComponent<SpriteRenderer>().color = Color.blue;
                    break;
                case 4: //2nd hit, change color to #356dba
                    _shield.GetComponent<SpriteRenderer>().color = Color.cyan;
                    break;
                case 6: //3rd hit, 
                    _numHits = 0;
                    _hasShield = false;
                    _shield.SetActive(false);
                    break;
                default:
                    break;
            }
            return;
        }

        if(_numHits == 2){
            _lives -= 1;
            _uiManager.UpdateLives(_lives);  
            Debug.Log("Lives: " + _lives);
            _numHits = 0;
        }

        switch(_lives){
            case 2: //first hit
                _leftEngine.SetActive(true);
                break;
            case 1: //second hit
                _rightEngine.SetActive(true);
                break;
            case 0:
                _uiManager.GameOver();

                Destroy(this.gameObject);
                _audioManager.PlayAudio(_explosionSound);

                if(_spawnManager){
                    _spawnManager.OnPlayerDeath();
                }
                break;
            default:
                break;
        }             
    }

    public void PowerUp(int powerupId){
        _audioManager.PlayAudio(_powerupSound);
        switch(powerupId){
            case 0: //Triple shot
                Debug.Log(powerupId + " Triple Shot");
                _hasTrippleShot = true;
                StartCoroutine(PowerDown(powerupId, _powerupDuration));
                break;
            case 1: //Speed boost
                Debug.Log(powerupId + " Speed Boost");
                _speed = 10f;
                StartCoroutine(PowerDown(powerupId, _powerupDuration));
                break;
            case 2: //Shield
                Debug.Log(powerupId + " Shield Collected");
                _hasShield = true;
                //enable shield visualizer
                _shield.SetActive(true);
                break;
            default:
                Debug.Log("unknown powerup");
                break;
        }
    }

    private IEnumerator PowerDown(int powerupId, float waitTime){
        yield return new WaitForSeconds(waitTime);
        switch(powerupId){
            case 0:
                _hasTrippleShot = false;
                break;
            case 1:
                _speed = 5f;
                break;
            case 2:
                _hasShield = false;
                _shield.SetActive(false);
                break;
        }
    }

    public void AddToScore(int points){
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void ManageAmmo(){
        _shotsRemaining--;
        _uiManager.UpdateAmmo(_shotsRemaining);
        //TODO: if 10 shots remaining, text color = yello
        //TODO: if 5 shots remaining, text color = red
        //TODO: if 3 shots remaining, blink text
        //TODO: if 0 shots remaining, stop blink
    }

}
