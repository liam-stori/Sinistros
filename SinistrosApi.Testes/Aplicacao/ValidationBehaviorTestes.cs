using FluentAssertions;
using FluentValidation;
using MediatR;
using SinistrosApi.Application.Comportamentais;
using SinistrosApi.Application.Sinistros.Commands.AbrirSinistro;
using SinistrosApi.Application.Sinistros.Dtos;
using SinistrosApi.Domain.Comum;
using Xunit;

namespace SinistrosApi.Testes.Aplicacao;

file record QuerySemResultado() : IRequest<string>;

file class ValidadorSempreInvalidoString : AbstractValidator<QuerySemResultado>
{
    public ValidadorSempreInvalidoString() => RuleFor(x => x).Must(_ => false).WithMessage("inválido");
}

file class RespostaComFalhaNull
{
    public static RespostaComFalhaNull? Falha(string erro, TipoErro tipo) => null;
}

file record QueryComFalhaNull() : IRequest<RespostaComFalhaNull>;

file class ValidadorSempreInvalidoNull : AbstractValidator<QueryComFalhaNull>
{
    public ValidadorSempreInvalidoNull() => RuleFor(x => x).Must(_ => false).WithMessage("inválido");
}

public class ValidationBehaviorTestes
{
    #region Sem validadores

    [Fact]
    public async Task Dado_SemValidadores_Quando_Handle_Entao_ChamaNext()
    {
        // Arrange
        var behavior = new ValidationBehavior<AbrirSinistroCommand, Resultado<SinistroDto>>([]);
        var command = new AbrirSinistroCommand(Guid.NewGuid(), 5000m);

        var nextFoiChamado = false;
        Task<Resultado<SinistroDto>> next(CancellationToken _)
        {
            nextFoiChamado = true;
            return Task.FromResult(Resultado<SinistroDto>.Sucesso(new SinistroDto(
                Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, "Aberto", 5000m, null, null)));
        }

        // Act
        await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        nextFoiChamado.Should().BeTrue();
    }

    #endregion

    #region Validação passa

    [Fact]
    public async Task Dado_ValidacaoComSucesso_Quando_Handle_Entao_ChamaNext()
    {
        // Arrange
        var behavior = new ValidationBehavior<AbrirSinistroCommand, Resultado<SinistroDto>>(
            [new AbrirSinistroValidador()]);

        var command = new AbrirSinistroCommand(Guid.NewGuid(), 5000m);

        var nextFoiChamado = false;
        Task<Resultado<SinistroDto>> next(CancellationToken _)
        {
            nextFoiChamado = true;
            return Task.FromResult(Resultado<SinistroDto>.Sucesso(new SinistroDto(
                Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, "Aberto", 5000m, null, null)));
        }

        // Act
        await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        nextFoiChamado.Should().BeTrue();
    }

    #endregion

    #region Validação falha

    [Fact]
    public async Task Dado_ValidacaoComFalha_Quando_Handle_Entao_RetornaFalhaSemChamarNext()
    {
        // Arrange
        var behavior = new ValidationBehavior<AbrirSinistroCommand, Resultado<SinistroDto>>(
            [new AbrirSinistroValidador()]);

        var commandInvalido = new AbrirSinistroCommand(Guid.Empty, 0m);

        var nextFoiChamado = false;
        Task<Resultado<SinistroDto>> next(CancellationToken _)
        {
            nextFoiChamado = true;
            return Task.FromResult(Resultado<SinistroDto>.Sucesso(null!));
        }

        // Act
        var resultado = await behavior.Handle(commandInvalido, next, CancellationToken.None);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.Validacao);
        nextFoiChamado.Should().BeFalse();
    }

    #endregion

    #region Resposta sem Falha / Falha retorna null

    [Fact]
    public async Task Dado_RespostaSemMetodoFalha_Quando_ValidacaoFalha_Entao_LancaInvalidOperationException()
    {
        // Arrange
        var behavior = new ValidationBehavior<QuerySemResultado, string>([new ValidadorSempreInvalidoString()]);

        Task<string> next(CancellationToken _)
        {
            return Task.FromResult("ok");
        }

        // Act
        var act = async () => await behavior.Handle(new QuerySemResultado(), next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*não implementa Resultado.Falha*");
    }

    [Fact]
    public async Task Dado_MetodoFalhaRetornaNull_Quando_ValidacaoFalha_Entao_LancaInvalidOperationException()
    {
        // Arrange
        var behavior = new ValidationBehavior<QueryComFalhaNull, RespostaComFalhaNull>([new ValidadorSempreInvalidoNull()]);

        Task<RespostaComFalhaNull> next(CancellationToken _)
        {
            return Task.FromResult(new RespostaComFalhaNull());
        }

        // Act
        var act = async () => await behavior.Handle(new QueryComFalhaNull(), next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Falha retornou null*");
    }

    #endregion
}
