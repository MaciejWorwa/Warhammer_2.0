using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header("Cechy pierwszorzêdowe")]//RADA MICHA£A: zrobic slownik ktorego kluczami sa nazwy cech a wartosciami wartosci tych cech
    public int WW;
    public int US;
    public int K;
    public int Odp;
    public int Zr;
    public int Int;
    public int SW;
    public int Ogd;

    [Header("Cechy drugorzêdowe")]
    public int S;
    public int Wt;
    public int Sz;
    public int tempSz;
    public int Mag;
    public int maxHealth;
    public int tempHealth;

    [Header("Inicjatywa, parowanie, uniki")]
    public int Initiative;
    public bool existDodge; // informacja o tym, czy postaæ posiada zdolnoœæ uniku
    [HideInInspector] public bool canParry = true; // informacja o tym, czy postac mo¿e parowaæ atak
    [HideInInspector] public bool canDodge; // informacja o tym, czy postac mo¿e unikaæ ataku
                                            //[HideInInspector] public int actionsNumber; // TO CHCIA£BYM U¯YÆ W PRZYSZ£OŒCI, NA RAZIE JEST NIEUZYWANE
    [HideInInspector] public bool criticalCondition = false;

    [Header("Broñ")]
    public int Weapon_S;
    public double AttackRange;
    public int reloadTime;
    public int reloadLeft;
    public bool Druzgoczacy;
    public bool PrzebijajacyZbroje;

    [Header("Punkty zbroi")]
    public int PZ_head;
    public int PZ_arms;
    public int PZ_torso;
    public int PZ_legs;

    public void SetBaseStatsByRace(Player.Rasa rasa)
    {
        if (rasa == Player.Rasa.Czlowiek)
        {
            WW = 20 + Random.Range(2, 21);
            US = 20 + Random.Range(2, 21);
            K = 20 + Random.Range(2, 21);
            Odp = 20 + Random.Range(2, 21);
            Zr = 20 + Random.Range(2, 21);
            Int = 20 + Random.Range(2, 21);
            SW = 20 + Random.Range(2, 21);
            Ogd = 20 + Random.Range(2, 21);
            maxHealth = Random.Range(10, 14);
            Sz = 4;
            Mag = 0;
        }
        else if (rasa == Player.Rasa.Elf)
        {
            WW = 20 + Random.Range(2, 21);
            US = 30 + Random.Range(2, 21);
            K = 20 + Random.Range(2, 21);
            Odp = 20 + Random.Range(2, 21);
            Zr = 30 + Random.Range(2, 21);
            Int = 20 + Random.Range(2, 21);
            SW = 20 + Random.Range(2, 21);
            Ogd = 20 + Random.Range(2, 21);
            maxHealth = Random.Range(9, 13);
            Sz = 5;
            Mag = 0;
        }
        else if (rasa == Player.Rasa.Krasnolud)
        {
            WW = 30 + Random.Range(2, 21);
            US = 20 + Random.Range(2, 21);
            K = 20 + Random.Range(2, 21);
            Odp = 30 + Random.Range(2, 21);
            Zr = 10 + Random.Range(2, 21);
            Int = 20 + Random.Range(2, 21);
            SW = 20 + Random.Range(2, 21);
            Ogd = 10 + Random.Range(2, 21);
            maxHealth = Random.Range(11, 15);
            Sz = 3;
            Mag = 0;
        }
        else if (rasa == Player.Rasa.Niziolek)
        {
            WW = 10 + Random.Range(2, 21);
            US = 30 + Random.Range(2, 21);
            K = 10 + Random.Range(2, 21);
            Odp = 10 + Random.Range(2, 21);
            Zr = 30 + Random.Range(2, 21);
            Int = 20 + Random.Range(2, 21);
            SW = 20 + Random.Range(2, 21);
            Ogd = 30 + Random.Range(2, 21);
            maxHealth = Random.Range(8, 12);
            Sz = 4;
            Mag = 0;
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
        Debug.Log("¯ywotnoœæ spad³a poni¿ej 0. Wynik rzutu na obra¿enia krytyczne: " + criticalValue);
        criticalCondition = true;
    }

    /* TO NA PRZYSZ£OŒÆ MO¯E SIÊ PRZYDAÆ, NA RAZIE SPOKO DZIA£A NAWET BEZ TEGO
    public void ResetActionsNumber()
    {
        actionsNumber = 0;
    }


    public void TakeAction() // wykonanie akcji
    {
        actionsNumber++;
    }

    public void TakeDoubleAction() // wykonanie akcji podwójnej
    {
        actionsNumber += 2;
    }
    */
}

