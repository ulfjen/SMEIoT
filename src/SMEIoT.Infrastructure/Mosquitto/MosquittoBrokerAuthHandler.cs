using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using SMEIoT.Infrastructure.Data;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace SMEIoT.Infrastructure.Mosquitto
{
    /// we uses a tcp socket to handle broker request.
    public class MosquittoBrokerAuthHandler : ConnectionHandler
    {
        private readonly ILogger<MosquittoBrokerAuthHandler> _logger;
        private readonly IServiceProvider _provider;

        public MosquittoBrokerAuthHandler(IServiceProvider provider, ILogger<MosquittoBrokerAuthHandler> logger)
        {
            _logger = logger;
            _provider = provider;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            _logger.LogInformation(connection.ConnectionId + " connected");

            while (true)
            {
                var result = await connection.Transport.Input.ReadAsync();
                var buffer = result.Buffer;


            using (var scope = _provider.CreateScope())
            {
            var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
            foreach (var d in db.Devices) {
                await connection.Transport.Output.WriteAsync(System.Text.Encoding.ASCII.GetBytes(d.Name));
            }
            }
            // }
            //     foreach (var segment in buffer)
            //     {
            //         await connection.Transport.Output.WriteAsync(segment);
            //     }

                if (result.IsCompleted)
                {
                    break;
                }

                connection.Transport.Input.AdvanceTo(buffer.End);
            }

            _logger.LogInformation(connection.ConnectionId + " disconnected");
        }
    }
}