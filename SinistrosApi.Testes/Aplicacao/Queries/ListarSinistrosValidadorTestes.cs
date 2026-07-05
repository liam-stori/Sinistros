using FluentAssertions;
using SinistrosApi.Application.Sinistros.Queries.Listar;
using Xunit;

namespace SinistrosApi.Testes.Aplicacao.Queries;

public class ListarSinistrosValidadorTestes
{
    private readonly ListarSinistrosValidador _validador = new();

    #region Válido

    [Fact]
    public async Task Dado_QueryValida_Quando_Validar_Entao_RetornaSucesso()
    {
        // Arrange
        var query = new ListarSinistrosQuery(null, null, null, 1, 20);

        // Act
        var resultado = await _validador.ValidateAsync(query);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Dado_DataInicioEFimIguais_Quando_Validar_Entao_RetornaSucesso()
    {
        // Arrange
        var data = DateTime.UtcNow.AddDays(-1);
        var query = new ListarSinistrosQuery(null, data, data, 1, 20);

        // Act
        var resultado = await _validador.ValidateAsync(query);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Dado_DataFimMaiorQueDataInicio_Quando_Validar_Entao_RetornaSucesso()
    {
        // Arrange
        var dataInicio = DateTime.UtcNow.AddDays(-7);
        var dataFim = DateTime.UtcNow;
        var query = new ListarSinistrosQuery(null, dataInicio, dataFim, 1, 20);

        // Act
        var resultado = await _validador.ValidateAsync(query);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    #endregion

    #region Pagina

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Dado_PaginaMenorQueUm_Quando_Validar_Entao_RetornaFalha(int pagina)
    {
        // Arrange
        var query = new ListarSinistrosQuery(null, null, null, pagina, 20);

        // Act
        var resultado = await _validador.ValidateAsync(query);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(ListarSinistrosQuery.Pagina));
    }

    #endregion

    #region TamanhoPagina

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(200)]
    public async Task Dado_TamanhoPaginaForaDosLimites_Quando_Validar_Entao_RetornaFalha(int tamanhoPagina)
    {
        // Arrange
        var query = new ListarSinistrosQuery(null, null, null, 1, tamanhoPagina);

        // Act
        var resultado = await _validador.ValidateAsync(query);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(ListarSinistrosQuery.TamanhoPagina));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task Dado_TamanhoPaginaDentroDoLimite_Quando_Validar_Entao_RetornaSucesso(int tamanhoPagina)
    {
        // Arrange
        var query = new ListarSinistrosQuery(null, null, null, 1, tamanhoPagina);

        // Act
        var resultado = await _validador.ValidateAsync(query);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    #endregion

    #region DataFim

    [Fact]
    public async Task Dado_DataFimAnteriorADataInicio_Quando_Validar_Entao_RetornaFalha()
    {
        // Arrange
        var dataInicio = DateTime.UtcNow;
        var dataFim = DateTime.UtcNow.AddDays(-1);
        var query = new ListarSinistrosQuery(null, dataInicio, dataFim, 1, 20);

        // Act
        var resultado = await _validador.ValidateAsync(query);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(ListarSinistrosQuery.DataFim));
    }

    [Fact]
    public async Task Dado_ApenasDataFimPreenchida_Quando_Validar_Entao_NaoValidaOrdemDeDatas()
    {
        // Arrange
        var query = new ListarSinistrosQuery(null, null, DateTime.UtcNow, 1, 20);

        // Act
        var resultado = await _validador.ValidateAsync(query);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Dado_ApenasDataInicioPreenchida_Quando_Validar_Entao_NaoValidaOrdemDeDatas()
    {
        // Arrange
        var query = new ListarSinistrosQuery(null, DateTime.UtcNow, null, 1, 20);

        // Act
        var resultado = await _validador.ValidateAsync(query);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    #endregion
}
