using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _trippleShotPrefab;

    [SerializeField]
    private GameObject _shield;

    [SerializeField]
    private float _fireRate = 0.15f;
    private float nextFire = 0.0f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    private float _powerupDuration = 5.0f;

    private bool _hasTrippleShot = false;
    private bool _hasShield = false;
    private int _score;

    [SerializeField]
    private AudioManager _audioManager;

    [SerializeField]
    private GameObject _rightEngine, _leftEngine;
    
    [Header("Audio and Sound Effects")]
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _laserShotSound;
    [SerializeField]
    private AudioClip _explosionSound;

    // Start is called before the first frame update
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

        _audioSource = GetComponent<AudioSource>();
        if(!_audioSource){
            Debug.LogError("'_audioSource' is NULL.");
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
            _audioSource.clip = _laserShotSound;
            _audioSource.Play();
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


    public void Damage(){
        
        if(_hasShield){
            _hasShield = false;
            return; 
        }
        _lives -= 1;
        Debug.Log("Lives: " + _lives);
        _uiManager.UpdateLives(_lives);  
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
        
        switch(powerupId){
            case 0: //Triple shot
                Debug.Log(powerupId + " Triple Shot");
                _hasTrippleShot = true;
                break;
            case 1: //Speed boost
                Debug.Log(powerupId + " Speed Boost");
                _speed = 10f;
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
        StartCoroutine(PowerDown(powerupId, _powerupDuration));
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
}
