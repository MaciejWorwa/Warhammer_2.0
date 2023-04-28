using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System;
using TMPro;

public class SaveSystem : MonoBehaviour
{
    [SerializeField] TMP_Dropdown savedFilesDropdown; // Dropdown z wszystkimi savami
    private static string dropdownText = "";

    #region Save functions
    public void SaveAllCharactersStats()
    {
        if(GameObject.Find("NameOfSaveInput").GetComponent<TMP_InputField>().text.Length < 1)
        {
            GameObject.Find("MessageManager").GetComponent<MessageManager>().ShowMessage($"<color=red>Zapis nieudany. Nie podano nazwy pliku.</color>", 4f);
            Debug.Log($"<color=red>Zapis nieudany. Nie podano nazwy pliku.</color>");
            return;
        }

        List<Stats> allStats = new List<Stats>();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        GameObject[] characters = enemies.Concat(players).ToArray();

        foreach (var character in characters)
        {
            allStats.Add(character.GetComponent<Stats>());
        }

        SaveCharacterStats(allStats.ToArray());

        GameObject.Find("MessageManager").GetComponent<MessageManager>().ShowMessage($"<color=green>Zapisano stan gry.</color>", 3f);
        Debug.Log($"<color=green>Zapisano stan gry.</color>");
    }

    public static void SaveCharacterStats(Stats[] characters)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        // Utworzenie listy nazw postaci w 'characters'
        List<string> charNames = new List<string>();
        foreach (var character in characters)
        {
            charNames.Add(character.gameObject.name);
        }

        // Stworzenie folderu dla zapisów
        string savesFolderName = GameObject.Find("NameOfSaveInput").GetComponent<TMP_InputField>().text;
        Directory.CreateDirectory(Application.persistentDataPath + "/" + savesFolderName);

        // Pobranie listy zapisanych plików
        string[] files = Directory.GetFiles(Application.persistentDataPath + "/" + savesFolderName, "*.fun");

        // Sprawdzenie, czy w liście znajdują się pliki, których nazwa nie pasuje do nazw postaci w 'characters' i usunięcie ich
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            if (!charNames.Contains(fileName))
            {
                Debug.Log("usuwam " + fileName);
                File.Delete(file);
            }
        }

        // Zapis wszystkich postaci w 'characters'
        foreach (var character in characters)
        {
            string charName = character.gameObject.name;
            string path = Application.persistentDataPath + "/" + savesFolderName + "/" + charName + ".fun";

            FileStream stream = new FileStream(path, FileMode.Create);
            GameData data = new GameData(character);
            formatter.Serialize(stream, data);
            stream.Close();
        }
    }
    #endregion

    #region Update saved files dropdown
    public void SetSavedFilesDropdown()
    {
        // Resetuje opcje dropdownu
        savedFilesDropdown.ClearOptions();

        List<string> options = new List<string>();

        // Wykonuje pętle dla wszystkich folderów wewnątrz głównego folderu z save'ami
        foreach (string saveFolder in Directory.GetDirectories(Application.persistentDataPath))
        {
            options.Add(Path.GetFileName(saveFolder));
        }

        savedFilesDropdown.AddOptions(options);
    }
    #endregion


    #region Load all characters stats
    public void LoadAllCharactersStats()
    {
        if (savedFilesDropdown.GetComponent<TMP_Dropdown>().options.Count == 0)
            return;

        // Odznaczenie postaci, jeżeli podczas kliknięcia "Load" była zaznaczona, aby przyciski akcji nie zostawały widocznie w złym miejscu
        if (Character.trSelect != null)
        {
            Character.selectedCharacter.transform.localScale = new Vector3(1f, 1f, 1f);
            Character.trSelect = null;
            if (Character.selectedCharacter.CompareTag("Player"))
                    Character.selectedCharacter.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
            else if (Character.selectedCharacter.CompareTag("Enemy"))
                    Character.selectedCharacter.GetComponent<Renderer>().material.color = new Color(255, 0, 0);

            GameObject.Find("ButtonManager").GetComponent<ButtonManager>().ShowOrHideActionsButtons(Character.selectedCharacter, false);
            MovementManager.canMove = true;

            // Zresetowanie koloru podswietlonych pol w zasiegu ruchu
            GameObject.Find("Grid").GetComponent<GridManager>().ResetTileColors();
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        GameObject[] characters = enemies.Concat(players).ToArray();

        // Utworzenie listy nazw postaci w 'characters'
        List<string> charNames = new List<string>();
        foreach (var character in characters)
        {
            charNames.Add(character.gameObject.name);
        }

        dropdownText = savedFilesDropdown.GetComponent<TMP_Dropdown>().options[savedFilesDropdown.value].text;

        // Pobranie listy zapisanych plików
        string[] files = Directory.GetFiles(Application.persistentDataPath + "/" + dropdownText, "*.fun");

        // Wczytuje każdy plik o nazwie istniejącej w liście nazw obecnie istniejących na polu bitwy postaci
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            if (!charNames.Contains(fileName))
            {
                if (fileName.StartsWith("P")) // Sprawdza, czy ten string zaczyna się o litery 'P'. W ten sposób wykryje Playera.
                {
                    GameObject.Find("CharacterManager").GetComponent<CharacterManager>().CreateNewCharacter("Player", fileName);
                }
                if (fileName.StartsWith("E")) // Sprawdza, czy ten string zaczyna się o litery 'E'. w ten sposób wykryje Enemy.
                {
                    GameObject.Find("CharacterManager").GetComponent<CharacterManager>().CreateNewCharacter("Enemy", fileName);
                }
            }
        }

        // Opóźniam wczytanie statystyk, bo program nie jest w stanie zrobić tego natychmiastowo. Cały kod, który tu był przeniosłem do metody LoadWithDelay
        Invoke("LoadWithDelay", 0.01f);   
    }
    #endregion

    #region Load with delay
    void LoadWithDelay()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        CharacterManager.playersAmount = players.Length;
        CharacterManager.enemiesAmount = enemies.Length;

        GameObject[] characters = enemies.Concat(players).ToArray();

        // Utworzenie list unikalnych postaci
        List<GameObject> uniqueCharacters = new List<GameObject>();
        foreach (GameObject character in characters)
        {
            if (!uniqueCharacters.Exists(e => e.name == name))
                uniqueCharacters.Add(character);
        }

        // Usunięcie duplikatów
        foreach (GameObject character in uniqueCharacters)
        {
            int count = characters.Count(e => e.name == character.name);
            if (count > 1)
            {
                for (int i = 1; i < count; i++)
                {
                    Destroy(GameObject.Find(character.name));
                }
            }
        }

        foreach (var character in uniqueCharacters)
        {           
            string path = Application.persistentDataPath + "/" + dropdownText + "/" + character.name + ".fun";

            // Sprawdza, czy w pliku zapisu znajduje się dana postać. Jesli się znajduje to wczytuje jej staty i pozycje, a jak jej tam nie ma to ją usuwa.
            if (File.Exists(path))
            {
                GameData data = LoadCharacterStats(character.name);

                character.GetComponent<Character>().rasa = (Character.Rasa)Array.IndexOf(Enum.GetNames(typeof(Character.Rasa)), data.Rasa);;


                Stats stats = character.GetComponent<Stats>();

                // Wczytaj wartości z 'data' do ich odpowiednikow z komponentu Stats
                foreach (var field in data.GetType().GetFields())
                {
                    var value = field.GetValue(data);
                    var componentField = stats.GetType().GetField(field.Name);
                    if (componentField != null && value != null)
                    {
                        componentField.SetValue(stats, value);
                    }
                }

                character.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);

                RoundManager.roundNumber = data.NumberOfRounds;
            }
            else
            {
                // Usuń postać
                Destroy(character);
                if (character.CompareTag("Player"))
                    CharacterManager.playersAmount--;
                if (character.CompareTag("Enemy"))
                    CharacterManager.enemiesAmount--;
            }
        }

        GameObject.Find("MessageManager").GetComponent<MessageManager>().ShowMessage($"<color=green>Wczytano stan gry.</color>", 3f);
        Debug.Log($"<color=green>Wczytano stan gry.</color>");
    }
    #endregion

    #region Load character stats
    public static GameData LoadCharacterStats(string charName)
    {
        string path = Application.persistentDataPath + "/" + dropdownText + "/" + charName + ".fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError($"Zapis gry dla gracza {charName} nie został znaleziony w " + path);
            return null;
        }
    }
    #endregion

    #region Delete selected save file
    public void DeleteSaveFile()
    {
        if (savedFilesDropdown.GetComponent<TMP_Dropdown>().options.Count == 0)
            return;

        dropdownText = savedFilesDropdown.GetComponent<TMP_Dropdown>().options[savedFilesDropdown.value].text;

        string path = Application.persistentDataPath + "/" + dropdownText;
        Directory.Delete(path, true);

        GameObject.Find("MessageManager").GetComponent<MessageManager>().ShowMessage($"Zapis {dropdownText} został usunięty.", 3f);
        Debug.Log($"Zapis {dropdownText} został usunięty.");

        SetSavedFilesDropdown();
    }
    #endregion

    public void CheckForEmptySavesDropdown(GameObject LoadGameConfirmPanel)
    {
        if (savedFilesDropdown.GetComponent<TMP_Dropdown>().options.Count == 0)
        {
            GameObject.Find("LoadGamePanel").SetActive(false);
            if (LoadGameConfirmPanel.activeSelf)
            {
                LoadGameConfirmPanel.SetActive(false);
            }

            GameManager.PanelIsOpen = false;

            GameObject.Find("MessageManager").GetComponent<MessageManager>().ShowMessage($"Nie wybrano pliku.", 3f);
            Debug.Log($"Nie wybrano pliku.");
        }
    }
}
