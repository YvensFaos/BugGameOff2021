using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CollectibleGrouper : MonoBehaviour
{
    [SerializeField]
    private int necessaryAmount;

    [SerializeField] 
    private Text collectibleText;
    
    [SerializeField]
    private UnityEvent afterCompleted;
    
    private int _currentAmount;

    private void Start()
    {
        UpdateText();
    }
    
    public void AddCollectible()
    {
        _currentAmount++;
        UpdateText();

        if (_currentAmount >= necessaryAmount)
        {
            afterCompleted.Invoke();
        }
    }

    private void UpdateText()
    {
        collectibleText.text = $"{_currentAmount}/{necessaryAmount}";
    }
}