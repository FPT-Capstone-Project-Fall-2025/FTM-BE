using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FTM.API.Extensions
{
    public static class OpenTelemetryConfiguration
    {
        public static void AddOpenTelemetryConfig(this IServiceCollection serrvices, IHostEnvironment env)
        {
            serrvices.AddOpenTelemetry()
                .WithTracing(tracer =>
                {
                    tracer
                        // Service metadata
                        .SetResourceBuilder(ResourceBuilder.CreateDefault()
                            .AddService("FamilyTree")
                            .AddAttributes(new[]
                            {
                                new KeyValuePair<string, object>("environment", env.EnvironmentName),
                                new KeyValuePair<string, object>("service.version", "1.0.0"),
                                new KeyValuePair<string, object>("service.instance.id", Environment.MachineName)
                            })
                        )

                        // Incoming HTTP requests (ASP.NET Core)
                        .AddAspNetCoreInstrumentation(options =>
                        {
                            // Add exception details to spans
                            options.EnrichWithException = (activity, exception) =>
                            {
                                activity?.SetTag("exception.type", exception.GetType().Name);
                                activity?.SetTag("exception.message", exception.Message);
                            };

                            // Filter out health checks and static content
                            options.Filter = context =>
                            {
                                var path = context.Request.Path.Value;
                                return path != null && !path.Contains("health") && !path.Contains("swagger");
                            };

                            options.EnrichWithHttpRequest = (activity, request) =>
                            {
                                activity.DisplayName = $"API {request.Path}";
                                activity.SetTag("http.route", request.Path);
                            };
                        })

                        // Outgoing HTTP requests
                        .AddHttpClientInstrumentation(options =>
                        {
                            options.FilterHttpRequestMessage = request =>
                            {
                                return !request.RequestUri!.AbsolutePath.Contains("health");
                            };
                        })


                        //PostgreSQL instrumentation
                        .AddNpgsql()

                        .AddEntityFrameworkCoreInstrumentation(options =>
                        {
                            options.EnrichWithIDbCommand = (activity, command) =>
                            {
                                if (activity == null || command == null) return;

                                activity.SetTag("db.system", "postgresql");
                                activity.SetTag("db.name", command.Connection?.Database);
                                activity.SetTag("db.statement", command.CommandText);

                                var sqlSnippet = command.CommandText.Length > 50
                                    ? command.CommandText.Substring(0, 50) + "..."
                                    : command.CommandText;
                                activity.DisplayName = $"ft.query:{sqlSnippet.Split(" ").First()}";
                            };
                        })

                        // Exporters
                        .AddConsoleExporter() // debug traces locally
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri("http://localhost:4318/v1/traces");
                            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                        });
                });
        }
    }
}
