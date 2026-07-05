using FluentAssertions;
using SinistrosApi.Application.Sinistros.Queries.ObterPorId;
using Xunit;

namespace SinistrosApi.Testes.Aplicacao.Queries;

public class ObterSinistroPorIdValidadorTestes
{
    private readonly ObterSinistroPorIdValidador _validador = new();

    #region Válido

    [Fact]
    public async Task Dado_IdValido_Quando_Validar_Entao_RetornaSucesso()
    {
        // Arrange
        var query = new ObterSinistroPorIdQuery(Guid.NewGuid());

        // Act
        var resultado = await _validador.ValidateAsync(query);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    #endregion

    #region Id

    [Fact]
    public async Task Dado_IdVazio_Quando_Validar_Entao_RetornaFalha()
    {
        // Arrange
        var query = new ObterSinistroPorIdQuery(Guid.Empty);

        // Act
        var resultado = await _validador.ValidateAsync(query);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(ObterSinistroPorIdQuery.Id));
    }

    #endregion
}
