using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScenesManager : MonoBehaviour
{
    [SerializeField] GameObject quitGamePanel;

    public static string saveFileName = ""; // Nazwa zapisu gry wybierana z poziomu menu startowego

    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index, LoadSceneMode.Single);
    }

    private void Start()
    {
        // Gdy gra jest uruchamiana przyciskiem "Wczytaj" z manu startowego
        if(saveFileName.Length > 0 && SceneManager.GetActiveScene().buildIndex == 2)
        {
            // Wywo³aj funkcjê wczytuj¹c¹ stan gry
            GameObject.Find("SaveSystem").GetComponent<SaveSystem>().LoadAllCharactersStats(false);
        }
    }

    public void SetSaveFileName(TMP_Dropdown savedFilesDropdown)
    {
        saveFileName = savedFilesDropdown.GetComponent<TMP_Dropdown>().options[savedFilesDropdown.value].text;
    }
}
