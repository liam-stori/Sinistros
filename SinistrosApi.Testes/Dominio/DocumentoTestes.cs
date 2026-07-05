using FluentAssertions;
using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Enumeradores;
using SinistrosApi.Domain.ObjetosDeValor;
using Xunit;

namespace SinistrosApi.Testes.Dominio;

public class DocumentoTestes
{
    #region CPF — válido

    [Fact]
    public void Dado_CpfValido_Quando_Criar_Entao_RetornaSucesso()
    {
        // Arrange
        const string cpf = "52998224725";

        // Act
        var resultado = Documento.Criar(TipoDocumento.Cpf, cpf);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        resultado.Valor.Tipo.Should().Be(TipoDocumento.Cpf);
        resultado.Valor.Numero.Should().Be(cpf);
    }

    [Fact]
    public void Dado_CpfValidoComMascara_Quando_Criar_Entao_IgnoraMascaraERetornaSucesso()
    {
        // Arrange
        const string cpfComMascara = "529.982.247-25";

        // Act
        var resultado = Documento.Criar(TipoDocumento.Cpf, cpfComMascara);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        resultado.Valor.Numero.Should().Be("52998224725");
    }

    #endregion

    #region CPF — inválido

    [Theory]
    [InlineData("5299822472")]
    [InlineData("529982247250")]
    [InlineData("11111111111")]
    [InlineData("00000000000")]
    [InlineData("99999999999")]
    [InlineData("letras")]
    public void Dado_CpfInvalido_Quando_Criar_Entao_RetornaFalha(string? cpf)
    {
        // Act
        var resultado = Documento.Criar(TipoDocumento.Cpf, cpf);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.Validacao);
    }

    [Fact]
    public void Dado_CpfComDigitoVerificadorInvalido_Quando_Criar_Entao_RetornaFalha()
    {
        // Arrange
        const string cpfInvalido = "52998224726";

        // Act
        var resultado = Documento.Criar(TipoDocumento.Cpf, cpfInvalido);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.Validacao);
    }

    #endregion

    #region CNPJ — válido

    [Fact]
    public void Dado_CnpjValido_Quando_Criar_Entao_RetornaSucesso()
    {
        // Arrange
        const string cnpj = "11222333000181";

        // Act
        var resultado = Documento.Criar(TipoDocumento.Cnpj, cnpj);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        resultado.Valor.Tipo.Should().Be(TipoDocumento.Cnpj);
        resultado.Valor.Numero.Should().Be(cnpj);
    }

    [Fact]
    public void Dado_CnpjValidoComMascara_Quando_Criar_Entao_IgnoraMascaraERetornaSucesso()
    {
        // Arrange
        const string cnpjComMascara = "11.222.333/0001-81";

        // Act
        var resultado = Documento.Criar(TipoDocumento.Cnpj, cnpjComMascara);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        resultado.Valor.Numero.Should().Be("11222333000181");
    }

    #endregion

    #region CNPJ — inválido

    [Theory]
    [InlineData("1122233300018")]
    [InlineData("112223330001810")]
    public void Dado_CnpjComQuantidadeErradaDeDigitos_Quando_Criar_Entao_RetornaFalha(string cnpj)
    {
        // Act
        var resultado = Documento.Criar(TipoDocumento.Cnpj, cnpj);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.Validacao);
    }

    [Fact]
    public void Dado_CnpjComPrimeiroDigitoVerificadorInvalido_Quando_Criar_Entao_RetornaFalha()
    {
        // Arrange
        const string cnpjInvalido = "11222333000191";

        // Act
        var resultado = Documento.Criar(TipoDocumento.Cnpj, cnpjInvalido);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.Validacao);
    }

    [Fact]
    public void Dado_CnpjComSegundoDigitoVerificadorInvalido_Quando_Criar_Entao_RetornaFalha()
    {
        // Arrange
        const string cnpjInvalido = "11222333000182";

        // Act
        var resultado = Documento.Criar(TipoDocumento.Cnpj, cnpjInvalido);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.Validacao);
    }

    [Fact]
    public void Dado_CnpjHomogeneo_Quando_Criar_Entao_RetornaFalha()
    {
        // Arrange
        const string cnpjHomogeneo = "11111111111111";

        // Act
        var resultado = Documento.Criar(TipoDocumento.Cnpj, cnpjHomogeneo);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.Validacao);
    }

    #endregion

    #region TipoDocumento desconhecido

    [Fact]
    public void Dado_TipoDocumentoDesconhecido_Quando_Criar_Entao_RetornaFalha()
    {
        // Act
        var resultado = Documento.Criar((TipoDocumento)99, "12345");

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.Validacao);
    }

    #endregion

    #region Equals e GetHashCode

    [Fact]
    public void Dado_DoisDocumentosComMesmoTipoENumero_Quando_Comparar_Entao_SaoIguais()
    {
        // Arrange
        var doc1 = Documento.Criar(TipoDocumento.Cpf, "52998224725").Valor;
        var doc2 = Documento.Criar(TipoDocumento.Cpf, "52998224725").Valor;

        // Act & Assert
        doc1.Equals(doc2).Should().BeTrue();
        doc1.GetHashCode().Should().Be(doc2.GetHashCode());
    }

    [Fact]
    public void Dado_DocumentosComNumerosDistintos_Quando_Comparar_Entao_SaoDiferentes()
    {
        // Arrange
        var doc1 = Documento.Criar(TipoDocumento.Cpf, "52998224725").Valor;
        var doc2 = Documento.Criar(TipoDocumento.Cpf, "07893840718").Valor;

        // Act & Assert
        doc1.Equals(doc2).Should().BeFalse();
    }

    [Fact]
    public void Dado_DocumentosComTiposDistintos_Quando_Comparar_Entao_SaoDiferentes()
    {
        // Arrange
        var docCpf = Documento.Criar(TipoDocumento.Cpf, "52998224725").Valor;

        // Act & Assert
        docCpf.Equals(null).Should().BeFalse();
        docCpf!.Equals("string").Should().BeFalse();
    }

    #endregion

    #region ToString

    [Fact]
    public void Dado_CpfValido_Quando_ChamarToString_Entao_RetornaFormatado()
    {
        // Arrange
        var documento = Documento.Criar(TipoDocumento.Cpf, "52998224725").Valor;

        // Act
        var formatado = documento.ToString();

        // Assert
        formatado.Should().Be("529.982.247-25");
    }

    [Fact]
    public void Dado_CnpjValido_Quando_ChamarToString_Entao_RetornaFormatado()
    {
        // Arrange
        var documento = Documento.Criar(TipoDocumento.Cnpj, "11222333000181").Valor;

        // Act
        var formatado = documento.ToString();

        // Assert
        formatado.Should().Be("11.222.333/0001-81");
    }

    #endregion
}
