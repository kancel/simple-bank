using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleBank
{
    public class Bank
    {
        public Bank(string name, string tagline)
        {
            Name = name;
            Tagline = tagline;
        }
        public string Name { get; private set; }
        public string Tagline { get; private set; }
        public ICollection<Account> Accounts { get; private set; } = new List<Account>();
        public ICollection<AccountOwner> AccountOwners { get; private set; } = new List<AccountOwner>();

        public Guid AddAccountOwner(string name)
        {
            var accountOwner = new AccountOwner(name);
            AccountOwners.Add(accountOwner);
            return accountOwner.AccountOwnerId;
        }
        public Guid? AddAccount(Guid ownerId, decimal startingBalance, AccountType accountType, out string errorMessage)
        {
            if (!AccountOwners.Any(x => x.AccountOwnerId == ownerId))
            {
                errorMessage = "Owner not found.";
                return null;
            }
            if (startingBalance < 0)
            {
                errorMessage = "Starting balance must be a nonnegative decimal.";
                return null;
            }
            var account = new Account(ownerId, startingBalance, accountType);
            Accounts.Add(account);
            errorMessage = null;
            return account.AccountId;
        }

        public decimal? Deposit(Guid accountId, decimal amount, out string errorMessage)
        {

            var account = Accounts.SingleOrDefault(x => x.AccountId == accountId);
            if (account == null)
            {
                errorMessage = "Account not found.";
                return null;
            }
            if (amount <= 0)
            {
                errorMessage = "Amount must be a positive decimal.";
                return null;
            }
            errorMessage = null;
            return account.AddTransaction(TransactionType.Deposit, amount);
        }

        public decimal? Withdraw(Guid accountId, decimal amount, out string errorMessage)
        {
            var account = Accounts.SingleOrDefault(x => x.AccountId == accountId);
            if (account == null)
            {
                errorMessage = "Account not found.";
                return null;
            }
            if (amount <= 0)
            {
                errorMessage = "Amount must be a positive decimal.";
                return null;
            }
            if (account.AccountType == AccountType.Investment_Individual && amount > 500)
            {
                errorMessage = "The withdrawal limit for this account is $500.";
                return null;
            }
            errorMessage = null;
            return account.AddTransaction(TransactionType.Withdrawl, -amount);
        }

        public decimal? Transfer(Guid withdrawalAccountId, Guid depositAccountId, decimal amount, out decimal? depositAccountBalance, out string errorMessage)
        {
            if (withdrawalAccountId == depositAccountId)
            {
                errorMessage = "Can't transfer between the same account.";
                depositAccountBalance = null;
                return null;
            }
            var withdrawalAccount = Accounts.SingleOrDefault(x => x.AccountId == withdrawalAccountId);
            if (withdrawalAccount == null)
            {
                errorMessage = "Withdrawal account not found.";
                depositAccountBalance = null;
                return null;
            }
            var depositAccount = Accounts.SingleOrDefault(x => x.AccountId == depositAccountId);
            if (depositAccount == null)
            {
                errorMessage = "Deposit account not found.";
                depositAccountBalance = null;
                return null;
            }
            if (withdrawalAccount.OwnerId != depositAccount.OwnerId)
            {
                errorMessage = "Accounts must have the same owner.";
                depositAccountBalance = null;
                return null;
            }
            if (amount <= 0)
            {
                errorMessage = "Amount must be a positive decimal.";
                depositAccountBalance = null;
                return null;
            }
            if (withdrawalAccount.AccountType == AccountType.Investment_Individual && amount > 500)
            {
                errorMessage = "The withdrawal limit for this account is $500.";
                depositAccountBalance = null;
                return null;
            }
            var withdrawalAccountBalance = withdrawalAccount.AddTransaction(TransactionType.Transfer, -amount);
            depositAccountBalance = depositAccount.AddTransaction(TransactionType.Transfer, amount);
            errorMessage = null;
            return withdrawalAccountBalance;
        }
    }
}
