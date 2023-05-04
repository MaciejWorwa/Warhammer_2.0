using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    [SerializeField] GameObject quitGamePanel;

    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index, LoadSceneMode.Single);
    }
}
