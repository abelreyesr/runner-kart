using TMPro;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverText;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private string _hpSpriteName;
    [SerializeField] private PlayerController _player;

    void OnEnable()
    {
        _player.OnCoinUpdate += UpdateCoins;
        _player.OnHpUpdate += UpdateHp;
        _player.OnGameOver += GameOver;
    }

    void OnDisable()
    {
        _player.OnCoinUpdate -= UpdateCoins;
        _player.OnHpUpdate -= UpdateHp;
        _player.OnGameOver -= GameOver;
    }

    private void UpdateCoins(int coins)
    {
        _coinText.text = coins.ToString();
    }

    private void UpdateHp(int hp)
    {
        string txt = "";

        for(int i = 0; i < hp; i++)
        {
            txt += $"<sprite name=\"{_hpSpriteName}\">";
        }

        _hpText.text = txt;
    }

    private void GameOver()
    {
        _gameOverText.SetActive(true);
    }
}
