using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _laserAlienitedPrefab;
    private AudioManager _audioManager;

    private Rigidbody2D enemyRb;

    [SerializeField] private AudioClip _explosionSound;
    [SerializeField] private AudioClip _laserSound;
    private bool _laserIsAlienited = false;
    
    [SerializeField] private float _speed = 4.0f;
    private float _gravitateSpeed = 0.6f;

    private const int _POINTS = 10;
    private const int _POWERPOINTS = 15;
    private Player _playerScript;
    
    private SpawnManager _spawnManager;

    private Animator _enemyAnimator;

    private float _fireRate = 3.0f;
    private float _canFire = -1f;
    private int _counter = 0;
    private int _direction = 0;
    public int currWaveEnemyCount = 1;
    [SerializeField] public GameObject enemyShield;
    private float _distanceToPlayer = 0f;
    private GameManager _gameManager;

    void Start(){
        _playerScript = GameObject.Find("Player").transform.GetComponent<Player>();
        if(!_playerScript){
            Debug.LogError("Player is NULL");
        }

        _spawnManager = GameObject.Find("Spawn_Manager").transform.GetComponent<SpawnManager>();
        if(!_spawnManager){
            Debug.LogError("Spawn_Manager is NULL");
        }

        _enemyAnimator = GetComponent<Animator>();
        if(!_enemyAnimator){
            Debug.LogError("Enemy Animator is NULL");
        }

        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if(!_audioManager){
            Debug.LogError("Audio Manager is NULL.");
        }

        enemyRb = GetComponent<Rigidbody2D>();
        if(!enemyRb){
            Debug.LogError("Enemy RigidBody is NULL.");
        }

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if(!_gameManager){
            Debug.Log("Game Manager is NULL.");
        }

    }

    void Update()
    {   
        
        
        
        if(!_gameManager.isGameOver()){
            _distanceToPlayer = Vector3.Distance(this.transform.position, _gameManager.playerPosition);
            if( _distanceToPlayer <= 5.0f){
                enemyRb.AddForce((_gameManager.playerPosition - transform.position) * _gravitateSpeed);
            }else{
                if(_counter == 60){
                    int randomInt = Random.Range(0,3);
                    _direction = randomInt;
                    _counter = 0;
                }else{
                    _counter++;
                }
                ChangeDirection(_direction);
            }
        }
        
        RespawnIfOutOfBounds();

        if(Time.time > _canFire){
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser;
            Vector3 currPostion = transform.position;
            if(_laserIsAlienited){
                enemyLaser = Instantiate(_laserAlienitedPrefab, currPostion, Quaternion.identity);
            }else{
                enemyLaser = Instantiate(_laserPrefab, currPostion, Quaternion.identity);
            }
            
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for(int i = 0; i < lasers.Length; i++){
                bool hasReverseFire = false;
                if(tag == "SmartEnemy" && (currPostion.y < _gameManager.playerPosition.y)){
                    hasReverseFire = true;
                }
                lasers[i].ActivateEnemyLaser(hasReverseFire);
            }
        }
    }

    void CalculateMovement(){
        if(transform.position.y < -4f){
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 5f, 0);
        }
    }

    void RespawnIfOutOfBounds(){
        if(transform.position.y < -8f || transform.position.y > 8 || transform.position.x < -11 || transform.position.x > 11){ 
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 5f, 0);
        }
    }

    void ChangeDirection(int caseNum){

        switch(caseNum){
            case 0:
                transform.Translate(Vector3.down * _speed * Time.deltaTime);
                break;
            case 1:
                transform.Translate(Vector3.up * _speed * Time.deltaTime);
                break;
            case 2:
                transform.Translate(Vector3.left * _speed * Time.deltaTime);
                break;
            case 3:
                transform.Translate(Vector3.right * _speed * Time.deltaTime);
                break;
            default:
                transform.Translate(Vector3.down * _speed * Time.deltaTime);
                break;
        }

    }

    private void OnTriggerEnter2D(Collider2D other){
             
        if(other.tag == ("Player")){
            _audioManager.PlayAudio(_explosionSound);

            _enemyAnimator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            
            Destroy(GetComponent<Collider2D>(), 2f);
            Destroy(this.gameObject, 2f);
            if(_playerScript){
                _playerScript.Damage(this.tag);
            }
            _spawnManager._enemyCount--;
        }
        else if(other.tag == ("Laser") || other.tag == "LaserPowerup"){

            if(enemyShield.activeSelf){
                enemyShield.SetActive(false);
            }else{
                _audioManager.PlayAudio(_explosionSound);

                Destroy(other.gameObject);
                _enemyAnimator.SetTrigger("OnEnemyDeath");
                _speed = 0;
                
                Destroy(GetComponent<Collider2D>(), 2f);
                Destroy(this.gameObject, 2f);
                
                if(other.tag == "Laser"){
                    _playerScript.AddToScore(_POINTS);
                }else{
                    _playerScript.AddToScore(_POWERPOINTS);
                }
                _spawnManager._enemyCount--;
            }
        }
        else if(other.tag == "Alienite"){
            _audioManager.PlayAudio(_explosionSound);
            _laserIsAlienited = true;
        }
        
    }
}
