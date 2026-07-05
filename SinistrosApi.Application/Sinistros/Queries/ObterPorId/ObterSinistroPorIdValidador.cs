using FluentValidation;

namespace SinistrosApi.Application.Sinistros.Queries.ObterPorId;

public class ObterSinistroPorIdValidador : AbstractValidator<ObterSinistroPorIdQuery>
{
    public ObterSinistroPorIdValidador()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id do sinistro é obrigatório.");
    }
}
