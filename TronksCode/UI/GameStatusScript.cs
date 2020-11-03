using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameStatusScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI EnemyCountText;
    [SerializeField] private TextMeshProUGUI AmmoCountText;
    [SerializeField] private TextMeshProUGUI LevelCountText;
    [SerializeField] private GameObject musicVolume, effectVolume;

    public List<GameObject> enemiesInLevel = new List<GameObject>();

    void Start()
    {
        //ammoCount = Player.Instance.Entity.FireMechanic.maxBulletMagazine;
        //UpdateAmmoCount(ammoCount);
        //UpdateLevelCount();

        musicVolume.GetComponent<Slider>().value = NeverUnload.Instance.musicLevel;
        effectVolume.GetComponent<Slider>().value = NeverUnload.Instance.effectLevel;
    }

    public void UpdateEnemyCount()
    {
        if(EnemyCountText != null)
            EnemyCountText.text = "Enemies: " + enemiesInLevel.Count.ToString();
    }

    //public void UpdateAmmoCount(int ammo)
    //{
    //    ammoCount = ammo;
    //    if(AmmoCountText != null)
    //        AmmoCountText.text = ammoCount.ToString();
    //}

    //private void UpdateLevelCount()
    //{
    //    levelCount = SceneManager.GetActiveScene().buildIndex - 1;
    //    if(LevelCountText != null)
    //        LevelCountText.text = "Level " + levelCount.ToString();
    //}
}
