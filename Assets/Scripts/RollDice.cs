using UnityEngine;
using UnityEngine.UI;

public class RollDice : MonoBehaviour
{
    private MessageManager messageManager;

    [SerializeField] private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        // Odniesienie do Menadzera Wiadomosci wyswietlanych na ekranie gry
        messageManager = GameObject.Find("MessageManager").GetComponent<MessageManager>();
    }

    public void RollK100(GameObject button)
    {
        GameObject character = Character.selectedCharacter;

        if (character == null)
        {
            messageManager.ShowMessage("Musisz najpierw zaznaczyć konkretną postać.", 3f);
            Debug.Log("Musisz najpierw zaznaczyć konkretną postać.");
            return;
        }


        int rollResult = Random.Range(1, 101);

        int value = (int)character.GetComponent<Stats>().GetType().GetField(button.name).GetValue(character.GetComponent<Stats>());

        if ((value + (slider.value * 10)) >= rollResult)
        {
            messageManager.ShowMessage($"<color=green>Rzut na {button.name}: {rollResult}. Wartość cechy: {value}. Poziom sukcesu: {(value + (slider.value * 10)) - rollResult}</color>", 6f);
            Debug.Log($"<color=green>Rzut na {button.name}: {rollResult}. Wartość cechy: {value}. Poziom sukcesu: {(value + (slider.value * 10)) - rollResult}</color>");
        }
        else
        {
            messageManager.ShowMessage($"<color=red>Rzut na {button.name}: {rollResult}. Wartość cechy: {value}. Poziom porażki: {rollResult - (value + (slider.value * 10))}</color>", 6f);
            Debug.Log($"<color=red>Rzut na {button.name}: {rollResult}. Wartość cechy: {value}. Poziom porażki: {rollResult - (value + (slider.value * 10))}</color>");
        }
    }
}
