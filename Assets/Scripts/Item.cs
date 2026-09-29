using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected PlayerController _player;

    public void Init(PlayerController player)
    {
        _player = player;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == _player.gameObject)
        {
            OnHit(_player);
        }
    }

    protected abstract void OnHit(PlayerController player);
    protected abstract void Consume();
}
