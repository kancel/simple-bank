using System;

namespace SimpleBank
{
    public class Transaction
    {
        public Transaction(TransactionType transactionType, decimal amount, Guid? transferAccountId = null)
        {
            TransactionType = transactionType;
            TransactionDateUTC = DateTime.UtcNow;
            Amount = amount;
            TransferAccountId = transferAccountId;
        }

        public TransactionType TransactionType { get; private set; }
        public DateTime TransactionDateUTC { get; private set; }
        public decimal Amount { get; private set; }
        public Guid? TransferAccountId { get; private set; }
    }

    public enum TransactionType
    {
        Deposit,
        Withdrawl,
        Transfer,
    }
}
