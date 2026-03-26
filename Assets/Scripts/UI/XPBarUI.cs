using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBarUI : MonoBehaviour
{
    [Header("XP Bars")]
    public Image xpFinishImage;   // XP để finish level
    public Image xpUpgradeImage;  // XP để upgrade level

    [Header("Level Text")]
    public TMP_Text levelText;

    private void OnEnable()
    {
        StartCoroutine(InitRoutine());
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnXPChanged -= UpdateXPBar;
        }
    }

    IEnumerator InitRoutine()
    {
        // Chờ 1 frame để GameManager load xong levelData
        yield return null;

        if (GameManager.Instance != null)
        {
            // Đăng ký event
            GameManager.Instance.OnXPChanged += UpdateXPBar;

            // Cập nhật ban đầu
            UpdateXPBar(
                GameManager.Instance.finishedXP,
                GameManager.Instance.upgradeXP
            );

            UpdateLevelText();
        }
    }

    void UpdateXPBar(float totalXP, float upgradeXP)
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        // Lấy XP cần để finish level
        float finishXP = gm.GetFinishXP();
        // Lấy XP còn thiếu để upgrade level tiếp theo
        float upgradeXPMax = gm.GetUpgradeXPRequired();

        // Fill bar XP hoàn thành level
        if (xpFinishImage != null && finishXP > 0f)
        {
            xpFinishImage.fillAmount = Mathf.Clamp01(totalXP / finishXP);
        }

        // Fill bar XP upgrade
        if (xpUpgradeImage != null && upgradeXPMax > 0f)
        {
            xpUpgradeImage.fillAmount = Mathf.Clamp01(upgradeXP / upgradeXPMax);
        }
    }

    void UpdateLevelText()
    {
        var gm = GameManager.Instance;
        if (gm == null || levelText == null) return;

        // Lấy level hiện tại từ LevelLoader
        int level = gm.levelLoader != null ? gm.levelLoader.CurrentLevel : 1;

        levelText.text = $"Level {level}";
    }
}