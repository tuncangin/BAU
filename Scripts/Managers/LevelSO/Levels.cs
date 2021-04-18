using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Levels / New Level", fileName = "New Level")] //Yeni level oluşturulacağı zaman proje üstünde sağ click yapıp new level'ı seçmek yeterli
public class Levels : ScriptableObject //Level listesi için level özelliklerini taşıyan scriptable object
{
    public int levelNumber; 
    public GameObject LevelObject; //Level'a ait prefab
    public String description;
    public Vector3 playerPosition; //Level başladığında player position'ı
}
