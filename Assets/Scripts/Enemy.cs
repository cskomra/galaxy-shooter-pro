using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    public GameObject _enemyPrefab;

    private const int _POINTS = 10;
    private Player _player;

    void Start(){
        _player = GameObject.Find("Player").transform.GetComponent<Player>();
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
        //Player player;
        if(other.tag == ("Player")){
            //player = other.transform.GetComponent<Player>();
            if(_player){
                _player.Damage();
            }
            Destroy(this.gameObject);
        }
        else if(other.tag == ("Laser")){
            // get handle to player
            //player = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Player>();
            Destroy(other.gameObject);
            Destroy(this.gameObject);
            //increase player.score
            if(_player){
                Debug.Log("Adding to Score");
                _player.AddToScore(_POINTS);
            }
        }

    }
}
