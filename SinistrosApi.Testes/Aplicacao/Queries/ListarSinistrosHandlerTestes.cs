using FluentAssertions;
using NSubstitute;
using SinistrosApi.Application.Interfaces;
using SinistrosApi.Application.Sinistros.Queries.Listar;
using SinistrosApi.Domain.Enumeradores;
using Xunit;

namespace SinistrosApi.Testes.Aplicacao.Queries;

public class ListarSinistrosHandlerTestes
{
    private readonly ISinistroRepositorio _sinistroRepositorio;
    private readonly ListarSinistrosHandler _handler;

    public ListarSinistrosHandlerTestes()
    {
        _sinistroRepositorio = Substitute.For<ISinistroRepositorio>();
        _handler = new ListarSinistrosHandler(_sinistroRepositorio);
    }

    #region Lista vazia

    [Fact]
    public async Task Dado_NenhumSinistroNoRepositorio_Quando_Listar_Entao_RetornaListaVaziaComTotalZero()
    {
        // Arrange
        _sinistroRepositorio
            .ListarAsync(null, null, null, 1, 20, CancellationToken.None)
            .Returns((new List<Domain.Entidades.Sinistro>(), 0));

        var query = new ListarSinistrosQuery(null, null, null, 1, 20);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        resultado.Valor.Itens.Should().BeEmpty();
        resultado.Valor.Total.Should().Be(0);
        resultado.Valor.Pagina.Should().Be(1);
        resultado.Valor.TamanhoPagina.Should().Be(20);
    }

    #endregion

    #region Lista com resultados

    [Fact]
    public async Task Dado_SinistrosNoRepositorio_Quando_Listar_Entao_RetornaItensMapeadosEPaginacao()
    {
        // Arrange
        var sinistro1 = Fabrica.SinistroAberto(valor: 3000m);
        var sinistro2 = Fabrica.SinistroAberto(valor: 7000m);
        var itens = new List<Domain.Entidades.Sinistro> { sinistro1, sinistro2 };

        _sinistroRepositorio
            .ListarAsync(null, null, null, 1, 20, CancellationToken.None)
            .Returns((itens, 2));

        var query = new ListarSinistrosQuery(null, null, null, 1, 20);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        resultado.Valor.Itens.Should().HaveCount(2);
        resultado.Valor.Total.Should().Be(2);
        resultado.Valor.Itens[0].Id.Should().Be(sinistro1.Id);
        resultado.Valor.Itens[0].ValorEstimado.Should().Be(3000m);
        resultado.Valor.Itens[1].Id.Should().Be(sinistro2.Id);
        resultado.Valor.Itens[1].ValorEstimado.Should().Be(7000m);
    }

    [Fact]
    public async Task Dado_SinistroAprovado_Quando_Listar_Entao_DtoContemValorAprovadoECamposCorretos()
    {
        // Arrange
        var sinistro = Fabrica.SinistroAprovado();
        sinistro.AtualizarStatus(StatusSinistro.Encerrado, null, 4500m, DateTime.UtcNow);

        _sinistroRepositorio
            .ListarAsync(null, null, null, 1, 20, CancellationToken.None)
            .Returns((new List<Domain.Entidades.Sinistro> { sinistro }, 1));

        var query = new ListarSinistrosQuery(null, null, null, 1, 20);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        var dto = resultado.Valor.Itens.Single();
        dto.ApoliceId.Should().Be(sinistro.ApoliceId);
        dto.DataAbertura.Should().Be(sinistro.DataAbertura);
        dto.Status.Should().Be("Encerrado");
        dto.ValorAprovado.Should().Be(4500m);
        dto.MotivoNegativa.Should().BeNull();
    }

    [Fact]
    public async Task Dado_SinistroNegado_Quando_Listar_Entao_DtoContemMotivoNegativa()
    {
        // Arrange
        var sinistro = Fabrica.SinistroEmAnalise();
        const string motivo = "Documentação insuficiente.";
        sinistro.AtualizarStatus(StatusSinistro.Negado, motivo, null, DateTime.UtcNow);

        _sinistroRepositorio
            .ListarAsync(null, null, null, 1, 20, CancellationToken.None)
            .Returns((new List<Domain.Entidades.Sinistro> { sinistro }, 1));

        var query = new ListarSinistrosQuery(null, null, null, 1, 20);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var dto = resultado.Valor.Itens.Single();
        dto.MotivoNegativa.Should().Be(motivo);
        dto.ValorAprovado.Should().BeNull();
    }

    [Fact]
    public async Task Dado_FiltroPorStatus_Quando_Listar_Entao_RepassaFiltroAoRepositorio()
    {
        // Arrange
        _sinistroRepositorio
            .ListarAsync(StatusSinistro.Aberto, null, null, 2, 10, CancellationToken.None)
            .Returns((new List<Domain.Entidades.Sinistro>(), 0));

        var query = new ListarSinistrosQuery(StatusSinistro.Aberto, null, null, 2, 10);

        // Act
        var resultado = await _handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        resultado.Valor.Pagina.Should().Be(2);
        resultado.Valor.TamanhoPagina.Should().Be(10);

        await _sinistroRepositorio.Received(1)
            .ListarAsync(StatusSinistro.Aberto, null, null, 2, 10, CancellationToken.None);
    }

    #endregion
}
