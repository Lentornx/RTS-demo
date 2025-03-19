using UnityEngine;
using System.Reflection;

public class UniversalButton : MonoBehaviour
{
    public GameObject targetObject;
    public string functionName; 

    void Start()
    {
        TouchManager.Instance.OnTouchEnded += HandleTouch;
    }

    void HandleTouch(Vector2 touchPosition)
    {
        Collider2D hitCollider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(touchPosition));

        if (hitCollider != null && hitCollider.gameObject == gameObject)
        {
            Debug.LogWarning("clicked");
            InvokeOnTarget();
        }
    }

    void InvokeOnTarget()
    {
        if (targetObject == null || string.IsNullOrEmpty(functionName)) return;

        Component[] components = targetObject.GetComponents<Component>();

        foreach (Component comp in components)
        {
            MethodInfo method = comp.GetType().GetMethod(functionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null)
            {
                method.Invoke(comp, null); 
                return;
            }
        }

        Debug.LogWarning($"Function '{functionName}' not found on {targetObject.name}");
    }

    void OnDestroy()
    {
        if (TouchManager.Instance != null)
        {
            TouchManager.Instance.OnTouchEnded -= HandleTouch;
        }
    }
}
