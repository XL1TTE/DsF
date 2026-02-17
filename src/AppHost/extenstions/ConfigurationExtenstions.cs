
using Microsoft.Extensions.Configuration;

namespace Extenstions
{
    internal static class ConfigurationExtenstions
    {
        internal static T? FromConfig<T>(this IDistributedApplicationBuilder builder, string path)
        {
            return builder.Configuration.GetValue<T>(path);
        }
    }
}
