using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MagicManager : MonoBehaviour
{
    private int powerLevelBonus;
    private GameObject wizard;

    private MessageManager messageManager;

    public static bool targetSelecting;
    public static GameObject target;

    [SerializeField] private GameObject etherArmorButton;

    void Start()
    {
        // Odniesienie do Menadzera Wiadomosci wyswietlanych na ekranie gry
        messageManager = GameObject.Find("MessageManager").GetComponent<MessageManager>();
    }

    // Splatanie magii
    public void ChannelingMagic()
    {
        wizard = Character.selectedCharacter;

        int rollResult = Random.Range(1, 101);

        if (wizard.GetComponent<Stats>().SW >= rollResult)
        {
            powerLevelBonus += wizard.GetComponent<Stats>().Mag;
            messageManager.ShowMessage($"Wynik rzutu: {rollResult}. Splatanie magii zakończone sukcesem.", 4f);
            Debug.Log($"Wynik rzutu: {rollResult}. Splatanie magii zakończone sukcesem.");
        }
        else
        {
            powerLevelBonus = 0;
            messageManager.ShowMessage($"Wynik rzutu: {rollResult}. Splatanie magii zakończone niepowodzeniem.", 4f);
            Debug.Log($"Wynik rzutu: {rollResult}. Splatanie magii zakończone niepowodzeniem.");
        }
    }

    // Rzut na poziom mocy
    #region Roll for power level
    public int PowerLevelRoll()
    {
        // Zresetowanie poziomu mocy
        int powerLevel = 0;

        // Ustalenie kto rzuca zaklęcie
        wizard = Character.selectedCharacter;

        // Lista i słownik wszystkich wyników rzutów, potrzebne do sprawdzenia wystąpienia manifestacji chaosu
        List<int> allRollResults = new List<int>();
        Dictionary<int, int> doubletCount = new Dictionary<int, int>();

        // Rzuty na poziom mocy w zależności od wartości Magii
        for (int i = 0; i < wizard.GetComponent<Stats>().Mag; i++)
        {
            int rollResult = Random.Range(1, 11);
            allRollResults.Add(rollResult);
            powerLevel += rollResult;

            messageManager.ShowMessage($"Wynik rzutu na poziom mocy, kość {i + 1}: {rollResult}", 6f);
            Debug.Log($"Wynik rzutu na poziom mocy, kość {i+1}: {rollResult}");
        }
        messageManager.ShowMessage($"Uzyskany poziom mocy: <color=red>{powerLevel + powerLevelBonus}</color>", 6f);
        Debug.Log($"Uzyskany poziom mocy: <color=red>{powerLevel + powerLevelBonus}</color>");

        // Liczenie dubletów
        foreach (int rollResult in allRollResults)
        {
            if (doubletCount.ContainsKey(rollResult))
                doubletCount[rollResult] += 1; // jeśli wartość istnieje w słowniku, zwiększamy jej licznik
            else
                doubletCount.Add(rollResult, 1); // jeśli wartość nie istnieje w słowniku, dodajemy ją i ustawiamy licznik na 1
        }

        // Rzuty na manifestację w zależności od ilości wyników, które się powtórzyły
        foreach (KeyValuePair<int, int> kvp in doubletCount)
        {
            int value = kvp.Key;
            int count = kvp.Value;
            if (count == 2)
            {
                int rollResult = Random.Range(1, 101);
                messageManager.ShowMessage($"Wartość {value} występuje {count} razy.", 6f);
                Debug.Log($"Wartość {value} występuje {count} razy.");
                messageManager.ShowMessage($"<color=red>POMNIEJSZA MANIFESTACJA CHAOSU!</color> Wynik rzutu na manifestację: {rollResult}", 6f);
                Debug.Log($"<color=red>POMNIEJSZA MANIFESTACJA CHAOSU!</color> Wynik rzutu na manifestację: {rollResult}");
            }
            else if (count == 3)
            {
                int rollResult = Random.Range(1, 101);
                messageManager.ShowMessage($"Wartość {value} występuje {count} razy.", 6f);
                Debug.Log($"Wartość {value} występuje {count} razy.");
                messageManager.ShowMessage($"<color=red>POWAŻNA MANIFESTACJA CHAOSU!</color> Wynik rzutu na manifestację: {rollResult}", 6f);
                Debug.Log($"<color=red>POWAŻNA MANIFESTACJA CHAOSU!</color> Wynik rzutu na manifestację: {rollResult}");
            }
            else if (count > 3)
            {
                int rollResult = Random.Range(1, 101);
                messageManager.ShowMessage($"Wartość {value} występuje {count} razy.", 6f);
                Debug.Log($"Wartość {value} występuje {count} razy.");
                messageManager.ShowMessage($"<color=red>KATASTROFALNA MANIFESTACJA CHAOSU!</color> Wynik rzutu na manifestację: {rollResult}", 6f);
                Debug.Log($"<color=red>KATASTROFALNA MANIFESTACJA CHAOSU!</color> Wynik rzutu na manifestację: {rollResult}");
            }
        }

        return powerLevel;
    }
    #endregion

    public void SetSpellToOffensive()
    {
        Character.selectedCharacter.GetComponent<Stats>().OffensiveSpell = true;
    }

    public void SetSpellToDeffensive()
    {
        Character.selectedCharacter.GetComponent<Stats>().OffensiveSpell = false;
    }

    #region Healing spell
    public void HealingSpell(GameObject target)
    {
        GameObject character = Character.selectedCharacter;

        int powerLevel = PowerLevelRoll();

        if(powerLevel < character.GetComponent<Stats>().PowerRequired)
        {
            messageManager.ShowMessage($"<color=red>Nie udało się uzyskać wystarczającego poziomu mocy.</color>", 6f);
            Debug.Log($"<color=red>Nie udało się uzyskać wystarczającego poziomu mocy.</color>");
            return;
        }
        else
            powerLevel = (Random.Range(1,11) + character.GetComponent<Stats>().Mag);
        
        // Leczy punkty życia 
        target.GetComponent<Stats>().tempHealth += powerLevel;
        // Zapobiega wyleczeniu ponad maksymalne punkty życia
        if (target.GetComponent<Stats>().tempHealth > target.GetComponent<Stats>().maxHealth)
            target.GetComponent<Stats>().tempHealth = target.GetComponent<Stats>().maxHealth;

        messageManager.ShowMessage($"<color=#00FF9A>{character.GetComponent<Stats>().Name} wyleczył {powerLevel} punktów życia dla {target.GetComponent<Stats>().Name}</color>", 6f);
        Debug.Log($"<color=#00FF9A>{character.GetComponent<Stats>().Name} wyleczył {powerLevel} punktów życia dla {target.GetComponent<Stats>().Name}</color>");

    }
    #endregion

    #region Get magic damage
    public void GetMagicDamage(GameObject target)
    {
        GameObject character = Character.selectedCharacter;

        double range = character.GetComponent<Stats>().SpellRange;

        if(range < Vector3.Distance(character.transform.position, target.transform.position))
        {
            messageManager.ShowMessage($"<color=red>Cel jest poza zasięgiem zaklęcia.</color>", 3f);
            Debug.Log($"<color=red>Cel jest poza zasięgiem zaklęcia.</color>");
            return;
        }

        int powerLevel = PowerLevelRoll();

        if(powerLevel < character.GetComponent<Stats>().PowerRequired)
        {
            messageManager.ShowMessage($"<color=red>Nie udało się uzyskać wystarczającego poziomu mocy.</color>", 6f);
            Debug.Log($"<color=red>Nie udało się uzyskać wystarczającego poziomu mocy.</color>");
            return;
        }

        int rollResult = Random.Range(1,11);
        int damage = character.GetComponent<Stats>().Spell_S + rollResult;
        double areaSize = character.GetComponent<Stats>().AreaSize;
        int castDuration = character.GetComponent<Stats>().CastDuration;

        int armor = GameObject.Find("AttackManager").GetComponent<AttackManager>().CheckAttackLocalization(target);

        messageManager.ShowMessage($"<color=#00FF9A>{character.GetComponent<Stats>().Name}</color> wyrzucił {rollResult} i zadał <color=#00FF9A>{damage} obrażeń.</color>", 8f);
        Debug.Log($"{character.GetComponent<Stats>().Name} wyrzucił {rollResult} i zadał {damage} obrażeń.");

        if(!character.GetComponent<Stats>().IgnoreArmor && damage > (target.GetComponent<Stats>().Wt + armor))
        {
            target.GetComponent<Stats>().tempHealth -= (damage - (target.GetComponent<Stats>().Wt + armor));

            messageManager.ShowMessage(target.GetComponent<Stats>().Name + " znegował " + (target.GetComponent<Stats>().Wt + armor) + " obrażeń.", 8f);
            Debug.Log(target.GetComponent<Stats>().Name + " znegował " + (target.GetComponent<Stats>().Wt + armor) + " obrażeń.");

            messageManager.ShowMessage($"<color=red> Punkty życia {target.GetComponent<Stats>().Name}: {target.GetComponent<Stats>().tempHealth}/{target.GetComponent<Stats>().maxHealth}</color>", 8f);
            Debug.Log($"Punkty życia {target.GetComponent<Stats>().Name}: {target.GetComponent<Stats>().tempHealth}/{target.GetComponent<Stats>().maxHealth}");
        }
        else if (character.GetComponent<Stats>().IgnoreArmor && damage > target.GetComponent<Stats>().Wt)
        {
            target.GetComponent<Stats>().tempHealth -= (damage - target.GetComponent<Stats>().Wt);

            messageManager.ShowMessage(target.GetComponent<Stats>().Name + " znegował " + (target.GetComponent<Stats>().Wt) + " obrażeń.", 8f);
            Debug.Log(target.GetComponent<Stats>().Name + " znegował " + (target.GetComponent<Stats>().Wt) + " obrażeń.");

            messageManager.ShowMessage($"<color=red> Punkty życia {target.GetComponent<Stats>().Name}: {target.GetComponent<Stats>().tempHealth}/{target.GetComponent<Stats>().maxHealth}</color>", 8f);
            Debug.Log($"Punkty życia {target.GetComponent<Stats>().Name}: {target.GetComponent<Stats>().tempHealth}/{target.GetComponent<Stats>().maxHealth}");
        }
        else
        {
            messageManager.ShowMessage($"Atak <color=red>{character.GetComponent<Stats>().Name}</color> nie przebił się przez pancerz.", 6f);
            Debug.Log($"Atak {character.GetComponent<Stats>().Name} nie przebił się przez pancerz.");
        }   
    }
    #endregion

    #region Ether Armor spell mechanics
    public void EtherArmorSpell()
    {
        Stats charStats = Character.selectedCharacter.GetComponent<Stats>();

        if (charStats.etherArmorActive == true)
        {
            charStats.etherArmorActive = false;
            etherArmorButton.GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);

            charStats.PZ_head = 0;
            charStats.PZ_arms = 0;
            charStats.PZ_torso = 0;
            charStats.PZ_legs = 0;

            messageManager.ShowMessage($"<color=red>Pancerz Eteru dezaktywowany.</color>", 3f);
            Debug.Log($"<color=red>Pancerz Eteru dezaktywowany.</color>");
            return;
        }
        if (charStats.PZ_head > 0 || charStats.PZ_arms > 0 || charStats.PZ_torso > 0 || charStats.PZ_legs > 0)
        {
            messageManager.ShowMessage($"<color=red>Nie możesz rzucać Pancerzu Eteru jeśli nosisz zbroję.</color>", 3f);
            Debug.Log($"<color=red>Nie możesz rzucać Pancerzu Eteru jeśli nosisz zbroję.</color>");
            return;
        }

        int powerLevel = PowerLevelRoll();
        if(powerLevel < 5)
        {
            messageManager.ShowMessage($"<color=red>Nie udało się uzyskać wystarczającego poziomu mocy.</color>", 6f);
            Debug.Log($"<color=red>Nie udało się uzyskać wystarczającego poziomu mocy.</color>");
            return;
        }

        charStats.etherArmorActive = true;
        etherArmorButton.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);

        charStats.PZ_head = charStats.Mag;
        charStats.PZ_arms = charStats.Mag;
        charStats.PZ_torso = charStats.Mag;
        charStats.PZ_legs = charStats.Mag;

        messageManager.ShowMessage($"<color=#00FF9A>Pancerz Eteru aktywowany.</color>", 6f);
        Debug.Log($"<color=#00FF9A>Pancerz Eteru aktywowany.</color>");
    }
    #endregion
}
