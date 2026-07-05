using FluentAssertions;
using NSubstitute;
using SinistrosApi.Application.Interfaces;
using SinistrosApi.Application.Sinistros.Commands.AbrirSinistro;
using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Enumeradores;
using Xunit;

namespace SinistrosApi.Testes.Aplicacao.Commands;

public class AbrirSinistroHandlerTestes
{
    private readonly ISinistroRepositorio _sinistroRepositorio;
    private readonly IApoliceRepositorio _apoliceRepositorio;
    private readonly TimeProvider _timeProvider;
    private readonly AbrirSinistroHandler _handler;

    public AbrirSinistroHandlerTestes()
    {
        _sinistroRepositorio = Substitute.For<ISinistroRepositorio>();
        _apoliceRepositorio = Substitute.For<IApoliceRepositorio>();
        _timeProvider = Substitute.For<TimeProvider>();

        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2026, 7, 5, 12, 0, 0, TimeSpan.Zero));

        _handler = new AbrirSinistroHandler(_sinistroRepositorio, _apoliceRepositorio, _timeProvider);
    }

    #region Apólice não encontrada

    [Fact]
    public async Task Dado_ApoliceInexistente_Quando_AbrirSinistro_Entao_RetornaFalhaNaoEncontrado()
    {
        // Arrange
        var apoliceId = Guid.NewGuid();
        _apoliceRepositorio
            .ObterPorIdAsync(apoliceId, CancellationToken.None)
            .Returns((Domain.Entidades.Apolice?)null);

        var command = new AbrirSinistroCommand(apoliceId, 5000m);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.NaoEncontrado);
    }

    #endregion

    #region Apólice inativa

    [Fact]
    public async Task Dado_ApoliceInativa_Quando_AbrirSinistro_Entao_RetornaFalhaDeNegocio()
    {
        // Arrange
        var apolice = Fabrica.ApoliceAtiva();
        apolice.Inativar();

        _apoliceRepositorio
            .ObterPorIdAsync(apolice.Id, CancellationToken.None)
            .Returns(apolice);

        var command = new AbrirSinistroCommand(apolice.Id, 5000m);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.RegraDeNegocio);
    }

    #endregion

    #region Sucesso

    [Fact]
    public async Task Dado_ApoliceAtiva_Quando_AbrirSinistro_Entao_PersisteSinistroERetornaDto()
    {
        // Arrange
        var apolice = Fabrica.ApoliceAtiva(Ramo.Residencial);

        _apoliceRepositorio
            .ObterPorIdAsync(apolice.Id, CancellationToken.None)
            .Returns(apolice);

        var command = new AbrirSinistroCommand(apolice.Id, 12000m);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        resultado.Valor.ApoliceId.Should().Be(apolice.Id);
        resultado.Valor.ValorEstimado.Should().Be(12000m);
        resultado.Valor.Status.Should().Be("Aberto");

        await _sinistroRepositorio.Received(1).AdicionarAsync(
            Arg.Is<Domain.Entidades.Sinistro>(s =>
                s.ApoliceId == apolice.Id
                && s.ValorEstimado == 12000m),
            CancellationToken.None);
        await _sinistroRepositorio.Received(1).SalvarAlteracoesAsync(CancellationToken.None);
    }

    #endregion
}
