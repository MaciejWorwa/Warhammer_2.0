using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public static Stats enemyStats;
    private enum Rasa { Ork, Goblin, Troll, Wilk } // rozkminiæ gdzie przechowywaæ informacje o statystykach poczatkowych danej rasy, zeby nie bylo miliona linii kodu w Enemy
    [SerializeField] private Rasa rasa;

    private TMP_Text nameDisplay;
    private TMP_Text healthDisplay;
    private TMP_Text initiativeDisplay;

    public static GameObject selectedEnemy;
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

        // nadanie rasy
        rasa = (Rasa)Random.Range(0, 4);
        messageManager.ShowMessage($"Stworzy³eœ {this.gameObject.name} o rasie {rasa}", 3f);
        Debug.Log($"Stworzy³eœ {this.gameObject.name} o rasie {rasa}");

        // nadanie temu obiektowi klasy Stats
        enemyStats = this.gameObject.AddComponent<Stats>();
        this.gameObject.GetComponent<MovementManager>();

        // nadanie poczatkowego imienia takiego jak nazwa obiektu gry, np. Enemy 1
        enemyStats.Name = this.gameObject.name;

        // nadanie wartosci cech pierwszorzedowych (docelowo to bedzie robione rowniez w klasie Stats, tylko chce najpierw ogarnac import Jsona ze statami roznych wrogow)
        enemyStats.WW = 20 + Random.Range(2, 21);
        enemyStats.US = 20 + Random.Range(2, 21);
        enemyStats.K = 20 + Random.Range(2, 21);
        enemyStats.Odp = 20 + Random.Range(2, 21);
        enemyStats.Zr = 20 + Random.Range(2, 21);
        enemyStats.Int = 20 + Random.Range(2, 21);
        enemyStats.SW = 20 + Random.Range(2, 21);
        enemyStats.Ogd = 20 + Random.Range(2, 21);

        // nadanie wartosci cech drugorzedowych
        enemyStats.Sz = 4;
        enemyStats.Mag = 0;
        enemyStats.maxHealth = Random.Range(8, 15);


        // ustawienie aktualnych statystyk punktów ¿ycia i szybkosci zgodnie z poczatkowymi
        enemyStats.tempHealth = enemyStats.maxHealth;
        enemyStats.tempSz = enemyStats.Sz;

        //UWAGA!!!!!!!!!!!!!  nadanie inicjatywy, atakow, Si³y i Wt docelowo to bedzie robione rowniez w klasie Stats, tylko chce najpierw ogarnac import Jsona ze statami roznych wrogow 
        enemyStats.Initiative = enemyStats.Zr + Random.Range(1, 11);
        enemyStats.A = 1;
        enemyStats.S = Mathf.RoundToInt(enemyStats.K / 10);
        enemyStats.Wt = Mathf.RoundToInt(enemyStats.Odp / 10);


        // ustawienie bazowego zasiegu broni (bron do walki w zwarciu) i sily broni (dystansowa)
        enemyStats.Weapon_S = 3;
        enemyStats.AttackRange = 1.5;

        healthDisplay = this.transform.Find("healthPointsEnemy").gameObject.GetComponent<TMP_Text>();
        healthDisplay.transform.position = this.gameObject.transform.position;

        initiativeDisplay = this.transform.Find("initiativeEnemy").gameObject.GetComponent<TMP_Text>();
        initiativeDisplay.transform.position = new Vector3(this.gameObject.transform.position.x + 0.5f, this.gameObject.transform.position.y + 0.5f, this.gameObject.transform.position.z);

        nameDisplay = this.transform.Find("nameEnemy").gameObject.GetComponent<TMP_Text>();
        nameDisplay.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y - 0.6f, this.gameObject.transform.position.z);

    }

    void Update()
    {
        // wyswietlanie na biezaco aktualnych punktow zycia, imienia oraz inicjatywy
        healthDisplay.text = this.gameObject.GetComponent<Stats>().tempHealth + "/" + this.gameObject.GetComponent<Stats>().maxHealth;
        initiativeDisplay.text = this.gameObject.GetComponent<Stats>().Initiative.ToString();
        nameDisplay.text = this.gameObject.GetComponent<Stats>().Name.ToString();

        if (Input.GetKeyDown(KeyCode.E) && selectedEnemy.name == this.gameObject.name && GameManager.PanelIsOpen == false)
        {
            attackManager.Attack(selectedEnemy, Player.selectedPlayer);
            buttonManager.ShowOrHideActionsButtons(selectedEnemy, false);
        }

        if (enemyStats.tempHealth < 0 && enemyStats.criticalCondition == false)
        {
            enemyStats.GetCriticalHit();
        }

        if (Input.GetKeyDown(KeyCode.Delete) && trSelect != null)
        {
            Destroy(selectedEnemy);
        }
    }

    #region Select or deselect enemy method
    public void OnMouseDown()
    {
        GridManager grid = GameObject.Find("Grid").GetComponent<GridManager>();

        // Umozliwia zaznaczenie/odznaczenie postaci, tylko gdy inne postacie nie sa wybrane i panel edycji statystyk jest zamkniety
        if (!GameManager.PanelIsOpen && trSelect == null && Player.trSelect == null && AttackManager.targetSelecting != true || !GameManager.PanelIsOpen && trSelect == transform && Player.trSelect == null && AttackManager.targetSelecting != true)
        {
            if (trSelect == transform) // klikniecie na postac, ktora juz jest wybrana
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                trSelect = null;
                selectedEnemy.GetComponent<Renderer>().material.color = new Color(255, 0, 0);

                buttonManager.ShowOrHideActionsButtons(selectedEnemy, false);
                MovementManager.canMove = true;

                // Zresetowanie koloru podswietlonych pol w zasiegu ruchu
                grid.ResetTileColors();
            }
            else // klikniecie na postac, gdy zadna postac nie jest wybrana
            {
                trSelect = transform;
                transform.localScale = new Vector3(1.2f, 1.2f, 1f);

                selectedEnemy = this.gameObject;

                messageManager.ShowMessage($"Wybra³eœ {selectedEnemy.GetComponent<Stats>().Name}", 3f);
                Debug.Log("Wybra³eœ " + selectedEnemy.name);

                selectedEnemy.GetComponent<Renderer>().material.color = new Color(1.0f, 0.64f, 0.0f);

                buttonManager.ShowOrHideActionsButtons(selectedEnemy, true);

                MovementManager.canMove = false;

                //Zresetowanie szarzy i biegu
                GameObject.Find("MovementManager").GetComponent<MovementManager>().ResetChargeAndRun();

                // Zresetowanie koloru podswietlonych pol w zasiegu ruchu
                grid.ResetTileColors();
            }
        }

        // Jezeli jest aktywny tryb wybierania celu ataku, przekazuje informacje o kliknietym Enemy i wywoluje funkcje Attack traktujac wybranego Playera jako atakujacego i Enemy jako atakowanego.
        if (AttackManager.targetSelecting == true)
        {
            // Resetuje tryb wyboru celu ataku
            AttackManager.targetSelecting = false;

            // Sprawdza, czy atakujacym nie jest inny Enemy
            if (trSelect != null)
            {
                messageManager.ShowMessage($"<color=red>Nie mo¿esz atakowaæ swoich sojuszników.</color>", 3f);
                Debug.Log("Nie mo¿esz atakowaæ swoich sojuszników.");

                // Przywraca widocznosc przyciskow akcji
                buttonManager.ShowOrHideActionsButtons(selectedEnemy, true);
                // Resetuje szarze jesli jest aktywna
                if (MovementManager.Charge)
                    GameObject.Find("MovementManager").GetComponent<MovementManager>().ResetChargeAndRun();
                return;
            }
            selectedEnemy = this.gameObject;

            if(!MovementManager.Charge)
                attackManager.Attack(Player.selectedPlayer, selectedEnemy);
            else
                attackManager.ChargeAttack(Player.selectedPlayer, selectedEnemy);

            // Przywraca widocznosc przyciskow akcji atakujacej postaci
            buttonManager.ShowOrHideActionsButtons(Player.selectedPlayer, true);

            // Resetuje szarze jesli jest aktywna
            GameObject.Find("MovementManager").GetComponent<MovementManager>().ResetChargeAndRun();

        }
    }
    #endregion

}
