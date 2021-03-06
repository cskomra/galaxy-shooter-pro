using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool _isGameOver;
    //public float playerYPosition;
    public Vector3 playerPosition;

    private void Update(){
        if(Input.GetKeyDown(KeyCode.R) && _isGameOver){
            SceneManager.LoadScene(1); //Game Scene
        }

        //if escape key pressed, quit application
        if(Input.GetKey(KeyCode.Escape)){
            if(EditorApplication.isPlaying){
                EditorApplication.isPlaying = false;
            }
            Application.Quit();
        }

    }

    public void GameOver(){
        Debug.Log("GAME OVER!!!!!!!!!!!!");
        _isGameOver = true;
    }

    public bool isGameOver(){
        return _isGameOver;
    }
}
