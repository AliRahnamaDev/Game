using UnityEngine;

public interface IRespawnable
{
    void OnDeath();
    bool IsDead { get; }
    GameObject GetGameObject(); // برای گرفتن موقعیت یا Destroy/Respawn
}