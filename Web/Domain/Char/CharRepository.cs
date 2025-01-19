
namespace Web.Domain.Char;

public sealed class CharRepository
{
    private readonly Dictionary<string, Char> _chars = [];

    private long _accUid = 0;

    public CharRepository()
    {
        foreach (var n in Enumerable.Range(1, 10))
        {
            var charId = $"test_{n}";
            _chars.Add(charId, new Char
            {
                CharUid = ++_accUid,
                CharId = charId,
                CharLevel = n,
                CharExp = n * 10
            });
        }
    }

#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
    public async Task<Char?> Get(string charId)
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
    {
        return _chars.GetValueOrDefault(charId);
    }

#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
    public async Task<Dictionary<string, Char>> GetAll()
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
    {
        return _chars;
    }

#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
    public async Task Insert(Char entity)
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
    {
        entity.CharUid = Interlocked.Increment(ref _accUid);
    }

#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
    public async Task Update(Char entity)
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
    {
        _chars.Remove(entity.CharId);
        _chars.Add(entity.CharId, entity);
    }

#pragma warning disable CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
    public async Task Delete(Char entity)
#pragma warning restore CS1998 // 이 비동기 메서드에는 'await' 연산자가 없으며 메서드가 동시에 실행됩니다.
    {
        _chars.Remove(entity.CharId);
    }
}