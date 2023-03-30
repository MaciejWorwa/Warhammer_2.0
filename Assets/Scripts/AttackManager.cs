using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackManager : MonoBehaviour
{
    private float attackDistance;
    private bool targetDefended; // informacja o tym, czy postaci uda³o siê sparowaæ lub unikn¹æ atak

    private int bonus; // sumaryczna premia do WW lub US
    private int chargeBonus; //premia za szar¿ê
    private int aimingBonus; //premia za przycelowanie

    //private GameObject playerAimButton;
    //private GameObject enemyAimButton;

    private GameObject[] aimButtons;

    void Start()
    {
        aimButtons = GameObject.FindGameObjectsWithTag("AimButton");
    }

    void Update()
    {
        bonus = chargeBonus + aimingBonus;
    }

    //public void FindAndSetActionsButtons(GameObject AB, int Enemy0Player1)
    //{
    //    if (Enemy0Player1 == 1)
    //        playerAimButton = AB;
    //    else if (Enemy0Player1 == 0)
    //        enemyAimButton = AB;
    //}

    public void Attack(GameObject attacker, GameObject target)
    {
        do
        {
            //liczy dystans pomiedzy walczacymi
            if (attacker != null && target != null)
            {
                attackDistance = Vector3.Distance(attacker.transform.position, target.transform.position);
            }

            if (Enemy.trSelect != null && Player.trSelect != null)
            {
                if (attackDistance <= attacker.GetComponent<Stats>().AttackRange)
                {
                    int wynik = Random.Range(1, 101);
                    bool hit = false;
                    //aimingBonus = attacker.GetComponent<Stats>().aimingBonus;

                    if (attackDistance > 1.5f)
                    {
                        if (attacker.GetComponent<Stats>().reloadLeft == 0)
                        {
                            if (wynik <= attacker.GetComponent<Stats>().US + bonus)
                                hit = true;
                            else
                                hit = false;

                            if (bonus > 0)
                                Debug.Log("Rzut na US: " + wynik + " Premia: " + bonus);
                            else
                                Debug.Log("Rzut na US: " + wynik);

                            attacker.GetComponent<Stats>().reloadLeft = attacker.GetComponent<Stats>().reloadTime;
                        }
                        else
                        {
                            Debug.Log("Broñ wymaga na³adowania.");
                            break;
                        }
                    }
                    if (attackDistance <= 1.5f)
                    {
                        //uwzglêdnienie bonusu do WW zwiazanego z szar¿¹
                        if (MovementManager.Charge == true)
                            chargeBonus = 10;
                        else
                            chargeBonus = 0;

                        if (wynik <= attacker.GetComponent<Stats>().WW + bonus)
                            hit = true;
                        else
                            hit = false;

                        if (bonus > 0)
                            Debug.Log("Rzut na WW: " + wynik + " Premia: " + bonus);
                        else
                            Debug.Log("Rzut na WW: " + wynik);
                    }

                    //wywo³anie funkcji parowania lub uniku jeœli postaæ jeszcze mo¿e to robiæ w tej rundzie
                    if (hit && attackDistance <= 1.5f)
                    {
                        //sprawdza, czy atakowana postac ma wieksza szanse na unik, czy na parowanie i na tej podstawie ustala kolejnosc tych akcji
                        if (target.GetComponent<Stats>().WW > target.GetComponent<Stats>().Zr)
                        {
                            if (target.GetComponent<Stats>().canParry)
                                ParryAttack(target);
                            else if (target.GetComponent<Stats>().canDodge)
                                DodgeAttack(target);
                        }
                        else
                        {
                            if (target.GetComponent<Stats>().canDodge)
                                DodgeAttack(target);
                            else if (target.GetComponent<Stats>().canParry)
                                ParryAttack(target);
                        }
                    }

                    // zresetowanie bonusu za celowanie, jeœli jest aktywny
                    if (aimingBonus != 0)
                        TakeAim();
                    //if (Enemy.selectedEnemy.GetComponent<Stats>().aimingBonus != 0)
                    //    TakeAimEnemy();
                    //if (Player.selectedPlayer.GetComponent<Stats>().aimingBonus != 0)
                    //    TakeAimPlayer();

                    if (hit == true && targetDefended != true)
                    {
                        int armor = CheckAttackLocalization(target);
                        int damage;
                        int rollResult;

                        //mechanika broni przebijajacej zbroje
                        if (attacker.GetComponent<Stats>().PrzebijajacyZbroje == true && armor >= 1)
                        {
                            armor--;
                        }

                        //mechanika bronii druzgoczacej
                        if (attacker.GetComponent<Stats>().Druzgoczacy == true)
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

                        //mechanika furii ulryka
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

                            if (target == Enemy.selectedEnemy && Enemy.selectedEnemy.GetComponent<Stats>().criticalCondition == true)//to trzeba przekminic, bo na razie jest static, wiec jak umrze jakakolwiek instancja Enemy to sie uruchamia nawet gdy zadamy obrazenia innemu
                            {
                                Enemy.selectedEnemy.GetComponent<Stats>().GetCriticalHit();
                            }
                            else if (Player.selectedPlayer.GetComponent<Stats>().criticalCondition == true)//to trzeba przekminic, bo na razie jest static, wiec jak umrze jakakolwiek instancja Enemy to sie uruchamia nawet gdy zadamy obrazenia innemu
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

    private void ParryAttack(GameObject target)
    {
        target.GetComponent<Stats>().canParry = false;
        int wynik = Random.Range(1, 101);

        Debug.Log("Rzut na parowanie: " + wynik);

        if (wynik <= target.GetComponent<Stats>().WW)
            targetDefended = true;
        else
            targetDefended = false;    
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
    }


    ////przycelowanie gracza
    //public void TakeAimPlayer()
    //{
    //    if (Player.selectedPlayer.GetComponent<Stats>().aimingBonus == 0)
    //    {
    //        Debug.Log("Przycelowanie");
    //        Player.selectedPlayer.GetComponent<Stats>().aimingBonus = 10;

    //        //zmiana koloru wszystkich przycisków AimButton, ¿eby by³o wiadomo, ¿e przycelowanie jest aktywne
    //        foreach (GameObject button in aimButtons)
    //        {
    //            button.GetComponent<Image>().color = Color.green;
    //        }
    //    }
    //    else
    //    {
    //        Player.selectedPlayer.GetComponent<Stats>().aimingBonus = 0;

    //        //przywrócenie domyœlnego koloru przyciskom
    //        foreach (GameObject button in aimButtons)
    //        {
    //            button.GetComponent<Image>().color = Color.white;
    //        }
    //    }
    //}

    ////przycelowanie wroga
    //public void TakeAimEnemy()
    //{
    //    if (Enemy.selectedEnemy.GetComponent<Stats>().aimingBonus == 0)
    //    {
    //        Debug.Log("Przycelowanie");
    //        Enemy.selectedEnemy.GetComponent<Stats>().aimingBonus = 10;

    //        //zmiana koloru wszystkich przycisków AimButton, ¿eby by³o wiadomo, ¿e przycelowanie jest aktywne
    //        foreach (GameObject button in aimButtons)
    //        {
    //            button.GetComponent<Image>().color = Color.green;
    //        }
    //    }
    //    else
    //    {
    //        Enemy.selectedEnemy.GetComponent<Stats>().aimingBonus = 0;

    //        //przywrócenie domyœlnego koloru przyciskom
    //        foreach (GameObject button in aimButtons)
    //        {
    //            button.GetComponent<Image>().color = Color.white;
    //        }
    //    }
    //}

    public void AttackPlayer()
    {
        Attack(Enemy.selectedEnemy, Player.selectedPlayer);
    }

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
}
