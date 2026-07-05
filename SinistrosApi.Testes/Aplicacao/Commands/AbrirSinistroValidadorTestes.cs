using FluentAssertions;
using SinistrosApi.Application.Sinistros.Commands.AbrirSinistro;
using Xunit;

namespace SinistrosApi.Testes.Aplicacao.Commands;

public class AbrirSinistroValidadorTestes
{
    private readonly AbrirSinistroValidador _validador = new();

    #region Válido

    [Fact]
    public async Task Dado_ComandoValido_Quando_Validar_Entao_RetornaSemErros()
    {
        // Arrange
        var command = new AbrirSinistroCommand(Guid.NewGuid(), 5000m);

        // Act
        var resultado = await _validador.ValidateAsync(command);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    #endregion

    #region ApoliceId

    [Fact]
    public async Task Dado_ApoliceIdVazio_Quando_Validar_Entao_RetornaErro()
    {
        // Arrange
        var command = new AbrirSinistroCommand(Guid.Empty, 5000m);

        // Act
        var resultado = await _validador.ValidateAsync(command);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(AbrirSinistroCommand.ApoliceId));
    }

    #endregion

    #region ValorEstimado

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task Dado_ValorEstimadoInvalido_Quando_Validar_Entao_RetornaErro(decimal valor)
    {
        // Arrange
        var command = new AbrirSinistroCommand(Guid.NewGuid(), valor);

        // Act
        var resultado = await _validador.ValidateAsync(command);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(AbrirSinistroCommand.ValorEstimado));
    }

    #endregion
}
