using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AverageLevelCounter : MonoBehaviour
{
    // JEST TO KLASA TYMCZASOWA. ULATWIA LICZENIE POZIOMOW I W PRZYSZLOSCI CHCIALBYM TUTAJ ROBIC TESTY TEGO ILE EXPA POWINNO SIE PRZYZNAWAC ZA KONKRETNYCH PRZECIWNIKOW.

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            CountAverageLevelOfCharacters();
        }
    }

    void CountAverageLevelOfCharacters()
    {
        int sumaCzlowiek= 0;
        int sumaKrasnolud= 0;
        int sumaElf= 0;
        int sumaNiziolek= 0;
        int suma = 0;

        int iloscElf = 0;
        int iloscCzlowiek = 0;
        int iloscKrasnolud = 0;
        int iloscNiziolek = 0;

        int maxLevel = 0;
        int minLevel = 0;

        GameObject[] gracze = GameObject.FindGameObjectsWithTag("Player");

        foreach(var gracz in gracze)
        {
            suma += gracz.GetComponent<Stats>().Level;

            if(maxLevel < gracz.GetComponent<Stats>().Level)
                maxLevel = gracz.GetComponent<Stats>().Level;
            if(minLevel > gracz.GetComponent<Stats>().Level || minLevel == 0)
                minLevel = gracz.GetComponent<Stats>().Level;

            if(gracz.GetComponent<Character>().rasa == Character.Rasa.Krasnolud)
            {
                sumaKrasnolud += gracz.GetComponent<Stats>().Level;
                iloscKrasnolud++;
            }
            else if(gracz.GetComponent<Character>().rasa == Character.Rasa.Człowiek)
            {
                iloscCzlowiek++;
                sumaCzlowiek += gracz.GetComponent<Stats>().Level;
            }
            else if(gracz.GetComponent<Character>().rasa == Character.Rasa.Elf)
            {
                iloscElf++;
                sumaElf += gracz.GetComponent<Stats>().Level;
            }
            else if(gracz.GetComponent<Character>().rasa == Character.Rasa.Niziołek)
            {
                sumaNiziolek += gracz.GetComponent<Stats>().Level;
                iloscNiziolek++;
            }
        }

        if(gracze.Length > 0)
            Debug.Log($"srednia ogolna: {suma / gracze.Length}");

        if (iloscCzlowiek > 0)
            Debug.Log($"srednia czlowiek: {sumaCzlowiek/iloscCzlowiek}");
        if (iloscKrasnolud > 0)
            Debug.Log($"srednia krasnolud: {sumaKrasnolud/iloscKrasnolud}");
        if (iloscElf > 0)
            Debug.Log($"srednia elf: {sumaElf/iloscElf}");
        if (iloscNiziolek > 0)
            Debug.Log($"srednia niziolek: {sumaNiziolek/iloscNiziolek}");

        Debug.Log($"ilosc ludzi {iloscCzlowiek}");
        Debug.Log($"ilosc krasnoludow {iloscKrasnolud}");
        Debug.Log($"ilosc elfow {iloscElf}");
        Debug.Log($"ilosc niziolkow {iloscNiziolek}");

        Debug.Log($"min. poziom: {minLevel}");
        Debug.Log($"max. poziom: {maxLevel}");
    }
}
