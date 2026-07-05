using FluentAssertions;
using SinistrosApi.Application.Sinistros.Queries.ObterHistorico;
using Xunit;

namespace SinistrosApi.Testes.Aplicacao.Queries;

public class ObterHistoricoValidadorTestes
{
    private readonly ObterHistoricoValidador _validador = new();

    #region Válido

    [Fact]
    public async Task Dado_SinistroIdValido_Quando_Validar_Entao_RetornaSucesso()
    {
        // Arrange
        var query = new ObterHistoricoQuery(Guid.NewGuid());

        // Act
        var resultado = await _validador.ValidateAsync(query);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    #endregion

    #region SinistroId

    [Fact]
    public async Task Dado_SinistroIdVazio_Quando_Validar_Entao_RetornaFalha()
    {
        // Arrange
        var query = new ObterHistoricoQuery(Guid.Empty);

        // Act
        var resultado = await _validador.ValidateAsync(query);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(ObterHistoricoQuery.SinistroId));
    }

    #endregion
}
