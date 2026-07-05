using FluentValidation;
using MediatR;
using SinistrosApi.Domain.Comum;

namespace SinistrosApi.Application.Comportamentais;

public class ValidationBehavior<TRequisicao, TResposta> : IPipelineBehavior<TRequisicao, TResposta>
    where TRequisicao : IRequest<TResposta>
{
    private readonly IEnumerable<IValidator<TRequisicao>> _validadores;

    public ValidationBehavior(IEnumerable<IValidator<TRequisicao>> validadores)
    {
        _validadores = validadores;
    }

    public async Task<TResposta> Handle(
        TRequisicao request,
        RequestHandlerDelegate<TResposta> next,
        CancellationToken cancellationToken)
    {
        if (!_validadores.Any())
            return await next();

        var contexto = new ValidationContext<TRequisicao>(request);

        var resultados = await Task.WhenAll(
            _validadores.Select(v => v.ValidateAsync(contexto, cancellationToken)));

        var falhas = resultados
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (!falhas.Any())
            return await next();

        var mensagemErro = string.Join(" | ", falhas.Select(f => f.ErrorMessage));

        var metodoFalha = typeof(TResposta).GetMethod("Falha", [typeof(string), typeof(TipoErro)]);

        if (metodoFalha is null)
            throw new InvalidOperationException($"{typeof(TResposta).Name} não implementa Resultado.Falha — não é possível curto-circuitar validação.");

        var resultado = metodoFalha.Invoke(null, [mensagemErro, TipoErro.Validacao])
            ?? throw new InvalidOperationException($"{typeof(TResposta).Name}.Falha retornou null inesperadamente.");

        return (TResposta)resultado;
    }
}
