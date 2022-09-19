using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace PortfolioGenExe
{
    internal class Program
    {
        private readonly IEnumerable<IGen> _generators;
        private readonly ILogger<Program> _logger;

        public Program(
            IEnumerable<IGen> generators,
            ILogger<Program> logger)
        {
            _generators = generators ?? throw new ArgumentNullException(nameof(generators));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        static async Task Main(string[] args)
        {
            IServiceCollection services = AddServices();
            using (ServiceProvider provider = services.BuildServiceProvider())
            {
                ILogger logger = provider.GetRequiredService<ILogger<Program>>();
                JsonSerializerOptions serializingOptions = provider.GetRequiredService<JsonSerializerOptions>();
                Program booster = provider.GetRequiredService<Program>();
                TemplateUpdateService templateUpdater = provider.GetRequiredService<TemplateUpdateService>();

                logger.LogInformation("Logging system is ready.");
                string template = await File.ReadAllTextAsync(@"D:\Repos\PortofolioV2\index.html");

                foreach (string file in Directory.EnumerateFiles("Data", "*.json"))
                {
                    using (Stream input = File.OpenRead(file))
                    {
                        DataMeta? dataMeta = JsonSerializer.Deserialize<DataMeta>(input, serializingOptions);
                        if (dataMeta is not null)
                        {
                            logger.LogDebug("DataMeta: {meta}", dataMeta.Default);

                            foreach (var result in booster.Generate(dataMeta))
                            {
                                logger.LogTrace(result.Target);
                                logger.LogTrace(result.HtmlPart);

                                templateUpdater.TryUpdate(template, result.Target, result.HtmlPart, out template);
                            }
                        }
                    }
                }

                await File.WriteAllTextAsync(@"D:\Repos\PortofolioV2\index.html", template, Encoding.UTF8);
            }
        }

        static IServiceCollection AddServices()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(configure =>
            {
                configure.AddSimpleConsole(opt =>
                {
                    opt.SingleLine = true;
                });
            });

            serviceCollection.AddSingleton<JsonSerializerOptions>(_ =>
            {
                JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
                options.PropertyNameCaseInsensitive = true;

                return options;
            });

            serviceCollection.TryAddSingleton<Program>();
            serviceCollection.TryAddListGenServices();

            serviceCollection.TryAddSingleton<TemplateUpdateService>();

            return serviceCollection;
        }

        public IEnumerable<(string Target, string HtmlPart)> Generate(DataMeta data)
        {
            foreach (IGen generator in _generators)
            {
                _logger.LogTrace("Running generator: {generator}", generator.GetType());
                if (generator.CanGen(data))
                {
                    _logger.LogInformation("Generator hit: {generator}", generator.GetType());
                    yield return (data.Target, generator.Generate(data));
                    break;
                }
            }
        }
    }
}