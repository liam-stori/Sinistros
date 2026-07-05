using FluentAssertions;
using SinistrosApi.Api.Requesters;
using SinistrosApi.Domain.Enumeradores;
using Xunit;

namespace SinistrosApi.Testes.Api;

public class AtualizarStatusRequestTestes
{
    #region ToCommand

    [Fact]
    public void Dado_StatusValidoEMotivo_Quando_ToCommand_Entao_MapeiaDadosCorretamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new AtualizarStatusRequest("Negado", "Documentação insuficiente.", null);

        // Act
        var command = request.ToCommand(id);

        // Assert
        command.Id.Should().Be(id);
        command.NovoStatus.Should().Be(StatusSinistro.Negado);
        command.Motivo.Should().Be("Documentação insuficiente.");
        command.ValorAprovado.Should().BeNull();
    }

    [Fact]
    public void Dado_StatusEncerradoComValorAprovado_Quando_ToCommand_Entao_MapeiaDadosCorretamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new AtualizarStatusRequest("Encerrado", null, 4500m);

        // Act
        var command = request.ToCommand(id);

        // Assert
        command.NovoStatus.Should().Be(StatusSinistro.Encerrado);
        command.ValorAprovado.Should().Be(4500m);
        command.Motivo.Should().BeNull();
    }

    [Fact]
    public void Dado_StatusInvalido_Quando_ToCommand_Entao_NovoStatusEhZero()
    {
        // Arrange
        var request = new AtualizarStatusRequest("StatusInexistente", null, null);

        // Act
        var command = request.ToCommand(Guid.NewGuid());

        // Assert
        command.NovoStatus.Should().Be((StatusSinistro)0);
    }

    #endregion
}
