using System.ComponentModel;
using System.Reflection;

public static class EnumExtensions
{
    public static string GetDescription<T>(this T enumValue) where T : Enum
    {
        var field = typeof(T).GetField(enumValue.ToString());
        if (field == null)
            return "Sem descrição";

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute == null ? enumValue.ToString() : attribute.Description; // Retorna o nome da enumeração se não houver atributo de descrição
    }
}