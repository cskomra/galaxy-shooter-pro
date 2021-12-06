using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;
    private bool _isEnemyLaser = false;

    private bool _laserBehindPlayer;
    private bool _firedFromBehind;

    private GameObject player;
    private GameManager _gameManager;

    void Start(){
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if(!_gameManager){
            Debug.Log("Game Manager is NULL.");
        }

        if(!_gameManager.isGameOver()){
            player = GameObject.Find("Player");
            if(!player){
                Debug.LogError("player not found");
            }
        }
    }

    void Update()
    {   
        if(_isEnemyLaser){
            MoveDown();

        }else{
            MoveUp();
        }
    }

    void MoveUp(){
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if(transform.position.y > 8.0f){
            if(transform.parent){
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    void MoveDown(){      
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if(transform.position.y < -8.0f){
            if(transform.parent){
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }else{
            //TODO: get player position from GameManager
            _laserBehindPlayer = transform.position.y < _gameManager.playerPosition.y;
            Debug.Log("laserBehindPlayer: " + _laserBehindPlayer);  
            //Debug.Log("player.transform.position.y: " + player.transform.position.y);  
        }
    }

    public void ActivateEnemyLaser(bool hasReverseFire){
        _isEnemyLaser = true;
        if(hasReverseFire){
            _isEnemyLaser = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Player" && _isEnemyLaser == true){
            Player player = other.GetComponent<Player>();
            if(player){
                player.Damage(this.tag);
            } 
        }
    }

}
