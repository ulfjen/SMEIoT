using System;
using System.Threading.Tasks;
using SMEIoT.Core.Entities;

namespace SMEIoT.Core.Interfaces
{
  public interface IDashboardService
  {
    Task<SystemHighlights> GetSystemHighlightsAsync();
  }
}
