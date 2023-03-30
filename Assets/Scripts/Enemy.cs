using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public static Stats enemyStats;
    private enum Rasa { Ork, Goblin, Troll, Wilk } // rozkminiæ gdzie przechowywaæ informacje o statystykach poczatkowych danej rasy, zeby nie bylo miliona linii kodu w Enemy
    [SerializeField] private Rasa rasa;

    private TMP_Text healthDisplay;
    private TMP_Text initiativeDisplay;

    public static GameObject selectedEnemy;
    public static Transform trSelect = null;
    public GameObject actionsButtons;

    private float attackDistance;

    private AttackManager attackManager;

    void Start()
    {
        //nadanie temu obiektowi klasy Stats
        enemyStats = this.gameObject.AddComponent<Stats>();
        this.gameObject.GetComponent<MovementManager>();

        //nadanie rasy
        rasa = (Rasa)Random.Range(0, 4);
        Debug.Log($"Stworzy³eœ {this.gameObject.name} o rasie {rasa}");

        //nadanie wartosci cech pierwszorzedowych
        enemyStats.WW = 20 + Random.Range(2, 21);
        enemyStats.US = 20 + Random.Range(2, 21);
        enemyStats.K = 20 + Random.Range(2, 21);
        enemyStats.Odp = 20 + Random.Range(2, 21);
        enemyStats.Zr = 20 + Random.Range(2, 21);
        enemyStats.Int = 20 + Random.Range(2, 21);
        enemyStats.SW = 20 + Random.Range(2, 21);
        enemyStats.Ogd = 20 + Random.Range(2, 21);

        //nadanie wartosci cech drugorzedowych
        enemyStats.Sz = 4;
        enemyStats.Mag = 0;
        enemyStats.maxHealth = Random.Range(10, 21);

        enemyStats.tempHealth = enemyStats.maxHealth;
        enemyStats.tempSz = enemyStats.Sz;

        //nadanie inicjatywy
        enemyStats.Initiative = enemyStats.Zr + Random.Range(1, 11); // docelowo to bedzie robione rowniez w klasie Stats, tylko chce najpierw ogarnac import Jsona ze statami roznych wrogow

        //nadanie wartosci punktow zbroi
        enemyStats.PZ_head = 1;
        enemyStats.PZ_arms = 2;
        enemyStats.PZ_torso = 3;
        enemyStats.PZ_legs = 4;

        //zasieg broni
        enemyStats.Weapon_S = 3;
        enemyStats.AttackRange = 1.5;

        actionsButtons = GameObject.Find("ActionsButtonsEnemy");

        attackManager = GameObject.Find("AttackManager").GetComponent<AttackManager>();

        healthDisplay = this.transform.Find("healthPointsEnemy").gameObject.GetComponent<TMP_Text>();
        healthDisplay.transform.position = this.gameObject.transform.position;

        initiativeDisplay = this.transform.Find("initiativeEnemy").gameObject.GetComponent<TMP_Text>();
        initiativeDisplay.transform.position = new Vector3(this.gameObject.transform.position.x + 0.5f, this.gameObject.transform.position.y + 0.5f, this.gameObject.transform.position.z);

    }

    void Update()
    {
        healthDisplay.text = this.gameObject.GetComponent<Stats>().tempHealth + "/" + this.gameObject.GetComponent<Stats>().maxHealth;
        initiativeDisplay.text = this.gameObject.GetComponent<Stats>().Initiative.ToString();

        if (Input.GetKeyDown(KeyCode.E) && selectedEnemy.name == this.gameObject.name)
        {
            attackManager.Attack(selectedEnemy, Player.selectedPlayer);
            actionsButtons.SetActive(false);
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
        if (trSelect != null)
        {
            if (trSelect == transform)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                trSelect = null;
                selectedEnemy.GetComponent<Renderer>().material.color = new Color(255, 0, 0);

                actionsButtons.SetActive(false);
                Tile.canMove = true;
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

                actionsButtons.SetActive(true);
                actionsButtons.transform.position = selectedEnemy.transform.position;

                if (GameObject.Find("ActionsButtonsPlayer") != null && Player.selectedPlayer != null)
                    GameObject.Find("ActionsButtonsPlayer").SetActive(false);
                Tile.canMove = false;
            }
        }
        else
        {
            trSelect = transform;
            transform.localScale = new Vector3(1.2f, 1.2f, 1f);

            selectedEnemy = this.gameObject;

            Debug.Log("Wybra³eœ " + selectedEnemy.name);
            selectedEnemy.GetComponent<Renderer>().material.color = new Color(1.0f, 0.64f, 0.0f);

            actionsButtons.SetActive(true);
            actionsButtons.transform.position = selectedEnemy.transform.position;
            
            if(GameObject.Find("ActionsButtonsPlayer") != null && Player.selectedPlayer != null)
                GameObject.Find("ActionsButtonsPlayer").SetActive(false);
            Tile.canMove = false;
        }
    }
}
