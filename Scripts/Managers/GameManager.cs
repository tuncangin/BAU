using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour //Oyun içi yönetimden sorumlu
{
    public static GameManager instance //singleton
    {
        get;
        private set;
    }
    
    [Header("Level Details")]
    public int levelNumber=0;
    public List<Levels> levels = new List<Levels>();
    private GameObject currentLevel;
    public GameObject LevelTransform;
    public GameObject Player;
    public int Score;

    private void Awake()
    {
        if (!instance) //singleton için gerekli düzenlemeler
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void NewGameSettings() //yeni level üretirken gerekli ayar varsa onları yapar, şimdilik sadece create ediyor ama ihtiyaç olursa bu fonksiyon doldurulabilir. 
    {
        CreateLevel();
    }
    
    public void CreateLevel()
        
        /*level üretir, level listesi üzerinden sırayla üretir. Eğer sona geldiyse tekrar ilk levela döner. Oyun kaydedilmez. Her session yeni baştan başlar. 
    Kaydetmek istesek playerprefs librarysini kullanıp level number verisini kaydedebiliriz. */
    
    {
        if (currentLevel)
        {
            Destroy(currentLevel);
        }
        currentLevel = Instantiate(levels[levelNumber].LevelObject, Vector3.zero, Quaternion.identity,LevelTransform.transform);
        Player.SetActive(true);
        Player.GetComponent<PlayerMovement>().SetDefaultSettings();
    }
    
    
    
    public void LevelCompleted()//Level complete olduğunda çağırılıyor 
    {
        StartCoroutine(LevelCompletedCo());//Görsel olarak hemen geçmemesi için delay konuldu
    }

    IEnumerator LevelCompletedCo(float delayTime = 1.5f) //Delay default olarak 1.5 saniye olarak ayarlandı
    {
        yield return new WaitForSeconds(delayTime);
        _LevelCompleted();
    }
    
    private void _LevelCompleted() //level bittiğinde çağırılır level sayısını arttırıp uidan gerekli fonksiyonları çağırır
    {
        levelNumber++;
        if (levelNumber == levels.Count)
        {
            levelNumber = 0;
        }
        UIManager.instance.LevelCompleted();
    }

    
    
    public void LevelFailed()  //Level fail olduğunda çağırılıyor 
    {
        StartCoroutine(LevelFailedCo()); //Görsel olarak hemen geçmemesi için delay konuldu
    }

    IEnumerator LevelFailedCo(float delayTime = 1.5f)  //Delay default olarak 1.5 saniye olarak ayarlandı
    {
        yield return new WaitForSeconds(delayTime);
        _LevelFailed();
    }

   

    private void _LevelFailed() //level kaybedildiğinde çağırlır level sayısını değiştirmez uidan gerekli fonksiyonları çağırır
    {
        UIManager.instance.LevelFailed();
    }
}
