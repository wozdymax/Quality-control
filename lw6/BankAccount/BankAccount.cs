using System;
using System.Collections.Generic;

namespace BankAccount;

public class BankAccount
{
    private static readonly Dictionary<string, decimal> OverdraftLimits = new()
    {
        ["STANDARD"] = 0,
        ["GOLD"] = 500,
        ["PLATINUM"] = 2000
    };

    private static readonly Dictionary<string, decimal> TierRequirements = new()
    {
        ["GOLD"] = 5000,
        ["PLATINUM"] = 20000
    };

    private const decimal LowBalanceThreshold = 100;
    private const decimal LowBalanceFee = 5;
    private const string AdminPin = "1234";

    public decimal Balance { get; private set; }
    public string Tier { get; private set; }
    public bool IsFrozen { get; private set; }

    public BankAccount(decimal initialBalance = 0, string tier = "STANDARD")
    {
        Balance = initialBalance;
        Tier = tier;
        IsFrozen = false;
    }

    public void Deposit(decimal amount)
    {
        if (IsFrozen)
            throw new InvalidOperationException("Операции по счету заблокированы");
        if (amount <= 0)
            throw new ArgumentException("Сумма пополнения должна быть больше 0");

        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (IsFrozen)
            throw new InvalidOperationException("Операции по счету заблокированы");
        if (amount <= 0)
            throw new ArgumentException("Сумма снятия должна быть больше 0");

        decimal limit = OverdraftLimits[Tier];

        if (Balance - amount < -limit)
            throw new InvalidOperationException("Превышен лимит овердрафта");

        Balance -= amount;

        if (Tier != "PLATINUM" && Balance < LowBalanceThreshold)
        {
            Balance -= LowBalanceFee;
        }
    }

    public void UpgradeTier(string newTier)
    {
        if (IsFrozen)
            throw new InvalidOperationException("Операции по счету заблокированы");
        if (!TierRequirements.ContainsKey(newTier))
            throw new ArgumentException("Неизвестный уровень обслуживания");

        decimal required = TierRequirements[newTier];
        if (Balance < required)
            throw new InvalidOperationException($"Недостаточно средств. Требуется {required}");

        Tier = newTier;
        decimal welcomeBonus = newTier == "GOLD" ? 50 : 200;
        Balance += welcomeBonus;
    }

    public void FreezeAccount()
    {
        IsFrozen = true;
    }

    public void UnfreezeAccount(string pin)
    {
        if (pin != AdminPin)
            throw new ArgumentException("Неверный PIN-код администратора");
        IsFrozen = false;
    }

    public decimal GetAvailableFunds()
    {
        decimal limit = OverdraftLimits[Tier];
        return Balance + limit;
    }
}