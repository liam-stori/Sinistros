using FluentAssertions;
using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Entidades;
using SinistrosApi.Domain.Enumeradores;
using Xunit;

namespace SinistrosApi.Testes.Dominio;

public class SinistroTestes
{
    #region Abrir

    [Fact]
    public void Dado_ApoliceAtiva_Quando_Abrir_Entao_RetornaSucesso()
    {
        // Arrange
        var apolice = Fabrica.ApoliceAtiva();

        // Act
        var resultado = Sinistro.Abrir(apolice, 5000m, DateTime.UtcNow.AddDays(-1));

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        resultado.Valor.Status.Should().Be(StatusSinistro.Aberto);
        resultado.Valor.ValorEstimado.Should().Be(5000m);
        resultado.Valor.ApoliceId.Should().Be(apolice.Id);
    }

    [Fact]
    public void Dado_ApoliceAtiva_Quando_Abrir_Entao_RegistraHistoricoInicialComStatusAberto()
    {
        // Arrange
        var apolice = Fabrica.ApoliceAtiva();
        var dataAbertura = DateTime.UtcNow.AddDays(-2);

        // Act
        var resultado = Sinistro.Abrir(apolice, 5000m, dataAbertura);

        // Assert
        resultado.Valor.Historico.Should().HaveCount(1);
        var entrada = resultado.Valor.Historico.Single();
        entrada.StatusAnterior.Should().BeNull();
        entrada.StatusNovo.Should().Be(StatusSinistro.Aberto);
        entrada.DataAlteracao.Should().Be(dataAbertura);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    public void Dado_ValorEstimadoInvalido_Quando_Abrir_Entao_RetornaFalha(decimal valor)
    {
        // Arrange
        var apolice = Fabrica.ApoliceAtiva();

        // Act
        var resultado = Sinistro.Abrir(apolice, valor, DateTime.UtcNow.AddDays(-1));

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.RegraDeNegocio);
    }

    [Theory]
    [InlineData(StatusApolice.Inativa)]
    [InlineData(StatusApolice.Vencida)]
    public void Dado_ApoliceNaoAtiva_Quando_Abrir_Entao_RetornaFalhaDeNegocio(StatusApolice statusFinal)
    {
        // Arrange
        var apolice = Fabrica.ApoliceAtiva();

        if (statusFinal == StatusApolice.Inativa)
            apolice.Inativar();
        else
            apolice.Vencer();

        // Act
        var resultado = Sinistro.Abrir(apolice, 5000m, DateTime.UtcNow.AddDays(-1));

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.RegraDeNegocio);
    }

    [Fact]
    public void Dado_ApoliceComVigenciaFutura_Quando_AbrirNaDataDeHoje_Entao_RetornaFalha()
    {
        // Arrange
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var apolice = Apolice.Criar("APL-FUTURA", Guid.NewGuid(), Ramo.Auto,
            hoje.AddMonths(1), hoje.AddYears(1)).Valor;

        // Act
        var resultado = Sinistro.Abrir(apolice, 5000m, DateTime.UtcNow);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.RegraDeNegocio);
    }

    #endregion

    #region AtualizarStatus — Transições inválidas

    [Theory]
    [InlineData(StatusSinistro.Aprovado)]
    [InlineData(StatusSinistro.Negado)]
    [InlineData(StatusSinistro.Encerrado)]
    public void Dado_SinistroAberto_Quando_TransitarParaStatusInvalido_Entao_RetornaFalhaDeNegocio(StatusSinistro novoStatus)
    {
        // Arrange
        var sinistro = Fabrica.SinistroAberto();

        // Act
        var resultado = sinistro.AtualizarStatus(novoStatus, "motivo qualquer", 1000m, DateTime.UtcNow);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.RegraDeNegocio);
        sinistro.Status.Should().Be(StatusSinistro.Aberto);
    }

    [Theory]
    [InlineData(StatusSinistro.Aberto)]
    [InlineData(StatusSinistro.Encerrado)]
    public void Dado_SinistroEmAnalise_Quando_TransitarParaStatusInvalido_Entao_RetornaFalhaDeNegocio(StatusSinistro novoStatus)
    {
        // Arrange
        var sinistro = Fabrica.SinistroEmAnalise();

        // Act
        var resultado = sinistro.AtualizarStatus(novoStatus, "motivo qualquer", 1000m, DateTime.UtcNow);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.RegraDeNegocio);
        sinistro.Status.Should().Be(StatusSinistro.EmAnalise);
    }

    [Fact]
    public void Dado_SinistroEncerrado_Quando_TentarTransitar_Entao_RetornaFalhaEStatusNaoMuda()
    {
        // Arrange
        var sinistro = Fabrica.SinistroAprovado();
        sinistro.AtualizarStatus(StatusSinistro.Encerrado, null, 4000m, DateTime.UtcNow.AddDays(-1));

        // Act
        var resultado = sinistro.AtualizarStatus(StatusSinistro.EmAnalise, null, null, DateTime.UtcNow);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        sinistro.Status.Should().Be(StatusSinistro.Encerrado);
    }

    #endregion

    #region AtualizarStatus — Transições válidas

    [Fact]
    public void Dado_SinistroAberto_Quando_TransitarParaEmAnalise_Entao_RetornaSucesso()
    {
        // Arrange
        var sinistro = Fabrica.SinistroAberto();

        // Act
        var resultado = sinistro.AtualizarStatus(StatusSinistro.EmAnalise, null, null, DateTime.UtcNow);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        sinistro.Status.Should().Be(StatusSinistro.EmAnalise);
    }

    [Fact]
    public void Dado_SinistroEmAnalise_Quando_NegadoSemMotivo_Entao_RetornaFalhaDeNegocio()
    {
        // Arrange
        var sinistro = Fabrica.SinistroEmAnalise();

        // Act
        var resultado = sinistro.AtualizarStatus(StatusSinistro.Negado, null, null, DateTime.UtcNow);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.RegraDeNegocio);
        sinistro.Status.Should().Be(StatusSinistro.EmAnalise);
    }

    [Fact]
    public void Dado_SinistroEmAnalise_Quando_NegadoComMotivo_Entao_SalvaMotivoERetornaSucesso()
    {
        // Arrange
        var sinistro = Fabrica.SinistroEmAnalise();
        const string motivo = "Documentação insuficiente para comprovação do sinistro.";

        // Act
        var resultado = sinistro.AtualizarStatus(StatusSinistro.Negado, motivo, null, DateTime.UtcNow);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        sinistro.Status.Should().Be(StatusSinistro.Negado);
        sinistro.MotivoNegativa.Should().Be(motivo);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void Dado_SinistroAprovado_Quando_EncerradoComValorAprovadoInvalido_Entao_RetornaFalhaDeNegocio(double? valorDouble)
    {
        // Arrange
        var sinistro = Fabrica.SinistroAprovado();
        decimal? valorAprovado = valorDouble.HasValue ? (decimal?)valorDouble.Value : null;

        // Act
        var resultado = sinistro.AtualizarStatus(StatusSinistro.Encerrado, null, valorAprovado, DateTime.UtcNow);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.RegraDeNegocio);
        sinistro.Status.Should().Be(StatusSinistro.Aprovado);
    }

    [Fact]
    public void Dado_SinistroAprovado_Quando_EncerradoComValorAprovado_Entao_SalvaValorERetornaSucesso()
    {
        // Arrange
        var sinistro = Fabrica.SinistroAprovado();

        // Act
        var resultado = sinistro.AtualizarStatus(StatusSinistro.Encerrado, null, 4500m, DateTime.UtcNow);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        sinistro.Status.Should().Be(StatusSinistro.Encerrado);
        sinistro.ValorAprovado.Should().Be(4500m);
    }

    #endregion

    #region Historico

    [Fact]
    public void Dado_SinistroAberto_Quando_TransitarParaEmAnalise_Entao_HistoricoAcumulaTransicao()
    {
        // Arrange
        var sinistro = Fabrica.SinistroAberto();
        var dataTransicao = DateTime.UtcNow;

        // Act
        sinistro.AtualizarStatus(StatusSinistro.EmAnalise, null, null, dataTransicao);

        // Assert
        sinistro.Historico.Should().HaveCount(2);
        var transicao = sinistro.Historico.Last();
        transicao.StatusAnterior.Should().Be(StatusSinistro.Aberto);
        transicao.StatusNovo.Should().Be(StatusSinistro.EmAnalise);
        transicao.DataAlteracao.Should().Be(dataTransicao);
    }

    #endregion
}
