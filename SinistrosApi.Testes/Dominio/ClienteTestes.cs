using FluentAssertions;
using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Entidades;
using SinistrosApi.Domain.Enumeradores;
using Xunit;

namespace SinistrosApi.Testes.Dominio;

public class ClienteTestes
{
    #region Criar — CPF

    [Fact]
    public void Dado_CpfValidoEMaiorDeIdade_Quando_Criar_Entao_RetornaSucesso()
    {
        // Arrange
        var dataNascimento = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-30);

        // Act
        var resultado = Cliente.Criar("João", "Silva", TipoDocumento.Cpf, "52998224725", dataNascimento);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        resultado.Valor.Nome.ToString().Should().Be("João Silva");
        resultado.Valor.Documento.Tipo.Should().Be(TipoDocumento.Cpf);
        resultado.Valor.DataReferencia.Should().Be(dataNascimento);
    }

    [Fact]
    public void Dado_CpfComExatamente18Anos_Quando_Criar_Entao_RetornaSucesso()
    {
        // Arrange
        var dataNascimento = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-18);

        // Act
        var resultado = Cliente.Criar("João", "Silva", TipoDocumento.Cpf, "52998224725", dataNascimento);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
    }

    [Fact]
    public void Dado_CpfComMenorDeIdade_Quando_Criar_Entao_RetornaFalhaDeValidacao()
    {
        // Arrange
        var dataNascimento = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-17);

        // Act
        var resultado = Cliente.Criar("João", "Silva", TipoDocumento.Cpf, "52998224725", dataNascimento);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.Validacao);
    }

    #endregion

    #region Criar — CNPJ

    [Fact]
    public void Dado_CnpjValido_Quando_Criar_Entao_RetornaSucesso()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-35);

        // Act
        var resultado = Cliente.Criar("Empresa", "Ltda", TipoDocumento.Cnpj, "11222333000181", dataReferencia);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
        resultado.Valor.Documento.Tipo.Should().Be(TipoDocumento.Cnpj);
    }

    [Fact]
    public void Dado_CnpjComDataReferenciaRecente_Quando_Criar_Entao_NaoValidaMaioridade()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1);

        // Act
        var resultado = Cliente.Criar("Empresa", "Ltda", TipoDocumento.Cnpj, "11222333000181", dataReferencia);

        // Assert
        resultado.EhSucesso.Should().BeTrue();
    }

    #endregion

    #region Criar — dados inválidos

    [Fact]
    public void Dado_NomeVazio_Quando_Criar_Entao_RetornaFalha()
    {
        // Arrange
        var dataNascimento = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-25);

        // Act
        var resultado = Cliente.Criar("", "Silva", TipoDocumento.Cpf, "52998224725", dataNascimento);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.Validacao);
    }

    [Fact]
    public void Dado_CpfInvalido_Quando_Criar_Entao_RetornaFalha()
    {
        // Arrange
        var dataNascimento = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-25);

        // Act
        var resultado = Cliente.Criar("João", "Silva", TipoDocumento.Cpf, "00000000000", dataNascimento);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.Validacao);
    }

    [Fact]
    public void Dado_CnpjInvalido_Quando_Criar_Entao_RetornaFalha()
    {
        // Arrange
        var dataReferencia = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-3);

        // Act
        var resultado = Cliente.Criar("João", "Silva", TipoDocumento.Cpf, "00000000000", dataReferencia);

        // Assert
        resultado.EhFalha.Should().BeTrue();
        resultado.TipoErro.Should().Be(TipoErro.Validacao);
    }

    #endregion
}
