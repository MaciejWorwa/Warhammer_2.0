using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public static Stats playerStats;
    public enum Rasa { Czlowiek, Elf, Krasnolud, Niziolek }
    public Rasa rasa;

    private TMP_Text nameDisplay;
    private TMP_Text healthDisplay;
    private TMP_Text initiativeDisplay;

    public static GameObject selectedPlayer;
    public static Transform trSelect = null;

    private ButtonManager buttonManager;

    private float attackDistance;

    [HideInInspector] public AttackManager attackManager;

    private MessageManager messageManager;

    void Start()
    {
        // Odniesienie do Menadzera Wiadomosci wyswietlanych na ekranie gry
        messageManager = GameObject.Find("MessageManager").GetComponent<MessageManager>();

        // Odniesienie do Managera Przyciskow
        buttonManager = GameObject.Find("ButtonManager").GetComponent<ButtonManager>();

        //Odniesienie do Managera Ataku
        attackManager = GameObject.Find("AttackManager").GetComponent<AttackManager>();
        //nadanie rasy
        rasa = (Rasa)Random.Range(0, 4);
        messageManager.ShowMessage($"Stworzyłeś {this.gameObject.name} o rasie {rasa}", 3f);
        Debug.Log($"Stworzyłeś {this.gameObject.name} o rasie {rasa}");

        //nadanie temu obiektowi klasy Stats
        playerStats = this.gameObject.AddComponent<Stats>();
        this.gameObject.GetComponent<MovementManager>();
        playerStats.Rasa = rasa.ToString();

        // nadanie poczatkowego imienia takiego jak nazwa obiektu gry, np. Player 1
        playerStats.Name = this.gameObject.name;

        //wygenerowanie poczatkowych statystyk w zaleznosci od rasy. Metoda ta jest zawarta w klasie Stats
        playerStats.SetBaseStatsByRace(rasa);

        // ustawienie aktualnych statystyk punktów życia i szybkosci zgodnie z poczatkowymi
        playerStats.tempHealth = playerStats.maxHealth;
        playerStats.tempSz = playerStats.Sz;

        // ustawienie bazowego zasiegu broni (bron do walki w zwarciu) i sily broni (dystansowa)
        playerStats.Weapon_S = 3;
        playerStats.AttackRange = 1.5;

        healthDisplay = this.transform.Find("healthPointsPlayer").gameObject.GetComponent<TMP_Text>();
        healthDisplay.transform.position = this.gameObject.transform.position;

        initiativeDisplay = this.transform.Find("initiativePlayer").gameObject.GetComponent<TMP_Text>();
        initiativeDisplay.transform.position = new Vector3(this.gameObject.transform.position.x + 0.5f, this.gameObject.transform.position.y + 0.5f, this.gameObject.transform.position.z);

        nameDisplay = this.transform.Find("namePlayer").gameObject.GetComponent<TMP_Text>();
        nameDisplay.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y - 0.6f, this.gameObject.transform.position.z);
    }

    void Update()
    {
        // wyswietlanie na biezaco aktualnych punktow zycia, imienia oraz inicjatywy
        healthDisplay.text = this.gameObject.GetComponent<Stats>().tempHealth + "/" + this.gameObject.GetComponent<Stats>().maxHealth;
        initiativeDisplay.text = this.gameObject.GetComponent<Stats>().Initiative.ToString();
        nameDisplay.text = this.gameObject.GetComponent<Stats>().Name.ToString();
 
        if (playerStats.tempHealth < 0 && playerStats.criticalCondition == false)
        {
            playerStats.GetCriticalHit();
        }

        if(Input.GetKeyDown(KeyCode.Delete) && trSelect != null)
        {
            Destroy(selectedPlayer);
        }
    }

    #region Select or deselect player method
    public void OnMouseDown()
    {
        GridManager grid = GameObject.Find("Grid").GetComponent<GridManager>();

        // Umozliwia zaznaczenie/odznaczenie postaci, tylko gdy inne postacie nie sa wybrane i panel edycji statystyk jest zamkniety
        if (!GameManager.PanelIsOpen && trSelect == null && Enemy.trSelect == null && AttackManager.targetSelecting != true || !GameManager.PanelIsOpen && trSelect == transform && Enemy.trSelect == null && AttackManager.targetSelecting != true)
        {
            if (trSelect == transform) // klikniecie na postac, ktora juz jest wybrana
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                trSelect = null;
                selectedPlayer.GetComponent<Renderer>().material.color = new Color(0, 255, 0);

                buttonManager.ShowOrHideActionsButtons(selectedPlayer, false);
                MovementManager.canMove = true;

                // Zresetowanie koloru podswietlonych pol w zasiegu ruchu
                grid.ResetTileColors();
            }
            else // klikniecie na postac, gdy zadna postac nie jest wybrana
            {
                trSelect = transform;
                transform.localScale = new Vector3(1.2f, 1.2f, 1f);

                selectedPlayer = this.gameObject;

                messageManager.ShowMessage($"Wybrałeś {selectedPlayer.GetComponent<Stats>().Name}", 3f);
                Debug.Log("Wybrałeś " + selectedPlayer.name);

                selectedPlayer.GetComponent<Renderer>().material.color = new Color(0f, 1f, 0.64f);

                buttonManager.ShowOrHideActionsButtons(selectedPlayer, true);

                MovementManager.canMove = false;

                //Zresetowanie szarzy i biegu
                GameObject.Find("MovementManager").GetComponent<MovementManager>().ResetChargeAndRun();

                // Zresetowanie koloru podswietlonych pol w zasiegu ruchu
                grid.ResetTileColors();
            }
        }

        // Jezeli jest aktywny tryb wybierania celu ataku, przekazuje informacje o kliknietym Playerze i wywoluje funkcje Attack traktujac wybranego Enemy jako atakujacego i Playera jako atakowanego.
        if(AttackManager.targetSelecting == true)
        {
            // Resetuje tryb wyboru celu ataku
            AttackManager.targetSelecting = false;

            // Sprawdza, czy atakujacym nie jest inny Player
            if (trSelect != null)
            {
                messageManager.ShowMessage($"<color=red>Nie możesz atakować swoich sojuszników.</color>", 3f);
                Debug.Log("Nie możesz atakować swoich sojuszników.");

                // Przywraca widocznosc przyciskow akcji
                buttonManager.ShowOrHideActionsButtons(selectedPlayer, true);
                // Resetuje szarze jesli jest aktywna
                if (MovementManager.Charge)
                    GameObject.Find("MovementManager").GetComponent<MovementManager>().ResetChargeAndRun();
                return;
            }
            selectedPlayer = this.gameObject;

            if (!MovementManager.Charge)
                attackManager.Attack(Enemy.selectedEnemy, selectedPlayer);
            else
                attackManager.ChargeAttack(Enemy.selectedEnemy, selectedPlayer);

            // Przywraca widocznosc przyciskow akcji atakujacej postaci
            buttonManager.ShowOrHideActionsButtons(Enemy.selectedEnemy, true);

            // Resetuje szarze jesli jest aktywna
            GameObject.Find("MovementManager").GetComponent<MovementManager>().ResetChargeAndRun();
        }
    }
    #endregion
}





