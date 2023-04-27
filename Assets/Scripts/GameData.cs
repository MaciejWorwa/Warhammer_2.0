using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int NumberOfRounds;

    [Header("Imi�")]
    public string Name;

    [Header("Rasa")]
    public string Rasa;

    public int Level;
    public int Exp;

    [Header("Cechy pierwszorz�dowe")]
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
    public int PP;

    [Header("Inicjatywa, parowanie, uniki")]
    public int Initiative; // inicjatywa
    public int Dodge; // informacja o tym, czy posta� posiada zdolno�� uniku
    public bool instantReload; // informacja o tym, czy posta� posiada zdolno�� blyskawicznego przeladowania
    [HideInInspector] public bool canParry = true; // informacja o tym, czy postac mo�e parowa� atak
    [HideInInspector] public bool canDodge; // informacja o tym, czy postac mo�e unika� ataku
    [HideInInspector] public int actionsLeft = 2; // akcje do wykorzystania w aktualnej rundzie walki
    [HideInInspector] public bool criticalCondition = false; // sprawdza czy �ycie postaci jest poni�ej 0
    [HideInInspector] public int parryBonus; // sumaryczna premia do WW przy parowaniu
    [HideInInspector] public int defensiveBonus; // premia za pozycje obronna
    [HideInInspector] public int aimingBonus; // premia za przycelowanie

    [Header("Bro�")]
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

    [Header("Statystyki zaklęć")]
    public int Spell_S;
    public int PowerRequired;
    public double SpellRange;
    public double AreaSize;
    public int CastDuration;
    public bool OffensiveSpell;
    public bool IgnoreArmor;
    [HideInInspector] public bool etherArmorActive = false; // Określa, czy postać ma aktywny pancerz eteru

    public float[] position;

    public GameData(Stats stats)
    {
        NumberOfRounds = RoundManager.roundNumber;

        Name = stats.Name;
        Rasa = stats.Rasa;
        Level = stats.Level;
        Exp = stats.Exp;
        WW = stats.WW;
        US = stats.US;
        K = stats.K;
        Odp = stats.Odp;
        Zr = stats.Zr;
        Int = stats.Int;
        SW = stats.SW;
        Ogd = stats.Ogd;
        
        A = stats.A;
        S = stats.S;
        Wt = stats.Wt;
        Sz = stats.Sz;
        tempSz = stats.tempSz;
        Mag = stats.Mag;
        maxHealth = stats.maxHealth;
        tempHealth = stats.tempHealth;
        PP = stats.PP;

        Initiative = stats.Initiative;
        Dodge = stats.Dodge;
        instantReload = stats.instantReload;
        canParry = stats.canParry;
        canDodge = stats.canDodge;
        actionsLeft = stats.actionsLeft;
        criticalCondition = stats.criticalCondition;
        parryBonus = stats.parryBonus;
        defensiveBonus = stats.defensiveBonus;
        aimingBonus = stats.aimingBonus;

        Weapon_S = stats.Weapon_S;
        AttackRange = stats.AttackRange;
        reloadTime = stats.reloadTime;
        reloadLeft = stats.reloadLeft;
        Ciezki = stats.Ciezki;
        Druzgoczacy = stats.Druzgoczacy;
        Parujacy = stats.Parujacy;
        Powolny = stats.Powolny;
        PrzebijajacyZbroje = stats.PrzebijajacyZbroje;
        Szybki = stats.Szybki;

        PZ_head = stats.PZ_head;
        PZ_arms = stats.PZ_arms;
        PZ_torso = stats.PZ_torso;
        PZ_legs = stats.PZ_legs;

        Spell_S = stats.Spell_S;
        PowerRequired = stats.PowerRequired;
        SpellRange = stats.SpellRange;
        AreaSize = stats.AreaSize;
        CastDuration = stats.CastDuration;
        OffensiveSpell = stats.OffensiveSpell;
        IgnoreArmor = stats.IgnoreArmor;
        etherArmorActive = stats.etherArmorActive;

        position = new float[3];
        position[0] = stats.gameObject.transform.position.x;
        position[1] = stats.gameObject.transform.position.y;
        position[2] = stats.gameObject.transform.position.z;
    }
}
