using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using SinistrosApi.Api.Controllers;
using SinistrosApi.Api.Requesters;
using SinistrosApi.Application.Sinistros.Commands.AbrirSinistro;
using SinistrosApi.Application.Sinistros.Commands.AtualizarStatus;
using SinistrosApi.Application.Sinistros.Dtos;
using SinistrosApi.Application.Sinistros.Queries.Listar;
using SinistrosApi.Application.Sinistros.Queries.ObterHistorico;
using SinistrosApi.Application.Sinistros.Queries.ObterPorId;
using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Enumeradores;
using Xunit;

namespace SinistrosApi.Testes.Api;

public class SinistrosControllerTestes
{
    private readonly IMediator _mediator;
    private readonly SinistrosController _controller;

    public SinistrosControllerTestes()
    {
        _mediator = Substitute.For<IMediator>();
        _controller = new SinistrosController(_mediator);
    }

    #region Abrir

    [Fact]
    public async Task Dado_ComandoValido_Quando_Abrir_Entao_Retorna201ComDto()
    {
        // Arrange
        var apoliceId = Guid.NewGuid();
        var sinistroId = Guid.NewGuid();
        var dto = new SinistroDto(sinistroId, apoliceId, DateTime.UtcNow, "Aberto", 5000m, null, null);
        var request = new AbrirSinistroRequest(apoliceId.ToString(), 5000m);

        _mediator
            .Send(new AbrirSinistroCommand(apoliceId, 5000m), CancellationToken.None)
            .Returns(Resultado<SinistroDto>.Sucesso(dto));

        // Act
        var resposta = await _controller.Abrir(request, CancellationToken.None);

        // Assert
        var created = resposta.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be("ObterPorId");
        created.Value.Should().Be(dto);
    }

    [Fact]
    public async Task Dado_ComandoInvalido_Quando_Abrir_Entao_Retorna400()
    {
        // Arrange
        var apoliceId = Guid.NewGuid();
        var request = new AbrirSinistroRequest(apoliceId.ToString(), 0m);

        _mediator
            .Send(new AbrirSinistroCommand(apoliceId, 0m), CancellationToken.None)
            .Returns(Resultado<SinistroDto>.Falha("valor inválido", TipoErro.Validacao));

        // Act
        var resposta = await _controller.Abrir(request, CancellationToken.None);

        // Assert
        resposta.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region ObterPorId

    [Fact]
    public async Task Dado_SinistroExistente_Quando_ObterPorId_Entao_Retorna200ComDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new SinistroDto(id, Guid.NewGuid(), DateTime.UtcNow, "Aberto", 5000m, null, null);

        _mediator
            .Send(new ObterSinistroPorIdQuery(id), CancellationToken.None)
            .Returns(Resultado<SinistroDto>.Sucesso(dto));

        // Act
        var resposta = await _controller.ObterPorId(id, CancellationToken.None);

        // Assert
        var ok = resposta.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(dto);
    }

    [Fact]
    public async Task Dado_SinistroInexistente_Quando_ObterPorId_Entao_Retorna404()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mediator
            .Send(new ObterSinistroPorIdQuery(id), CancellationToken.None)
            .Returns(Resultado<SinistroDto>.Falha("não encontrado", TipoErro.NaoEncontrado));

        // Act
        var resposta = await _controller.ObterPorId(id, CancellationToken.None);

        // Assert
        resposta.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region Listar

    [Fact]
    public async Task Dado_QueryValida_Quando_Listar_Entao_Retorna200ComPaginado()
    {
        // Arrange
        var paginado = new SinistrosPaginadosDto([], 0, 1, 20);

        _mediator
            .Send(new ListarSinistrosQuery(null, null, null, 1, 20), CancellationToken.None)
            .Returns(Resultado<SinistrosPaginadosDto>.Sucesso(paginado));

        // Act
        var resposta = await _controller.Listar(null, null, null, 1, 20, CancellationToken.None);

        // Assert
        var ok = resposta.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(paginado);
    }

    [Fact]
    public async Task Dado_QueryComFiltros_Quando_Listar_Entao_RepassaFiltrosAoMediator()
    {
        // Arrange
        var dataInicio = DateTime.UtcNow.AddDays(-7);
        var dataFim = DateTime.UtcNow;
        var paginado = new SinistrosPaginadosDto([], 0, 2, 10);

        _mediator
            .Send(new ListarSinistrosQuery(StatusSinistro.Aberto, dataInicio, dataFim, 2, 10), CancellationToken.None)
            .Returns(Resultado<SinistrosPaginadosDto>.Sucesso(paginado));

        // Act
        var resposta = await _controller.Listar(StatusSinistro.Aberto, dataInicio, dataFim, 2, 10, CancellationToken.None);

        // Assert
        resposta.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region AtualizarStatus

    [Fact]
    public async Task Dado_TransicaoValida_Quando_AtualizarStatus_Entao_Retorna204()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new AtualizarStatusRequest("EmAnalise", null, null);

        _mediator
            .Send(new AtualizarStatusCommand(id, StatusSinistro.EmAnalise, null, null), CancellationToken.None)
            .Returns(Resultado.Sucesso());

        // Act
        var resposta = await _controller.AtualizarStatus(id, request, CancellationToken.None);

        // Assert
        resposta.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Dado_TransicaoInvalida_Quando_AtualizarStatus_Entao_Retorna422()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new AtualizarStatusRequest("Encerrado", null, null);

        _mediator
            .Send(new AtualizarStatusCommand(id, StatusSinistro.Encerrado, null, null), CancellationToken.None)
            .Returns(Resultado.Falha("transição inválida", TipoErro.RegraDeNegocio));

        // Act
        var resposta = await _controller.AtualizarStatus(id, request, CancellationToken.None);

        // Assert
        resposta.Should().BeOfType<UnprocessableEntityObjectResult>();
    }

    #endregion

    #region ObterHistorico

    [Fact]
    public async Task Dado_SinistroComHistorico_Quando_ObterHistorico_Entao_Retorna200()
    {
        // Arrange
        var id = Guid.NewGuid();
        IReadOnlyList<HistoricoSinistroDto> historico = [];

        _mediator
            .Send(new ObterHistoricoQuery(id), CancellationToken.None)
            .Returns(Resultado<IReadOnlyList<HistoricoSinistroDto>>.Sucesso(historico));

        // Act
        var resposta = await _controller.ObterHistorico(id, CancellationToken.None);

        // Assert
        resposta.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Dado_SinistroInexistente_Quando_ObterHistorico_Entao_Retorna404()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mediator
            .Send(new ObterHistoricoQuery(id), CancellationToken.None)
            .Returns(Resultado<IReadOnlyList<HistoricoSinistroDto>>.Falha("não encontrado", TipoErro.NaoEncontrado));

        // Act
        var resposta = await _controller.ObterHistorico(id, CancellationToken.None);

        // Assert
        resposta.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion
}
