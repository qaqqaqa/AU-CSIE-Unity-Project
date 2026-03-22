// 範例：DoorTriggerHandler.cs (附加在門的觸發器物件上)
using UnityEngine;

public class DoorTriggerHandler : MonoBehaviour
{
    private OpenDown doorScript; // 保存門的 OpenDown 腳本參考

    void Awake() // 或者 Start()
    {
        // 獲取父物件或自身物件上的 OpenDown 腳本參考
        // 假設 OpenDown 腳本附加在觸發器的父物件上
        doorScript = GetComponentInParent<OpenDown>();
        if (doorScript == null)
        {
            Debug.LogError("DoorTriggerHandler: OpenDown script not found on parent object!", this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 檢查進入觸發器的物件是否是玩家 (例如，通過標籤或圖層)
        if (other.CompareTag("Player")) // 假設玩家物件有 "Player" 標籤
        {
            Debug.Log("Player entered door trigger.");
            // 在玩家腳本中保存這個 doorScript 的參考，或者讓玩家腳本知道可以與這個門互動
            // 這部分的邏輯需要在玩家腳本中實現
            // 例如: other.GetComponent<PlayerInteraction>().SetInteractiveDoor(doorScript);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 檢查離開觸發器的物件是否是玩家
        if (other.CompareTag("Player")) // 假設玩家物件有 "Player" 標籤
        {
            Debug.Log("Player exited door trigger.");
            // 在玩家腳本中清除對這個門的參考
            // 例如: other.GetComponent<PlayerInteraction>().ClearInteractiveDoor(doorScript);
        }
    }

    // 提供一個公開方法讓玩家腳本在需要時呼叫 Switch()
    public void InteractWithDoor()
    {
        if (doorScript != null)
        {
            doorScript.Switch();
        }
    }
}

// 範例：PlayerInteraction.cs (附加在玩家物件上)

