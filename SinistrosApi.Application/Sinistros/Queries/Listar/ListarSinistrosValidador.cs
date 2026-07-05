using FluentValidation;

namespace SinistrosApi.Application.Sinistros.Queries.Listar;

public class ListarSinistrosValidador : AbstractValidator<ListarSinistrosQuery>
{
    public ListarSinistrosValidador()
    {
        RuleFor(x => x.Pagina)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Página deve ser maior ou igual a 1.");

        RuleFor(x => x.TamanhoPagina)
            .InclusiveBetween(1, 100)
            .WithMessage("Tamanho de página deve ser entre 1 e 100.");

        RuleFor(x => x.DataFim)
            .GreaterThanOrEqualTo(x => x.DataInicio)
            .When(x => x.DataInicio.HasValue && x.DataFim.HasValue)
            .WithMessage("Data fim deve ser maior ou igual à data início.");
    }
}
