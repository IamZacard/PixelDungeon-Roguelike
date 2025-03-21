using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private Text damageText;     // Drag Text component here
    [SerializeField] private float floatSpeed = 1f; // Speed of upward movement
    [SerializeField] private float lifetime = 1f;   // How long it lasts

    public void SetDamage(int damage)
    {
        damageText.text = damage.ToString();
        Destroy(gameObject, lifetime);  // Auto-destroy after lifetime
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;  // Float upward
    }
}