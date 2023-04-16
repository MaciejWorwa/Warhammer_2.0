using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class AttackManager : MonoBehaviour
{
    public static bool targetSelecting; // tryb wyboru celu ataku. Jego wartosc mowi o tym, czy w danym momencie trwa wybieranie celu ataku

    private float attackDistance; // dystans pomiedzy walczacymi
    private bool targetDefended; // informacja o tym, czy postaci uda�o si� sparowa� lub unikn�� atak

    private int attackBonus; // sumaryczna premia do WW lub US przy ataku
    private int chargeBonus; //premia za szar��
    private int aimingBonus; //premia za przycelowanie

    private GameObject[] aimButtons; // przyciski celowania zarowno bohatera gracza jak i wroga

    private MessageManager messageManager;
    private MovementManager movementManager;

    void Start()
    {
        aimButtons = GameObject.FindGameObjectsWithTag("AimButton");

        // Odniesienie do Menadzera Wiadomosci wyswietlanych na ekranie gry
        messageManager = GameObject.Find("MessageManager").GetComponent<MessageManager>();

        // Odniesienie do Menadzera Ruchu
        movementManager = GameObject.Find("MovementManager").GetComponent<MovementManager>();
    }

    #region Selecting target function
    // Wybor celu ataku
    public void SelectTarget()
    {
        targetSelecting = true;
        messageManager.ShowMessage("Wybierz cel ataku, klikaj�c na niego.", 3f);
        Debug.Log("Wybierz cel ataku, klikaj�c na niego.");

        // Wylacza widocznosc przyciskow akcji postaci
        if (Player.trSelect != null && GameObject.Find("ActionsButtonsPlayer/Canvas") != null)
            GameObject.Find("ActionsButtonsPlayer/Canvas").SetActive(false);
        if (Enemy.trSelect != null && GameObject.Find("ActionsButtonsEnemy/Canvas") != null)
            GameObject.Find("ActionsButtonsEnemy/Canvas").SetActive(false);
    }
    #endregion

    #region Reload functions for player and enemy
    public void ReloadPlayer()
    {
        if (Player.selectedPlayer.GetComponent<Stats>().reloadLeft > 0)
            Player.selectedPlayer.GetComponent<Stats>().reloadLeft--;
        if (Player.selectedPlayer.GetComponent<Stats>().reloadLeft == 0)
        {
            messageManager.ShowMessage($"Bro� <color=green>{Player.selectedPlayer.name}</color> za�adowana.", 3f);
            Debug.Log($"Bro� {Player.selectedPlayer.name} za�adowana.");
        }
        else
        {
            messageManager.ShowMessage($"�adowanie broni <color=green>{Player.selectedPlayer.name}</color>. Pozosta�a/y {Player.selectedPlayer.GetComponent<Stats>().reloadLeft} akcja/e.", 4f);
            Debug.Log($"�adowanie broni {Player.selectedPlayer.name}. Pozosta�a/y {Player.selectedPlayer.GetComponent<Stats>().reloadLeft} akcja/e aby m�c strzeli�.");
        }
          
    }
    public void ReloadEnemy()
    {
        if (Enemy.selectedEnemy.GetComponent<Stats>().reloadLeft > 0)
            Enemy.selectedEnemy.GetComponent<Stats>().reloadLeft--;
        if (Enemy.selectedEnemy.GetComponent<Stats>().reloadLeft == 0)
        {
            messageManager.ShowMessage($"Bro� <color=red>{Enemy.selectedEnemy.name}</color> za�adowana.", 3f);
            Debug.Log($"Bro� <color=red>{Enemy.selectedEnemy.name}</color> za�adowana.");
        }
        else
        {
            messageManager.ShowMessage($"�adowanie broni <color=red>{Enemy.selectedEnemy.name}</color>. Pozosta�a/y {Enemy.selectedEnemy.GetComponent<Stats>().reloadLeft} akcja/e.", 4f);
            Debug.Log($"�adowanie broni {Enemy.selectedEnemy.name}. Pozosta�a/y {Enemy.selectedEnemy.GetComponent<Stats>().reloadLeft} akcja/e aby m�c strzeli�.");
        }
    }
    #endregion

    #region Attack function
    public void Attack(GameObject attacker, GameObject target)
    {
        do
        {
            // liczy dystans pomiedzy walczacymi
            if (attacker != null && target != null)
                attackDistance = Vector3.Distance(attacker.transform.position, target.transform.position);

            // Jezeli jest wykonywana szarza, to zasieg ataku jest ustawiany na zasieg broni do walki wrecz
            if (MovementManager.Charge)
                attackDistance = 1.5f;

            // sprawdza, czy dystans miedzy walczacymi jest mniejszy lub rowny zasiegowi broni atakujacego
            if (attackDistance <= attacker.GetComponent<Stats>().AttackRange)
            {
                int wynik = UnityEngine.Random.Range(1, 101);
                bool hit = false;

                // sprawdza czy atak jest atakiem dystansowym
                if (attackDistance > 1.5f)
                {
                    // sprawdza czy bron jest naladowana
                    if (attacker.GetComponent<Stats>().reloadLeft == 0)
                    {
                        hit = wynik <= attacker.GetComponent<Stats>().US + attackBonus; // zwraca do 'hit' wartosc 'true' jesli to co jest po '=' jest prawda. Jest to skrocona forma 'if/else'

                        if (attackBonus > 0)
                        {
                            messageManager.ShowMessage($"<color=green>{attacker.name}</color> Rzut na US: {wynik}  Premia: {attackBonus}", 6f);
                            Debug.Log($"{attacker.name} Rzut na US: {wynik}  Premia: {attackBonus}");
                        }
                        else
                        {
                            messageManager.ShowMessage($"<color=green>{attacker.name}</color> Rzut na US: {wynik}", 6f);
                            Debug.Log($"{attacker.name} Rzut na US: {wynik}");
                        }

                        // resetuje naladowanie broni po wykonaniu strzalu
                        attacker.GetComponent<Stats>().reloadLeft = attacker.GetComponent<Stats>().reloadTime;

                        // uwzglednia zdolnosc blyskawicznego przeladowania
                        if (attacker.GetComponent<Stats>().instantReload == true)
                            attacker.GetComponent<Stats>().reloadLeft--;
                    }
                    else
                    {
                        messageManager.ShowMessage($"<color=red>Bro� wymaga na�adowania</color>", 2f);
                        Debug.Log($"Bro� wymaga na�adowania");
                        break;
                    }
                }
                // sprawdza czy atak jest atakiem w zwarciu
                if (attackDistance <= 1.5f)
                {
                    //uwzgl�dnienie bonusu do WW zwiazanego z szar��
                    if (MovementManager.Charge)
                        chargeBonus = 10;
                    else
                        chargeBonus = 0;

                    attackBonus = chargeBonus + aimingBonus;

                    hit = wynik <= attacker.GetComponent<Stats>().WW + attackBonus; // zwraca do 'hit' wartosc 'true' jesli to co jest po '=' jest prawda. Jest to skrocona forma 'if/else'

                    if (attackBonus > 0)
                    {
                        messageManager.ShowMessage($"<color=green>{attacker.name}</color> Rzut na WW: {wynik}  Premia: {attackBonus}", 6f);
                        Debug.Log($"{attacker.name} Rzut na WW: {wynik}  Premia: {attackBonus}");
                    }
                    else
                    {
                        messageManager.ShowMessage($"<color=green>{attacker.name}</color> Rzut na WW: {wynik}", 6f);
                        Debug.Log($"{attacker.name} Rzut na WW: {wynik}");
                    }
                }

                //wywo�anie funkcji parowania lub uniku je�li posta� jeszcze mo�e to robi� w tej rundzie
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

                // zresetowanie bonusu za celowanie, je�li jest aktywny
                if (aimingBonus != 0)
                    TakeAim();

                if (hit && targetDefended != true)
                {
                    int armor = CheckAttackLocalization(target);
                    int damage;
                    int rollResult;

                    // mechanika broni przebijajacej zbroje
                    if (attacker.GetComponent<Stats>().PrzebijajacyZbroje && armor >= 1)
                        armor--;

                    // mechanika bronii druzgoczacej
                    if (attacker.GetComponent<Stats>().Druzgoczacy)
                    {
                        int roll1 = UnityEngine.Random.Range(1, 11);
                        int roll2 = UnityEngine.Random.Range(1, 11);
                        rollResult = roll1 >= roll2 ? roll1 : roll2;
                        messageManager.ShowMessage($"Atak druzgocz�c� broni�. Rzut 1: {roll1} Rzut 2: {roll2}", 6f);
                        Debug.Log($"Atak druzgocz�c� broni�. Rzut 1: {roll1} Rzut 2: {roll2}");
                    }
                    else
                        rollResult = UnityEngine.Random.Range(1, 11);

                    // mechanika broni ciezkiej. Czyli po pierszym CELNYM ataku bron traci ceche druzgoczacy. Wg podrecznika traci sie to po pierwszej rundzie, ale wole tak :)
                    if (attacker.GetComponent<Stats>().Ciezki)
                        attacker.GetComponent<Stats>().Druzgoczacy = false;

                    // mechanika furii ulryka
                    if (rollResult == 10)
                    {
                        int confirmRoll = UnityEngine.Random.Range(1, 101); //rzut na potwierdzenie furii
                        int additionalDamage = 0; //obrazenia ktore dodajemy do wyniku rzutu

                        if (attackDistance <= 1.5f)
                        {
                            if (attacker.GetComponent<Stats>().WW >= confirmRoll)
                            {
                                additionalDamage = UnityEngine.Random.Range(1, 11);
                                rollResult = rollResult + additionalDamage;
                                messageManager.ShowMessage($"Rzut na WW: {confirmRoll}.<color=red> FURIA ULRYKA! </color>", 6f);
                                Debug.Log($"Rzut na potwierdzenie {confirmRoll}. FURIA ULRYKA!");
                            }
                            else
                            {
                                rollResult = 10;
                                messageManager.ShowMessage($"Rzut na WW: {confirmRoll}. Nie uda�o si� potwierdzi� Furii Ulryka.", 6f);
                                Debug.Log($"Rzut na potwierdzenie {confirmRoll}. Nie uda�o si� potwierdzi� Furii Ulryka.");
                            }
                        }
                        else if (attackDistance > 1.5f)
                        {
                            if (attacker.GetComponent<Stats>().US >= confirmRoll)
                            {
                                additionalDamage = UnityEngine.Random.Range(1, 11);
                                rollResult = rollResult + additionalDamage;
                                messageManager.ShowMessage($"Rzut na US: {confirmRoll}.<color=red> FURIA ULRYKA! </color>", 6f);
                                Debug.Log($"Rzut na potwierdzenie {confirmRoll}. FURIA ULRYKA!");
                            }
                            else
                            {
                                rollResult = 10;
                                messageManager.ShowMessage($"Rzut na US: {confirmRoll}. Nie uda�o si� potwierdzi� Furii Ulryka.", 6f);
                                Debug.Log($"Rzut na potwierdzenie {confirmRoll}. Nie uda�o si� potwierdzi� Furii Ulryka.");
                            }
                        }

                        while (additionalDamage == 10)
                        {
                            additionalDamage = UnityEngine.Random.Range(1, 11);
                            rollResult = rollResult + additionalDamage;
                            messageManager.ShowMessage($"<color=red> KOLEJNA FURIA ULRYKA! </color>", 6f);
                            Debug.Log($"KOLEJNA FURIA ULRYKA!");
                        }
                    }

                    if (attackDistance <= 1.5f)
                        damage = rollResult + attacker.GetComponent<Stats>().S;
                    else
                        damage = rollResult + attacker.GetComponent<Stats>().Weapon_S;

                    messageManager.ShowMessage($"<color=green>{attacker.name}</color> wyrzuci� {rollResult} i zada� <color=green>{damage} obra�e�.</color>", 8f);
                    Debug.Log($"{attacker.name} wyrzuci� {rollResult} i zada� {damage} obra�e�.");

                    if (damage > (target.GetComponent<Stats>().Wt + armor))
                    {
                        target.GetComponent<Stats>().tempHealth -= (damage - (target.GetComponent<Stats>().Wt + armor));

                        messageManager.ShowMessage(target.name + " znegowa� " + (target.GetComponent<Stats>().Wt + armor) + " obra�e�.", 8f);
                        Debug.Log(target.name + " znegowa� " + (target.GetComponent<Stats>().Wt + armor) + " obra�e�.");

                        messageManager.ShowMessage($"<color=red> Punkty �ycia {target.name}: {target.GetComponent<Stats>().tempHealth}/{target.GetComponent<Stats>().maxHealth}</color>", 8f);
                        Debug.Log($"Punkty �ycia {target.name}: {target.GetComponent<Stats>().tempHealth}/{target.GetComponent<Stats>().maxHealth}");

                        //TO PONI�EJ DZIA�A ALE MUSIA�EM WY��CZY� TYMCZASOWO �EBY AUTOMATYCZNY COMBAT NIE WYWALA� B��D�W, BO W NIM NIE MA �ADNYCH SELECTEDENEMY ANI SELECTEDPLAYER
                        if (target == Enemy.selectedEnemy && Enemy.selectedEnemy.GetComponent<Stats>().criticalCondition == true)
                            Enemy.selectedEnemy.GetComponent<Stats>().GetCriticalHit();
                        else if (target == Player.selectedPlayer && Player.selectedPlayer.GetComponent<Stats>().criticalCondition == true)
                            Player.selectedPlayer.GetComponent<Stats>().GetCriticalHit();
                    }
                    else
                    {
                        messageManager.ShowMessage($"Atak <color=red>{attacker.name}</color> nie przebi� si� przez pancerz.", 6f);
                        Debug.Log($"Atak {attacker.name} nie przebi� si� przez pancerz.");
                    }
                }
                else
                {
                    messageManager.ShowMessage($"Atak <color=red>{attacker.name}</color> chybi�.", 6f);
                    Debug.Log($"Atak {attacker.name} chybi�.");
                }
                targetDefended = false; // przestawienie boola na false, �eby przy kolejnym ataku znowu musia� si� broni�, a nie by� obroniony na starcie
            }
            else
            {
                messageManager.ShowMessage($"<color=red>Cel ataku stoi poza zasi�giem.</color>", 3f);
                Debug.Log($"Cel ataku stoi poza zasi�giem.");
            }
        }
        while (false);
    }
    #endregion

    #region Check for attack localization function
    int CheckAttackLocalization(GameObject target)
    {
        int attackLocalization = UnityEngine.Random.Range(1, 101);
        int armor = 0;

        switch (attackLocalization)
        {
            case int n when (n >= 1 && n <= 15):
                messageManager.ShowMessage("Trafienie w g�ow�", 6f);
                Debug.Log("Trafienie w g�ow�");
                armor = target.GetComponent<Stats>().PZ_head;
                break;
            case int n when (n >= 16 && n <= 35):
                messageManager.ShowMessage("Trafienie w praw� r�k�", 6f);
                Debug.Log("Trafienie w praw� r�k�");
                armor = target.GetComponent<Stats>().PZ_arms;
                break;
            case int n when (n >= 36 && n <= 55):
                messageManager.ShowMessage("Trafienie w lew� r�k�", 6f);
                Debug.Log("Trafienie w lew� r�k�");
                armor = target.GetComponent<Stats>().PZ_arms;
                break;
            case int n when (n >= 56 && n <= 80):
                messageManager.ShowMessage("Trafienie w korpus", 6f);
                Debug.Log("Trafienie w korpus");
                armor = target.GetComponent<Stats>().PZ_torso;
                break;
            case int n when (n >= 81 && n <= 90):
                messageManager.ShowMessage("Trafienie w praw� nog�", 6f);
                Debug.Log("Trafienie w praw� nog�");
                armor = target.GetComponent<Stats>().PZ_legs;
                break;
            case int n when (n >= 91 && n <= 100):
                messageManager.ShowMessage("Trafienie w lew� nog�", 6f);
                Debug.Log("Trafienie w lew� nog�");
                armor = target.GetComponent<Stats>().PZ_legs;
                break;
        }
        return armor;

    }
    #endregion

    #region Parry and dodge
    private void ParryAttack(GameObject attacker, GameObject target)
    {
        // sprawdza, czy atakowany ma jakie� bonusy do parowania
        if (target.GetComponent<Stats>().Parujacy)
            target.GetComponent<Stats>().parryBonus += 10;
        if (attacker.GetComponent<Stats>().Powolny)
            target.GetComponent<Stats>().parryBonus += 10;
        if (attacker.GetComponent<Stats>().Szybki)
            target.GetComponent<Stats>().parryBonus -= 10;

        target.GetComponent<Stats>().canParry = false;
        int wynik = UnityEngine.Random.Range(1, 101);

        if (target.GetComponent<Stats>().parryBonus != 0)
        {
            messageManager.ShowMessage($"{target.name} Rzut na parowanie: {wynik} Bonus do parowania: {target.GetComponent<Stats>().parryBonus}", 5f);
            Debug.Log($"{target.name} Rzut na parowanie: {wynik} Bonus do parowania: {target.GetComponent<Stats>().parryBonus}");
        }
        else
        {
            messageManager.ShowMessage($"{target.name} Rzut na parowanie: {wynik}", 5f);
            Debug.Log($"{target.name} Rzut na parowanie: {wynik}");
        }

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
        int wynik = UnityEngine.Random.Range(1, 101);

        messageManager.ShowMessage($"{target.name} Rzut na unik: {wynik}", 5f);
        Debug.Log($"{target.name} Rzut na unik: {wynik}");

        if (wynik <= target.GetComponent<Stats>().Zr)
            targetDefended = true;
        else
            targetDefended = false;
    }
    #endregion

    #region Take aim
    public void TakeAim()
    {
        if (aimingBonus == 0)
        {
            messageManager.ShowMessage("Przycelowanie", 3f);
            Debug.Log("Przycelowanie");
            aimingBonus = 10;

            //zmiana koloru wszystkich przycisk�w AimButton, �eby by�o wiadomo, �e przycelowanie jest aktywne
            foreach (GameObject button in aimButtons)
            {
                button.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);
            }
        }
        else
        {
            aimingBonus = 0;

            //przywr�cenie domy�lnego koloru przyciskom
            foreach (GameObject button in aimButtons)
            {
                button.GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);
            }
        }

        attackBonus = chargeBonus + aimingBonus;
    }
    #endregion

    #region Opportunity attack function
    // Wykonanie ataku okazyjnego
    public void OpportunityAttack(GameObject attacker, GameObject target)
    {
        int wynik = UnityEngine.Random.Range(1, 101);
        bool hit = false;

        messageManager.ShowMessage($"Ruch spowodowa� atak okazyjny. Rzut na WW <color=red>{attacker.name}</color>: {wynik}", 6f);
        Debug.Log($"Ruch spowodowa� atak okazyjny. Rzut na WW {attacker.name}: {wynik}");

        hit = wynik <= attacker.GetComponent<Stats>().WW;  // zwraca do 'hit' wartosc 'true' jesli to co jest po '=' jest prawda. Jest to skrocona forma 'if/else'

        if (hit)
        {
            int armor = CheckAttackLocalization(target);
            int damage;
            int rollResult;

            // mechanika broni przebijajacej zbroje
            if (attacker.GetComponent<Stats>().PrzebijajacyZbroje && armor >= 1)
                armor--;

            // mechanika bronii druzgoczacej
            if (attacker.GetComponent<Stats>().Druzgoczacy)
            {
                int roll1 = UnityEngine.Random.Range(1, 11);
                int roll2 = UnityEngine.Random.Range(1, 11);
                rollResult = roll1 >= roll2 ? roll1 : roll2;
                messageManager.ShowMessage($"Atak druzgocz�c� broni�. Rzut 1: {roll1} Rzut 2: {roll2}", 6f);
                Debug.Log($"Atak druzgocz�c� broni�. Rzut 1: {roll1} Rzut 2: {roll2}");
            }
            else
                rollResult = UnityEngine.Random.Range(1, 11);

            // mechanika broni ciezkiej. Czyli po pierszym CELNYM ataku bron traci ceche druzgoczacy. Wg podrecznika traci sie to po pierwszej rundzie, ale wole tak :)
            if (attacker.GetComponent<Stats>().Ciezki)
                attacker.GetComponent<Stats>().Druzgoczacy = false;

            // mechanika furii ulryka
            if (rollResult == 10)
            {
                int confirmRoll = UnityEngine.Random.Range(1, 101); //rzut na potwierdzenie furii
                int additionalDamage = 0; //obrazenia ktore dodajemy do wyniku rzutu

                if (attackDistance <= 1.5f)
                {
                    if (attacker.GetComponent<Stats>().WW >= confirmRoll)
                    {
                        additionalDamage = UnityEngine.Random.Range(1, 11);
                        rollResult = rollResult + additionalDamage;
                        messageManager.ShowMessage($"Rzut na WW: {confirmRoll}.<color=red> FURIA ULRYKA! </color>", 6f);
                        Debug.Log($"Rzut na potwierdzenie {confirmRoll}. FURIA ULRYKA!");
                    }
                    else
                    {
                        rollResult = 10;
                        messageManager.ShowMessage($"Rzut na WW: {confirmRoll}. Nie uda�o si� potwierdzi� Furii Ulryka.", 6f);
                        Debug.Log($"Rzut na potwierdzenie {confirmRoll}. Nie uda�o si� potwierdzi� Furii Ulryka.");
                    }
                }
                else if (attackDistance > 1.5f)
                {
                    if (attacker.GetComponent<Stats>().US >= confirmRoll)
                    {
                        additionalDamage = UnityEngine.Random.Range(1, 11);
                        rollResult = rollResult + additionalDamage;
                        messageManager.ShowMessage($"Rzut na US: {confirmRoll}.<color=red> FURIA ULRYKA! </color>", 6f);
                        Debug.Log($"Rzut na potwierdzenie {confirmRoll}. FURIA ULRYKA!");
                    }
                    else
                    {
                        rollResult = 10;
                        messageManager.ShowMessage($"Rzut na US: {confirmRoll}. Nie uda�o si� potwierdzi� Furii Ulryka.", 6f);
                        Debug.Log($"Rzut na potwierdzenie {confirmRoll}. Nie uda�o si� potwierdzi� Furii Ulryka.");
                    }
                }

                while (additionalDamage == 10)
                {
                    additionalDamage = UnityEngine.Random.Range(1, 11);
                    rollResult = rollResult + additionalDamage;
                    messageManager.ShowMessage($"<color=red> KOLEJNA FURIA ULRYKA! </color>", 6f);
                    Debug.Log($"KOLEJNA FURIA ULRYKA!");
                }
            }

            damage = rollResult + attacker.GetComponent<Stats>().S;

            messageManager.ShowMessage($"<color=green>{attacker.name}</color> wyrzuci� {rollResult} i zada� <color=green>{damage} obra�e�.</color>", 8f);
            Debug.Log($"{attacker.name} wyrzuci� {rollResult} i zada� {damage} obra�e�.");

            if (damage > (target.GetComponent<Stats>().Wt + armor))
            {
                target.GetComponent<Stats>().tempHealth -= (damage - (target.GetComponent<Stats>().Wt + armor));

                messageManager.ShowMessage(target.name + " znegowa� " + (target.GetComponent<Stats>().Wt + armor) + " obra�e�.", 8f);
                Debug.Log(target.name + " znegowa� " + (target.GetComponent<Stats>().Wt + armor) + " obra�e�.");
                messageManager.ShowMessage($"<color=red> Punkty �ycia {target.name}: {target.GetComponent<Stats>().tempHealth}/{target.GetComponent<Stats>().maxHealth}</color>", 8f);
                Debug.Log($"Punkty �ycia {target.name}: {target.GetComponent<Stats>().tempHealth}/{target.GetComponent<Stats>().maxHealth}");

                if (target == Enemy.selectedEnemy && Enemy.selectedEnemy.GetComponent<Stats>().criticalCondition == true)
                    Enemy.selectedEnemy.GetComponent<Stats>().GetCriticalHit();
                else if (target == Player.selectedPlayer && Player.selectedPlayer.GetComponent<Stats>().criticalCondition == true)
                    Player.selectedPlayer.GetComponent<Stats>().GetCriticalHit();
            }
            else
            {
                messageManager.ShowMessage($"Atak <color=red>{attacker.name}</color> nie przebi� si� przez pancerz.", 6f);
                Debug.Log($"Atak {attacker.name} nie przebi� si� przez pancerz.");
            }
        }
        else
        {
            messageManager.ShowMessage($"Atak <color=red>{attacker.name}</color> chybi�.", 6f);
            Debug.Log($"Atak {attacker.name} chybi�.");
        }
    }
    #endregion

    #region Charge attack function
    public void ChargeAttack(GameObject attacker, GameObject target)
    {
        // wektor we wszystkie osiem kierunkow dookola pola z postacia bedaca celem ataku
        Vector3[] directions = { Vector3.right, Vector3.left, Vector3.up, Vector3.down, new Vector3(1, 1, 0), new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0) };

        // lista pol przylegajacych do postaci
        List<GameObject> adjacentTiles = new List<GameObject>();

        // Szuka pol w kazdym kierunku
        foreach (Vector3 direction in directions)
        {
            // znajdz wszystkie kolidery w miejscach przylegajacych do postaci bedacej celem ataku
            // jesli w jednym miejscu wystepuje wiecej niz jeden collider to oznacza, ze pole jest zajete przez postac, wtedy nie dodajemy tego collidera do listy adjacentTiles
            Collider2D[] collider = Physics2D.OverlapCircleAll(target.transform.position + direction, 0.1f);

            if (collider != null && collider.Length == 1 && collider[0].CompareTag("Tile"))
            {
                adjacentTiles.Add(collider[0].gameObject);
            }
        }

        // Zamienia liste na tablice, zeby pozniej mozna bylo ja posortowac
        GameObject[] adjacentTilesArray = adjacentTiles.ToArray();

        // Sortuje przylegajace do postaci pola wg odleglosci od atakujacego. Te ktore sa najblizej znajduja sie na poczatku tablicy
        Array.Sort(adjacentTilesArray, (x, y) => Vector3.Distance(x.transform.position, attacker.transform.position).CompareTo(Vector3.Distance(y.transform.position, attacker.transform.position)));

        // Sprawdza dystans do pola docelowego
        float distanceBetweenOpponents = (Mathf.Abs(attacker.transform.position.x - adjacentTilesArray[0].transform.position.x)) + (Mathf.Abs(attacker.transform.position.y - adjacentTilesArray[0].transform.position.y));

        if (distanceBetweenOpponents >= 3 && distanceBetweenOpponents <= attacker.GetComponent<Stats>().Sz * 2) // Jesli jest w zasiegu szarzy
        {
            //Wykonanie szarzy
            if (adjacentTilesArray != null)
            {
                attacker.GetComponent<Stats>().tempSz = attacker.GetComponent<Stats>().Sz * 2;

                MovementManager.canMove = true;
                GameObject.Find("MovementManager").GetComponent<MovementManager>().MoveSelectedCharacter(adjacentTilesArray[0], attacker);
                Physics2D.SyncTransforms(); // Synchronizuje collidery (inaczej Collider2D nie wykrywa zmian pozycji postaci)

                Attack(attacker, target); // Wykonywany jest jeden atak z bonusem +10, bo to szarza

                // Zresetowanie szarzy
                attacker.GetComponent<Stats>().tempSz = attacker.GetComponent<Stats>().Sz;
                movementManager.SetCharge();
                MovementManager.canMove = false;
            }
        }
        else if (distanceBetweenOpponents < 3) // Jesli jest zbyt blisko na szarze
        {
            messageManager.ShowMessage($"<color=red>Zbyt ma�a odleg�o�� na wykonanie szar�y.</color>", 3f);
            Debug.Log("Zbyt ma�a odleg�o�� na wykonanie szar�y");
            movementManager.SetCharge();
        }
        else
        {
            messageManager.ShowMessage($"<color=red>Cel ataku stoi poza zasi�giem szar�y.</color>", 3f);
            Debug.Log($"Cel ataku stoi poza zasi�giem szar�y.");
            movementManager.SetCharge();
        }
    }
    #endregion
}