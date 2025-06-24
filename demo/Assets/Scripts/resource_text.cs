using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshPro scoreText;
    public ResourceManager ResourceManager;


    private void Awake()
    {
        scoreText = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        scoreText.text = ResourceManager.wood.ToString();
    }
}
