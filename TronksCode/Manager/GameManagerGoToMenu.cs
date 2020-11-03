using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace S3
{
    public class GameManagerGoToMenu : MonoBehaviour
    {
        private GameManagerMaster GameManagerMaster;

        void OnEnable()
        {
            if (GameManagerMaster == null)
                SetInitialReferences();

            GameManagerMaster.MenuSceneEvent += GoToMenuScene;
        }

        void OnDisable()
        {
            GameManagerMaster.MenuSceneEvent -= GoToMenuScene;
        }

        void SetInitialReferences()
        {
            GameManagerMaster = GetComponent<GameManagerMaster>();
        }

        void GoToMenuScene()
        {
            Application.LoadLevel(0);
        }
    }

}
