using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using SMEIoT.Infrastructure.Data;
using System;
using System.Text;
using System.Buffers;
using Microsoft.Extensions.DependencyInjection;
using SMEIoT.Core.Exceptions;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Services;

namespace SMEIoT.Infrastructure.Mosquitto
{
  /// we uses a tcp socket to handle broker request.
  /// at this level we don't have DI lifetime.
  public class MosquittoBrokerAuthHandler : ConnectionHandler
  {
    public const int BufferSize = 512;

    private readonly ILogger<MosquittoBrokerAuthHandler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public MosquittoBrokerAuthHandler(IServiceScopeFactory scopeFactory, ILogger<MosquittoBrokerAuthHandler> logger)
    {
      _logger = logger;
      _scopeFactory = scopeFactory;
    }

    private StringBuilder ProcessLine(Encoding encoding, in ReadOnlySequence<byte> buffer)
    {
      var builder = new StringBuilder(BufferSize);
      foreach (var segment in buffer)
      {
        builder.Append(encoding.GetString(segment.Span));
      }
      var str = builder.ToString();
      _logger.LogDebug(str);

      // return async directly with DI causes an exception that will fail the socket pipeline.
      return ProcessLineAsync(str).GetAwaiter().GetResult();
    }

    private async Task<StringBuilder> ProcessLineAsync(string decoded)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var messageHandler = scope.ServiceProvider.GetService<IMosquittoBrokerMessageService>();
        return await messageHandler.ProcessDecodedMessageAsync(decoded);
      }
    }

    private static bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
    {
      // Look for a EOL in the buffer.
      SequencePosition? position = buffer.PositionOf((byte)'\n');

      if (position == null)
      {
        line = default;
        return false;
      }

      // Skip the line + the \n.
      line = buffer.Slice(0, position.Value);
      buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
      return true;
    }

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
      _logger.LogInformation(connection.ConnectionId + " connected");
      var encoding = new ASCIIEncoding();

      while (true)
      {
        var result = await connection.Transport.Input.ReadAsync();
        var buffer = result.Buffer;

        while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
        {
          // Process the line.
          var builder = ProcessLine(encoding, line);
          builder.Append('\n');
          var resp = builder.ToString();
          _logger.LogDebug(resp);

          await connection.Transport.Output.WriteAsync(new ReadOnlyMemory<byte>(encoding.GetBytes(resp)));
        }

        connection.Transport.Input.AdvanceTo(buffer.Start, buffer.End);

        if (result.IsCompleted)
        {
          break;
        }
      }

      _logger.LogInformation(connection.ConnectionId + " disconnected");
    }
  }
}
