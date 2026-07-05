using FluentValidation;

namespace SinistrosApi.Application.Sinistros.Commands.AbrirSinistro;

public class AbrirSinistroValidador : AbstractValidator<AbrirSinistroCommand>
{
    public AbrirSinistroValidador()
    {
        RuleFor(x => x.ApoliceId)
            .NotEmpty()
            .WithMessage("Identificador da apólice ausente ou inválido.");

        RuleFor(x => x.ValorEstimado)
            .GreaterThan(0)
            .WithMessage("Valor estimado deve ser maior que zero.");
    }
}
