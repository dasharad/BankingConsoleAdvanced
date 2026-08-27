namespace BankingConsoleAdvanced.Models;

public abstract class Account
{
    protected Account(
        string accountNumber,
        Customer customer,
        decimal openingBalance)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("Account number is required.", nameof(accountNumber));

        if (openingBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(openingBalance));

        AccountNumber = accountNumber;
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        Balance = openingBalance;
        IsActive = true;
    }

    public string AccountNumber { get; }
    public Customer Customer { get; }
    public decimal Balance { get; private set; }
    public bool IsActive { get; private set; }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Deposit must be greater than zero.");

        EnsureActive();
        Balance += amount;
    }

    public virtual void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Withdrawal must be greater than zero.");

        EnsureActive();

        if (amount > Balance)
            throw new InsufficientBalanceException(AccountNumber, Balance, amount);

        ChangeBalance(-amount);
    }

    public void Deactivate() => IsActive = false;

    protected void ChangeBalance(decimal delta) => Balance += delta;

    protected void EnsureActive()
    {
        if (!IsActive)
            throw new InvalidOperationException($"Account {AccountNumber} is inactive.");
    }

    public override string ToString() =>
        $"{AccountNumber,-8} | {Customer.Name,-10} | {GetType().Name,-16} | ₹{Balance,12:N2} | Active={IsActive}";
}

public sealed class SavingsAccount(
    string accountNumber,
    Customer customer,
    decimal openingBalance,
    decimal interestRate)
    : Account(accountNumber, customer, openingBalance)
{
    public decimal InterestRate { get; } = interestRate;
}

public sealed class CurrentAccount(
    string accountNumber,
    Customer customer,
    decimal openingBalance,
    decimal overdraftLimit)
    : Account(accountNumber, customer, openingBalance)
{
    public decimal OverdraftLimit { get; } = overdraftLimit;

    public override void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        if (amount > Balance + OverdraftLimit)
            throw new InsufficientBalanceException(AccountNumber, Balance + OverdraftLimit, amount);

        EnsureActive();

        ChangeBalance(-amount);
    }
}
