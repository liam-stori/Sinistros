using SinistrosApi.Domain.Comum;

namespace SinistrosApi.Domain.ObjetosDeValor;

public sealed class NomeCompleto
{
    public string Nome { get; }
    public string Sobrenome { get; }

    private NomeCompleto(string nome, string sobrenome)
    {
        Nome = nome;
        Sobrenome = sobrenome;
    }

    public static Resultado<NomeCompleto> Criar(string nome, string sobrenome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Resultado<NomeCompleto>.Falha("Nome é obrigatório.", TipoErro.Validacao);

        if (string.IsNullOrWhiteSpace(sobrenome))
            return Resultado<NomeCompleto>.Falha("Sobrenome é obrigatório.", TipoErro.Validacao);

        return Resultado<NomeCompleto>.Sucesso(new NomeCompleto(nome.Trim(), sobrenome.Trim()));
    }

    public override string ToString() => $"{Nome} {Sobrenome}";

    public override bool Equals(object? obj)
        => obj is NomeCompleto outro && Nome == outro.Nome && Sobrenome == outro.Sobrenome;

    public override int GetHashCode() => HashCode.Combine(Nome, Sobrenome);
}
