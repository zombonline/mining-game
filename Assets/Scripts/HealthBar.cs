using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health health;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        health.onCurrentHealthUpdated += HandleCurrentHealthUpdated;
    }
    private void OnDisable()
    {
        health.onCurrentHealthUpdated -= HandleCurrentHealthUpdated;
    }

    private void HandleCurrentHealthUpdated(float newHealth)
    {
        slider.value = health.GetCurrentHealthPercentage();
    }
}
