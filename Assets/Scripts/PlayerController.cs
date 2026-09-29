using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int _maxHp = 3;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxSteerSpeed;
    [SerializeField] private float _steerAcceleration;
    [SerializeField] private float _stunDuration;
    [SerializeField] private float _wallBounceMinSpeed;
    [Range(0f, 1f)] [SerializeField] private float _wallBounceMult;

    [Header("Visuals")]
    [SerializeField] private float _minAnimSpeed;
    [SerializeField] private float _maxAnimSpeed;
    [SerializeField] private ParticleSystem _explosionParticles;
    [SerializeField] private ParticleSystem _bigExplosionParticles;
    [SerializeField] private ParticleSystem _smokeHitParticles;

    [Header("Refs")]
    [SerializeField] private InputActionReference _movementInputAction;
    [SerializeField] private InputActionReference _restartInputAction;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Collider2D _collider;

    private int _hp;
    private bool _isDead = false;
    private float _movementInput;
    private  float _speed;
    private float _lastVelX;
    public bool IsDead => _isDead;
    public float Speed => _speed;
    public float MaxSpeed => _maxSpeed;
    public float SteeringSpeed => _rb.linearVelocityX;
    public float MaxSteeringSpeed => _maxSteerSpeed;

    private bool _stunned = false;
    private float _stunTime;
    public bool Stunned => _stunned;

    private int _coins = 0;

    public Action<int> OnCoinUpdate;
    public Action<int> OnHpUpdate;
    public Action OnGameOver;

    void Awake()
    {
        _hp = _maxHp;
        OnHpUpdate?.Invoke(_hp);
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        if (_animator == null) _animator = GetComponent<Animator>();
    }

    private void OnEnable() => _restartInputAction.action.performed += TryRestart;
    private void OnDisable() => _restartInputAction.action.performed -= TryRestart;

    void Update()
    {
        HandleStun();
        ProcessInput();
        HandleMovement();
        HandleSteering();
        SetAnimationSpeed();

        _lastVelX = _rb.linearVelocityX;
    }

    private void SetAnimationSpeed()
    {
        if (_stunned)
        {
            _animator.speed = 1f;
            return;
        }

        float t = _speed / _maxSpeed;
        float animSpeed = Mathf.Lerp(_minAnimSpeed, _maxAnimSpeed, t);

        _animator.speed = animSpeed;
    }

    private void HandleStun()
    {
        if (_stunTime < Time.time) _stunned = false;

        _animator.SetBool("stunned", _stunned);
    }

    private void ProcessInput()
    {
        if (_stunned)
        {
            _movementInput = 0;
            return;
        }

        _movementInput = _movementInputAction.action.ReadValue<float>();
    }

    private void HandleMovement()
    {
        if(_speed < _maxSpeed && !_stunned)
        {
            _speed = Mathf.Min(_maxSpeed, _speed + _acceleration * Time.deltaTime);
        }
    }

    private void HandleSteering()
    {
        float curVel = _rb.linearVelocityX;
        float targetVel = _maxSteerSpeed * _movementInput;

        float newVel = Mathf.MoveTowards(curVel, targetVel, _steerAcceleration * Time.deltaTime);

        _rb.linearVelocity = new Vector2(newVel, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if(Mathf.Abs(_lastVelX) > _wallBounceMinSpeed) WallBounce();
        }
    }

    private void WallBounce()
    {
        _rb.linearVelocity = new Vector2(-_lastVelX * _wallBounceMult, _rb.linearVelocityY);
    }

    public void Hit()
    {
        if (_stunned || _isDead) return;

        _hp--;
        OnHpUpdate?.Invoke(_hp);

        _smokeHitParticles.Play();

        if(_hp <= 0)
        {
            _isDead = true;
            StartCoroutine(DeathCoroutine());
        }
        else
        {
            _stunned = true;
            _stunTime = Time.time + _stunDuration;

            _speed *= 0.2f;
        }
    }

    private IEnumerator DeathCoroutine()
    {
        _stunned = true;
        _stunTime = Time.time + float.PositiveInfinity;
        _speed *= 0.2f;

        _explosionParticles.Play();
        var emission = _explosionParticles.emission;
        float defaultRate = emission.rateOverTime.constant;

        float rate = defaultRate;
        float growth = 2f;

        // que vayan subiendo las explosiones hasta que acabe
        while (rate < defaultRate * 20f)
        {
            rate *= Mathf.Pow(growth, Time.deltaTime);

            emission.rateOverTime = rate;

            yield return null;
        }

        _explosionParticles.Stop();
        _bigExplosionParticles.Play();
        yield return new WaitForSeconds(0.1f);
        _sprite.enabled = false;
        _collider.enabled = false;
        yield return new WaitForSeconds(2f);
        OnGameOver?.Invoke();
    }

    public void PickupCoin()
    {
        _coins++;
        OnCoinUpdate?.Invoke(_coins);
    }

    private void TryRestart(InputAction.CallbackContext ctx)
    {
        if (!_isDead) return; 
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
