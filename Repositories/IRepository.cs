namespace BankingConsoleAdvanced.Repositories;

public interface IRepository<T> where T : class
{
    void Add(T item);
    T? FirstOrDefault(Func<T, bool> predicate);
    IReadOnlyCollection<T> GetAll();
    int Count { get; }
}
