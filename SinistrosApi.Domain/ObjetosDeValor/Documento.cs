using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Enumeradores;

namespace SinistrosApi.Domain.ObjetosDeValor;

public sealed class Documento
{
    public TipoDocumento Tipo { get; }
    public string Numero { get; }

    public override string ToString() => Tipo == TipoDocumento.Cpf
        ? $"{Numero[..3]}.{Numero[3..6]}.{Numero[6..9]}-{Numero[9..11]}"
        : $"{Numero[..2]}.{Numero[2..5]}.{Numero[5..8]}/{Numero[8..12]}-{Numero[12..14]}";

    private Documento(TipoDocumento tipo, string numero)
    {
        Tipo = tipo;
        Numero = numero;
    }

    public static Resultado<Documento> Criar(TipoDocumento tipo, string? numero)
    {
        var numeroLimpo = new string((numero ?? string.Empty).Where(char.IsDigit).ToArray());

        var validacao = tipo switch
        {
            TipoDocumento.Cpf  => ValidarCpf(numeroLimpo),
            TipoDocumento.Cnpj => ValidarCnpj(numeroLimpo),
            _ => Resultado.Falha("Tipo de documento desconhecido.", TipoErro.Validacao)
        };

        if (validacao.EhFalha)
            return Resultado<Documento>.Falha(validacao.Erro!, validacao.TipoErro);

        return Resultado<Documento>.Sucesso(new Documento(tipo, numeroLimpo));
    }

    private static Resultado ValidarCpf(string numero)
    {
        if (numero.Length != 11)
            return Resultado.Falha("CPF deve conter 11 dígitos.", TipoErro.Validacao);

        if (!DigitosVerificadoresCpfValidos(numero))
            return Resultado.Falha("CPF inválido.", TipoErro.Validacao);

        return Resultado.Sucesso();
    }

    private static Resultado ValidarCnpj(string numero)
    {
        if (numero.Length != 14)
            return Resultado.Falha("CNPJ deve conter 14 dígitos.", TipoErro.Validacao);

        if (!DigitosVerificadoresCnpjValidos(numero))
            return Resultado.Falha("CNPJ inválido.", TipoErro.Validacao);

        return Resultado.Sucesso();
    }

    private static bool DigitosVerificadoresCpfValidos(string cpf)
    {
        if (cpf.Distinct().Count() == 1)
            return false;

        var d = cpf.Select(c => c - '0').ToArray();

        var soma = 0;
        for (var i = 0; i < 9; i++)
            soma += d[i] * (10 - i);

        var r1 = soma % 11;
        var digito1 = r1 < 2 ? 0 : 11 - r1;
        if (d[9] != digito1)
            return false;

        soma = 0;
        for (var i = 0; i < 10; i++)
            soma += d[i] * (11 - i);
        var r2 = soma % 11;

        var digito2 = r2 < 2 ? 0 : 11 - r2;

        return d[10] == digito2;
    }

    private static bool DigitosVerificadoresCnpjValidos(string cnpj)
    {
        if (cnpj.Distinct().Count() == 1)
            return false;

        var d = cnpj.Select(c => c - '0').ToArray();
        int[] pesos1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] pesos2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        var soma = pesos1.Select((p, i) => d[i] * p).Sum();
        var r1 = soma % 11;
        var digito1 = r1 < 2 ? 0 : 11 - r1;

        if (d[12] != digito1)
            return false;

        soma = pesos2.Select((p, i) => d[i] * p).Sum();
        var r2 = soma % 11;
        var digito2 = r2 < 2 ? 0 : 11 - r2;

        return d[13] == digito2;
    }

    public override bool Equals(object? obj)
        => obj is Documento outro && Tipo == outro.Tipo && Numero == outro.Numero;

    public override int GetHashCode() => HashCode.Combine(Tipo, Numero);
}
