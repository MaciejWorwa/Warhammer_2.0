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
    public GameObject actionsButtons;

    private float attackDistance;

    [HideInInspector] public AttackManager attackManager;

    void Start()
    {
        // nadanie rasy
        rasa = (Rasa)Random.Range(0, 4);
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
        enemyStats.maxHealth = Random.Range(10, 21);


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

        actionsButtons = GameObject.Find("ActionsButtonsEnemy");

        attackManager = GameObject.Find("AttackManager").GetComponent<AttackManager>();

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

        if (Input.GetKeyDown(KeyCode.E) && selectedEnemy.name == this.gameObject.name && StatsEditor.EditorIsOpen == false)
        {
            attackManager.Attack(selectedEnemy, Player.selectedPlayer);
            actionsButtons.transform.Find("Canvas").gameObject.SetActive(false);
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

    public void OnMouseDown()
    {
        GridManager grid = GameObject.Find("Grid").GetComponent<GridManager>();

        // Gdy otwarty jest edytor statystyk to uniemozliwia wybieranie postaci
        if (!StatsEditor.EditorIsOpen)
        {
            if (trSelect != null)
            {
                if (trSelect == transform)
                {
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    trSelect = null;
                    selectedEnemy.GetComponent<Renderer>().material.color = new Color(255, 0, 0);

                    actionsButtons.transform.Find("Canvas").gameObject.SetActive(false); // Dezaktywuje jedynie Canvas przypisany do obiektu ActionsButton, a nie ca³y obiekt
                    MovementManager.canMove = true;

                    // Zresetowanie koloru podswietlonych pol w zasiegu ruchu
                    grid.ResetTileColors();
                }
                else
                {
                    trSelect.localScale = new Vector3(1f, 1f, 1f);
                    selectedEnemy.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                    trSelect = transform;
                    transform.localScale = new Vector3(1.2f, 1.2f, 1f);

                    selectedEnemy = this.gameObject;

                    Debug.Log("Wybra³eœ " + selectedEnemy.name);
                    selectedEnemy.GetComponent<Renderer>().material.color = new Color(1.0f, 0.64f, 0.0f);

                    actionsButtons.transform.Find("Canvas").gameObject.SetActive(true);
                    actionsButtons.transform.position = selectedEnemy.transform.position;
                    ShowOrHideMagicButtons();

                    if (GameObject.Find("ActionsButtonsPlayer/Canvas") != null && Player.selectedPlayer != null)
                        GameObject.Find("ActionsButtonsPlayer/Canvas").SetActive(false);
                    MovementManager.canMove = false;

                    //Zresetowanie szarzy i biegu
                    GameObject.Find("MovementManager").GetComponent<MovementManager>().ResetChargeAndRun();

                    // Zresetowanie koloru podswietlonych pol w zasiegu ruchu
                    grid.ResetTileColors();
                }
            }
            else
            {
                trSelect = transform;
                transform.localScale = new Vector3(1.2f, 1.2f, 1f);

                selectedEnemy = this.gameObject;

                Debug.Log("Wybra³eœ " + selectedEnemy.name);
                selectedEnemy.GetComponent<Renderer>().material.color = new Color(1.0f, 0.64f, 0.0f);

                actionsButtons.transform.Find("Canvas").gameObject.SetActive(true);
                actionsButtons.transform.position = selectedEnemy.transform.position;
                ShowOrHideMagicButtons();

                if (GameObject.Find("ActionsButtonsPlayer/Canvas") != null && Player.selectedPlayer != null)
                    GameObject.Find("ActionsButtonsPlayer/Canvas").SetActive(false);
                MovementManager.canMove = false;

                //Zresetowanie szarzy i biegu
                GameObject.Find("MovementManager").GetComponent<MovementManager>().ResetChargeAndRun();

                // Zresetowanie koloru podswietlonych pol w zasiegu ruchu
                grid.ResetTileColors();
            }
        }    
    }

    // Okreœla, czy s¹ widoczne przyciski splatania magii i rzucania zaklêæ
    public void ShowOrHideMagicButtons()
    {
        if (selectedEnemy.GetComponent<Stats>().Mag > 0)
        {
            GameObject.Find("ActionsButtonsEnemy/Canvas/ChannelingButton").SetActive(true);
            GameObject.Find("ActionsButtonsEnemy/Canvas/SpellButton").SetActive(true);
        }
        else
        {
            GameObject.Find("ActionsButtonsEnemy/Canvas/ChannelingButton").SetActive(false);
            GameObject.Find("ActionsButtonsEnemy/Canvas/SpellButton").SetActive(false);
        }
    }
}
