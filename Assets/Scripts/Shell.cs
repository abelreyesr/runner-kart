using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Shell : Item
{
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _minSideSpeed;
    [SerializeField] private float _maxSideSpeed;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Rigidbody2D _rb;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _bumpSfx;

    private int _dir;
    private float _sideSpeed;
    private float _speed;

    private void Awake()
    {
        _dir = Random.value < 0.5f ? -1 : 1;
        _sideSpeed = Random.Range(_minSideSpeed, _maxSideSpeed);
        _speed = Random.Range(_minSpeed, _maxSpeed);

        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(_dir * _sideSpeed, -(_player.Speed + _speed));
        if (CheckWall())
        {
            _dir = -_dir;
            _audioSource.PlayOneShot(_bumpSfx);
        } 
    }

    private bool CheckWall() => Physics2D.Raycast(transform.position, Vector2.right * _dir, 0.5f, _groundLayer); 

    protected override void OnHit(PlayerController player)
    {
        player.Hit();
        Destroy(gameObject);
    }

    protected override void Consume()
    {
        Destroy(gameObject);
    }
}
