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
  public class MosquittoBrokerAuthHandler : ConnectionHandler
  {
    private readonly ILogger<MosquittoBrokerAuthHandler> _logger;
    private readonly IServiceProvider _provider;
    private readonly MosquittoClientAuthenticationService _clientService; // we need to subscribe the client as well as authenticate ourselves.

    public MosquittoBrokerAuthHandler(IServiceProvider provider, ILogger<MosquittoBrokerAuthHandler> logger)
    {
      _logger = logger;
      _provider = provider;
      _clientService = provider.GetService<MosquittoClientAuthenticationService>();
    }

    private string ProcessLine(in ReadOnlySequence<byte> buffer)
    {
      var str = "";
      foreach (var segment in buffer)
      {
          str += Encoding.UTF8.GetString(segment.Span);
      }
      _logger.LogDebug(str);
      if (str.StartsWith("POSTMETA ")) {
          return "OK";
      }
      else if (str.StartsWith("GETPSK ")) {
        var id = str.AsSpan().Slice("GETPSK ".Length).ToString();
        if (id == _clientService.ClientName) {
          return _clientService.ClientPsk;
        }

        using (var scope = _provider.CreateScope())
        {
          // this should be moved out of Infrastructure.
          var service = scope.ServiceProvider.GetService<IDeviceService>();
          try {
            var device = service.GetDeviceByNameAsync(id).GetAwaiter().GetResult();
            if (device.PreSharedKey != null) {
              return device.PreSharedKey;
            }
          } catch (EntityNotFoundException _) {
            return "FAIL";
          }
        }
      }
      return "FAIL";
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
        var utf8 = new UTF8Encoding();

        while (true)
        {
            var result = await connection.Transport.Input.ReadAsync();
            var buffer = result.Buffer;


            while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
            {
                // Process the line.
                var resp = ProcessLine(line) + "\n";
                _logger.LogDebug(resp);
                byte[] pass = utf8.GetBytes(resp);

                await connection.Transport.Output.WriteAsync(new ReadOnlyMemory<byte>(pass));
            }

            // Tell the PipeReader how much of the buffer has been consumed.

//             _logger.LogInformation("hello");
// foreach (var segment in buffer)
//                 {
//                                 _logger.LogInformation("hello");

//                     Console.WriteLine(segment);
//                 }

        // using (var scope = _provider.CreateScope())
        // {
        // var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
        // foreach (var d in db.Devices) {
        //     await connection.Transport.Output.WriteAsync(System.Text.Encoding.ASCII.GetBytes(d.Name));
        // }
        // }
        // }
        //     foreach (var segment in buffer)
        //     {
        //         await connection.Transport.Output.WriteAsync(segment);
        //     }
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

// async Task ProcessLinesAsync(Socket socket)
// {
//     var pipe = new Pipe();
//     Task writing = FillPipeAsync(socket, pipe.Writer);
//     Task reading = ReadPipeAsync(pipe.Reader);

//     await Task.WhenAll(reading, writing);
// }

// async Task FillPipeAsync(Socket socket, PipeWriter writer)
// {
//     const int minimumBufferSize = 512;

//     while (true)
//     {
//         // Allocate at least 512 bytes from the PipeWriter.
//         Memory<byte> memory = writer.GetMemory(minimumBufferSize);
//         try
//         {
//             int bytesRead = await socket.ReceiveAsync(memory, SocketFlags.None);
//             if (bytesRead == 0)
//             {
//                 break;
//             }
//             // Tell the PipeWriter how much was read from the Socket.
//             writer.Advance(bytesRead);
//         }
//         catch (Exception ex)
//         {
//             LogError(ex);
//             break;
//         }

//         // Make the data available to the PipeReader.
//         FlushResult result = await writer.FlushAsync();

//         if (result.IsCompleted)
//         {
//             break;
//         }
//     }

//      // By completing PipeWriter, tell the PipeReader that there's no more data coming.
//     await writer.CompleteAsync();
// }

// async Task ReadPipeAsync(PipeReader reader)
// {
//     while (true)
//     {
//         ReadResult result = await reader.ReadAsync();
//         ReadOnlySequence<byte> buffer = result.Buffer;

//         while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
//         {
//             // Process the line.
//             ProcessLine(line);
//         }

//         // Tell the PipeReader how much of the buffer has been consumed.
//         reader.AdvanceTo(buffer.Start, buffer.End);

//         // Stop reading if there's no more data coming.
//         if (result.IsCompleted)
//         {
//             break;
//         }
//     }

//     // Mark the PipeReader as complete.
//     await reader.CompleteAsync();
// }

// bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
// {
//     // Look for a EOL in the buffer.
//     SequencePosition? position = buffer.PositionOf((byte)'\n');

//     if (position == null)
//     {
//         line = default;
//         return false;
//     }

//     // Skip the line + the \n.
//     line = buffer.Slice(0, position.Value);
//     buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
//     return true;
// }
