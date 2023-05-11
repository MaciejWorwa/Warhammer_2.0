using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MagicManager : MonoBehaviour
{
    private int powerLevelBonus;

    private MessageManager messageManager;
    private CharacterManager characterManager;

    public static bool targetSelecting;
    public static GameObject target;

    [SerializeField] private GameObject etherArmorButton;

    void Start()
    {
        // Odniesienie do Menadzera Wiadomosci wyswietlanych na ekranie gry
        messageManager = GameObject.Find("MessageManager").GetComponent<MessageManager>();

        characterManager = GameObject.Find("CharacterManager").GetComponent<CharacterManager>();
    }

    // Splatanie magii
    public void ChannelingMagic()
    {
        GameObject character = Character.selectedCharacter;
        Stats charStats = Character.selectedCharacter.GetComponent<Stats>();

        if(charStats.ChannelingMagic == 0)
        {
            messageManager.ShowMessage($"<color=red>Postać nie potrafi splatać magii.</color>", 3f);
            Debug.Log($"Postać nie potrafi splatać magii.");
            return;
        }

        if (charStats.actionsLeft > 0)
            characterManager.TakeAction(charStats);
        else if (GameManager.StandardMode)
        {
            messageManager.ShowMessage($"<color=red>Postać nie może wykonać tylu akcji w tej rundzie.</color>", 3f);
            Debug.Log($"Postać nie może wykonać tylu akcji w tej rundzie.");
            return;
        }

        int rollResult = Random.Range(1, 101);
        int bonus = 0;
        if (charStats.MagicSense == true)
            bonus = 10;
        for (int i = 0; i < charStats.ChannelingMagic; i++)
            bonus += i * 10;

        if (charStats.SW + bonus >= rollResult)
        {
            powerLevelBonus += charStats.Mag;
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
        GameObject character = Character.selectedCharacter;
        Stats charStats = Character.selectedCharacter.GetComponent<Stats>();

        // Lista i słownik wszystkich wyników rzutów, potrzebne do sprawdzenia wystąpienia manifestacji chaosu
        List<int> allRollResults = new List<int>();
        Dictionary<int, int> doubletCount = new Dictionary<int, int>();

        // Rzuty na poziom mocy w zależności od wartości Magii
        for (int i = 0; i < charStats.Mag; i++)
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
        Stats charStats = Character.selectedCharacter.GetComponent<Stats>();

        if (charStats.CastDurationLeft == 1 && charStats.actionsLeft > 0 || charStats.CastDurationLeft > 1 && charStats.actionsLeft == 1)
        {
            characterManager.TakeAction(charStats);
            charStats.CastDurationLeft--;
        }
        else if (charStats.CastDurationLeft > 1 && charStats.actionsLeft == 2)
        {
            characterManager.TakeDoubleAction(charStats);
            charStats.CastDurationLeft -= 2;
        }
        else if (GameManager.StandardMode && charStats.actionsLeft == 0)
        {
            messageManager.ShowMessage($"<color=red>Postać nie może wykonać tylu akcji w tej rundzie.</color>", 3f);
            Debug.Log($"Postać nie może wykonać tylu akcji w tej rundzie.");
            return;
        }

        // Jeżeli nie skończył rzucać zaklęcia w tej rundzie to akcja jest przerywana
        if (charStats.CastDurationLeft > 0 && GameManager.StandardMode)
        {
            messageManager.ShowMessage($"Pozostała/y {charStats.CastDurationLeft} akcja/e aby rzucić zaklęcie.", 4f);
            Debug.Log($"Pozostała/y {charStats.CastDurationLeft} akcja/e aby rzucić zaklęcie.");
            return;
        }

        // Zresetowanie czasu rzucania zaklęcia
        charStats.CastDurationLeft = charStats.CastDuration;

        int powerLevel = PowerLevelRoll();

        if(powerLevel < charStats.PowerRequired)
        {
            messageManager.ShowMessage($"<color=red>Nie udało się uzyskać wystarczającego poziomu mocy.</color>", 6f);
            Debug.Log($"<color=red>Nie udało się uzyskać wystarczającego poziomu mocy.</color>");
            return;
        }
        else
            powerLevel = (Random.Range(1,11) + charStats.Mag);
        
        // Leczy punkty życia 
        target.GetComponent<Stats>().tempHealth += powerLevel;
        // Zapobiega wyleczeniu ponad maksymalne punkty życia
        if (target.GetComponent<Stats>().tempHealth > target.GetComponent<Stats>().maxHealth)
            target.GetComponent<Stats>().tempHealth = target.GetComponent<Stats>().maxHealth;

        messageManager.ShowMessage($"<color=#00FF9A>{charStats.Name} wyleczył {powerLevel} punktów życia dla {target.GetComponent<Stats>().Name}</color>", 6f);
        Debug.Log($"<color=#00FF9A>{charStats.Name} wyleczył {powerLevel} punktów życia dla {target.GetComponent<Stats>().Name}</color>");

    }
    #endregion

    #region Get magic damage
    public void GetMagicDamage(GameObject target)
    {
        GameObject character = Character.selectedCharacter;
        Stats charStats = Character.selectedCharacter.GetComponent<Stats>();
        string targetName = target.GetComponent<Stats>().Name;

        if (charStats.CastDurationLeft == 1 && charStats.actionsLeft > 0 || charStats.CastDurationLeft > 1 && charStats.actionsLeft == 1)
        {
            characterManager.TakeAction(charStats);
            charStats.CastDurationLeft--;
        }
        else if (charStats.CastDurationLeft > 1 && charStats.actionsLeft == 2)
        {
            characterManager.TakeDoubleAction(charStats);
            charStats.CastDurationLeft -= 2;
        }
        else if (GameManager.StandardMode && charStats.actionsLeft == 0)
        {
            messageManager.ShowMessage($"<color=red>Postać nie może wykonać tylu akcji w tej rundzie.</color>", 3f);
            Debug.Log($"Postać nie może wykonać tylu akcji w tej rundzie.");
            return;
        }

        // Jeżeli nie skończył rzucać zaklęcia w tej rundzie to akcja jest przerywana
        if (charStats.CastDurationLeft > 0 && GameManager.StandardMode)
        {
            messageManager.ShowMessage($"Pozostała/y {charStats.CastDurationLeft} akcja/e aby rzucić zaklęcie.", 4f);
            Debug.Log($"Pozostała/y {charStats.CastDurationLeft} akcja/e aby rzucić zaklęcie.");
            return;
        }

        double range = charStats.SpellRange;

        if (range < Vector3.Distance(character.transform.position, target.transform.position))
        {
            messageManager.ShowMessage($"<color=red>Cel jest poza zasięgiem zaklęcia.</color>", 3f);
            Debug.Log($"<color=red>Cel jest poza zasięgiem zaklęcia.</color>");
            return;
        }

        // Zresetowanie czasu rzucania zaklęcia
        charStats.CastDurationLeft = charStats.CastDuration;

        int powerLevel = PowerLevelRoll();

        if (powerLevel < charStats.PowerRequired)
        {
            messageManager.ShowMessage($"<color=red>Nie udało się uzyskać wystarczającego poziomu mocy.</color>", 6f);
            Debug.Log($"<color=red>Nie udało się uzyskać wystarczającego poziomu mocy.</color>");
            return;
        }

        int rollResult = Random.Range(1, 11);
        int damage = charStats.Spell_S + rollResult;
        double areaSize = charStats.AreaSize;

        int armor = GameObject.Find("AttackManager").GetComponent<AttackManager>().CheckAttackLocalization(target.GetComponent<Stats>());

        messageManager.ShowMessage($"<color=#00FF9A>{charStats.Name}</color> wyrzucił {rollResult} i zadał <color=#00FF9A>{damage} obrażeń.</color>", 8f);
        Debug.Log($"{charStats.Name} wyrzucił {rollResult} i zadał {damage} obrażeń.");
     

        if (areaSize > 0)
        {
            Collider2D[] allTargets = Physics2D.OverlapCircleAll(target.transform.position, (float)areaSize);

            foreach (var tar in allTargets)
            {
                if (tar.gameObject.CompareTag("Player") || tar.gameObject.CompareTag("Enemy"))
                {
                    Stats targetStats = tar.gameObject.GetComponent<Stats>();
                    int damageReduction = targetStats.Wt + (charStats.IgnoreArmor ? 0 : armor);

                    if (damage > damageReduction)
                    {
                        targetStats.tempHealth -= (damage - damageReduction);
                        messageManager.ShowMessage($"{targetStats.Name} znegował {damageReduction} obrażeń.", 8f);
                        Debug.Log($"{targetStats.Name} znegował {damageReduction} obrażeń.");
                    }
                    else
                    {
                        messageManager.ShowMessage($"Atak {charStats.Name} nie przebił się przez pancerz.", 6f);
                        Debug.Log($"Atak {charStats.Name} nie przebił się przez pancerz.");
                    }

                    messageManager.ShowMessage($"Punkty życia {targetStats.Name}: {targetStats.tempHealth}/{targetStats.maxHealth}", 8f);
                    Debug.Log($"Punkty życia {targetStats.Name}: {targetStats.tempHealth}/{targetStats.maxHealth}");
                }
            }
        }
        else
        {
            Stats targetStats = target.GetComponent<Stats>();
            int damageReduction = targetStats.Wt + (charStats.IgnoreArmor ? 0 : armor);

            if (damage > damageReduction)
            {
                targetStats.tempHealth -= (damage - damageReduction);
                messageManager.ShowMessage($"{targetName} znegował {damageReduction} obrażeń.", 8f);
                Debug.Log($"{targetName} znegował {damageReduction} obrażeń.");
            }
            else
            {
                messageManager.ShowMessage($"Atak {charStats.Name} nie przebił się przez pancerz.", 6f);
                Debug.Log($"Atak {charStats.Name} nie przebił się przez pancerz.");
            }

            messageManager.ShowMessage($"Punkty życia {targetName}: {targetStats.tempHealth}/{targetStats.maxHealth}", 8f);
            Debug.Log($"Punkty życia {targetName}: {targetStats.tempHealth}/{targetStats.maxHealth}");
        }
    }
    #endregion

    #region Ether Armor spell mechanics
    public void EtherArmorSpell()
    {
        Stats charStats = Character.selectedCharacter.GetComponent<Stats>();

        if (charStats.actionsLeft > 0)
            characterManager.TakeAction(charStats);
        else if (GameManager.StandardMode)
        {
            messageManager.ShowMessage($"<color=red>Postać nie może wykonać tylu akcji w tej rundzie.</color>", 3f);
            Debug.Log($"Postać nie może wykonać tylu akcji w tej rundzie.");
            return;
        }

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
