using FluentValidation;

namespace SinistrosApi.Application.Sinistros.Commands.AtualizarStatus;

public class AtualizarStatusValidador : AbstractValidator<AtualizarStatusCommand>
{
    public AtualizarStatusValidador()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id do sinistro é obrigatório.");

        RuleFor(x => x.NovoStatus)
            .IsInEnum()
            .WithMessage("Status ausente ou inválido.");
    }
}
