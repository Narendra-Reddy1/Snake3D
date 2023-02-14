using SnakeGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitSceneInitializer : MonoBehaviour
{
    #region Unity Methods
    private void Start()
    {
        AsyncOperation handle = SceneManager.LoadSceneAsync(Constants.LOBBY_SCENE, LoadSceneMode.Additive);
        handle.completed += (op) =>
        {
            op.allowSceneActivation = true;
            Debug.Log($"Loaded Lobby");
            SceneManager.UnloadSceneAsync(Constants.INIT_SCENE);
            GlobalEventHandler.TriggerEvent(EventID.REQUEST_PHOTON_TO_CONNECT_MASTER_SERVER);
        };

    }
    #endregion Unity Methods
}
