using UnityEngine;

public class Banana : Item
{


    void Update()
    {
        transform.position += Vector3.down * _player.Speed * Time.deltaTime;
    }

    protected override void OnHit(PlayerController player)
    {
        player.Hit();
        Consume();
    }

    protected override void Consume()
    {
        Destroy(gameObject);
    }
}
