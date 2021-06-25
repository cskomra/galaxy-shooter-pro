using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // move down at speed of 3 (adjust in the inspector)
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        // when leave screen, destry self
        if(transform.position.y < -4.5f){
            Destroy(this.gameObject);
        }
    }

    //OnTriggerCollision
    private void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Player"){
            //Powerup collected! ...enable powerup
            Player player = other.transform.GetComponent<Player>();
            if(player){
                player.setPowerUpOn();
            }
            Destroy(this.gameObject);
        }
    }
}
