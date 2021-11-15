using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alienite : MonoBehaviour
{
    private Player _player;
    [SerializeField] private float _speed = 5.0f;
    private const int _ALIENITEAMMO = 1;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").transform.GetComponent<Player>();
        if(!_player){
            Debug.LogError("Player is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement(){

        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if(transform.position.y < -4f){
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 5f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.tag == ("Player")){
            Destroy(GetComponent<Collider2D>(), 2f);
            Destroy(this.gameObject, 2f);
        }
        else if(other.tag == ("Enemy")){            
            Debug.Log("Enemy Alienited");
        }
    }
}
