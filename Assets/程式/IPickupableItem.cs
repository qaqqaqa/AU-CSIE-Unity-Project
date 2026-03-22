// IPickupableItem.cs
public interface IPickupableItem
{
    void OnPickup(); // 當道具被玩家拾起時呼叫
    void OnDrop();   // 當道具被玩家放下時呼叫
}