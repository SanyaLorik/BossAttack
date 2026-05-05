using MediaKit_M.SkinChanger;
using UnityEngine;
using Zenject;

public class PlayerBankAdapter : PurchaseAdapter
{
    [Inject] private PlayerBank _playerBank;

    public override bool CanSpend(int money)
    {
        return _playerBank.CanBuy(money);
    }

    public override void Spend(int money)
    {
        _playerBank.SpendMoney(money);
    }
}