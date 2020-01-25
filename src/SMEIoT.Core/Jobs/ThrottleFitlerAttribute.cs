using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;

namespace SMEIoT.Core.Jobs
{
  public sealed class ThrottleFitlerAttribute : JobFilterAttribute,
     IClientFilter, IServerFilter, IElectStateFilter
  {
    private static readonly TimeSpan s_lockTimeout = TimeSpan.FromSeconds(5);
    private const string TimestampKey = "Timestamp";
    private const string FingerprintPrefix = "_p:";
    private const string LockKeyPrefix = "_l:";
    private const int MaxHangfireKeyLength = 91; // Hangfire resource (key) limit length is 100. Lock has a prefix with `Hangfire:` 

    private readonly TimeSpan _seconds;

    private readonly string? _fingerPrintFormat;

    /// <summary>
    /// Debounce the background task, preventing it from being called until the end
    /// of the lockout period.
    /// </summary>
    /// <param name="seconds">The length of the lockout period in seconds.</param>
    /// <param name="resourceFormat">format to generate a postfix for certian jobs</param>
    public ThrottleFitlerAttribute(int seconds, string? resourceFormat = null)
    {
      _seconds = TimeSpan.FromSeconds(seconds);
      _fingerPrintFormat = resourceFormat;
    }

    public void OnCreating(CreatingContext context)
    {
      // If the job is created in anything other than an Enqueued state, check the
      // debounce lock state. This allows short scheduled versions to pass through.
      if (!(context.InitialState is EnqueuedState))
      {
        return;
      }

      try
      {
        using (context.Connection.AcquireDistributedLock(GetFingerprintWithPrefix(context.Job, LockKeyPrefix), s_lockTimeout))
        {
          var timestamp = GetTimestamp(context.Connection, context.Job);

          if (TimestampInWindow(timestamp, _seconds))
          {
            context.Canceled = true;
          }
          else
          {
            // Set the timestamp - this will add the lock key, or update
            // and extend the lock.
            context.Connection.SetRangeInHash(GetFingerprintWithPrefix(context.Job, FingerprintPrefix), new Dictionary<string, string>
          {
            { TimestampKey, DateTimeOffset.UtcNow.ToString("o") }
          });
          }
        }
      }
      catch
      {
        // continue from the exception
      }
    }

    public void OnCreated(CreatedContext context)
    {
    }

    public void OnPerforming(PerformingContext context)
    {
    } 
    public void OnPerformed(PerformedContext context)
    {
    }

    public void OnStateElection(ElectStateContext context)
    {
      var timestamp = GetTimestamp(context.Connection, context.BackgroundJob.Job);

      if (context.CandidateState is DeletedState)
      {
        if (!TimestampInWindow(timestamp, _seconds))
        {
          RemoveFingerprint(context.Connection, context.BackgroundJob.Job);
        }
      }
    }


    /// <summary>
    /// Fetch the debounce starting timestamp.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="job"></param>
    /// <returns></returns>
    private DateTimeOffset? GetTimestamp(IStorageConnection connection, Job job)
    {
      var fingerprint = connection.GetAllEntriesFromHash(GetFingerprintWithPrefix(job, FingerprintPrefix));

      if (fingerprint != null
          && fingerprint.ContainsKey(TimestampKey)
          && DateTimeOffset.TryParse(fingerprint[TimestampKey], null, DateTimeStyles.RoundtripKind, out var timestamp))
      {
        return timestamp;
      }

      return null;
    }

    private bool TimestampInWindow(DateTimeOffset? timestamp, TimeSpan windowSpan)
    {
      if (timestamp == null) { return false; }

      return DateTimeOffset.UtcNow <= timestamp.Value.Add(windowSpan);
    }

    /// <summary>
    /// Remove the fingerprint for the job.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="job"></param>
    private void RemoveFingerprint(IStorageConnection connection, Job job)
    {
      try
      {
        using (connection.AcquireDistributedLock(GetFingerprintWithPrefix(job, LockKeyPrefix), s_lockTimeout))
        using (var transaction = connection.CreateWriteTransaction())
        {
          transaction.RemoveHash(GetFingerprintWithPrefix(job, FingerprintPrefix));
          transaction.Commit();
        }
      }
      catch
      {
        // continue from exception
      }
    }

    /// <summary>
    /// Build the fingerprint for the given job. The format is:
    /// {class}.{method}.{params}
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    private StringBuilder ExtractFingerprintFromJob(Job job, StringBuilder sb)
    {
      // Cannot fingerprint anon funcs.
      if (job.Type == null || job.Method == null)
      {
        return sb;
      }

      sb.AppendJoin('.', job.Type.Name, job.Method.Name);
      if (_fingerPrintFormat != null)
      {
        sb.Append(string.Format(_fingerPrintFormat, (object[])job.Args));
      }
      else
      {
        sb.AppendJoin('.', job.Args);
      }

      return sb;
    }

    private string GetFingerprintWithPrefix(Job job, string prefix)
    {
      var sb = new StringBuilder(128);
      sb.Append(prefix);
      sb = ExtractFingerprintFromJob(job, sb);

      return sb.ToString(0, Math.Min(sb.Length, MaxHangfireKeyLength));
    }
  }
}
