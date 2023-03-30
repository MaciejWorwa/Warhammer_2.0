using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementManager : MonoBehaviour
{
    public static bool canMove = true; // okresla czy postac moze wykonac ruch
    public static bool Charge; // szar¿a
    public static bool Run; //bieg

    [SerializeField] private GameObject chargeButton;
    [SerializeField] private GameObject runButton;

    public void SetCharge()
    {
        if(Charge != true)
        {
            Charge = true;
            chargeButton.GetComponent<Image>().color = Color.white;
            Run = false;
            runButton.GetComponent<Image>().color = Color.gray;
        } 
        else
        {
            Charge = false;
            chargeButton.GetComponent<Image>().color = Color.gray;
        } 
    }

    public void SetRun()
    {
        if (Run != true)
        {
            Run = true;
            runButton.GetComponent<Image>().color = Color.white;
            Charge = false;
            chargeButton.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            Run = false;
            runButton.GetComponent<Image>().color = Color.gray;
        }
    }

    //aktywuje mozliwosc ruchu
    public void ActiveMove()
    {
        canMove = true;

        //wy³¹czanie widocznosci buttonow akcji oraz odznaczanie drugiej zaznaczonej postaci, je¿eli taka istnieje
        if (Player.trSelect != null && GameObject.Find("ActionsButtonsPlayer") != null)
        {
            GameObject.Find("ActionsButtonsPlayer").SetActive(false);
            if(Enemy.trSelect != null)
            {
                Enemy.trSelect = null;
                Enemy.selectedEnemy.transform.localScale = new Vector3(1f, 1f, 1f);
                Enemy.selectedEnemy.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
            }
        }
        if (Enemy.trSelect != null && GameObject.Find("ActionsButtonsEnemy") != null)
        {
            GameObject.Find("ActionsButtonsEnemy").SetActive(false);
            if (Player.trSelect != null)
            {
                Player.trSelect = null;
                Player.selectedPlayer.transform.localScale = new Vector3(1f, 1f, 1f);
                Player.selectedPlayer.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
            }  
        }
    }

    public void MoveSelectedCharacter(GameObject selectedTile)
    {
        //RUCH POSTACI. Na przysz³oœæ: rozkminiæ jak zrobiæ, ¿eby wylicza³ zasiêg ruchu uwzglêdniaj¹c inne postacie, które ma na linii ruchu i powinien je omin¹æ

        if (Player.trSelect != null && Enemy.trSelect == null && canMove)
        {
            //zmienia predkosc w zaleznoœci, czy s¹ zaznaczone przyciski szar¿y lub biegu
            if (Charge)
                Player.selectedPlayer.GetComponent<Stats>().tempSz = Player.selectedPlayer.GetComponent<Stats>().Sz * 2;
            else if (Run)
                Player.selectedPlayer.GetComponent<Stats>().tempSz = Player.selectedPlayer.GetComponent<Stats>().Sz * 3;
            else
                Player.selectedPlayer.GetComponent<Stats>().tempSz = Player.selectedPlayer.GetComponent<Stats>().Sz;

            //ustala aktualn¹ szybkosc
            int movementRange = Player.selectedPlayer.GetComponent<Stats>().tempSz;

            //zbiera informacje o pozycji wybranej postaci oraz kliknietego pola (tile)
            Vector3 charPos = Player.selectedPlayer.transform.position;
            Vector3 tilePos = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, 0);

            //sprawdza dystans do przebycia
            if ((Mathf.Abs(charPos.x - tilePos.x)) + (Mathf.Abs(charPos.y - tilePos.y)) <= movementRange)
            {
                CheckForOpportunityAttack(Player.selectedPlayer, selectedTile);
                Player.selectedPlayer.transform.position = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, 0);
            }
            else
                Debug.Log("Wybrane pole jest poza zasiêgiem ruchu postaci.");
        }
        else if (Enemy.trSelect != null && Player.trSelect == null && canMove)
        {
            //zmienia predkosc w zaleznoœci, czy s¹ zaznaczone przyciski szar¿y liub biegu
            if (Charge)
                Enemy.selectedEnemy.GetComponent<Stats>().tempSz = Enemy.selectedEnemy.GetComponent<Stats>().Sz * 2;
            else if (Run)
                Enemy.selectedEnemy.GetComponent<Stats>().tempSz = Enemy.selectedEnemy.GetComponent<Stats>().Sz * 3;
            else
                Enemy.selectedEnemy.GetComponent<Stats>().tempSz = Enemy.selectedEnemy.GetComponent<Stats>().Sz;

            //ustala aktualn¹ szybkosc
            int movementRange = Enemy.selectedEnemy.GetComponent<Stats>().tempSz;

            //zbiera informacje o pozycji wybranej postaci oraz kliknietego pola (tile)
            Vector3 charPos = Enemy.selectedEnemy.transform.position;
            Vector3 tilePos = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, 0);

            //sprawdza dystans do przebycia
            if ((Mathf.Abs(charPos.x - tilePos.x)) + (Mathf.Abs(charPos.y - tilePos.y)) <= movementRange)
            {
                CheckForOpportunityAttack(Enemy.selectedEnemy, selectedTile);
                Enemy.selectedEnemy.transform.position = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, 0);
            }
            else
                Debug.Log("Wybrane pole jest poza zasiêgiem ruchu postaci.");
        }
        else if (Enemy.trSelect != null && Player.trSelect != null && canMove)
            Debug.Log("Nie mo¿esz poruszyæ siê dwoma postaciami jednoczeœnie.");
    }


    // Sprawdza czy ruch powoduje atak okazyjny
    void CheckForOpportunityAttack(GameObject movingCharacter, GameObject selectedTile)
    {
        if (movingCharacter == Player.selectedPlayer)
        {
            // Stworzenie listy wszystkich wrogow
            GameObject[] nearEnemies;
            nearEnemies = GameObject.FindGameObjectsWithTag("Enemy");

            // Atak okazyjny wywolywany dla kazdego wroga bedacego w zwarciu z bohaterem gracza
            foreach (GameObject enemy in nearEnemies)
            {
                // Sprawdzenie ilu wrogow jest w zwarciu z bohaterem gracza i czy ruch bohatera gracza powoduje oddalenie siê od nich (czyli atak okazyjny)
                float distanceFromOpponent = Vector3.Distance(Player.selectedPlayer.transform.position, enemy.transform.position);
                float distanceFromOpponentAfterMove = Vector3.Distance(selectedTile.transform.position, enemy.transform.position);

                if (distanceFromOpponent <= 1.8f && distanceFromOpponentAfterMove > 1.8f)
                {
                    // Wywo³anie ataku okazyjnego w klasie AttackManager
                    AttackManager attackManager = GameObject.Find("AttackManager").GetComponent<AttackManager>();
                    attackManager.OpportunityAttack(enemy, Player.selectedPlayer);
                }
            }
        }
        if (movingCharacter == Enemy.selectedEnemy)
        {
            // Stworzenie listy wszystkich bohaterow graczy
            GameObject[] nearPlayers;
            nearPlayers = GameObject.FindGameObjectsWithTag("Player");

            // Atak okazyjny wywolywany dla kazdego bohatera gracza bedacego w zwarciu z wrogiem
            foreach (GameObject player in nearPlayers)
            {
                // Sprawdzenie ilu bohaterow gracza jest w zwarciu z wrogiem i czy ruch wroga powoduje oddalenie siê od nich (czyli atak okazyjny)
                float distanceFromOpponent = Vector3.Distance(Enemy.selectedEnemy.transform.position, player.transform.position);
                float distanceFromOpponentAfterMove = Vector3.Distance(selectedTile.transform.position, player.transform.position);

                if (distanceFromOpponent <= 1.8f && distanceFromOpponentAfterMove > 1.8f)
                {
                    // Wywo³anie ataku okazyjnego w klasie AttackManager
                    AttackManager attackManager = GameObject.Find("AttackManager").GetComponent<AttackManager>();
                    attackManager.OpportunityAttack(player, Enemy.selectedEnemy);
                }
            }
        }
    }

}




