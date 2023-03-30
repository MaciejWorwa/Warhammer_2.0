using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public static Stats playerStats;
    public enum Rasa { Czlowiek, Elf, Krasnolud, Niziolek }
    [SerializeField] private Rasa rasa;

    private TMP_Text healthDisplay;
    private TMP_Text initiativeDisplay;

    public static GameObject selectedPlayer;
    public static Transform trSelect = null;
    public GameObject actionsButtons;

    private float attackDistance;

    private AttackManager attackManager;

    void Start()
    {

        //nadanie rasy
        rasa = (Rasa)Random.Range(0, 4);
        Debug.Log($"Stworzy³eœ {this.gameObject.name} o rasie {rasa}");

        //nadanie temu obiektowi klasy Stats
        playerStats = this.gameObject.AddComponent<Stats>();
        this.gameObject.GetComponent<MovementManager>();

        //wygenerowanie poczatkowych statystyk w zaleznosci od rasy. Metoda ta jest zawarta w klasie Stats
        playerStats.SetBaseStatsByRace(rasa);

        playerStats.tempHealth = playerStats.maxHealth;
        playerStats.tempSz = playerStats.Sz;

        //nadanie wartosci punktow zbroi
        playerStats.PZ_head = 1;
        playerStats.PZ_arms = 2;
        playerStats.PZ_torso = 3;
        playerStats.PZ_legs = 4;

        //zasieg broni
        playerStats.Weapon_S = 3;
        playerStats.AttackRange = 1.5;

        actionsButtons = GameObject.Find("ActionsButtonsPlayer");

        attackManager = GameObject.Find("AttackManager").GetComponent<AttackManager>();

        healthDisplay = this.transform.Find("healthPointsPlayer").gameObject.GetComponent<TMP_Text>();
        healthDisplay.transform.position = this.gameObject.transform.position;

        initiativeDisplay = this.transform.Find("initiativePlayer").gameObject.GetComponent<TMP_Text>();
        initiativeDisplay.transform.position = new Vector3(this.gameObject.transform.position.x + 0.5f, this.gameObject.transform.position.y + 0.5f, this.gameObject.transform.position.z);
    }

    void Update()
    {
        healthDisplay.text = this.gameObject.GetComponent<Stats>().tempHealth + "/" + this.gameObject.GetComponent<Stats>().maxHealth;
        initiativeDisplay.text = this.gameObject.GetComponent<Stats>().Initiative.ToString();

        if (Input.GetKeyDown(KeyCode.P) && selectedPlayer.name == this.gameObject.name)
        {
            attackManager.Attack(selectedPlayer, Enemy.selectedEnemy);
            actionsButtons.SetActive(false);
        }
 
        if (playerStats.tempHealth < 0 && playerStats.criticalCondition == false)
        {
            playerStats.GetCriticalHit();
        }

        if(Input.GetKeyDown(KeyCode.Delete) && trSelect != null)
        {
            Destroy(selectedPlayer);
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
                selectedPlayer.GetComponent<Renderer>().material.color = new Color(0, 255, 0);

                actionsButtons.SetActive(false);
                Tile.canMove = true;
            }
            else
            {
                trSelect.localScale = new Vector3(1f, 1f, 1f);
                selectedPlayer.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
                trSelect = transform;
                transform.localScale = new Vector3(1.2f, 1.2f, 1f);

                selectedPlayer = this.gameObject;

                Debug.Log("Wybra³eœ " + selectedPlayer.name);
                selectedPlayer.GetComponent<Renderer>().material.color = new Color(0f, 1f, 0.64f);

                actionsButtons.SetActive(true);
                actionsButtons.transform.position = selectedPlayer.transform.position;

                if (GameObject.Find("ActionsButtonsEnemy") != null && Enemy.selectedEnemy != null)
                    GameObject.Find("ActionsButtonsEnemy").SetActive(false);
                Tile.canMove = false;
            }
        }
        else
        {
            trSelect = transform;
            transform.localScale = new Vector3(1.2f, 1.2f, 1f);

            selectedPlayer = this.gameObject;

            Debug.Log("Wybra³eœ " + selectedPlayer.name);
            selectedPlayer.GetComponent<Renderer>().material.color = new Color(0f, 1f, 0.64f);

            actionsButtons.SetActive(true);
            actionsButtons.transform.position = selectedPlayer.transform.position;
           
            if (GameObject.Find("ActionsButtonsEnemy") != null && Enemy.selectedEnemy != null)
                GameObject.Find("ActionsButtonsEnemy").SetActive(false);
            Tile.canMove = false;
        }
    }
}



    

