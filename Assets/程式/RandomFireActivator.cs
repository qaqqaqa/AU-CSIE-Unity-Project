using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFireActivator : MonoBehaviour
{
    [Header("🔥 一般火焰 (任務目標)")]
    [Tooltip("請將場景中所有【可撲滅】的火焰物件拖到這裡")]
    public List<GameObject> fireObjects;

    [Tooltip("最少啟用幾個一般火焰")]
    [Range(1, 100)]
    public int minFiresToActivate = 1;

    [Tooltip("最多啟用幾個一般火焰")]
    [Range(1, 100)]
    public int maxFiresToActivate = 4;

    [Header("☠️ 不可控大火 (僅大火模式出現)")]
    [Tooltip("請將場景中所有【不可撲滅】的惡意火焰物件拖到這裡")]
    public List<GameObject> uncontrollableFires; // 新增的列表

    [Tooltip("大火模式下，要額外啟用幾個不可控火焰")]
    public int uncontrollableCount = 3;

    void Start()
    {
        ActivateRandomFires();
    }

    public void ActivateRandomFires()
    {
        // --- 1. 初始化：先把所有火都關掉 ---
        if (fireObjects != null)
        {
            foreach (var fire in fireObjects) if (fire != null) fire.SetActive(false);
        }
        if (uncontrollableFires != null)
        {
            foreach (var fire in uncontrollableFires) if (fire != null) fire.SetActive(false);
        }

        // --- 2. 處理一般火焰 (任務目標) ---
        int min = minFiresToActivate;
        int max = maxFiresToActivate;

        // 根據難度調整一般火焰數量
        if (MainMenuController.SelectedDifficulty == 2)
        {
            // 大火模式基礎數量增加
            min = 6;
            max = 10;
        }

        int amountToActivate = Random.Range(min, max + 1);
        if (fireObjects != null)
            amountToActivate = Mathf.Min(amountToActivate, fireObjects.Count);

        List<GameObject> availableFires = new List<GameObject>(fireObjects);
        List<FireScript> activatedFireScripts = new List<FireScript>();

        for (int i = 0; i < amountToActivate; i++)
        {
            if (availableFires.Count == 0) break;

            int randomIndex = Random.Range(0, availableFires.Count);
            GameObject fireToActivate = availableFires[randomIndex];

            if (fireToActivate != null)
            {
                fireToActivate.SetActive(true);
                FireScript fs = fireToActivate.GetComponent<FireScript>();
                if (fs != null) activatedFireScripts.Add(fs);
            }
            availableFires.RemoveAt(randomIndex);
        }

        // --- 3. 處理不可控大火 (僅限大火模式) ---
        if (MainMenuController.SelectedDifficulty == 2 && uncontrollableFires != null && uncontrollableFires.Count > 0)
        {
            Debug.Log("🔥 大火模式特效：啟動不可控惡意火焰！");

            List<GameObject> availableHazards = new List<GameObject>(uncontrollableFires);
            int hazardsToSpawn = Mathf.Min(uncontrollableCount, availableHazards.Count);

            for (int i = 0; i < hazardsToSpawn; i++)
            {
                int randomIndex = Random.Range(0, availableHazards.Count);
                GameObject hazardToActivate = availableHazards[randomIndex];

                if (hazardToActivate != null)
                {
                    hazardToActivate.SetActive(true);
                    // 注意：我們【不】把這些火加入 activatedFireScripts
                    // 因為它們不可撲滅，不需要被 GameManager 追蹤
                }
                availableHazards.RemoveAt(randomIndex);
            }
        }

        // --- 4. 回報任務給 GameManager ---
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetFiresToTrack(activatedFireScripts);
        }
    }
}