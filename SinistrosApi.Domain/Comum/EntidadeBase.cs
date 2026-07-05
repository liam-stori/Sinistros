namespace SinistrosApi.Domain.Comum;

public abstract class EntidadeBase
{
    public Guid Id { get; protected set; }

    protected EntidadeBase()
    {
        Id = Guid.NewGuid();
    }
}
