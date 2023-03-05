using UnityEngine;

public class LevelArea : MonoBehaviour
{
    [SerializeField] private GameObject entities;
    [SerializeField] private GameObject viewBlockers;
    [SerializeField] private bool revealed;


    private void Awake()
    {
        if (revealed) return;
        
        Hide();
    }


    public void Reveal()
    {
        entities.SetActive(true);
        viewBlockers.SetActive(false);
    }


    private void Hide()
    {
        entities.SetActive(false);
        viewBlockers.SetActive(true);
    }
}