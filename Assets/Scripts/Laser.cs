using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;
    private bool _isEnemyLaser = false;

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
        }
    }

    public void ActivateEnemyLaser(){
        _isEnemyLaser = true;
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
