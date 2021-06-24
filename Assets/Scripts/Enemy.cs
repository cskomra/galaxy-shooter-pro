using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    public GameObject _enemyPrefab;

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

    private void OnTriggerEnter(Collider other){
        
        if(other.tag == ("Player")){
            Player player = other.transform.GetComponent<Player>();
            if(player){
                player.Damage();
            }
            Destroy(this.gameObject);
        }
        else if(other.tag == ("Laser")){
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }

    }
}
