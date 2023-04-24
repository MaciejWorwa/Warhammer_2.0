using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public class SaveSystem : MonoBehaviour
{
    public void SaveAllCharactersStats()
    {
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

    public void LoadAllCharactersStats()
    {
        // Odznaczenie postaci, jeżeli podczas kliknięcia "Load" była zaznaczona, aby przyciski akcji nie zostawały widocznie w złym miejscu
        if (Player.trSelect != null)
        {
            Player.selectedPlayer.transform.localScale = new Vector3(1f, 1f, 1f);
            Player.trSelect = null;
            Player.selectedPlayer.GetComponent<Renderer>().material.color = new Color(0, 255, 0);

            GameObject.Find("ButtonManager").GetComponent<ButtonManager>().ShowOrHideActionsButtons(Player.selectedPlayer, false);
            MovementManager.canMove = true;

            // Zresetowanie koloru podswietlonych pol w zasiegu ruchu
            GameObject.Find("Grid").GetComponent<GridManager>().ResetTileColors();
        }
        if (Enemy.trSelect != null)
        {
            Enemy.selectedEnemy.transform.localScale = new Vector3(1f, 1f, 1f);
            Enemy.trSelect = null;
            Enemy.selectedEnemy.GetComponent<Renderer>().material.color = new Color(255, 0, 0);

            GameObject.Find("ButtonManager").GetComponent<ButtonManager>().ShowOrHideActionsButtons(Enemy.selectedEnemy, false);
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

        // Pobranie listy zapisanych plików
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.fun");

        // Sprawdzenie, czy w liście znajdują się pliki, których nazwa nie pasuje do nazw postaci w 'characters' i usunięcie ich
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            if (!charNames.Contains(fileName))
            {
                if (fileName.StartsWith("P")) // Sprawdza, czy ten string zaczyna się o litery 'P'. W ten sposób wykryje Playera.
                {
                    GameObject.Find("CreateTeam").GetComponent<CreateTeam>().CreateNewPlayer(fileName);
                }
                if (fileName.StartsWith("E")) // Sprawdza, czy ten string zaczyna się o litery 'E'. w ten sposób wykryje Enemy.
                {
                    GameObject.Find("CreateEnemy").GetComponent<CreateEnemy>().CreateNewEnemy(fileName);
                }
            }
        }

        // Opóźniam wczytanie statystyk, bo program nie jest w stanie zrobić tego natychmiastowo. Cały kod, który tu był przeniosłem do metody LoadWithDelay
        Invoke("LoadWithDelay", 0.01f);   
    }

    void LoadWithDelay()
    {

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        GameObject[] characters = enemies.Concat(players).ToArray();

        foreach (var character in characters)
        {
            string path = Application.persistentDataPath + "/" + character.name + ".fun";

            // Sprawdza, czy w pliku zapisu znajduje się dana postać. Jesli się znajduje to wczytuje jej staty i pozycje, a jak jej tam nie ma to ją usuwa.
            if (File.Exists(path))
            {
                GameData data = LoadCharacterStats(character.name);

                // Wczytaj wartości do komponentu Stats
                character.GetComponent<Stats>().Name = data.Name;
                character.GetComponent<Stats>().maxHealth = data.maxHealth;
                character.GetComponent<Stats>().tempHealth = data.tempHealth;
                character.GetComponent<Stats>().gameObject.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
                character.GetComponent<Stats>().Rasa = data.Rasa;
                character.GetComponent<Stats>().Level = data.Level;
                character.GetComponent<Stats>().Exp = data.Exp;
                character.GetComponent<Stats>().WW = data.WW;
                character.GetComponent<Stats>().US = data.US;
                character.GetComponent<Stats>().K = data.K;
                character.GetComponent<Stats>().Odp = data.Odp;
                character.GetComponent<Stats>().Zr = data.Zr;
                character.GetComponent<Stats>().Int = data.Int;
                character.GetComponent<Stats>().SW = data.SW;
                character.GetComponent<Stats>().Ogd = data.Ogd;
                character.GetComponent<Stats>().A = data.A;
                character.GetComponent<Stats>().S = data.S;
                character.GetComponent<Stats>().Wt = data.Wt;
                character.GetComponent<Stats>().Sz = data.Sz;
                character.GetComponent<Stats>().tempSz = data.tempSz;
                character.GetComponent<Stats>().Mag = data.Mag;
                character.GetComponent<Stats>().PP = data.PP;
                character.GetComponent<Stats>().Initiative = data.Initiative;
                character.GetComponent<Stats>().Dodge = data.Dodge;
                character.GetComponent<Stats>().instantReload = data.instantReload;
                character.GetComponent<Stats>().canParry = data.canParry;
                character.GetComponent<Stats>().canDodge = data.canDodge;
                character.GetComponent<Stats>().actionsLeft = data.actionsLeft;
                character.GetComponent<Stats>().criticalCondition = data.criticalCondition;
                character.GetComponent<Stats>().parryBonus = data.parryBonus;
                character.GetComponent<Stats>().defensiveBonus = data.defensiveBonus;
                character.GetComponent<Stats>().aimingBonus = data.aimingBonus;
                character.GetComponent<Stats>().Weapon_S = data.Weapon_S;
                character.GetComponent<Stats>().AttackRange = data.AttackRange;
                character.GetComponent<Stats>().reloadTime = data.reloadTime;
                character.GetComponent<Stats>().reloadLeft = data.reloadLeft;
                character.GetComponent<Stats>().Ciezki = data.Ciezki;
                character.GetComponent<Stats>().Druzgoczacy = data.Druzgoczacy;
                character.GetComponent<Stats>().Parujacy = data.Parujacy;
                character.GetComponent<Stats>().Powolny = data.Powolny;
                character.GetComponent<Stats>().PrzebijajacyZbroje = data.PrzebijajacyZbroje;
                character.GetComponent<Stats>().Szybki = data.Szybki;
                character.GetComponent<Stats>().PZ_head = data.PZ_head;
                character.GetComponent<Stats>().PZ_arms = data.PZ_arms;
                character.GetComponent<Stats>().PZ_torso = data.PZ_torso;
                character.GetComponent<Stats>().PZ_legs = data.PZ_legs;

                Vector3 position;
                position.x = data.position[0];
                position.y = data.position[1];
                position.z = data.position[2];

                character.transform.position = position;

                RoundManager.roundNumber = data.NumberOfRounds;
            }
            else
            {
                // Usuń postać
                Destroy(character);
                if (character.CompareTag("Player"))
                    CreateTeam.playersAmount--; ;
                if (character.CompareTag("Enemy"))
                    CreateEnemy.enemiesAmount--;
            }

        }

        GameObject.Find("MessageManager").GetComponent<MessageManager>().ShowMessage($"<color=green>Wczytano stan gry.</color>", 3f);
        Debug.Log($"<color=green>Wczytano stan gry.</color>");
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

        // Pobranie listy zapisanych plików
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.fun");

        // Sprawdzenie, czy w liście znajdują się pliki, których nazwa nie pasuje do nazw postaci w 'characters' i usunięcie ich
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            if (!charNames.Contains(fileName))
            {
                File.Delete(file);
            }
        }

        // Zapis wszystkich postaci w 'characters'
        foreach (var character in characters)
        {
            string charName = character.gameObject.name;
            string path = Application.persistentDataPath + "/" + charName + ".fun";

            FileStream stream = new FileStream(path, FileMode.Create);
            GameData data = new GameData(character);
            formatter.Serialize(stream, data);
            stream.Close();
        }
    }


    public static GameData LoadCharacterStats(string charName)
    {
        string path = Application.persistentDataPath + "/" + charName + ".fun";
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
}
