using FluentValidation;

namespace SinistrosApi.Application.Sinistros.Queries.ObterHistorico;

public class ObterHistoricoValidador : AbstractValidator<ObterHistoricoQuery>
{
    public ObterHistoricoValidador()
    {
        RuleFor(x => x.SinistroId)
            .NotEmpty()
            .WithMessage("Id do sinistro é obrigatório.");
    }
}
