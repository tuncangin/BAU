using TMPro;
using UnityEngine;


public class UIManager : MonoBehaviour //UI yönetiminden sorumlu 
{
    public static UIManager instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("Game Scenes")]
    public GameObject MainMenu;
    public GameObject levelCompletedScene;
    public GameObject levelFailedScene;
    public GameObject InGameScene;
    public GameObject PauseScene;

    [Header("Player Scores")] 
    public int PlayerScore; //Level create ile beraber lastscore a eşitlenip devam etmeli ve level complete gelirse lastscore'u değiştirmeli, fail olursa lastscore'a eşitlenmeli
    public int LastScore = 0; //LevelCompleted fonkisyonu içinde kaydolmalı
    public TextMeshProUGUI Score;


    public void Collected() //Collectible alındığında çağırılıyor, skoru arttırıp ekrana yazdırıyor
    {
        PlayerScore+=10;
        Score.text = PlayerScore.ToString();
    }
    
    public void LevelFailed() //Level kaybedildiğinde game manager tarafından çağırılır gerekli sahne düzenlemelerini yapar
    {
        PlayerScore = LastScore;
        Score.text = PlayerScore.ToString();
        InGameScene.SetActive(false);
        levelFailedScene.SetActive(true);
    }
    
    public void LevelCompleted() //Level kazanıldığında game manager tarafından çağırılır gerekli sahne düzenlemelerini yapar
    {
        LastScore = PlayerScore;
        Score.text = PlayerScore.ToString();
        InGameScene.SetActive(false);
        levelCompletedScene.SetActive(true);
        
    }
    

    public void ContinueButton() //Pause menudeki continue butonu gerekli sahne düzenlemelerini yapar, time scalei tekrar 1 e çeker
    {
        InGameScene.SetActive(true);
        PauseScene.SetActive(false);
        Time.timeScale = 1;
    }

    public void NextLevelButton() //Level completed ekranındaki next level butonu gerekli sahne düzenlemelerini yapar, yeni level çağırır
    {
        levelCompletedScene.SetActive(false);
        InGameScene.SetActive(true);
        GameManager.instance.NewGameSettings();
    }

    public void RestartLevelButton() //Level failed ekranındaki restart butonu gerekli sahne düzenlemelerini yapar, mevcut levelı yeniden çağırır
    {
        Score.text = PlayerScore.ToString();
        levelFailedScene.SetActive(false);
        InGameScene.SetActive(true);
        GameManager.instance.NewGameSettings();
    }

    public void PlayButton() //Ana menüden play'e basıldığında sahne değiştirip yeni level fonksiyonunu çağırır
    {
        MainMenu.SetActive(false);
        InGameScene.SetActive(true);
        GameManager.instance.NewGameSettings();
    }

    

    public void PauseButton()  //oyun ekranındadki pause butonu gerekli sahne düzenlemelerini yapar, timescalei 0 yapar oyunu durdurur.
    { 
        InGameScene.SetActive(false);
        PauseScene.SetActive(true);
        Time.timeScale = 0;
    }

    public void ExitButton() //oyundan çıkış
    {
        Application.Quit();
    }
}
