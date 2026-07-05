using Microsoft.AspNetCore.Mvc;
using SinistrosApi.Domain.Comum;

namespace SinistrosApi.Api.Comum;

[ApiController]
public abstract class ControladorBase : ControllerBase
{
    protected IActionResult Responder(Resultado resultado)
        => Resolver(resultado, NoContent);

    protected IActionResult Responder<T>(Resultado<T> resultado)
        => Resolver(resultado, () => Ok(resultado.Valor));

    protected IActionResult CriadoEm<T>(string action, object routeValues, Resultado<T> resultado)
        => Resolver(resultado, () => CreatedAtAction(action, routeValues, resultado.Valor));

    private IActionResult Resolver(Resultado resultado, Func<IActionResult> aoSucesso)
    {
        return resultado.EhSucesso
            ? aoSucesso()
            : RespostaDeErro(resultado);
    }

    private IActionResult RespostaDeErro(Resultado resultado)
    {
        var resultadoErro = new { erro = resultado.Erro };
        return resultado.TipoErro switch
        {
            TipoErro.NaoEncontrado => NotFound(resultadoErro),
            TipoErro.RegraDeNegocio => UnprocessableEntity(resultadoErro),
            TipoErro.Validacao => BadRequest(resultadoErro),
            _ => StatusCode(500, resultadoErro)
        };
    }
}
