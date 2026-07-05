using FluentAssertions;
using SinistrosApi.Api.Requesters;
using Xunit;

namespace SinistrosApi.Testes.Api;

public class AbrirSinistroRequestTestes
{
    #region ToCommand

    [Fact]
    public void Dado_GuidValidoEValorPreenchido_Quando_ToCommand_Entao_MapeiaDadosCorretamente()
    {
        // Arrange
        var apoliceId = Guid.NewGuid();
        var request = new AbrirSinistroRequest(apoliceId.ToString(), 8000m);

        // Act
        var command = request.ToCommand();

        // Assert
        command.ApoliceId.Should().Be(apoliceId);
        command.ValorEstimado.Should().Be(8000m);
    }

    [Fact]
    public void Dado_GuidInvalido_Quando_ToCommand_Entao_ApoliceIdEhGuidEmpty()
    {
        // Arrange
        var request = new AbrirSinistroRequest("não-é-um-guid", 5000m);

        // Act
        var command = request.ToCommand();

        // Assert
        command.ApoliceId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void Dado_ValorEstimadoNulo_Quando_ToCommand_Entao_ValorEhZero()
    {
        // Arrange
        var request = new AbrirSinistroRequest(Guid.NewGuid().ToString(), null);

        // Act
        var command = request.ToCommand();

        // Assert
        command.ValorEstimado.Should().Be(0m);
    }

    #endregion
}
