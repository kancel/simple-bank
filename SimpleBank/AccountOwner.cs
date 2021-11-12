using System;

namespace SimpleBank
{
    public class AccountOwner
    {
        public AccountOwner(string name)
        {
            AccountOwnerId = Guid.NewGuid();
            Name = name;
        }
        public Guid AccountOwnerId { get; private set; }
        public string Name { get; private set; }
    }
}
