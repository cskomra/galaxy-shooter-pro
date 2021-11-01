using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //handle to Text
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _ammoText;
    [SerializeField] private Text _healthText;
    [SerializeField] private Text _restartText;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Image _livesImage;
    [SerializeField] private Sprite[] _livesSprites;

    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _livesImage.sprite = _livesSprites[3];
        _scoreText.text = "Score: 0";
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if(!_gameManager){
            Debug.Log("Game Manager is NULL.");
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateScore(int score){
        _scoreText.text = "Score: " + score.ToString();
    }

    public void UpdateAmmo(int ammo){
        _ammoText.text = "Ammo: " + ammo.ToString();
    }

    public void UpdateLives(int currentLives){
        if(!_gameManager.isGameOver()){
            if(_livesImage){
                _livesImage.sprite = _livesSprites[currentLives];
            }else{
                Debug.Log("Lives Image is NULL");
            }
        }
    }

    public void GameOver(){
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        _gameManager.GameOver();
        StartCoroutine(FlickerGameOver());
    }

    private IEnumerator FlickerGameOver(){
        while(true){
            _gameOverText.text = _gameOverText.text == "" ? "Game Over!" : "";
            yield return new WaitForSeconds(0.25f);
        }
    }
}
