namespace SinistrosApi.Domain.Comum;

public class Resultado
{
    public bool EhSucesso { get; }
    public bool EhFalha => !EhSucesso;
    public string? Erro { get; }
    public TipoErro TipoErro { get; }

    protected Resultado(bool ehSucesso, string? erro, TipoErro errorType)
    {
        EhSucesso = ehSucesso;
        Erro = erro;
        TipoErro = errorType;
    }

    public static Resultado Sucesso() => new(true, null, TipoErro.Nenhum);
    public static Resultado Falha(string erro, TipoErro tipo) => new(false, erro, tipo);
}

public class Resultado<T> : Resultado
{
    public T Valor { get; } = default!;

    private Resultado(T valor) : base(true, null, TipoErro.Nenhum) => Valor = valor;
    private Resultado(string erro, TipoErro tipo) : base(false, erro, tipo) { }

    public static Resultado<T> Sucesso(T valor) => new(valor);
    public static new Resultado<T> Falha(string erro, TipoErro tipo) => new(erro, tipo);
}
