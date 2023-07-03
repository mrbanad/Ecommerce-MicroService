using System.Reflection;
using System.Web;

namespace Hamkare.Settings;

public static class ThemeSetting
{
    public const string ThemeMainLayoutPath = "Hamkare.Pages.Themes.{0}.MainLayout";

    public static string ActiveTheme { get; private set; } = "GanjEsar";

    public static event Action OnChange;

    public static void SetTheme(string name)
    {
        ActiveTheme = name;

        NotifyStateChanged();
    }

    private static void NotifyStateChanged() => OnChange?.Invoke();

    public static Type? GetPage(string? name)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var path = $"Hamkare.Pages.Themes.{ActiveTheme}.Pages";
        if (string.IsNullOrEmpty(name) || name == "/")
        {
            return assembly.GetTypes().FirstOrDefault(type =>
                string.Equals(type.Name, "Index", StringComparison.OrdinalIgnoreCase) &&
                type.Namespace != null &&
                type.Namespace.StartsWith(path, StringComparison.OrdinalIgnoreCase));
        }

        var route = name.TrimStart('/').Split('/');

        if (route.Length == 1)
        {
            var pageName = route[0];
            return assembly.GetTypes().FirstOrDefault(type =>
                string.Equals(type.Name, pageName, StringComparison.OrdinalIgnoreCase) &&
                type.Namespace != null &&
                type.Namespace == path);
        }

        for (var i = 0; i < route.Length - 1; i++)
            path += "." + route[i];

        var page = route.Last();
        return assembly.GetTypes().FirstOrDefault(type =>
            string.Equals(type.Name, page, StringComparison.OrdinalIgnoreCase) &&
            type.Namespace != null &&
            type.Namespace == path);
    }

    public static Dictionary<string, string> GetParameters(string? parameters)
    {
        if (string.IsNullOrEmpty(parameters))
            return new Dictionary<string, string>();

        var queryParameters = HttpUtility.ParseQueryString(parameters);
        return queryParameters.AllKeys
            .ToDictionary(key => key, key => queryParameters[key]);
    }
}