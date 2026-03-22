// ItemComponent.cs
using UnityEngine;

// 此腳本用於附加到所有可拾取道具的 Prefab 或 GameObject 上
// 讓 PlayerController 知道這是一個道具，並存儲其 ItemData
public class ItemComponent : MonoBehaviour
{
    public ItemData itemData; // 儲存這個道具的 ItemData (Scriptable Object)

    // 您可以在這裡添加其他與道具行為相關的公共方法或屬性
    // 例如：
    // public void Initialize(ItemData data) {
    //     itemData = data;
    //     // 根據 itemData 設定模型、材質等
    // }
}
