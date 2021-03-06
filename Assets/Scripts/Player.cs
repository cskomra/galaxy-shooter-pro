using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Components")]
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private int _score;
    private AudioManager _audioManager;
    private CameraShake _cameraShake;
    private GameManager _gameManager;


    [Header("Health")]
    [SerializeField] private int _lives = 3;
    private int _numHits = 0;
    [SerializeField]
    private int _healthPowerupPoints = 1;
    private PlayerHealth _playerHealth;

    [Header("Movement")]
    [SerializeField] private float _speed = 5f;
    private float _initialSpeed;
    [SerializeField] private GameObject _thruster;
    [SerializeField] private bool _thrusterIsAvailable = true;
    [SerializeField] private GameObject _rightEngine, _leftEngine;

    [Header("Power")]
    private float _powerupDuration = 5.0f;
    [SerializeField]
    private int _shotsRemaining = 15;
    [SerializeField] private int _ammoPowerupPoints = 10;
    private int _alienitePoints = 1;
    [SerializeField] private float _fireRate = 0.15f;
    private bool _laserPowerOn = false;
    public float nextFire = 0.0f;
    private float _laserPowerTime = 0.0f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _trippleShotPrefab;
    [SerializeField] private GameObject _laserPowerupPrefab;
    private bool _hasTrippleShot = false;
    [SerializeField] private GameObject _shield;
    private bool _hasShield = false;

    
    [Header("Audio")]
    [SerializeField] private AudioClip _powerupSound;
    [SerializeField] private AudioClip _laserShotSound;
    [SerializeField] private AudioClip _explosionSound;

    void Start()
    {
        _initialSpeed = _speed;

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if(!_spawnManager){
            Debug.LogError("Spawn Manager is NULL.");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(!_uiManager){
            Debug.LogError("UI Manager is NULL.");
        }

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if(!_gameManager){
            Debug.Log("Game Manager is NULL.");
        }

        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if(!_audioManager){
            Debug.LogError("Audio Manager is NULL.");
        }

        _playerHealth = GetComponent<PlayerHealth>();
        if(!_playerHealth){
            Debug.LogError("Player Health is NULL.");
        }

        _cameraShake = GameObject.FindWithTag("MainCamera").transform.GetComponent<CameraShake>();
        if(!_cameraShake){
            Debug.LogError("Main Camera is NULL.");
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
                    int randomWeight = Random.Range(0, 10);
                    if(randomWeight < 3 ) {
                        if(_laserPowerOn == false){
                            _laserPowerOn = true;
                            _laserPowerTime = Time.time + 5;
                        }
                        if(Time.time < _laserPowerTime){
                            Instantiate(_laserPowerupPrefab, offset, Quaternion.identity);
                        }else{
                            _laserPowerOn = false;
                            Instantiate(_laserPrefab, offset, Quaternion.identity);
                        }
                    }else{
                        Instantiate(_laserPrefab, offset, Quaternion.identity);
                    }
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

        //post position to Game Manager
        _gameManager.playerPosition = transform.position;
    }

    void ResetSpeed(){
        if(!_thrusterIsAvailable){
            StartCoroutine(ThrusterCoolDownRoutine());
        }
        _speed = _initialSpeed;
    }

    IEnumerator ThrusterCoolDownRoutine(){
        yield return new WaitForSeconds(1f);

        while(!_thrusterIsAvailable){
            _playerHealth.health += Time.deltaTime;
            //_uiManager.UpdateThrusterSlider(_thrusterPowerLevel);

            if(_playerHealth.health >= 100f){
            //    _uiManager.ThrusterSliderColor(Color.blue);
                _thrusterIsAvailable = true;
            }
            yield return null;
        }
    }

    void ManageSpeed(){
        float thrustSpeed = 4.0f;

        if(_thrusterIsAvailable){
            if(Input.GetKeyDown(KeyCode.LeftShift)){
                _speed += thrustSpeed;
                _thruster.SetActive(true);

                //Use thrusters 
                _playerHealth.health -= Time.deltaTime * 200.0f;

                // enter cool down
                if(_playerHealth.health <= 10f){
                    _thruster.SetActive(false);
                    _thrusterIsAvailable = false;
                    _playerHealth.health = 0;
                    ResetSpeed();
                }
            }

            if(Input.GetKeyUp(KeyCode.LeftShift)){
                _speed -= thrustSpeed;
                _thruster.SetActive(false);
            }
        }
        
    }

    public void Damage(string perpetrator){
        _audioManager.PlayAudio(_explosionSound); //also played in Enemy.OnCollision
        _numHits++;

        if(perpetrator == "AlienitedLaser"){
            AddToScore(-5);
        }

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
            _numHits = 0;
        }

        EngineManager(_lives);

        _cameraShake.StartShake();
                    
    }

    void EngineManager(int lives){
        switch(_lives){
            case 3: // both engines working
                _leftEngine.SetActive(false);
                _rightEngine.SetActive(false);
                break;
            case 2: // one working engine
                _leftEngine.SetActive(true);
                _rightEngine.SetActive(false);
                break;
            case 1: // no working engines
                _leftEngine.SetActive(true);
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
            case 3: //Ammo
                Debug.Log(powerupId + " Ammo Collected");
                AddToAmmo(_ammoPowerupPoints);
                break;
            case 4: //Health
                Debug.Log(powerupId + " Health Collected");
                AddToHealth(_healthPowerupPoints);
                break;
            case 5: //Health
                Debug.Log(powerupId + " Alienited");
                LoseAmmo(_alienitePoints);
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

    private void AddToAmmo(int ammo){
        _shotsRemaining += ammo;
        _uiManager.UpdateAmmo(_shotsRemaining);
    }

    public void LoseAmmo(int ammo){
        _shotsRemaining -= ammo;
        _uiManager.UpdateAmmo(_shotsRemaining);
    }
    //TODO - refactor to AmmoManager

    private void AddToHealth(int health){
        if(_lives < 3){
            _lives += health;
            _uiManager.UpdateLives(_lives);
            EngineManager(_lives);
        }
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
