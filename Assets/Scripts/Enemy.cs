using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private GameObject _laserPrefab;
    private AudioManager _audioManager;

    [SerializeField]
    private AudioClip _explosionSound;
    [SerializeField]
    private AudioClip _laserSound;

    [SerializeField]
    private float _speed = 4.0f;

    private const int _POINTS = 10;
    private const int _POWERPOINTS = 15;
    private Player _player;

    private Animator _enemyAnimator;

    private float _fireRate = 3.0f;
    private float _canFire = -1f;
    private int _counter = 0;
    private int _direction = 0;

    void Start(){
        _player = GameObject.Find("Player").transform.GetComponent<Player>();
        if(!_player){
            Debug.LogError("Player is NULL");
        }

        _enemyAnimator = GetComponent<Animator>();
        if(!_enemyAnimator){
            Debug.LogError("Enemy Animator is NULL");
        }

        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if(!_audioManager){
            Debug.LogError("Audio Manager is NULL.");
        }

    }

    void Update()
    {
        
        if(_counter == 60){
            int randomInt = Random.Range(0,3);
            _direction = randomInt;
            _counter = 0;
        }else{
            _counter++;
        }

        ChangeDirection(_direction);
        CalculateMovement();

        if(Time.time > _canFire){
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for(int i = 0; i < lasers.Length; i++){
                lasers[i].ActivateEnemyLaser();
            }
        }
    }

    void CalculateMovement(){

        if(transform.position.y < -4f){
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 5f, 0);
        }
    }

    void LookAtPlayer(){
        Vector3 playerPosition = _player.transform.position;
        playerPosition.y = transform.position.y;
        transform.LookAt(playerPosition);
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
        
        _audioManager.PlayAudio(_explosionSound);
        
        if(other.tag == ("Player")){
            _enemyAnimator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            
            Destroy(GetComponent<Collider2D>(), 2f);
            Destroy(this.gameObject, 2f);
            if(_player){
                _player.Damage();
            }
            
        }
        else if(other.tag == ("Laser") || other.tag == "LaserPowerup"){
            
            Destroy(other.gameObject);
            _enemyAnimator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            
            Destroy(GetComponent<Collider2D>(), 2f);
            Destroy(this.gameObject, 2f);
            
            
            if(other.tag == "Laser"){
                Debug.Log("Adding to Score");
                _player.AddToScore(_POINTS);
            }else{
                Debug.Log("POWER POINTS!!!!!");
                _player.AddToScore(_POWERPOINTS);
            }
            
        }
    }
}
