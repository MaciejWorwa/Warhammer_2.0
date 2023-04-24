using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // Potrzebne przy edytowaniu inspektora i szukaniu pliku .json w folderze Resources

/*
// EDYTOWANIE INSPEKTORA TAK, ABY WYSWIETLAL SLOWNIK WRAZ Z JEGO KLUCZAMI I WARTOSCIAMI I MOZNA BYLO TO ZMIENIAC (ALE OBECNIE JEST TO WADLIWE).
[CustomEditor(typeof(Stats))]
public class StatsEditor : Editor
{
    public override void OnInspectorGUI()
    {        
        base.OnInspectorGUI(); // Wyświetlenie wszystkiego z poprzedniego inspektora
        Stats stats = (Stats)target;

        EditorGUILayout.LabelField("Cechy Pierwszorzędowe"); // Nadanie Headera słownikowi

        // wyświetlanie wartości dla każdego klucza
        foreach (string key in stats.CechyPierwszorzedowe.Keys)
        {
            int value = stats.CechyPierwszorzedowe[key];
            int newValue = EditorGUILayout.IntField(key, value);
            if (newValue != value)
            {
                stats.CechyPierwszorzedowe[key] = newValue;
            }
        }
    }
}
*/

public class Stats : MonoBehaviour
{
    [Header("Imię")]
    public string Name;

    [Header("Rasa")]
    public string Rasa;

    public int Level;
    public int Exp;

    /*
    // ZROBIENIE SŁOWNIKA. ALE NIEZBYT WSPOLGRA TO Z WYSWIETLANIEM I ZMIENIANIEM WARTOSCI Z POZIOMU INSPEKTORA PODCZAS URUCHOMIONEJ GRY.
    void Start()
    {
        CechyPierwszorzedowe = new Dictionary<string, int>();
        string[] cechy = { "WW", "US", "K", "Odp", "Zr", "Int", "SW", "Ogd" };

        foreach (string cecha in cechy)
        {
            CechyPierwszorzedowe.Add(cecha, 20 + Random.Range(2, 21));
        }
    }
    */

    [Header("Cechy pierwszorzędowe")]//RADA MICHAŁA: zrobic slownik ktorego kluczami sa nazwy cech a wartosciami wartosci tych cech
    public int WW;
    public int US;
    public int K;
    public int Odp;
    public int Zr;
    public int Int;
    public int SW;
    public int Ogd;
    // public Dictionary<string, int> CechyPierwszorzedowe;

    [Header("Cechy drugorzędowe")]
    public int A;
    public int S;
    public int Wt;
    public int Sz;
    [HideInInspector] public int tempSz;
    public int Mag;
    public int maxHealth;
    public int tempHealth;
    public int PP;

    [Header("Inicjatywa, parowanie, uniki")]
    public int Initiative; // inicjatywa
    public int Dodge; // informacja o tym, czy postać posiada zdolność uniku
    public bool instantReload; // informacja o tym, czy postać posiada zdolność blyskawicznego przeladowania
    [HideInInspector] public bool canParry = true; // informacja o tym, czy postac może parować atak
    [HideInInspector] public bool canDodge; // informacja o tym, czy postac może unikać ataku
    [HideInInspector] public int actionsLeft = 2; // akcje do wykorzystania w aktualnej rundzie walki
    [HideInInspector] public bool criticalCondition = false; // sprawdza czy życie postaci jest poniżej 0
    [HideInInspector] public int parryBonus; // sumaryczna premia do WW przy parowaniu
    [HideInInspector] public int defensiveBonus; // premia za pozycje obronna
    [HideInInspector] public int aimingBonus; // premia za przycelowanie

    [Header("Broń")]
    public int Weapon_S;
    public double AttackRange;
    public int reloadTime = 1;
    public int reloadLeft;
    public bool Ciezki;
    public bool Druzgoczacy;
    public bool Parujacy;
    public bool Powolny;
    public bool PrzebijajacyZbroje;
    public bool Szybki;

    [Header("Punkty zbroi")]
    public int PZ_head;
    public int PZ_arms;
    public int PZ_torso;
    public int PZ_legs;

    // Próba wczytania bestiariusza i zaktualizowania wartości cech zgodnie z plikiem .json (NIEUDANA, lista potworow w inspektorze jest pusta)
    /*
    public TextAsset jsonBestiariusz;

    //[System.Serializable]
    public class Bestiariusz
    {
        public Stats[] potwory;
    }

    public Bestiariusz mojBestiariusz = new Bestiariusz();
    public Stats[] potwory;

    void Start()
    {
        jsonBestiariusz = Resources.Load<TextAsset>("Bestiariusz");
        //mojBestiariusz = JsonUtility.FromJson<Bestiariusz>(jsonBestiariusz.text);
        potwory = JsonUtility.FromJson<Stats[]>(jsonBestiariusz.text);

        //potwory = mojBestiariusz.potwory;
    }
    */

    #region Set base stats for players by race function
    public void SetBaseStatsByRace(Player.Rasa rasa)
    {
        WW = 20 + Random.Range(2, 21);
        US = 20 + Random.Range(2, 21);
        K = 20 + Random.Range(2, 21);
        Odp = 20 + Random.Range(2, 21);
        Zr = 20 + Random.Range(2, 21);
        Int = 20 + Random.Range(2, 21);
        SW = 20 + Random.Range(2, 21);
        Ogd = 20 + Random.Range(2, 21);
        A = 1;
        Mag = 0;
        Sz = 4;

        int rollMaxHealth = Random.Range(1, 11);
        if (rollMaxHealth <= 3)
            maxHealth = 10;
        else if (rollMaxHealth <= 6)
            maxHealth = 11;
        else if (rollMaxHealth <= 9)
            maxHealth = 12;
        else if (rollMaxHealth == 10)
            maxHealth = 13;

        int rollPP = Random.Range(1, 11);
        if (rollPP <= 4)
            PP = 2;
        else if (rollPP <= 7)
            PP = 3;
        else if (rollPP >= 8)
            PP = 3;


        if (rasa == Player.Rasa.Elf)
        {
            US += 10;
            Zr += 10;
            maxHealth -= 1;
            Sz = 5;
            PP--;
        }
        else if (rasa == Player.Rasa.Krasnolud)
        {
            WW += 10;
            Odp += 10;
            Zr -= 10;
            Ogd -= 10;
            maxHealth += 1;
            Sz = 3;
            if(PP != 3)
                PP--;
        }
        else if (rasa == Player.Rasa.Niziolek)
        {
            WW -= 10;
            US += 10;
            K -= 10;
            Odp -= 10;
            Zr += 10;
            Ogd += 10;
            maxHealth -= 2;
            if (rollPP <= 7 && rollPP > 4)
                PP--;
        }

        Initiative = Zr + Random.Range(1, 11);
        S = Mathf.RoundToInt(K / 10);
        Wt = Mathf.RoundToInt(Odp / 10);
    }
    #endregion

    #region Reset parry and dodge function
    public void ResetParryAndDodge()
    {
        canParry = true;
        if (Dodge > 0) //sprawdzenie czy postac posiada zdolnosc Unik
            canDodge = true;
    }
    #endregion

    #region Get critical hit function
    public void GetCriticalHit()
    {
        int criticalValue = Random.Range(1, 101);

        GameObject.Find("MessageManager").GetComponent<MessageManager>().ShowMessage($"<color=red>Żywotność spadła poniżej 0.</color> Wynik rzutu na obrażenia krytyczne: <color=red>{criticalValue}</color>", 6f);
        Debug.Log("Żywotność spadła poniżej 0. Wynik rzutu na obrażenia krytyczne: " + criticalValue);
        criticalCondition = true;
    }
    #endregion

    #region Zarzadzanie akcjami postaci
    public void ResetActionsNumber()
    {
        actionsLeft = 2;
        //Debug.Log($"Nowa runda. {this.gameObject.name} pozostały {actionsLeft} akcje.");
    }

    public void TakeAction() // wykonanie akcji
    {
        actionsLeft--;
        Debug.Log($"{this.gameObject.name} wykonał akcję pojedynczą. Pozostała {actionsLeft} akcja w tej rundzie.");
    }

    public void TakeDoubleAction() // wykonanie akcji podwójnej
    {
        actionsLeft -= 2;
        Debug.Log($"{this.gameObject.name} wykonał akcję podwójną. Pozostało {actionsLeft} akcji w tej rundzie.");
    }
    #endregion
}

