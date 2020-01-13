using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMEIoT.Core.Entities;
using SMEIoT.Core.Interfaces;

namespace SMEIoT.Core.Services
{
  public class MqttEntityIdentifierSuggestionService : IMqttEntityIdentifierSuggestionService
  {
    private readonly IMqttIdentifierService _mqttIdentifierService;
    private readonly IList<string> _identifierCandidates;
    private readonly IApplicationDbConnection _dbConnection;
    private readonly Random _rand;
    public const string IdentifierDictionaryFilePath = "identifier-candidates.txt";

    public MqttEntityIdentifierSuggestionService(
      IMqttIdentifierService mqttIdentifierService,
      IIdentifierDictionaryFileAccessor identifierDictionary,
      IApplicationDbConnection dbConnection)
    {
      _mqttIdentifierService = mqttIdentifierService;
      _identifierCandidates = identifierDictionary.ListIdentifiers(IdentifierDictionaryFilePath);
      _dbConnection = dbConnection;
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
        throw new ArgumentException($"Impossible to generate {numWords} words. Should be positive");
      }

      var retries = 3;
      while (retries-- > 0)
      {
        var name = GenerateRandomIdentifier(numWords);
        if (!_dbConnection.ExecuteScalar<bool>("SELECT COUNT(DISTINCT 1) FROM devices WHERE normalized_name = @NormalizedName;", new {NormalizedName = Device.NormalizeName(name)}))
        {
          return Task.FromResult(name);
        }
      }
      throw new SystemException("Can't generate device names after retried 3 times.");
    }

    public string? GetARandomIdentifierCandidatesForSensor(string deviceName)
    {
      var candidates = _mqttIdentifierService.ListSensorNamesByDeviceName(deviceName).ToList();
      if (candidates.Count == 0)
      {
        return null;
      }
      var idx = _rand.Next(candidates.Count);
      return candidates[idx];
    }
  }
}
