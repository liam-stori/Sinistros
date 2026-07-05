using System.ComponentModel;
using System.Reflection;

namespace SinistrosApi.Domain.Comum;

public static class EnumExtensions
{
    public static string ObterDescricao(this Enum valor)
    {
        var membro = valor.GetType().GetField(valor.ToString());
        var descricao = membro?.GetCustomAttribute<DescriptionAttribute>();
        return descricao?.Description ?? valor.ToString();
    }
}
