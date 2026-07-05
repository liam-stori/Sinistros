using FluentAssertions;
using NSubstitute;
using SinistrosApi.Application.Interfaces;
using SinistrosApi.Application.Sinistros.Queries.ObterHistorico;
using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Entidades;
using SinistrosApi.Domain.Enumeradores;
using Xunit;

namespace SinistrosApi.Testes.Aplicacao.Queries;

public class ObterHistoricoHandlerTestes
{
    private readonly ISinistroRepositorio _sinistroRepositorio;
    private readonly ObterHistoricoHandler _handler;

    public ObterHistoricoHandlerTestes()
    {
        _sinistroRepositorio = Substitute.For<ISinistroRepositorio>();
        _handler = new ObterHistoricoHandler(_sinistroRepositorio);
    }

    #region Sinistro não encontrado

    [Fact]
    public async Task Dado_SinistroInexistente_Quando_ObterHistorico_Entao_RetornaFalhaNaoEncontrado()
    {
        // Arrange
        var sinistroId = Guid.NewGuid();
        _sinistroRepositorio
            .ObterHistoricoAsync(sinistroId, CancellationToken.None)
            .Returns((IReadOnlyList<HistoricoSinistro>?)null);

        var query = new ObterHistoricoQuery(sinistroId);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.NaoEncontrado);
    }

    #endregion

    #region Histórico retornado

    [Fact]
    public async Task Dado_SinistroComHistorico_Quando_ObterHistorico_Entao_RetornaListaDeDtosOrdenada()
    {
        // Arrange
        var sinistroId = Guid.NewGuid();
        var dataAbertura = DateTime.UtcNow.AddDays(-5);
        var dataTransicao = DateTime.UtcNow.AddDays(-3);

        IReadOnlyList<HistoricoSinistro> historico =
        [
            HistoricoSinistro.Criar(sinistroId, null, StatusSinistro.Aberto, null, dataAbertura),
            HistoricoSinistro.Criar(sinistroId, StatusSinistro.Aberto, StatusSinistro.EmAnalise, null, dataTransicao)
        ];

        _sinistroRepositorio
            .ObterHistoricoAsync(sinistroId, CancellationToken.None)
            .Returns(historico);

        var query = new ObterHistoricoQuery(sinistroId);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        resultado.Valor.Should().HaveCount(2);

        var primeira = resultado.Valor[0];
        primeira.Id.Should().NotBeEmpty();
        primeira.StatusAnterior.Should().BeNull();
        primeira.StatusNovo.Should().Be("Aberto");
        primeira.DataAlteracao.Should().Be(dataAbertura);
        primeira.Motivo.Should().BeNull();

        var segunda = resultado.Valor[1];
        segunda.StatusAnterior.Should().Be("Aberto");
        segunda.StatusNovo.Should().Be("Em Análise");
        segunda.DataAlteracao.Should().Be(dataTransicao);
    }

    [Fact]
    public async Task Dado_HistoricoComMotivo_Quando_ObterHistorico_Entao_DtoContemMotivo()
    {
        // Arrange
        var sinistroId = Guid.NewGuid();
        const string motivo = "Documentação insuficiente.";

        IReadOnlyList<HistoricoSinistro> historico =
        [
            HistoricoSinistro.Criar(sinistroId, StatusSinistro.EmAnalise, StatusSinistro.Negado, motivo, DateTime.UtcNow)
        ];

        _sinistroRepositorio
            .ObterHistoricoAsync(sinistroId, CancellationToken.None)
            .Returns(historico);

        var query = new ObterHistoricoQuery(sinistroId);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        resultado.Valor.Single().Motivo.Should().Be(motivo);
    }

    #endregion
}
