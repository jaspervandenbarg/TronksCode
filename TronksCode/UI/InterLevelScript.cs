using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class InterLevelScript : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI interLevelText;
    private int level;
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            level = SceneManager.GetActiveScene().buildIndex - 1;
            InterLevelText(level);
        }
    }

    private void InterLevelText(int number = 1)
    {
        //interLevelText.text = string.Format("Level {0} \nThus far... \nYou still have {1} lives \nShot {2} bullets \nLayed {3} mines \nDestroyed {4} enemy tanks \nSpend {5} seconds in slomotion which is actualy just {6} seconds for normal people",
        //    number, NeverUnload.Instance.playerLives, NeverUnload.Instance.bulletsFired, NeverUnload.Instance.minesLayed, NeverUnload.Instance.destroyedTanks, Math.Round(NeverUnload.Instance.slomoSeconds, 2), Math.Round(NeverUnload.Instance.realSeconds, 2));
        interLevelText.text = string.Format("Level {0} \nLives {1}",
            number, NeverUnload.Instance.playerLives);
    }
}
