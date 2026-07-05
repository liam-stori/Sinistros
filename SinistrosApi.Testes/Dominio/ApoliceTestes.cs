using FluentAssertions;
using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Entidades;
using SinistrosApi.Domain.Enumeradores;
using Xunit;

namespace SinistrosApi.Testes.Dominio;

public class ApoliceTestes
{
    #region Criar

    [Fact]
    public void Dado_DatasValidas_Quando_Criar_Entao_RetornaSucesso()
    {
        // Arrange
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var clienteId = Guid.NewGuid();

        // Act
        var resultado = Apolice.Criar("APL-001", clienteId, Ramo.Auto, hoje.AddYears(-1), hoje.AddYears(1));

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        resultado.Valor.NumeroApolice.Should().Be("APL-001");
        resultado.Valor.ClienteId.Should().Be(clienteId);
        resultado.Valor.Ramo.Should().Be(Ramo.Auto);
        resultado.Valor.Status.Should().Be(StatusApolice.Ativa);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Dado_DataInicioNaoAnteriorADataFim_Quando_Criar_Entao_RetornaFalha(int diasAMaisNoInicio)
    {
        // Arrange
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var inicio = hoje.AddDays(diasAMaisNoInicio);

        // Act
        var resultado = Apolice.Criar("APL-001", Guid.NewGuid(), Ramo.Auto, inicio, hoje);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.RegraDeNegocio);
    }

    #endregion

    #region EstaVigenteEAtiva

    [Fact]
    public void Dado_ApoliceAtivaDentroVigencia_Quando_VerificarVigencia_Entao_RetornaVerdadeiro()
    {
        // Arrange
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var apolice = Apolice.Criar("APL-001", Guid.NewGuid(), Ramo.Auto,
            hoje.AddYears(-1), hoje.AddYears(1)).Valor;

        // Act
        var vigente = apolice.EstaVigenteEAtiva(hoje);

        // Assert
        vigente.Should().BeTrue();
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(2)]
    public void Dado_ApoliceAtiva_Quando_VerificarVigenciaForaDoPeriodo_Entao_RetornaFalso(int anosDesvio)
    {
        // Arrange
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var apolice = Apolice.Criar("APL-001", Guid.NewGuid(), Ramo.Auto,
            hoje.AddYears(-1), hoje.AddYears(1)).Valor;
        var dataForaDoPeriodo = hoje.AddYears(anosDesvio);

        // Act
        var vigente = apolice.EstaVigenteEAtiva(dataForaDoPeriodo);

        // Assert
        vigente.Should().BeFalse();
    }

    [Theory]
    [InlineData(StatusApolice.Inativa)]
    [InlineData(StatusApolice.Vencida)]
    public void Dado_ApoliceNaoAtiva_Quando_VerificarVigencia_Entao_RetornaFalso(StatusApolice statusFinal)
    {
        // Arrange
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var apolice = Apolice.Criar("APL-001", Guid.NewGuid(), Ramo.Auto,
            hoje.AddYears(-1), hoje.AddYears(1)).Valor;

        if (statusFinal == StatusApolice.Inativa)
            apolice.Inativar();
        else
            apolice.Vencer();

        // Act
        var vigente = apolice.EstaVigenteEAtiva(hoje);

        // Assert
        vigente.Should().BeFalse();
    }

    #endregion

    #region Vencer

    [Fact]
    public void Dado_ApoliceAtiva_Quando_Vencer_Entao_AlteraStatusParaVencida()
    {
        // Arrange
        var apolice = Fabrica.ApoliceAtiva();

        // Act
        var resultado = apolice.Vencer();

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        apolice.Status.Should().Be(StatusApolice.Vencida);
    }

    [Theory]
    [InlineData(StatusApolice.Inativa)]
    [InlineData(StatusApolice.Vencida)]
    public void Dado_ApoliceNaoAtiva_Quando_Vencer_Entao_RetornaFalha(StatusApolice statusFinal)
    {
        // Arrange
        var apolice = Fabrica.ApoliceAtiva();

        if (statusFinal == StatusApolice.Inativa)
            apolice.Inativar();
        else
            apolice.Vencer();

        // Act
        var resultado = apolice.Vencer();

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.RegraDeNegocio);
    }

    #endregion

    #region Inativar

    [Fact]
    public void Dado_ApoliceAtiva_Quando_Inativar_Entao_AlteraStatusParaInativa()
    {
        // Arrange
        var apolice = Fabrica.ApoliceAtiva();

        // Act
        var resultado = apolice.Inativar();

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        apolice.Status.Should().Be(StatusApolice.Inativa);
    }

    [Theory]
    [InlineData(StatusApolice.Vencida)]
    [InlineData(StatusApolice.Inativa)]
    public void Dado_ApoliceNaoAtiva_Quando_Inativar_Entao_RetornaFalha(StatusApolice statusFinal)
    {
        // Arrange
        var apolice = Fabrica.ApoliceAtiva();

        if (statusFinal == StatusApolice.Vencida)
            apolice.Vencer();
        else
            apolice.Inativar();

        // Act
        var resultado = apolice.Inativar();

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.RegraDeNegocio);
    }

    #endregion
}
