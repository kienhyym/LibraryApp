using System.Text.Json;

namespace LibraryApp.Helpers;

public static class SessionHelper
{
    public static void SetObject<T>(
        ISession session,
        string key,
        T value)
    {
        session.SetString(
            key,
            JsonSerializer.Serialize(value));
    }

    public static T? GetObject<T>(
        ISession session,
        string key)
    {
        var value = session.GetString(key);

        return value == null
            ? default
            : JsonSerializer.Deserialize<T>(value);
    }
}