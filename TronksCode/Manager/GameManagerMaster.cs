using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace S3
{
    public class GameManagerMaster : MonoBehaviour
    {
        public delegate void GameManagerEventManagerHandler();
        public event GameManagerEventManagerHandler MenuToggleEvent;
        public event GameManagerEventManagerHandler InventoryUIToggleEvent;
        public event GameManagerEventManagerHandler RestartLevelEvent;
        public event GameManagerEventManagerHandler MenuSceneEvent;
        public event GameManagerEventManagerHandler InterLevelMenuSceneEvent;
        public event GameManagerEventManagerHandler GameOverEvent;

        public bool isGameOver;
        public bool isInterLevelMenuOn;
        public bool isMenuOn;
        public bool isInvetoryUIOn;

        public void CallMenuToggleEvent()
        {
            if(MenuToggleEvent != null)
                MenuToggleEvent();
        }

        public void CallInventoryUIToggleEvent()
        {
            if(InventoryUIToggleEvent != null)
                InventoryUIToggleEvent();
        }

        public void CallRestartLevelEvent()
        {
            if(RestartLevelEvent != null)
                RestartLevelEvent();
        }

        public void CallMenuSceneEvent()
        {
            if (MenuSceneEvent != null)
                MenuSceneEvent();
        }

        public void CallInterLevelMenuSceneEvent()
        {
            if(InterLevelMenuSceneEvent != null)
                InterLevelMenuSceneEvent();
        }

        public void CallGameOverEvent()
        {
            if(GameOverEvent != null)
            {
                isGameOver = true;
                GameOverEvent();
            }
        }
    }
}


