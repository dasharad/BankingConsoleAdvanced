namespace BankingConsoleAdvanced.Repositories;

public sealed class InMemoryRepository<T> : IRepository<T> where T : class
{
    private readonly List<T> _items = [];

    public int Count => _items.Count;

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
    }

    public T? FirstOrDefault(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _items.FirstOrDefault(predicate);
    }

    public IReadOnlyCollection<T> GetAll() => _items.AsReadOnly();
}
