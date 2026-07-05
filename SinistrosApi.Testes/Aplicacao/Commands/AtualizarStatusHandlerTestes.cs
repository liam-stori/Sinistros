using FluentAssertions;
using NSubstitute;
using SinistrosApi.Application.Interfaces;
using SinistrosApi.Application.Sinistros.Commands.AtualizarStatus;
using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Enumeradores;
using Xunit;

namespace SinistrosApi.Testes.Aplicacao.Commands;

public class AtualizarStatusHandlerTestes
{
    private readonly ISinistroRepositorio _sinistroRepositorio;
    private readonly TimeProvider _timeProvider;
    private readonly AtualizarStatusHandler _handler;

    public AtualizarStatusHandlerTestes()
    {
        _sinistroRepositorio = Substitute.For<ISinistroRepositorio>();
        _timeProvider = Substitute.For<TimeProvider>();

        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2026, 7, 5, 12, 0, 0, TimeSpan.Zero));

        _handler = new AtualizarStatusHandler(_sinistroRepositorio, _timeProvider);
    }

    #region Sinistro não encontrado

    [Fact]
    public async Task Dado_SinistroInexistente_Quando_AtualizarStatus_Entao_RetornaFalhaNaoEncontrado()
    {
        // Arrange
        var sinistroId = Guid.NewGuid();
        _sinistroRepositorio
            .ObterPorIdAsync(sinistroId, CancellationToken.None)
            .Returns((Domain.Entidades.Sinistro?)null);

        var command = new AtualizarStatusCommand(sinistroId, StatusSinistro.EmAnalise, null, null);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.NaoEncontrado);
        await _sinistroRepositorio.DidNotReceive().SalvarAlteracoesAsync(CancellationToken.None);
    }

    #endregion

    #region Transição inválida

    [Fact]
    public async Task Dado_SinistroAberto_Quando_TransitarDiretoParaAprovado_Entao_RetornaFalhaENaoSalva()
    {
        // Arrange
        var sinistro = Fabrica.SinistroAberto();
        _sinistroRepositorio
            .ObterPorIdAsync(sinistro.Id, CancellationToken.None)
            .Returns(sinistro);

        var command = new AtualizarStatusCommand(sinistro.Id, StatusSinistro.Aprovado, null, null);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.RegraDeNegocio);
        await _sinistroRepositorio.DidNotReceive().SalvarAlteracoesAsync(CancellationToken.None);
    }

    #endregion

    #region Sucesso

    [Fact]
    public async Task Dado_SinistroAberto_Quando_TransitarParaEmAnalise_Entao_SalvaERetornaSucesso()
    {
        // Arrange
        var sinistro = Fabrica.SinistroAberto();
        _sinistroRepositorio
            .ObterPorIdAsync(sinistro.Id, CancellationToken.None)
            .Returns(sinistro);

        var command = new AtualizarStatusCommand(sinistro.Id, StatusSinistro.EmAnalise, null, null);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        await _sinistroRepositorio.Received(1).SalvarAlteracoesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Dado_SinistroEmAnalise_Quando_NegadoComMotivo_Entao_SalvaERetornaSucesso()
    {
        // Arrange
        var sinistro = Fabrica.SinistroEmAnalise();
        _sinistroRepositorio
            .ObterPorIdAsync(sinistro.Id, CancellationToken.None)
            .Returns(sinistro);

        var command = new AtualizarStatusCommand(sinistro.Id, StatusSinistro.Negado, "Dano pré-existente.", null);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        await _sinistroRepositorio.Received(1).SalvarAlteracoesAsync(CancellationToken.None);
    }

    #endregion
}
