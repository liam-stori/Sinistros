using FluentAssertions;
using NSubstitute;
using SinistrosApi.Application.Interfaces;
using SinistrosApi.Application.Sinistros.Queries.ObterPorId;
using SinistrosApi.Domain.Comum;
using Xunit;

namespace SinistrosApi.Testes.Aplicacao.Queries;

public class ObterSinistroPorIdHandlerTestes
{
    private readonly ISinistroRepositorio _sinistroRepositorio;
    private readonly ObterSinistroPorIdHandler _handler;

    public ObterSinistroPorIdHandlerTestes()
    {
        _sinistroRepositorio = Substitute.For<ISinistroRepositorio>();
        _handler = new ObterSinistroPorIdHandler(_sinistroRepositorio);
    }

    #region Sinistro não encontrado

    [Fact]
    public async Task Dado_SinistroInexistente_Quando_ObterPorId_Entao_RetornaFalhaNaoEncontrado()
    {
        // Arrange
        var sinistroId = Guid.NewGuid();
        _sinistroRepositorio
            .ObterPorIdAsync(sinistroId, CancellationToken.None)
            .Returns((Domain.Entidades.Sinistro?)null);

        var query = new ObterSinistroPorIdQuery(sinistroId);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.NaoEncontrado);
    }

    #endregion

    #region Sinistro encontrado

    [Fact]
    public async Task Dado_SinistroExistente_Quando_ObterPorId_Entao_RetornaDtoComDadosCorretos()
    {
        // Arrange
        var sinistro = Fabrica.SinistroAberto(valor: 8000m);
        _sinistroRepositorio
            .ObterPorIdAsync(sinistro.Id, CancellationToken.None)
            .Returns(sinistro);

        var query = new ObterSinistroPorIdQuery(sinistro.Id);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        resultado.Valor.Id.Should().Be(sinistro.Id);
        resultado.Valor.ApoliceId.Should().Be(sinistro.ApoliceId);
        resultado.Valor.ValorEstimado.Should().Be(8000m);
        resultado.Valor.Status.Should().Be("Aberto");
    }

    #endregion
}
