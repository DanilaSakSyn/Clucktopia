using System;
using UnityEngine;

namespace Projects
{
    public class PlayerCurrency : MonoBehaviour
    {
        [SerializeField] private string currencyName = "Coins";

        public string CurrencyName
        {
            get => currencyName;
            set => currencyName = value;
        }


        public int CurrentMoney => Wallet.Instance.CurrentMoney;


        public bool SpendMoney(int amount) => Wallet.Instance.SpendMoney(amount);

        public void AddMoney(int amount) => Wallet.Instance.AddMoney(amount);
    }
}