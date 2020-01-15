using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;
using SMEIoT.Core.Exceptions;

namespace SMEIoT.Core.Services
{
  public class MqttEntityIdentifierSuggestionService : IMqttEntityIdentifierSuggestionService
  {
    private readonly IMqttIdentifierService _mqttIdentifierService;
    private readonly IList<string> _identifierCandidates;
    private readonly IApplicationDbContext _dbContext;
    private readonly Random _rand;
    public const string IdentifierDictionaryFilePath = "identifier-candidates.txt";

    public MqttEntityIdentifierSuggestionService(
      IMqttIdentifierService mqttIdentifierService,
      IIdentifierDictionaryFileAccessor identifierDictionary,
      IApplicationDbContext dbContext)
    {
      _mqttIdentifierService = mqttIdentifierService;
      _identifierCandidates = identifierDictionary.ListIdentifiers(IdentifierDictionaryFilePath);
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

    public Task<string> GenerateRandomIdentifierForDeviceAsync(int numWords = 2)
    {
      if (numWords < 1)
      {
        throw new InvalidArgumentException($"Impossible to generate {numWords} words. Should be positive", "numWords");
      }

      var retries = 3;
      while (retries-- > 0)
      {
        var name = GenerateRandomIdentifier(numWords);
        var normalized = Device.NormalizeName(name);
        var count = _dbContext.Devices.Where(d => d.NormalizedName == normalized).Count();
        if (count == 0)
        {
          return Task.FromResult(name);
        }
      }
      throw new SystemException("Can't generate device names after retried 3 times.");
    }

    public async Task<string?> GetOneIdentifierCandidateForSensorAsync(string deviceName)
    {
      var names = await _mqttIdentifierService.ListSensorNamesByDeviceNameAsync(deviceName);
      var candidates = names.ToList();
      if (candidates.Count == 0)
      {
        return null;
      }
      var idx = _rand.Next(candidates.Count);
      return candidates[idx];
    }
  }
}
