using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    private Vector3 _cameraOriginalPosition;
    private float _shakeDuration = 0.5f;
    private float _shakeMagnitude = 0.04f;
    private bool _shake = false;

    // Start is called before the first frame update
    void Start()
    {
        _cameraOriginalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(_shake){
            if(_shakeDuration > 0){
                transform.position = transform.position + Random.insideUnitSphere * _shakeMagnitude;
                _shakeDuration -= Time.deltaTime;
            }else{
                transform.position = _cameraOriginalPosition;
                _shake = false;
            }
        }
    }

    public void StartShake(){
        _shake = true;
        _shakeDuration = 0.5f;
    }
}
