using UnityEngine;

public class Coin : Item
{
    void Update()
    {
        transform.position += Vector3.down * _player.Speed * Time.deltaTime;
    }

    protected override void OnHit(PlayerController player)
    {
        player.PickupCoin();
        Destroy(gameObject);
    }

    protected override void Consume()
    {
        Destroy(gameObject);
    }
}
