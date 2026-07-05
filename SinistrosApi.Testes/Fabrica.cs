using SinistrosApi.Domain.Entidades;
using SinistrosApi.Domain.Enumeradores;

namespace SinistrosApi.Testes;

internal static class Fabrica
{
    private static readonly DateOnly HojeUtc = DateOnly.FromDateTime(DateTime.UtcNow);

    internal static Apolice ApoliceAtiva(Ramo ramo = Ramo.Auto)
    {
        return Apolice.Criar(
            "APL-TESTE",
            Guid.NewGuid(),
            ramo,
            HojeUtc.AddYears(-1),
            HojeUtc.AddYears(1)
        ).Valor;
    }

    internal static Sinistro SinistroAberto(Apolice? apolice = null, decimal valor = 5000m)
    {
        apolice ??= ApoliceAtiva();
        return Sinistro.Abrir(apolice, valor, DateTime.UtcNow.AddDays(-5)).Valor;
    }

    internal static Sinistro SinistroEmAnalise(Apolice? apolice = null)
    {
        var sinistro = SinistroAberto(apolice);
        sinistro.AtualizarStatus(StatusSinistro.EmAnalise, null, null, DateTime.UtcNow.AddDays(-3));
        return sinistro;
    }

    internal static Sinistro SinistroAprovado(Apolice? apolice = null)
    {
        var sinistro = SinistroEmAnalise(apolice);
        sinistro.AtualizarStatus(StatusSinistro.Aprovado, null, null, DateTime.UtcNow.AddDays(-1));
        return sinistro;
    }
}
