using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;
using SimpleBank;
using System;

namespace SimpleBankTests
{
    [TestClass]
    public class BankTests
    {
        private Faker _faker = new Faker();
        private Bank _bank;
        private Guid _account1Id;
        private Guid _account2Id;
        private Guid _account3Id;
        private Guid _account4Id;
        private decimal _account1StartingBalance = 500;
        private decimal _account2StartingBalance = 3000;
        private decimal _account3StartingBalance = 1000;
        private decimal _account4StartingBalance = 10000;

        [TestInitialize]
        public void TestInitialize()
        {
            _bank = new Bank("The Simple Bank", "Banking made simple... because it has to be :)");
            var accountOwner1Id = _bank.AddAccountOwner("John Deer");
            var accountOwner2Id = _bank.AddAccountOwner("Jane Doe");
            _account1Id = _bank.AddAccount(accountOwner1Id, _account1StartingBalance, AccountType.Checking, out _).Value;
            _account2Id = _bank.AddAccount(accountOwner1Id, _account2StartingBalance, AccountType.Investment_Individual, out _).Value;
            _account3Id = _bank.AddAccount(accountOwner2Id, _account3StartingBalance, AccountType.Checking, out _).Value;
            _account4Id = _bank.AddAccount(accountOwner2Id, _account4StartingBalance, AccountType.Investment_Corporate, out _).Value;
        }

        [TestMethod]
        public void Deposit_ShouldReturnExpected_WhenAccountDoesNotExist()
        {
            //Arrange

            //Act
            var result = _bank.Deposit(Guid.Empty, 100, out string errorMessage);

            //Assert
            result.ShouldBeNull();
            errorMessage.ShouldEqual("Account not found.");
        }

        [TestMethod]
        public void Deposit_ShouldReturnExpected_WhenAccountExists_AndAmountIsNegative()
        {
            //Arrange

            //Act
            var result = _bank.Deposit(_account1Id, -1, out string errorMessage);

            //Assert
            result.ShouldBeNull();
            errorMessage.ShouldEqual("Amount must be a positive decimal.");
        }

        [TestMethod]
        public void Deposit_ShouldReturnExpected_WhenAccountExists_AndAmountIsZero()
        {
            //Arrange

            //Act
            var result = _bank.Deposit(_account1Id, 0, out string errorMessage);

            //Assert
            result.ShouldBeNull();
            errorMessage.ShouldEqual("Amount must be a positive decimal.");
        }

        [TestMethod]
        public void Deposit_ShouldReturnExpected_WhenAccountExists_AndAmountIsPositive()
        {
            //Arrange
            var amount = _faker.Random.Decimal(1, 500);
            var expected = _account1StartingBalance + amount;

            //Act
            var result = _bank.Deposit(_account1Id, amount, out string errorMessage);

            //Assert
            result.ShouldEqual(expected);
            errorMessage.ShouldBeNull();
        }
        [TestMethod]
        public void Withdraw_ShouldReturnExpected_WhenAccountDoesNotExist()
        {
            //Arrange

            //Act
            var result = _bank.Withdraw(Guid.Empty, 100, out string errorMessage);

            //Assert
            result.ShouldBeNull();
            errorMessage.ShouldEqual("Account not found.");
        }

        [TestMethod]
        public void Withdraw_ShouldReturnExpected_WhenAccountExists_AndAmountIsNegative()
        {
            //Arrange

            //Act
            var result = _bank.Withdraw(_account1Id, -1, out string errorMessage);

            //Assert
            result.ShouldBeNull();
            errorMessage.ShouldEqual("Amount must be a positive decimal.");
        }

        [TestMethod]
        public void Withdraw_ShouldReturnExpected_WhenAccountExists_AndAmountIsZero()
        {
            //Arrange

            //Act
            var result = _bank.Withdraw(_account1Id, 0, out string errorMessage);

            //Assert
            result.ShouldBeNull();
            errorMessage.ShouldEqual("Amount must be a positive decimal.");
        }

        [TestMethod]
        public void Withdraw_ShouldReturnExpected_WhenAccountExists_AndAccountTypeIsIndividual_AndAmountIsGreaterThan500()
        {
            //Arrange
            var amount = _faker.Random.Decimal(501, 999);

            //Act
            var result = _bank.Withdraw(_account2Id, amount, out string errorMessage);

            //Assert
            result.ShouldBeNull();
            errorMessage.ShouldEqual("The withdrawal limit for this account is $500.");
        }

        [TestMethod]
        public void Withdraw_ShouldReturnExpected_WhenAccountExists_AndAccountTypeIsIndividual_AndAmountIsEqualTo500()
        {
            //Arrange
            var amount = 500;
            var expected = _account2StartingBalance - amount;

            //Act
            var result = _bank.Withdraw(_account2Id, amount, out string errorMessage);

            //Assert
            result.ShouldEqual(expected);
            errorMessage.ShouldBeNull();
        }

        [TestMethod]
        public void Withdraw_ShouldReturnExpected_WhenAccountExists_AndAccountTypeIsIndividual_AndAmountIsLessThan500()
        {
            //Arrange
            var amount = _faker.Random.Decimal(1, 499);
            var expected = _account2StartingBalance - amount;

            //Act
            var result = _bank.Withdraw(_account2Id, amount, out string errorMessage);

            //Assert
            result.ShouldEqual(expected);
            errorMessage.ShouldBeNull();
        }

        [TestMethod]
        public void Withdraw_ShouldReturnExpected_WhenAccountExists_AndAccountTypeIsNotIndividual_AndAmountIsPositive()
        {
            //Arrange
            var amount = _faker.Random.Decimal(1, 999);
            var expected = _account1StartingBalance - amount;

            //Act
            var result = _bank.Withdraw(_account1Id, amount, out string errorMessage);

            //Assert
            result.ShouldEqual(expected);
            errorMessage.ShouldBeNull();
        }

        [TestMethod]
        public void Transfer_ShouldReturnExpected_WhenWithdrawalAndDepositAccount()
        {
            //Arrange

            //Act
            var result = _bank.Transfer(_account1Id, _account1Id, 100, out decimal? depositAccountBalance, out string errorMessage);

            //Assert
            result.ShouldBeNull();
            depositAccountBalance.ShouldBeNull();
            errorMessage.ShouldEqual("Can't transfer between the same account.");
        }

        [TestMethod]
        public void Transfer_ShouldReturnExpected_WhenWithdrawalAccountDoesNotExist()
        {
            //Arrange

            //Act
            var result = _bank.Transfer(Guid.Empty, _account2Id, 100, out decimal? depositAccountBalance, out string errorMessage);

            //Assert
            result.ShouldBeNull();
            depositAccountBalance.ShouldBeNull();
            errorMessage.ShouldEqual("Withdrawal account not found.");
        }

        [TestMethod]
        public void Transfer_ShouldReturnExpected_WhenWithdrawalAccountExists_AndDepositAccountDoesNotExist()
        {
            //Arrange

            //Act
            var result = _bank.Transfer(_account1Id, Guid.Empty, 100, out decimal? depositAccountBalance, out string errorMessage);

            //Assert
            result.ShouldBeNull();
            depositAccountBalance.ShouldBeNull();
            errorMessage.ShouldEqual("Deposit account not found.");
        }

        [TestMethod]
        public void Transfer_ShouldReturnExpected_WhenBothAccountsExist_AndBothAccountsDoNotHaveSameOwner()
        {
            //Arrange

            //Act
            var result = _bank.Transfer(_account1Id, _account3Id, 100, out decimal? depositAccountBalance, out string errorMessage);

            //Assert
            result.ShouldBeNull();
            depositAccountBalance.ShouldBeNull();
            errorMessage.ShouldEqual("Accounts must have the same owner.");
        }

        [TestMethod]
        public void Transfer_ShouldReturnExpected_WhenBothAccountsExistAndHaveSameOwner_AndAmountIsNegative()
        {
            //Arrange

            //Act
            var result = _bank.Transfer(_account1Id, _account2Id, -1, out decimal? depositAccountBalance, out string errorMessage);

            //Assert
            result.ShouldBeNull();
            depositAccountBalance.ShouldBeNull();
            errorMessage.ShouldEqual("Amount must be a positive decimal.");
        }

        [TestMethod]
        public void Transfer_ShouldReturnExpected_WhenBothAccountsExistAndHaveSameOwner_AndAmountIsZero()
        {
            //Arrange

            //Act
            var result = _bank.Transfer(_account1Id, _account2Id, 0, out decimal? depositAccountBalance, out string errorMessage);

            //Assert
            result.ShouldBeNull();
            depositAccountBalance.ShouldBeNull();
            errorMessage.ShouldEqual("Amount must be a positive decimal.");
        }

        [TestMethod]
        public void Transfer_ShouldReturnExpected_WhenBothAccountsExistsAndHaveSameOwner_AndAccountTypeIsIndividual_AndAmountIsGreaterThan500()
        {
            //Arrange
            var amount = _faker.Random.Decimal(501, 999);

            //Act
            var result = _bank.Transfer(_account2Id, _account1Id, amount, out decimal? depositAccountBalance, out string errorMessage);

            //Assert
            result.ShouldBeNull();
            depositAccountBalance.ShouldBeNull();
            errorMessage.ShouldEqual("The withdrawal limit for this account is $500.");
        }

        [TestMethod]
        public void Transfer_ShouldReturnExpected_WhenBothAccountsExistsAndHaveSameOwner_AndAccountTypeIsIndividual_AndAmountIsEqualTo500()
        {
            //Arrange
            var amount = 500;
            var expected = _account2StartingBalance - amount;
            var expectedDepositAccountBalance = _account1StartingBalance + amount;

            //Act
            var result = _bank.Transfer(_account2Id, _account1Id, amount, out decimal? depositAccountBalance, out string errorMessage);

            //Assert
            result.ShouldEqual(expected);
            depositAccountBalance.ShouldEqual(expectedDepositAccountBalance);
            errorMessage.ShouldBeNull();
        }

        [TestMethod]
        public void Transfer_ShouldReturnExpected_WhenBothAccountsExistsAndHaveSameOwner_AndAccountTypeIsIndividual_AndAmountIsLessThan500()
        {
            //Arrange
            var amount = _faker.Random.Decimal(1, 499);
            var expected = _account2StartingBalance - amount;
            var expectedDepositAccountBalance = _account1StartingBalance + amount;

            //Act
            var result = _bank.Transfer(_account2Id, _account1Id, amount, out decimal? depositAccountBalance, out string errorMessage);

            //Assert
            result.ShouldEqual(expected);
            depositAccountBalance.ShouldEqual(expectedDepositAccountBalance);
            errorMessage.ShouldBeNull();
        }

        [TestMethod]
        public void Transfer_ShouldReturnExpected_WhenBothAccountsExistsAndHaveSameOwner_AndAccountTypeIsNotIndividual_AndAmountIsPositive()
        {
            //Arrange
            var amount = _faker.Random.Decimal(1, 999);
            var expected = _account1StartingBalance - amount;
            var expectedDepositAccountBalance = _account2StartingBalance + amount;

            //Act
            var result = _bank.Transfer(_account1Id, _account2Id, amount, out decimal? depositAccountBalance, out string errorMessage);

            //Assert
            result.ShouldEqual(expected);
            depositAccountBalance.ShouldEqual(expectedDepositAccountBalance);
            errorMessage.ShouldBeNull();
        }

    }
}
