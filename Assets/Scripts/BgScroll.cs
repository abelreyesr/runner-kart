using UnityEngine;

public class BgScroll : MonoBehaviour
{

    [SerializeField] private float _yMin;
    [SerializeField] private Transform transform1;
    [SerializeField] private Transform transform2;
    [SerializeField] private PlayerController _player;

    private float _scrollSpeed;

    void Update()
    {
        _scrollSpeed = _player.Speed;

        Scroll(transform1);
        Scroll(transform2);
    }

    private void Scroll(Transform t)
    {
        t.position+= -t.up * _scrollSpeed * Time.deltaTime;

        if (t.position.y <= _yMin) t.position += -t.up * _yMin*2;
    }
}
