using UnityEngine;

public class SpriteSteerAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private PlayerController _player;

    void Awake()
    {
        if (_player == null) _player = GetComponent<PlayerController>();
    }

    void LateUpdate()
    {
        if (_player.Stunned) return;

        // valor entre 0 i 1 depenent a on gira el jugador
        float angle = Mathf.InverseLerp(-_player.MaxSteeringSpeed, _player.MaxSteeringSpeed, _player.SteeringSpeed);
        angle *= _sprites.Length -1 ;

        int facing = Mathf.RoundToInt(angle);
        _renderer.sprite = _sprites[facing];
    }
}
