using FluentAssertions;
using SinistrosApi.Application.Sinistros.Commands.AtualizarStatus;
using SinistrosApi.Domain.Enumeradores;
using Xunit;

namespace SinistrosApi.Testes.Aplicacao.Commands;

public class AtualizarStatusValidadorTestes
{
    private readonly AtualizarStatusValidador _validador = new();

    #region Válido

    [Fact]
    public async Task Dado_ComandoValido_Quando_Validar_Entao_RetornaSemErros()
    {
        // Arrange
        var command = new AtualizarStatusCommand(Guid.NewGuid(), StatusSinistro.EmAnalise, null, null);

        // Act
        var resultado = await _validador.ValidateAsync(command);

        // Assert
        resultado.IsValid.Should().BeTrue();
    }

    #endregion

    #region Id

    [Fact]
    public async Task Dado_IdVazio_Quando_Validar_Entao_RetornaErro()
    {
        // Arrange
        var command = new AtualizarStatusCommand(Guid.Empty, StatusSinistro.EmAnalise, null, null);

        // Act
        var resultado = await _validador.ValidateAsync(command);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(AtualizarStatusCommand.Id));
    }

    #endregion

    #region NovoStatus

    [Fact]
    public async Task Dado_StatusForaDoEnum_Quando_Validar_Entao_RetornaErro()
    {
        // Arrange
        var command = new AtualizarStatusCommand(Guid.NewGuid(), (StatusSinistro)0, null, null);

        // Act
        var resultado = await _validador.ValidateAsync(command);

        // Assert
        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(AtualizarStatusCommand.NovoStatus));
    }

    #endregion
}
