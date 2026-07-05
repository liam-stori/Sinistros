using System.Diagnostics.CodeAnalysis;
using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Enumeradores;
using SinistrosApi.Domain.ObjetosDeValor;

namespace SinistrosApi.Domain.Entidades;

public class Cliente : EntidadeBase
{
    public NomeCompleto Nome { get; private set; } = null!;
    public Documento Documento { get; private set; } = null!;
    /// <summary>
    /// Para CPF (pessoa física): data de nascimento. Usada para validação de maioridade (18+).
    /// Para CNPJ (pessoa jurídica): data de referência cadastral (ex: fundação). Sem validação de idade.
    /// </summary>
    public DateOnly DataReferencia { get; private set; }

    [ExcludeFromCodeCoverage]
    protected Cliente() { }

    private Cliente(NomeCompleto nome, Documento documento, DateOnly dataReferencia)
    {
        Nome = nome;
        Documento = documento;
        DataReferencia = dataReferencia;
    }

    public static Resultado<Cliente> Criar(string nome, string sobrenome, TipoDocumento tipoDocumento,
        string numeroDocumento, DateOnly dataReferencia)
    {
        var resultadoNome = NomeCompleto.Criar(nome, sobrenome);
        if (resultadoNome.EhFalha)
            return Resultado<Cliente>.Falha(resultadoNome.Erro!, resultadoNome.TipoErro);

        var resultadoDocumento = Documento.Criar(tipoDocumento, numeroDocumento);
        if (resultadoDocumento.EhFalha)
            return Resultado<Cliente>.Falha(resultadoDocumento.Erro!, resultadoDocumento.TipoErro);

        if (tipoDocumento == TipoDocumento.Cpf && dataReferencia.AddYears(18) > DateOnly.FromDateTime(DateTime.UtcNow))
            return Resultado<Cliente>.Falha("O titular deve ser maior de idade.", TipoErro.Validacao);

        return Resultado<Cliente>.Sucesso(new Cliente(resultadoNome.Valor, resultadoDocumento.Valor, dataReferencia));
    }
}
