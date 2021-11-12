using System;
using System.Collections.Generic;

namespace SimpleBank
{
    public class Account
    {
        public Account(Guid ownerId, decimal startingBalance, AccountType accountType)
        {
            AccountId = Guid.NewGuid();
            OwnerId = ownerId;
            Balance = startingBalance;
            AccountType = accountType;
        }
        public Guid AccountId { get; private set; }
        public Guid OwnerId { get; private set; }
        public decimal Balance { get; private set; }
        public AccountType AccountType { get; private set; }
        public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();
        public decimal? AddTransaction(TransactionType transactionType, decimal amount)
        {
            var transaction = new Transaction(transactionType, amount);
            Transactions.Add(transaction);
            Balance += amount;
            return Balance;
        }
    }

    public enum AccountType
    {
        Checking,
        Investment_Individual,
        Investment_Corporate
    }
}
