using UnityEngine;

public class Enemy : MonoBehaviour
{
    private AudioManager _audioManager;

    [SerializeField]
    private AudioClip _explosionSound;

    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    public GameObject _enemyPrefab;

    private const int _POINTS = 10;
    private Player _player;

    //handle to animator componsent
    private Animator _enemyAnimator;

    void Start(){
        _player = GameObject.Find("Player").transform.GetComponent<Player>();
        if(!_player){
            Debug.LogError("Player is NULL");
        }

        //assign component
        _enemyAnimator = GetComponent<Animator>();
        if(!_enemyAnimator){
            Debug.LogError("Enemy Animator is NULL");
        }

        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if(!_audioManager){
            Debug.LogError("Audio Manager is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // move down at 4m/sec
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        // if bottom of screen(-6.5), respown at top(6.5)
        if(transform.position.y < -4f){
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 5f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        
        _audioManager.PlayAudio(_explosionSound);
        
        if(other.tag == ("Player")){
            _enemyAnimator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2f);
            if(_player){
                _player.Damage();
            }
            
        }
        else if(other.tag == ("Laser")){
            
            Destroy(other.gameObject);
            _enemyAnimator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2f);
            
            Debug.Log("Adding to Score");
            _player.AddToScore(_POINTS);
        }
    }
}
