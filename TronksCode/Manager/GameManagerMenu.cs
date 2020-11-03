using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace S3
{
    public class GameManagerMenu : MonoBehaviour
    {
        private GameManagerMaster GameManagerMaster;
        public GameObject menu;

        void Start()
        {
            //ToggleMenu();
        }

        // Update is called once per frame
        void Update()
        {
            CheckForMenuToggleRequest();
        }

        void OnEnable()
        {
            if (GameManagerMaster == null)
                SetInitialReferences();

            GameManagerMaster.GameOverEvent += ToggleMenu;
        }

        void OnDisable()
        {
            GameManagerMaster.GameOverEvent -= ToggleMenu;

        }

        void SetInitialReferences()
        {
            GameManagerMaster = GetComponent<GameManagerMaster>();
        }

        void CheckForMenuToggleRequest()
        {
            if(Input.GetKeyDown(KeyCode.Escape) && !GameManagerMaster.isGameOver && !GameManagerMaster.isInterLevelMenuOn && !GameManagerMaster.isInvetoryUIOn)
                ToggleMenu();
        }

        void ToggleMenu()
        {
            if (menu != null)
            {
                menu.SetActive(!menu.activeSelf);
                GameManagerMaster.isMenuOn = !GameManagerMaster.isMenuOn;
                GameManagerMaster.CallMenuSceneEvent();
            }
            else
                Debug.LogError("No Menu Found");
        }
    }
}

