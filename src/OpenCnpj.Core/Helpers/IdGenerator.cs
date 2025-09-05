namespace OpenCnpj.Core.Helpers;

public class IdGenerator
{
    private long _currentId;

    public IdGenerator(long start = 0)
    {
        _currentId = start;
    }

    /// <summary>
    /// Retorna o próximo Id disponível (incremental).
    /// </summary>
    public long NextId()
    {
        return Interlocked.Increment(ref _currentId);
    }

    /// <summary>
    /// Ajusta o contador interno se o valor informado for maior que o atual.
    /// Isso garante que novos Ids não colidam com registros já existentes no banco.
    /// </summary>
    public void SetIfGreater(long value)
    {
        long initialValue, computed;
        do
        {
            initialValue = _currentId;
            if (value <= initialValue)
                return; // não precisa ajustar

            computed = value;
        }
        while (Interlocked.CompareExchange(ref _currentId, computed, initialValue) != initialValue);
    }

    /// <summary>
    /// Retorna o valor atual do contador.
    /// </summary>
    public long Current => Volatile.Read(ref _currentId);
}