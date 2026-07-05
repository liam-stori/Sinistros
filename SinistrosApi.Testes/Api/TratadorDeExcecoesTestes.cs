using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SinistrosApi.Api.Middleware;
using System.Text.Json;
using Xunit;

namespace SinistrosApi.Testes.Api;

public class TratadorDeExcecoesTestes
{
    #region Sem exceção

    [Fact]
    public async Task Dado_NextSemExcecao_Quando_Invocar_Entao_PassaAdiante()
    {
        // Arrange
        var nextFoiChamado = false;

        Task next(HttpContext _)
        {
            nextFoiChamado = true;
            return Task.CompletedTask;
        }

        var middleware = new TratadorDeExcecoes(next);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextFoiChamado.Should().BeTrue();
        context.Response.StatusCode.Should().Be(200);
    }

    #endregion

    #region Com exceção

    [Fact]
    public async Task Dado_NextLancaExcecao_Quando_Invocar_Entao_Retorna500ComMensagemEsperada()
    {
        // Arrange
        Task next(HttpContext _) => throw new Exception("falha inesperada");

        var middleware = new TratadorDeExcecoes(next);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(500);
        context.Response.ContentType.Should().Contain("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await JsonDocument.ParseAsync(context.Response.Body);
        body.RootElement.GetProperty("erro").GetString().Should().Be("Ocorreu um erro inesperado.");
    }

    #endregion
}
