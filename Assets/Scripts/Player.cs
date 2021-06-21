using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        // set the player's starting position
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // move player to the right
        transform.Translate(Vector3.left * _speed * Time.deltaTime);
    }
}
