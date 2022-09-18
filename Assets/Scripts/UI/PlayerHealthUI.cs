using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
   Text levelText;
   Image healthSlider;
   Image expSlider;

    CharacterStats playerStats;

    private void Awake()
    {
        //levelText = gameObject.GetComponent<Text>(); 
        // 这样写不对，因为该脚本挂载在canvas上，拿不到text组件

        levelText = transform.GetChild(2).GetComponent<Text>();
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();

    }

    private void Update()
    {    
        playerStats = GameManager.Instance.playerStats;
        levelText.text = "LEVEL " + playerStats.characterData.currentLevel.ToString("00");
        UpdateHealth();
        UpdateExp();

    }

    void UpdateHealth()
    {
        float slidPersent = (float)playerStats.CurrentHealth / playerStats.MaxHealth;
        healthSlider.fillAmount = slidPersent;
    }

    void UpdateExp()
    {
        float slidPersent = (float)playerStats.characterData.currentExp / playerStats.characterData.baseExp;
        expSlider.fillAmount = slidPersent;
    }
}
