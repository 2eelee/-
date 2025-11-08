using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int currentHP;
    public int maxHP;

    public TMP_Text healthBar;
    void Start()
    {
        healthBar.text = "HP: " + currentHP + " / " + maxHP;
    }
    public void ChangeHealth(int amount)
    {
        currentHP += amount;
        if (currentHP <= 0)
        {
            gameObject.SetActive(false);
        }
        healthBar.text = "HP: " + currentHP + " / " + maxHP;
    }
}
