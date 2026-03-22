// InteractableItem.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractableItem : MonoBehaviour
{
    public ItemData itemData; // 這個物件對應的道具資料

    // 當被成功撿起後，可以呼叫此方法將其從場景中移除
    public void NotifyPickup()
    {
        Debug.Log($"玩家撿起了 {itemData.itemName}");
        Destroy(gameObject);
    }
}
