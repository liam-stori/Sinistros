using MediatR;
using Microsoft.AspNetCore.Mvc;
using SinistrosApi.Api.Comum;
using SinistrosApi.Api.Requesters;
using SinistrosApi.Application.Sinistros.Queries.Listar;
using SinistrosApi.Application.Sinistros.Queries.ObterHistorico;
using SinistrosApi.Application.Sinistros.Queries.ObterPorId;
using SinistrosApi.Domain.Enumeradores;

namespace SinistrosApi.Api.Controllers;

[Route("api/sinistros")]
public class SinistrosController : ControladorBase
{
    private readonly IMediator _mediator;

    public SinistrosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Abrir(
        [FromBody] AbrirSinistroRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await _mediator.Send(request.ToCommand(), cancellationToken);
        return CriadoEm(nameof(ObterPorId), new { id = resultado.Valor?.Id }, resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var resultado = await _mediator.Send(new ObterSinistroPorIdQuery(id), cancellationToken);
        return Responder(resultado);
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] StatusSinistro? status,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _mediator.Send(
            new ListarSinistrosQuery(status, dataInicio, dataFim, pagina, tamanhoPagina),
            cancellationToken);
        return Responder(resultado);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> AtualizarStatus(
        [FromRoute] Guid id,
        [FromBody] AtualizarStatusRequest request,
        CancellationToken cancellationToken)
    {
        var resultado = await _mediator.Send(request.ToCommand(id), cancellationToken);
        return Responder(resultado);
    }

    [HttpGet("{id:guid}/historico")]
    public async Task<IActionResult> ObterHistorico(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var resultado = await _mediator.Send(new ObterHistoricoQuery(id), cancellationToken);
        return Responder(resultado);
    }
}
