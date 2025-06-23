using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICode : MonoBehaviour
{
    [System.Serializable]
    public class UIElement
    {
        public string name; // Nazwa elementu UI
        public GameObject element; // Obiekt UI
    }

    // mo¿liwe stany UI - running, paused, settings
    public enum UIState
    {
        Running,
        Paused,
        Settings,
        Building,
        Map
    }

    public UIState ui_state;

    public List<UIElement> UIPauseElements;
    public List<UIElement> UIUnpauseElements;

    private GameObject pauseButton;
    private GameObject budynekButton;
    private GameObject Budynki;


    private GameObject UIPause;
    private GameObject UIUnpause;
    private GameObject UIMapa;

    void Awake()
    {
        // Inicjalizacja list
        UIPauseElements = new List<UIElement>();
        UIUnpauseElements = new List<UIElement>();

        // ZnajdŸ wszystkie istotne obiekty
        FindAllObjects();

        // Ustaw domyœlny stan gry
        SetUIState(UIState.Paused);


        // Zbieranie elementów UI dla ró¿nych stanów
        //CollectUIElements(UIPause, UIPauseElements);
        //CollectUIElements(UIUnpause, UIUnpauseElements);
    }

    /// <summary>
    /// Znajduje i przypisuje referencje do wszystkich istotnych obiektów UI.
    /// </summary>
    private void FindAllObjects()
    {
        // ZnajdŸ PauseButton
        pauseButton = GameObject.Find("PauseButton");
        if (pauseButton == null)
        {
            Debug.LogWarning("Nie znaleziono obiektu PauseButton!");
        }

        // ZnajdŸ UIPause
        UIPause = GameObject.Find("UIPause");
        if (UIPause == null)
        {
            Debug.LogWarning("Nie znaleziono obiektu UIPause!");
        }

        UIMapa = GameObject.Find("UIMapa");
        if (UIMapa == null)
        {
            Debug.LogWarning("Nie znaleziono obiektu UIMapa!");
        }


        // ZnajdŸ UIUnpause
        UIUnpause = GameObject.Find("UIUnpause");
        if (UIUnpause == null)
        {
            Debug.LogWarning("Nie znaleziono obiektu UIUnpause!");
        }
        else
        {
            // ZnajdŸ BudynekButton w UIUnpause
            Transform buildingButtonTransform = UIUnpause.transform.Find("BudynekButton");
            if (buildingButtonTransform != null)
            {
                budynekButton = buildingButtonTransform.gameObject;
            }
            else
            {
                Debug.LogWarning("Nie znaleziono BudynekButton w UIUnpause!");
            }

            Transform buildingsTransform = UIUnpause.transform.Find("Budynki");
            if (buildingsTransform != null)
            {
                Budynki = buildingsTransform.gameObject;
            }
            else
            {
                Debug.LogWarning("Nie znaleziono Budynki w UIUnpause!");
            }

        }

    }

    /*
    /// <summary>
    /// Zbiera wszystkie elementy UI z podanego kontenera i dodaje je do listy.
    /// </summary>
    private void CollectUIElements(GameObject container, List<UIElement> targetList)
    {
        if (container == null)
        {
            Debug.LogWarning("Kontener jest null, nie mo¿na zebraæ elementów UI.");
            return;
        }

        foreach (Transform child in container.transform)
        {
            targetList.Add(new UIElement
            {
                name = child.gameObject.name,
                element = child.gameObject
            });
            Debug.Log($"Zebrano element UI: {child.gameObject.name} z kontenera '{container.name}'.");
        }

        Debug.Log($"Zebrano {targetList.Count} elementów z kontenera '{container.name}'.");
    }
    */

    /// <summary>
    /// Ustawia stan UI i zarz¹dza widocznoœci¹ elementów.
    /// </summary>
    public void SetUIState(UIState state)
    {
        ui_state = state;

        switch (state)
        {
            case UIState.Running:
                UIPause.SetActive(false);
                UIUnpause.SetActive(true);
                SetOpacity(pauseButton, 0.8f);
                SetOpacity(budynekButton, 0.8f);
                Budynki.SetActive(false);
                break;

            case UIState.Paused:
                UIPause.SetActive(true);
                UIUnpause.SetActive(false);
                UIMapa.SetActive(false);
                SetOpacity(pauseButton, 1.0f);
                break;

            case UIState.Settings:
                break;

            case UIState.Building:
                Budynki.SetActive(true);
                SetOpacity(budynekButton, 1.0f);
                break;
            case UIState.Map:
                UIMapa.SetActive(true);
                UIPause.SetActive(false);
                break;
        }
    }


    /// <summary>
    /// Ustawia przezroczystoœæ dla danego obiektu.
    /// </summary>
    private void SetOpacity(GameObject obj, float alpha)
    {
        if (obj == null) return;

        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
        else
        {
            Debug.LogWarning($"{obj.name} nie ma komponentu SpriteRenderer!");
        }
    }

    /// <summary>
    /// Toggluje widocznoœæ elementów w UIPause i UIUnpause oraz zmienia przezroczystoœæ PauseButton w zale¿noœci od stanu gry.
    /// </summary>
    public void PauseButtonClicked()
    {
        SetUIState(ui_state == UIState.Paused ? UIState.Running : UIState.Paused);
    }

    public void BudynekButtonClicked()
    {
        if (Budynki != null)
        {
            if (Budynki.activeSelf)
            {
                Budynki.SetActive(false);
                SetUIState(UIState.Running);
            }
            else
            {
                Budynki.SetActive(true);
                SetUIState(UIState.Building);
            }
        }
        else
        {
            Debug.LogWarning("Budynki nie zosta³y przypisane!");
        }
    }

    public void KonkretnyBudynekClicked()
    {
        Debug.Log("kliknieto budynek");
    }

    public void MapOptionClicked()
    {
        if (UIMapa != null)
        {
            if (UIMapa.activeSelf)
            {
                SetUIState(UIState.Paused);
            }
            else
            {
                SetUIState(UIState.Map);
            }
        }
        else
        {
            Debug.LogWarning("Mapa nie zosta³a przypisana!");
        }
    }
}