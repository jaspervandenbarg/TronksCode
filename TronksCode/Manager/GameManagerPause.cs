using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace S3
{
    public class GameManagerPause : MonoBehaviour
    {
        private GameManagerMaster GameManagerMaster;
        private bool isPaused;

        void OnEnable()
        {
            if(GameManagerMaster == null)
                SetInitialReferences();

            GameManagerMaster.MenuToggleEvent += TogglePause;
            GameManagerMaster.InventoryUIToggleEvent += TogglePause;
        }

        void OnDisable()
        {
            GameManagerMaster.MenuToggleEvent -= TogglePause;
            GameManagerMaster.InventoryUIToggleEvent -= TogglePause;
        }

        void SetInitialReferences()
        {
            GameManagerMaster = GetComponent<GameManagerMaster>();
        }

        void TogglePause()
        {
            if (isPaused)
            {
                Time.timeScale = 1;
                isPaused = false;
            }
            else
            {
                Time.timeScale = 0;
                isPaused = true;
            }
        }
    }
}

