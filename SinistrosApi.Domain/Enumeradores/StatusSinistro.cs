using System.ComponentModel;

namespace SinistrosApi.Domain.Enumeradores;

public enum StatusSinistro
{
    Aberto = 1,

    [Description("Em Análise")]
    EmAnalise,

    Aprovado,
    Negado,
    Encerrado
}
