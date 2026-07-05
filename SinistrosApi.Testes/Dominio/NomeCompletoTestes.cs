using FluentAssertions;
using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.ObjetosDeValor;
using Xunit;

namespace SinistrosApi.Testes.Dominio;

public class NomeCompletoTestes
{
    #region Criar

    [Fact]
    public void Dado_NomeESobrenomeValidos_Quando_Criar_Entao_RetornaSucesso()
    {
        // Act
        var resultado = NomeCompleto.Criar("João", "Silva");

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        resultado.Valor.Nome.Should().Be("João");
        resultado.Valor.Sobrenome.Should().Be("Silva");
    }

    [Theory]
    [InlineData("", "Silva")]
    [InlineData("   ", "Silva")]
    [InlineData("João", "")]
    [InlineData("João", "   ")]
    public void Dado_NomeOuSobrenomeVazioOuEspacos_Quando_Criar_Entao_RetornaFalha(string nome, string sobrenome)
    {
        // Act
        var resultado = NomeCompleto.Criar(nome, sobrenome);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.Validacao);
    }

    [Fact]
    public void Dado_NomeComEspacosNasExtremidades_Quando_Criar_Entao_AplicaTrim()
    {
        // Act
        var resultado = NomeCompleto.Criar("       João    ", "  Silva ");

        // Assert
        resultado.Valor.Nome.Should().Be("João");
        resultado.Valor.Sobrenome.Should().Be("Silva");
    }

    #endregion

    #region Equals e GetHashCode

    [Fact]
    public void Dado_DoisNomesComMesmoNomeESobrenome_Quando_Comparar_Entao_SaoIguais()
    {
        // Arrange
        var nome1 = NomeCompleto.Criar("João", "Silva").Valor;
        var nome2 = NomeCompleto.Criar("João", "Silva").Valor;

        // Act & Assert
        nome1.Equals(nome2).Should().BeTrue();
        nome1.GetHashCode().Should().Be(nome2.GetHashCode());
    }

    [Fact]
    public void Dado_NomesComSobrenomesDistintos_Quando_Comparar_Entao_SaoDiferentes()
    {
        // Arrange
        var nome1 = NomeCompleto.Criar("João", "Silva").Valor;
        var nome2 = NomeCompleto.Criar("João", "Santos").Valor;

        // Act & Assert
        nome1.Equals(nome2).Should().BeFalse();
    }

    [Fact]
    public void Dado_NomeComparadoComNull_Quando_Comparar_Entao_RetornaFalse()
    {
        // Arrange
        var nome = NomeCompleto.Criar("João", "Silva").Valor;

        // Act & Assert
        nome.Equals(null).Should().BeFalse();
        nome!.Equals("string").Should().BeFalse();
    }

    #endregion

    #region ToString

    [Fact]
    public void Dado_NomeCompleto_Quando_ChamarToString_Entao_RetornaNomeESobrenomeConcatenados()
    {
        // Arrange
        var nomeCompleto = NomeCompleto.Criar("João", "Silva").Valor;

        // Act
        var texto = nomeCompleto.ToString();

        // Assert
        texto.Should().Be("João Silva");
    }

    #endregion
}
