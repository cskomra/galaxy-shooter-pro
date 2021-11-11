using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float health;
    [SerializeField] private Slider _healthSlider;

    // Start is called before the first frame update
    void Start()
    {
        health = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        _healthSlider.value = health;

        /* if((int)_healthSlider.value < 1){
            _healthSlider.image.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time, 1f));
        }else{
            _healthSlider.image.color = Color.Lerp(Color.red, Color.green, currentHealth / maxHealth);
        } */
    }

    void TestHealthSlider(){
        //test slider
        if(Input.GetKeyDown(KeyCode.Z)) {if (health >= 0) health -= 1;}
        if(Input.GetKeyDown(KeyCode.X)) {if (health <=100) health += 1;}
    }
}
