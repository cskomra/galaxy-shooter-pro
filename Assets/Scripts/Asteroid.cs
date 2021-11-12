using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private AudioManager _audioManager;

    [SerializeField]
    private AudioClip _explosionSound;

    [SerializeField]
    private float _rotateSpeed = 19.0f;

    [SerializeField]
    private GameObject _explosion;

    private GameObject _asteroidExplosion;

    private Animator _explosionAnim;

    private SpawnManager _spawnManager;

    void Start(){
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if(!_spawnManager){
            Debug.Log("Spawn Manager is NULL.");
        }

        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if(!_audioManager){
            Debug.LogError("Audio Manager is NULL.");
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other){
        Debug.Log("Asteroid Hit!");
        if(other.tag == "Laser" || other.tag == "LaserPowerup"){
            _audioManager.PlayAudio(_explosionSound);
            _asteroidExplosion = Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            Destroy(this.gameObject, 0.6f);
        }
    }

}
