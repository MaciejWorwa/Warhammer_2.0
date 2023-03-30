using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackManager : MonoBehaviour
{
    private float attackDistance; // dystans pomiedzy walczacymi
    private bool targetDefended; // informacja o tym, czy postaci uda³o siê sparowaæ lub unikn¹æ atak

    private int attackBonus; // sumaryczna premia do WW lub US przy ataku
    private int chargeBonus; //premia za szar¿ê
    private int aimingBonus; //premia za przycelowanie

    private GameObject[] aimButtons; // przyciski celowania zarowno bohatera gracza jak i wroga

    void Start()
    {
        aimButtons = GameObject.FindGameObjectsWithTag("AimButton");
    }

    public void Attack(GameObject attacker, GameObject target)
    {
        do
        {
            // liczy dystans pomiedzy walczacymi
            if (attacker != null && target != null)
            {
                attackDistance = Vector3.Distance(attacker.transform.position, target.transform.position);
            }

            // sprawdza czy s¹ wybrane dwie postacie, ktore ze soba walcza
            if (Enemy.trSelect != null && Player.trSelect != null)
            {
                // sprawdza, czy dystans miedzy walczacymi jest mniejszy lub rowny zasiegowi broni atakujacego
                if (attackDistance <= attacker.GetComponent<Stats>().AttackRange)
                {
                    int wynik = Random.Range(1, 101);
                    bool hit = false;
                    //aimingBonus = attacker.GetComponent<Stats>().aimingBonus;

                    // sprawdza czy atak jest atakiem dystansowym
                    if (attackDistance > 1.5f)
                    {
                        // sprawdza czy bron jest naladowana
                        if (attacker.GetComponent<Stats>().reloadLeft == 0)
                        {
                            if (wynik <= attacker.GetComponent<Stats>().US + attackBonus)
                                hit = true;
                            else
                                hit = false;

                            if (attackBonus > 0)
                                Debug.Log("Rzut na US: " + wynik + " Premia: " + attackBonus);
                            else
                                Debug.Log("Rzut na US: " + wynik);

                            // resetuje naladowanie broni po wykonaniu strzalu
                            attacker.GetComponent<Stats>().reloadLeft = attacker.GetComponent<Stats>().reloadTime;
                        }
                        else
                        {
                            Debug.Log("Broñ wymaga na³adowania.");
                            break;
                        }
                    }
                    // sprawdza czy atak jest atakiem w zwarciu
                    if (attackDistance <= 1.5f)
                    {
                        //uwzglêdnienie bonusu do WW zwiazanego z szar¿¹
                        if (MovementManager.Charge == true)
                            chargeBonus = 10;
                        else
                            chargeBonus = 0;

                        attackBonus = chargeBonus + aimingBonus;

                        if (wynik <= attacker.GetComponent<Stats>().WW + attackBonus)
                            hit = true;
                        else
                            hit = false;

                        if (attackBonus > 0)
                            Debug.Log("Rzut na WW: " + wynik + " Premia: " + attackBonus);
                        else
                            Debug.Log("Rzut na WW: " + wynik);
                    }

                    //wywo³anie funkcji parowania lub uniku jeœli postaæ jeszcze mo¿e to robiæ w tej rundzie
                    if (hit && attackDistance <= 1.5f)
                    {
                        //sprawdza, czy atakowana postac ma wieksza szanse na unik, czy na parowanie i na tej podstawie ustala kolejnosc tych akcji
                        if (target.GetComponent<Stats>().WW + target.GetComponent<Stats>().parryBonus > target.GetComponent<Stats>().Zr)
                        {
                            if (target.GetComponent<Stats>().canParry)
                                ParryAttack(attacker, target);
                            else if (target.GetComponent<Stats>().canDodge)
                                DodgeAttack(target);
                        }
                        else
                        {
                            if (target.GetComponent<Stats>().canDodge)
                                DodgeAttack(target);
                            else if (target.GetComponent<Stats>().canParry)
                                ParryAttack(attacker, target);
                        }
                    }

                    // zresetowanie bonusu za celowanie, jeœli jest aktywny
                    if (aimingBonus != 0)
                        TakeAim();

                    if (hit == true && targetDefended != true)
                    {
                        int armor = CheckAttackLocalization(target);
                        int damage;
                        int rollResult;

                        // mechanika broni przebijajacej zbroje
                        if (attacker.GetComponent<Stats>().PrzebijajacyZbroje && armor >= 1)
                        {
                            armor--;
                        }

                        // mechanika bronii druzgoczacej
                        if (attacker.GetComponent<Stats>().Druzgoczacy)
                        {
                            int roll1 = Random.Range(1, 11);
                            int roll2 = Random.Range(1, 11);
                            if (roll1 > roll2)
                                rollResult = roll1;
                            else
                                rollResult = roll2;
                            Debug.Log($"Atak druzgocz¹c¹ broni¹. Rzut 1: {roll1} Rzut 2: {roll2}");
                        }
                        else
                            rollResult = Random.Range(1, 11);

                        // mechanika broni ciezkiej. Czyli po pierszym CELNYM ataku bron traci ceche druzgoczacy. Wg podrecznika traci sie to po pierwszej rundzie, ale wole tak :)
                        if (attacker.GetComponent<Stats>().Ciezki)
                            attacker.GetComponent<Stats>().Druzgoczacy = false;


                        // mechanika furii ulryka
                        if (rollResult == 10)
                        {
                            int confirmRoll = Random.Range(1, 101); //rzut na potwierdzenie furii
                            int additionalDamage = 0; //obrazenia ktore dodajemy do wyniku rzutu

                            if (attackDistance <= 1.5f)
                            {
                                if (attacker.GetComponent<Stats>().WW >= confirmRoll)
                                {
                                    additionalDamage = Random.Range(1, 11);
                                    rollResult = rollResult + additionalDamage;
                                    Debug.Log($"<color=red> FURIA ULRYKA! </color>");
                                }
                                else
                                    rollResult = 10;
                            }
                            else if (attackDistance > 1.5f)
                            {
                                if (attacker.GetComponent<Stats>().US >= confirmRoll)
                                {
                                    additionalDamage = Random.Range(1, 11);
                                    rollResult = rollResult + additionalDamage;
                                    Debug.Log($"<color=red> FURIA ULRYKA! </color>");
                                }
                                else
                                    rollResult = 10;
                            }

                            while (additionalDamage == 10)
                            {
                                additionalDamage = Random.Range(1, 11);
                                rollResult = rollResult + additionalDamage;
                                Debug.Log($"<color=red> KOLEJNA FURIA ULRYKA! </color>");
                            }
                        }

                        if (attackDistance <= 1.5f)
                            damage = rollResult + attacker.GetComponent<Stats>().S;
                        else
                            damage = rollResult + attacker.GetComponent<Stats>().Weapon_S;

                        Debug.Log($"{attacker.name} wyrzuci³ {rollResult} i zada³ <color=green>{damage} obra¿eñ.</color>");

                        if (damage > (target.GetComponent<Stats>().Wt + armor))
                        {
                            target.GetComponent<Stats>().tempHealth -= (damage - (target.GetComponent<Stats>().Wt + armor));
                            Debug.Log(target.name + " znegowa³ " + (target.GetComponent<Stats>().Wt + armor) + " obra¿eñ.");
                            Debug.Log($"<color=red> Punkty ¿ycia {target.name}: {target.GetComponent<Stats>().tempHealth}/{target.GetComponent<Stats>().maxHealth}</color>");

                            if (target == Enemy.selectedEnemy && Enemy.selectedEnemy.GetComponent<Stats>().criticalCondition == true)
                            {
                                Enemy.selectedEnemy.GetComponent<Stats>().GetCriticalHit();
                            }
                            else if (Player.selectedPlayer.GetComponent<Stats>().criticalCondition == true)
                            {
                                Player.selectedPlayer.GetComponent<Stats>().GetCriticalHit();
                            }
                        }
                        else
                            Debug.Log($"Atak {attacker.name} nie przebi³ siê przez pancerz.");
                    }
                    else
                        Debug.Log($"Atak {attacker.name} chybi³.");
                    targetDefended = false; // przestawienie boola na false, ¿eby przy kolejnym ataku znowu musia³ siê broniæ, a nie by³ obroniony na starcie
                }
                else
                    Debug.Log("Cel ataku stoi poza zasiêgiem.");
            }
            else
                Debug.Log("Musisz wybraæ atakuj¹cego oraz cel ataku.");
        }
        while (false);
    }

    int CheckAttackLocalization(GameObject target)
    {
        int attackLocalization = Random.Range(1, 101);
        int armor = 0;

        if (attackLocalization >= 1 && attackLocalization <= 15)
        {
            Debug.Log("Trafienie w g³owê");
            armor = target.GetComponent<Stats>().PZ_head;
        }
        if (attackLocalization >= 16 && attackLocalization <= 35)
        {
            Debug.Log("Trafienie w praw¹ rêkê");
            armor = target.GetComponent<Stats>().PZ_arms;
        }
        if (attackLocalization >= 36 && attackLocalization <= 55)
        {
            Debug.Log("Trafienie w lew¹ rêkê");
            armor = target.GetComponent<Stats>().PZ_arms;
        }
        if (attackLocalization >= 56 && attackLocalization <= 80)
        {
            Debug.Log("Trafienie w korpus");
            armor = target.GetComponent<Stats>().PZ_torso;
        }
        if (attackLocalization >= 81 && attackLocalization <= 90)
        {
            Debug.Log("Trafienie w praw¹ nogê");
            armor = target.GetComponent<Stats>().PZ_legs;
        }
        if (attackLocalization >= 91 && attackLocalization <= 100)
        {
            Debug.Log("Trafienie w lew¹ nogê");
            armor = target.GetComponent<Stats>().PZ_legs;
        }
        return armor;
    }

    private void ParryAttack(GameObject attacker, GameObject target)
    {
        // sprawdza, czy atakowany ma jakieœ bonusy do parowania
        if (target.GetComponent<Stats>().Parujacy)
            target.GetComponent<Stats>().parryBonus += 10;
        if (attacker.GetComponent<Stats>().Powolny)
            target.GetComponent<Stats>().parryBonus += 10;
        if (attacker.GetComponent<Stats>().Szybki)
            target.GetComponent<Stats>().parryBonus -= 10;

        target.GetComponent<Stats>().canParry = false;
        int wynik = Random.Range(1, 101);

        if (target.GetComponent<Stats>().parryBonus != 0)
            Debug.Log($"Rzut na parowanie: {wynik} Bonus do parowania: {target.GetComponent<Stats>().parryBonus}");
        else
            Debug.Log("Rzut na parowanie: " + wynik);

        if (wynik <= target.GetComponent<Stats>().WW + target.GetComponent<Stats>().parryBonus)
            targetDefended = true;
        else
            targetDefended = false;

        // zresetowanie bonusu do parowania
        target.GetComponent<Stats>().parryBonus = 0;
    }

    private void DodgeAttack(GameObject target)
    {
        target.GetComponent<Stats>().canDodge = false;
        int wynik = Random.Range(1, 101);

        Debug.Log("Rzut na unik: " + wynik);

        if (wynik <= target.GetComponent<Stats>().Zr)
            targetDefended = true;
        else
            targetDefended = false;
    }

    //TO PONI¯EJ DZIA£A DOBRZE, ALE KA¯DE PRZYCELOWANIE WROGA DODAJE ROWNIEZ BONUS GRACZOWI I ODWROTNIE.TRZEBA WTEDY WYKONYWAC ATAKI W ODPOWIEDNIEJ KOLEJNOSCI
    //przycelowanie
    public void TakeAim()
    {
        if (aimingBonus == 0)
        {
            Debug.Log("Przycelowanie");
            aimingBonus = 10;

            //zmiana koloru wszystkich przycisków AimButton, ¿eby by³o wiadomo, ¿e przycelowanie jest aktywne
            foreach (GameObject button in aimButtons)
            {
                button.GetComponent<Image>().color = Color.green;
            }
        }
        else
        {
            aimingBonus = 0;

            //przywrócenie domyœlnego koloru przyciskom
            foreach (GameObject button in aimButtons)
            {
                button.GetComponent<Image>().color = Color.white;
            }
        }

        attackBonus = chargeBonus + aimingBonus;
    }

    // Zaatakowanie bohatera gracza
    public void AttackPlayer()
    {
        Attack(Enemy.selectedEnemy, Player.selectedPlayer);
    }
    // Zaatakowanie wroga
    public void AttackEnemy()
    {
        Attack(Player.selectedPlayer, Enemy.selectedEnemy);
    }

    public void ReloadPlayer()
    {
        if (Player.selectedPlayer.GetComponent<Stats>().reloadLeft > 0)
            Player.selectedPlayer.GetComponent<Stats>().reloadLeft--;
        if (Player.selectedPlayer.GetComponent<Stats>().reloadLeft == 0)
            Debug.Log("Broñ za³adowana.");
        else
            Debug.Log($"£adowanie broni. Pozosta³a/y {Player.selectedPlayer.GetComponent<Stats>().reloadLeft} akcja/e aby móc strzeliæ.");

    }
    public void ReloadEnemy()
    {
        if (Enemy.selectedEnemy.GetComponent<Stats>().reloadLeft > 0)
            Enemy.selectedEnemy.GetComponent<Stats>().reloadLeft--;
        if (Enemy.selectedEnemy.GetComponent<Stats>().reloadLeft == 0)
            Debug.Log("Broñ za³adowana.");
        else
            Debug.Log($"£adowanie broni. Pozosta³a/y {Enemy.selectedEnemy.GetComponent<Stats>().reloadLeft} akcja/e aby móc strzeliæ.");
    }

    // Wykonanie ataku okazyjnego
    public void OpportunityAttack(GameObject attacker, GameObject target)
    {
        int wynik = Random.Range(1, 101);
        bool hit = false;

        Debug.Log($"Ruch spowodowa³ atak okazyjny. Rzut na WW <color=red>{attacker.name}</color>: {wynik}");

        if (wynik <= attacker.GetComponent<Stats>().WW)
                hit = true;
            else
                hit = false;

        if (hit == true)
        {
            int armor = CheckAttackLocalization(target);
            int damage;
            int rollResult;

            // mechanika broni przebijajacej zbroje
            if (attacker.GetComponent<Stats>().PrzebijajacyZbroje && armor >= 1)
            {
                armor--;
            }

            // mechanika bronii druzgoczacej
            if (attacker.GetComponent<Stats>().Druzgoczacy)
            {
                int roll1 = Random.Range(1, 11);
                int roll2 = Random.Range(1, 11);
                if (roll1 > roll2)
                    rollResult = roll1;
                else
                    rollResult = roll2;
                Debug.Log($"Atak druzgocz¹c¹ broni¹. Rzut 1: {roll1} Rzut 2: {roll2}");
            }
            else
                rollResult = Random.Range(1, 11);

            // mechanika broni ciezkiej. Czyli po pierszym CELNYM ataku bron traci ceche druzgoczacy. Wg podrecznika traci sie to po pierwszej rundzie, ale wole tak :)
            if (attacker.GetComponent<Stats>().Ciezki)
                attacker.GetComponent<Stats>().Druzgoczacy = false;

            // mechanika furii ulryka
            if (rollResult == 10)
            {
                int confirmRoll = Random.Range(1, 101); //rzut na potwierdzenie furii
                int additionalDamage = 0; //obrazenia ktore dodajemy do wyniku rzutu

                if (attackDistance <= 1.5f)
                {
                    if (attacker.GetComponent<Stats>().WW >= confirmRoll)
                    {
                        additionalDamage = Random.Range(1, 11);
                        rollResult = rollResult + additionalDamage;
                        Debug.Log($"<color=red> FURIA ULRYKA! </color>");
                    }
                    else
                        rollResult = 10;
                }
                else if (attackDistance > 1.5f)
                {
                    if (attacker.GetComponent<Stats>().US >= confirmRoll)
                    {
                        additionalDamage = Random.Range(1, 11);
                        rollResult = rollResult + additionalDamage;
                        Debug.Log($"<color=red> FURIA ULRYKA! </color>");
                    }
                    else
                        rollResult = 10;
                }

                while (additionalDamage == 10)
                {
                    additionalDamage = Random.Range(1, 11);
                    rollResult = rollResult + additionalDamage;
                    Debug.Log($"<color=red> KOLEJNA FURIA ULRYKA! </color>");
                }
            }

            damage = rollResult + attacker.GetComponent<Stats>().S;

            Debug.Log($"{attacker.name} wyrzuci³ {rollResult} i zada³ <color=green>{damage} obra¿eñ.</color>");

            if (damage > (target.GetComponent<Stats>().Wt + armor))
            {
                target.GetComponent<Stats>().tempHealth -= (damage - (target.GetComponent<Stats>().Wt + armor));
                Debug.Log(target.name + " znegowa³ " + (target.GetComponent<Stats>().Wt + armor) + " obra¿eñ.");
                Debug.Log($"<color=red> Punkty ¿ycia {target.name}: {target.GetComponent<Stats>().tempHealth}/{target.GetComponent<Stats>().maxHealth}</color>");

                if (target == Enemy.selectedEnemy && Enemy.selectedEnemy.GetComponent<Stats>().criticalCondition == true)
                {
                    Enemy.selectedEnemy.GetComponent<Stats>().GetCriticalHit();
                }
                else if (Player.selectedPlayer.GetComponent<Stats>().criticalCondition == true)
                {
                    Player.selectedPlayer.GetComponent<Stats>().GetCriticalHit();
                }
            }
            else
                Debug.Log($"Atak {attacker.name} nie przebi³ siê przez pancerz.");
        }
        else
            Debug.Log($"Atak {attacker.name} chybi³.");
    }
}
