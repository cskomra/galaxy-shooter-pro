using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
    //[SerializeField] private Slider _thrusterSlider;
    //[SerializeField] private GameObject _thrusterSliderFill;
    //private Image _thrusterSliderFillImage;

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

        /* _thrusterSliderFillImage = GameObject.Find("Fill").GetComponent<UnityEngine.UI.Image>();
        if(!_thrusterSliderFillImage){
            Debug.Log("Fill Image is NULL.");
        }

        _thrusterSliderFillImage.color = Color.blue; */

        /* _thrusterSlider = GameObject.Find("Thruster_Slider").GetComponent<UnityEngine.UI.Slider>();
        if(!_thrusterSlider){
            Debug.Log("Slider is NULL.");
        } */
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

    /* public void UpdateThrusterSlider(float val){
        Debug.Log("UPDATE SLIDER: " + val.ToString());
        _thrusterSlider.value = val;
        // fill.fillAmount
        _thrusterSliderFill.GetComponent<Image>().fillAmount = val;
        Debug.Log("Thruster VAL: " + _thrusterSlider.value.ToString());
    }

    public void ThrusterSliderColor(Color sliderColor){        
        _thrusterSliderFillImage.color = sliderColor;
    } */

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
