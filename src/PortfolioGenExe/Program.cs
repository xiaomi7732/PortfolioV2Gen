using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using PortfolioGenExe.YouTube;

namespace PortfolioGenExe
{
    internal class Program
    {
        private readonly YouTubeFeedReader _youTubeFeedReader;
        private readonly IEnumerable<IGen> _generators;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ILogger<Program> _logger;
        private static readonly string _targetHtml;

        static Program()
        {
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string settingsFilePath =Path.GetFullPath(Path.Combine(basePath, "settings.jsonc"));
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile(settingsFilePath, optional: false).Build();
            _targetHtml = Path.GetFullPath(configuration["htmlTarget"] ?? throw new InvalidOperationException("html file target is not specified."));
        }

        public Program(
            YouTubeFeedReader youTubeFeedReader,
            IEnumerable<IGen> generators,
            JsonSerializerOptions serializerOptions,
            ILogger<Program> logger)
        {
            logger.LogInformation("HTML target: {target}", _targetHtml);

            _youTubeFeedReader = youTubeFeedReader;
            _generators = generators ?? throw new ArgumentNullException(nameof(generators));
            _serializerOptions = serializerOptions ?? throw new ArgumentNullException(nameof(serializerOptions));
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
                await booster.RunBeforeGenAsync(default);

                TemplateUpdateService templateUpdater = provider.GetRequiredService<TemplateUpdateService>();

                logger.LogInformation("Logging system is ready.");
                string template = await File.ReadAllTextAsync(_targetHtml);

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

                await File.WriteAllTextAsync(_targetHtml, template, Encoding.UTF8);
            }
        }

        static IServiceCollection AddServices()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(logging => {
                logging.AddSimpleConsole().PrettyIt();
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
            serviceCollection.TryAddSingleton<YouTubeFeedReader>();

            return serviceCollection;
        }

        public async Task RunBeforeGenAsync(CancellationToken cancellationToken)
        {
            Task t1 = UpdateFromYouTubeFeedForData("Data/NewVideos.json", cancellationToken);
            Task t2 = UpdateFromYouTubeFeedForData("Data/NewVideoThumbnail.json", cancellationToken);

            await Task.WhenAll(t1, t2);
        }

        private async Task UpdateFromYouTubeFeedForData(string targetFile, CancellationToken cancellationToken)
        {
            DataMeta? data = null;
            JsonSerializerOptions serializerOptions = new JsonSerializerOptions(_serializerOptions);
            serializerOptions.WriteIndented = true;
            using (Stream inputStream = File.OpenRead(targetFile))
            {
                data = await JsonSerializer.DeserializeAsync<DataMeta>(inputStream, serializerOptions, cancellationToken).ConfigureAwait(false);
            }
            if (data is null)
            {
                throw new InvalidOperationException("No json source for new Videos.");
            }

            List<IDictionary<string, string>> newData = new List<IDictionary<string, string>>();
            await foreach (YouTubeItem item in _youTubeFeedReader.GetYouTubeItemsAsync(cancellationToken))
            {
                newData.Add(new Dictionary<string, string>
                {
                    ["text"] = item.Title,
                    ["link"] = item.LinkToVideo.ToString(),
                    ["thumbnailUri"] = item.ThumbnailUri.ToString(),
                });
            }

            data.Data = newData;

            string tempFileName = Path.GetTempFileName();
            using (Stream outputStream = File.OpenWrite(tempFileName))
            {
                await JsonSerializer.SerializeAsync(outputStream, data, serializerOptions, cancellationToken).ConfigureAwait(false);
            }
            File.Move(tempFileName, targetFile, overwrite: true);
        }

        public IEnumerable<(string Target, string HtmlPart)> Generate(DataMeta data)
        {
            foreach (IGen generator in _generators)
            {
                _logger.LogTrace("Running generator: {generator}", generator.GetType());
                if (generator.CanGen(data))
                {
                    _logger.LogDebug("Generator hit: {generator}", generator.GetType());
                    yield return (data.Target, generator.Generate(data));
                    break;
                }
            }
        }
    }
}