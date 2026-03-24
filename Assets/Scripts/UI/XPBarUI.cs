using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBarUI : MonoBehaviour
{
    public Image xpFinishImage;
    public Image xpUpgradeImage;
    public TMP_Text levelText;

    private void OnEnable()
    {
        StartCoroutine(InitRoutine());
    }

    IEnumerator InitRoutine()
    {
        yield return null;// Chờ 1 frame cho cái GameManager load cái file json xong

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnXPChanged += UpdateXPBar;

            UpdateXPBar(
                GameManager.Instance.finishedXP,
                GameManager.Instance.upgradeXP
            );
        }

        UpdateLevelText();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnXPChanged -= UpdateXPBar;
        }
    }

    void UpdateXPBar(float totalXP, float upgradeXP)
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        float finishXP = gm.GetFinishXP();
        float upgradeXPMax = gm.GetUpgradeXPRequired();

        if (xpFinishImage != null && finishXP > 0)
        {
            xpFinishImage.fillAmount = Mathf.Clamp01(totalXP / finishXP);
        }

        if (xpUpgradeImage != null && upgradeXPMax > 0)
        {
            xpUpgradeImage.fillAmount = Mathf.Clamp01(upgradeXP / upgradeXPMax);
        }
    }

    void UpdateLevelText()
    {
        var gm = GameManager.Instance;
        if (gm == null || levelText == null) return;

        int level = gm.levelLoader.levelNumber;

        levelText.text = $"Level {level}";
    }
}