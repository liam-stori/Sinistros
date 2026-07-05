using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SinistrosApi.Api.Comum;
using SinistrosApi.Domain.Comum;
using Xunit;

namespace SinistrosApi.Testes.Api;

public class ControladorBaseTestes
{
    private sealed class ControladorConcreto : ControladorBase
    {
        public IActionResult TestarResponder(Resultado resultado)
        {
            return Responder(resultado);
        }

        public IActionResult TestarResponderT<T>(Resultado<T> resultado)
        {
            return Responder(resultado);
        }

        public IActionResult TestarCriadoEm<T>(string action, object routeValues, Resultado<T> resultado)
        {
            return CriadoEm(action, routeValues, resultado);
        }
    }

    private readonly ControladorConcreto _controlador = new();

    #region Responder — sem valor

    [Fact]
    public void Dado_ResultadoSucesso_Quando_Responder_Entao_Retorna204()
    {
        // Arrange
        var resultado = Resultado.Sucesso();

        // Act
        var resposta = _controlador.TestarResponder(resultado);

        // Assert
        resposta.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void Dado_ErroNaoEncontrado_Quando_Responder_Entao_Retorna404()
    {
        // Arrange
        var resultado = Resultado.Falha("não encontrado", TipoErro.NaoEncontrado);

        // Act
        var resposta = _controlador.TestarResponder(resultado);

        // Assert
        resposta.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Dado_ErroRegraDeNegocio_Quando_Responder_Entao_Retorna422()
    {
        // Arrange
        var resultado = Resultado.Falha("regra violada", TipoErro.RegraDeNegocio);

        // Act
        var resposta = _controlador.TestarResponder(resultado);

        // Assert
        resposta.Should().BeOfType<UnprocessableEntityObjectResult>();
    }

    [Fact]
    public void Dado_ErroValidacao_Quando_Responder_Entao_Retorna400()
    {
        // Arrange
        var resultado = Resultado.Falha("campo inválido", TipoErro.Validacao);

        // Act
        var resposta = _controlador.TestarResponder(resultado);

        // Assert
        resposta.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Dado_ErroDesconhecido_Quando_Responder_Entao_Retorna500()
    {
        // Arrange
        var resultado = Resultado.Falha("erro interno", TipoErro.Nenhum);

        // Act
        var resposta = _controlador.TestarResponder(resultado);

        // Assert
        var objectResult = resposta.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region Responder — com valor

    [Fact]
    public void Dado_ResultadoSucessoComValor_Quando_Responder_Entao_Retorna200ComValor()
    {
        // Arrange
        var resultado = Resultado<string>.Sucesso("payload");

        // Act
        var resposta = _controlador.TestarResponderT(resultado);

        // Assert
        var ok = resposta.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be("payload");
    }

    [Fact]
    public void Dado_ResultadoFalhaComValor_Quando_Responder_Entao_Retorna404()
    {
        // Arrange
        var resultado = Resultado<string>.Falha("não encontrado", TipoErro.NaoEncontrado);

        // Act
        var resposta = _controlador.TestarResponderT(resultado);

        // Assert
        resposta.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region CriadoEm

    [Fact]
    public void Dado_ResultadoSucesso_Quando_CriadoEm_Entao_Retorna201ComActionEValor()
    {
        // Arrange
        var resultado = Resultado<string>.Sucesso("recurso");

        // Act
        var resposta = _controlador.TestarCriadoEm("ObterPorId", new { id = 1 }, resultado);

        // Assert
        var created = resposta.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be("ObterPorId");
        created.Value.Should().Be("recurso");
    }

    [Fact]
    public void Dado_ResultadoFalha_Quando_CriadoEm_Entao_RetornaErro()
    {
        // Arrange
        var resultado = Resultado<string>.Falha("inválido", TipoErro.Validacao);

        // Act
        var resposta = _controlador.TestarCriadoEm("ObterPorId", new { id = 1 }, resultado);

        // Assert
        resposta.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion
}
