using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementManager : MonoBehaviour
{
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

    public void ActiveMove()
    {
        Tile.canMove = true;

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
}
