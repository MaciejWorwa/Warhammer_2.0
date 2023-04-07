using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class StatsEditor : MonoBehaviour
{
    [SerializeField] private GameObject statsPanel;

    public static bool EditorIsOpen = false;

    public void PanelVisibility()
    {
        if (statsPanel.activeSelf)
        {
            statsPanel.SetActive(false);
            EditorIsOpen = false;
        }
        else
        {
            statsPanel.SetActive(true);
            EditorIsOpen = true;
        }
    }

    public void EditAttribute(GameObject textInput)
    {
        GameObject character = CharacterManager.GetSelectedCharacter();

        // Zamienia wprowadzony text na wartoœæ int
        int.TryParse(textInput.GetComponent<TMPro.TMP_InputField>().text, out int value);

        // Pobiera pole o nazwie takiej jak nazwa textInputa
        FieldInfo field = character.GetComponent<Stats>().GetType().GetField(textInput.name);

        // Jezeli znajdzie to zmienia wartosc cechy
        if (field != null && field.FieldType == typeof(int))
        {
            field.SetValue(character.GetComponent<Stats>(), value);
            Debug.Log($"Atrybut {field.Name} zmieniony na {value}");
        }
        else
            Debug.Log($"{field.Name} {value}");


        //To powyzej dziala tylko dla Int. Pomyslec jak dorobic jeszcze zeby przyjmowalo bool albo string, nie powinno to byc trudne
    }
}
