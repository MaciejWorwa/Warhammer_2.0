using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header("Rasa")]
    public string Rasa;

    [Header("Cechy pierwszorz�dowe")]//RADA MICHA�A: zrobic slownik ktorego kluczami sa nazwy cech a wartosciami wartosci tych cech
    public int WW;
    public int US;
    public int K;
    public int Odp;
    public int Zr;
    public int Int;
    public int SW;
    public int Ogd;

    [Header("Cechy drugorz�dowe")]
    public int A;
    public int S;
    public int Wt;
    public int Sz;
    [HideInInspector] public int tempSz;
    public int Mag;
    public int maxHealth;
    public int tempHealth;
    public int PS;

    [Header("Inicjatywa, parowanie, uniki")]
    public int Initiative; // inicjatywa
    public bool existDodge; // informacja o tym, czy posta� posiada zdolno�� uniku
    [HideInInspector] public bool canParry = true; // informacja o tym, czy postac mo�e parowa� atak
    [HideInInspector] public bool canDodge; // informacja o tym, czy postac mo�e unika� ataku
    //[HideInInspector] public int actionsNumber; // TO CHCIA�BYM U�Y� W PRZYSZ�O�CI, NA RAZIE JEST NIEUZYWANE
    [HideInInspector] public bool criticalCondition = false; // sprawdza czy �ycie postaci jest poni�ej 0
    [HideInInspector] public int parryBonus; // sumaryczna premia do WW przy parowaniu

    [Header("Bro�")]
    public int Weapon_S;
    public double AttackRange;
    public int reloadTime;
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

        if (rasa == Player.Rasa.Elf)
        {
            US += 10;
            Zr += 10;
            maxHealth -= 1;
            Sz = 5;
        }
        else if (rasa == Player.Rasa.Krasnolud)
        {
            WW += 10;
            Odp += 10;
            Zr -= 10;
            Ogd -= 10;
            maxHealth += 1;
            Sz = 3;
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
        }

        Initiative = Zr + Random.Range(1, 11);
    }

    void Update()
    {
        S = Mathf.RoundToInt(K / 10);
        Wt = Mathf.RoundToInt(Odp / 10);
    }

    public void ResetParryAndDodge()
    {
        canParry = true;
        if (existDodge) //sprawdzenie czy postac posiada zdolnosc Unik
            canDodge = true;
    }

    public void GetCriticalHit()
    {
        int criticalValue = Random.Range(1, 101);
        Debug.Log("�ywotno�� spad�a poni�ej 0. Wynik rzutu na obra�enia krytyczne: " + criticalValue);
        criticalCondition = true;
    }

    /* TO NA PRZYSZ�O�� MO�E SI� PRZYDA�, NA RAZIE SPOKO DZIA�A NAWET BEZ TEGO
    public void ResetActionsNumber()
    {
        actionsNumber = 0;
    }


    public void TakeAction() // wykonanie akcji
    {
        actionsNumber++;
    }

    public void TakeDoubleAction() // wykonanie akcji podw�jnej
    {
        actionsNumber += 2;
    }
    */
}

