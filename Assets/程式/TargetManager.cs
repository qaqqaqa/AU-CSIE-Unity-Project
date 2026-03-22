using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TargetManager : MonoBehaviour
{
    public Transform player;
    public TargetIndicator indicator;
    public float checkInterval = 1.0f; // 每隔多久檢查一次最近的火源

    private List<FireScript> allFires;

    void Start()
    {
        // --- 【核心修改點】 ---
        // 使用 Unity 推薦的、效能更好的新方法來尋找物件
        // FindObjectsSortMode.None 表示我們不需要排序，這樣速度最快
        allFires = new List<FireScript>(FindObjectsByType<FireScript>(FindObjectsSortMode.None));
        // --- 修改結束 ---

        Debug.Log($"【日誌 1】TargetManager 已啟動，在場景中找到了 {allFires.Count} 個火源。");

        // 如果找不到玩家，嘗試自動尋找（假設玩家物件有 "Player" 標籤）
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // 定期執行尋找最近火源的方法
        InvokeRepeating(nameof(FindClosestFire), 0.5f, checkInterval);
    }

    void FindClosestFire()
    {
        // 如果沒有玩家或指示器，或已經沒有火了，就停止
        if (player == null || indicator == null || allFires.Count == 0)
        {
            Debug.LogWarning("【日誌 2】TargetManager 檢查失敗：Player、Indicator 或 Fires 列表為空！");
            if (indicator != null) indicator.target = null;
            return;
        }

        // 移除已經被摧毀的火源 (以防萬一)
        allFires.RemoveAll(item => item == null);

        // 如果移除後火源為空，則清除目標並返回
        if (allFires.Count == 0)
        {
            indicator.target = null;
            return;
        }

        // 使用 Linq 找到距離玩家最近的火源
        FireScript closestFire = allFires.OrderBy(fire =>
            Vector3.Distance(player.position, fire.transform.position)
        ).FirstOrDefault();

        // 將最近的火源設為指示器的目標
        if (closestFire != null)
        {
            indicator.target = closestFire.transform;
            Debug.Log($"【日誌 3】TargetManager 已找到最近的火源 '{closestFire.name}' 並設定給 Indicator。");
        }
        else
        {
            indicator.target = null; // 如果沒有火了，清除目標
        }
    }

    // 這個公開方法可以給 FireScript 呼叫，以提升效率
    public void RemoveFireFromList(FireScript fire)
    {
        if (allFires.Contains(fire))
        {
            allFires.Remove(fire);
        }
    }
}