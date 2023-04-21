using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpManager : MonoBehaviour
{
    private MessageManager messageManager;

    void Start()
    {
        // Odniesienie do Menadzera Wiadomosci wyswietlanych na ekranie gry
        messageManager = GameObject.Find("MessageManager").GetComponent<MessageManager>();
    }

    // Ustawia poziom postaci (Przecietny poziom poczatkowej postaci to około 62. Krasnolud 62, Elf 66, Czlowiek 63, Niziolek 59). Zakres waha sie miedzy 54-74. 
    //Funkcja ta powinna byc wywolywana zawsze przy tworzeniu nowej postaci oraz przy kazdym jej rozwijaniu
    public void SetCharacterLevel(GameObject character)
    {
        Stats stats = character.GetComponent<Stats>();

        // Przekazuje wszystkie cechy, ktore sa istotne przy obliczaniu poziomu postaci
        int level = CountLevelPoints(new object[]
        {   
            stats.WW,
            stats.US,
            stats.K,
            stats.Odp,
            stats.Zr,
            stats.Int,
            stats.SW,
            stats.Ogd,
            stats.A,       
            stats.Sz,
            stats.Mag,
            stats.maxHealth,
            stats.PP,
            stats.PZ_head,
            stats.PZ_arms,
            stats.PZ_torso,
            stats.PZ_legs,
            stats.existDodge,
            stats.instantReload,
            stats.Ciezki,
            stats.Druzgoczacy,
            stats.Parujacy,
            stats.Powolny,
            stats.PrzebijajacyZbroje,
            stats.Szybki
        }, new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 }); // przekazuje indexy wartosci, ktore maja byc dodane w calosci, a nie podzielone przez 5 (jak w przypadku cech glownych)

        // Ustawia poziom postaci
        stats.Level = level;
        Debug.Log($"Poziom postaci {stats.Name}: {level}");
    }


    // Liczy ilosc punktow okreslajaca poziom postaci. Jest to suma kazdej z cech pierwszorzedowych podzielonych przez 5, kazdej cechy drugorzedowej, punktow zbroi, atrybutow broni i zdolnosci
    public static int CountLevelPoints(object[] variables, List<int> pierwszorzedowe)
    {
        int level = 0;

        for (int i = 0; i < variables.Length; i++)
        {
            object variable = variables[i];

            if (variable is bool b)
            {
                level += b ? 1 : 0;
            }
            else if (variable is int j)
            {
                if (pierwszorzedowe.Contains(i))
                {
                    level += j / 5;
                }
                else
                {
                    level += j;
                }
            }
            else
            {
                Debug.Log("Coś poszlo nie tak :(");
            }
        }

        return level;
    }


    // Zdobycie punktow dosiadczenia. (Dorobic zmienna Exp w klasie Stats. Zrobic mechanike rozwiniec postaci. Kazde rozwiniecie kosztuje 100 expa. Dorobic schematy rozwoju.)
    // Ta funkcja powinna byc wywolana po smierci postaci. Argumentami sa postac ktora ja zabila, oraz ta ktora zginela.
    public void GainExp(GameObject attacker, GameObject target)
    {
        Stats attackerStats = attacker.GetComponent<Stats>();
        Stats targetStats = target.GetComponent<Stats>();

        if(((targetStats.Level - attackerStats.Level + 20) /2) > 0)
        {
            attackerStats.Exp += (targetStats.Level - attackerStats.Level + 20) /2;
            messageManager.ShowMessage($"<color=#D82FDE>{attackerStats.Name} zdobył {(targetStats.Level - attackerStats.Level + 20) / 2} punktów doświadczenia. Posiada łącznie {attackerStats.Exp}.</color>", 5f);
            Debug.Log($"<color=#D82FDE>{attackerStats.Name} zdobył {(targetStats.Level - attackerStats.Level + 20) /2} punktów doświadczenia. Posiada łącznie {attackerStats.Exp}.</color>");
        }
        else
        {
            messageManager.ShowMessage($"{attackerStats.Name} zdobył 0 punktów doświadczenia. Posiada łącznie {attackerStats.Exp}.", 5f);
            Debug.Log($"{attackerStats.Name} zdobył 0 punktów doświadczenia. Posiada łącznie {attackerStats.Exp}.");
        }
    }


}
