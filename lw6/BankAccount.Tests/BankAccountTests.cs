using System;
using BankAccount;
using Xunit;

namespace BankAccount.Tests;

public class BankAccountTests
{
    [Fact]
    public void Create_default_accaunt()
    {
        var acc = new BankAccount();
        Assert.Equal(0, acc.Balance);
        Assert.Equal("STANDARD", acc.Tier);
        Assert.False(acc.IsFrozen);
    }

    [Fact]
    public void Create_custom_account()
    {
        var acc = new BankAccount(1000, "GOLD");
        Assert.Equal(1000, acc.Balance);
        Assert.Equal("GOLD", acc.Tier);
    }


    [Fact]
    public void Deposit_amount_to_account()
    {
        var acc = new BankAccount();
        acc.Deposit(500);
        Assert.Equal(500, acc.Balance);
    }

    [Fact]
    public void Deposit_amount_to_frozen_account()
    {
        var acc = new BankAccount();
        acc.FreezeAccount();
        Assert.Throws<InvalidOperationException>(() => acc.Deposit(100));
    }

    [Fact]
    public void Deposit_nonpositive_amount_to_account()
    {
        var acc = new BankAccount();
        Assert.Throws<ArgumentException>(() => acc.Deposit(-50));
    }


    [Fact]
    public void Withdraw_amount_less_than_balance()
    {
        var acc = new BankAccount(500);
        acc.Withdraw(200);
        Assert.Equal(300, acc.Balance);
    }

    [Fact]
    public void Withdraw_amount_greater_than_balance()
    {
        var acc = new BankAccount(100);
        Assert.Throws<InvalidOperationException>(() => acc.Withdraw(150));
    }

    [Fact]
    public void Withdraw_using_gold_tier_overdraft()
    {
        var acc = new BankAccount(100, "GOLD");
        acc.Withdraw(500);
        Assert.Equal(-405, acc.Balance);
    }

    [Fact]
    public void Withdraw_using_platinum_tier_overdraft()
    {
        var acc = new BankAccount(100, "PLATINUM");
        acc.Withdraw(1500);
        Assert.Equal(-1400, acc.Balance);
    }

    [Fact]
    public void Withdraw_when_remaining_balance_is_below_threshold()
    {
        var acc = new BankAccount(150);
        acc.Withdraw(100);
        Assert.Equal(45, acc.Balance);
    }

    [Fact]
    public void Withdraw_when_remaining_balance_exceeds_threshold()
    {
        var acc = new BankAccount(200);
        acc.Withdraw(100);
        Assert.Equal(100, acc.Balance);
    }

    [Fact]
    public void Withdraw_from_platinum_tier_account_without_fee()
    {
        var acc = new BankAccount(150, "PLATINUM");
        acc.Withdraw(100);
        Assert.Equal(50, acc.Balance);
    }

    [Fact]
    public void Withdraw_from_frozen_accounts()
    {
        var acc = new BankAccount(500);
        acc.FreezeAccount();
        Assert.Throws<InvalidOperationException>(() => acc.Withdraw(100));
    }

    [Fact]
    public void Withdraw_nonpositive_amount()
    {
        var acc = new BankAccount(500);
        Assert.Throws<ArgumentException>(() => acc.Withdraw(0));
        Assert.Throws<ArgumentException>(() => acc.Withdraw(-100));
    }


    [Fact]
    public void Upgrade_tier_to_gold_with_welcome_bonus()
    {
        var acc = new BankAccount(5000);
        acc.UpgradeTier("GOLD");
        Assert.Equal("GOLD", acc.Tier);
        Assert.Equal(5050, acc.Balance);
    }

    [Fact]
    public void Upgrade_tier_to_platinum_with_welcome_bonus()
    {
        var acc = new BankAccount(20000);
        acc.UpgradeTier("PLATINUM");
        Assert.Equal("PLATINUM", acc.Tier);
        Assert.Equal(20200, acc.Balance);
    }

    [Fact]
    public void Upgrade_tier_without_necessery_amount_in_account()
    {
        var acc = new BankAccount(4999);
        Assert.Throws<InvalidOperationException>(() => acc.UpgradeTier("GOLD"));
    }

    [Fact]
    public void Upgrade_to_non_existent_tier()
    {
        var acc = new BankAccount(10000);
        Assert.Throws<ArgumentException>(() => acc.UpgradeTier("DIAMOND"));
    }

    [Fact]
    public void Upgrade_tier_to_frozen_account()
    {
        var acc = new BankAccount(10000);
        acc.FreezeAccount();
        Assert.Throws<InvalidOperationException>(() => acc.UpgradeTier("GOLD"));
    }


    [Fact]
    public void Freeze_account()
    {
        var acc = new BankAccount();
        acc.FreezeAccount();
        Assert.True(acc.IsFrozen);
    }

    [Fact]
    public void Unfreeze_account_with_correct_pin()
    {
        var acc = new BankAccount();
        acc.FreezeAccount();
        acc.UnfreezeAccount("1234");
        Assert.False(acc.IsFrozen);
    }

    [Fact]
    public void Unfreeze_account_with_incorrect_pin()
    {
        var acc = new BankAccount();
        acc.FreezeAccount();
        Assert.Throws<ArgumentException>(() => acc.UnfreezeAccount("0000"));
    }


    [Fact]
    public void Get_available_funds_from_gold_tier_account_includes_overdraft()
    {
        var acc = new BankAccount(1000, "GOLD");
        Assert.Equal(1500, acc.GetAvailableFunds());
    }
}

