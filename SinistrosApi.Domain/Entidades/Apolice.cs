using System.Diagnostics.CodeAnalysis;
using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Enumeradores;

namespace SinistrosApi.Domain.Entidades;

public class Apolice : EntidadeBase
{
    public string NumeroApolice { get; private set => field = value.Trim(); } = null!;
    public Guid ClienteId { get; private set; }
    public Ramo Ramo { get; private set; }
    public StatusApolice Status { get; private set; }
    public DateOnly DataInicioVigencia { get; private set; }
    public DateOnly DataFimVigencia { get; private set; }

    [ExcludeFromCodeCoverage]
    protected Apolice() { }

    private Apolice(string numeroApolice, Guid clienteId, Ramo ramo, DateOnly dataInicioVigencia, DateOnly dataFimVigencia)
    {
        NumeroApolice = numeroApolice;
        ClienteId = clienteId;
        Ramo = ramo;
        Status = StatusApolice.Ativa;
        DataInicioVigencia = dataInicioVigencia;
        DataFimVigencia = dataFimVigencia;
    }

    public static Resultado<Apolice> Criar(string numeroApolice, Guid clienteId, Ramo ramo, DateOnly dataInicioVigencia, DateOnly dataFimVigencia)
    {
        if (dataInicioVigencia >= dataFimVigencia)
            return Resultado<Apolice>.Falha("Data de início da vigência deve ser anterior à data de fim.", TipoErro.RegraDeNegocio);

        return Resultado<Apolice>.Sucesso(new Apolice(numeroApolice, clienteId, ramo, dataInicioVigencia, dataFimVigencia));
    }

    public Resultado Vencer()
    {
        if (!EstaAtiva())
            return Resultado.Falha("Apólice não pode ser vencida pois não está ativa.", TipoErro.RegraDeNegocio);

        Status = StatusApolice.Vencida;
        return Resultado.Sucesso();
    }

    public Resultado Inativar()
    {
        if (!EstaAtiva())
            return Resultado.Falha("Apólice não pode ser inativada pois não está ativa.", TipoErro.RegraDeNegocio);

        Status = StatusApolice.Inativa;
        return Resultado.Sucesso();
    }

    public bool EstaVigenteEAtiva(DateOnly dataAtual)
    {
        return EstaAtiva()
            && DataInicioVigencia <= dataAtual
            && dataAtual <= DataFimVigencia;
    }

    private bool EstaAtiva() => Status == StatusApolice.Ativa;
}
