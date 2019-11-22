using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Core.Services
{
  public class DeviceSensorIdentifierSuggester : IDeviceSensorIdentifierSuggester
  {
    private readonly IMqttIdentifierService _mqttIdentifierService;
    private List<string> _identifierCandidates;
    private readonly IApplicationDbContext _dbContext;
    private Random _rand;

    public DeviceSensorIdentifierSuggester(
      IMqttIdentifierService mqttIdentifierService,
      IIdentifierDictionaryFileAccessor identifierDictionary,
      IApplicationDbContext dbContext)
    {
      _mqttIdentifierService = mqttIdentifierService;
      _identifierCandidates = identifierDictionary.ListIdentifiers();
      _dbContext = dbContext;
      _rand = new Random();
    }

    private string GenerateRandomIdentifier(int numWords)
    {
      var stringBuilder = new StringBuilder();
      while (numWords-- > 0)
      {
        var id = _rand.Next(_identifierCandidates.Count);
        stringBuilder.Append(_identifierCandidates[id]);
        if (numWords > 0)
        {
          stringBuilder.Append('-');
        }
      }
      return stringBuilder.ToString();
    }

    private string GenerateWithRetries(int retries = 10)
    {
      while (retries-- > 0)
      {
      }
    }
    
    public Task<string> GenerateRandomIdentifierForDeviceAsync(int numWords = 2)
    {
      if (numWords < 1)
      {
        throw new ArgumentException($"Impossible to generate {numWords} words. Should be positive");
      }
      
      var names = _dbContext.Devices.Select(d => d.Name);

      int retries = 3;
      while (true)
      {
        throw new NotImplementedException();
      }

    }

    public Task<string> GenerateRandomIdentifierForSensorAsync(int numWords = 2)
    {
      if (numWords < 1)
      {
        throw new ArgumentException($"Impossible to generate {numWords} words. Should be positive");
      }
      throw new NotImplementedException();
    }
  }
}
