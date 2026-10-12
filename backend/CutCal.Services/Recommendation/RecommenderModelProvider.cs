using CutCal.Recommender;
using Microsoft.Extensions.Logging;

namespace CutCal.Services.Recommendation;

public interface IRecommenderModelProvider
{
    /// <summary>The newest trained model, or null while the worker has not produced one yet.</summary>
    SalonRecommenderModel? Current { get; }
}

/// <summary>
/// Hands out the model the worker writes. The file is checked on every call and re-read only when it changed, so a
/// freshly trained model is used without restarting the API. A file that cannot be read keeps the previous model.
/// </summary>
public sealed class RecommenderModelProvider : IRecommenderModelProvider
{
    private readonly string _path;
    private readonly ILogger<RecommenderModelProvider> _logger;
    private readonly object _lock = new();
    private SalonRecommenderModel? _model;
    private DateTime? _loadedWriteTime;

    public RecommenderModelProvider(RecommenderSettings settings, ILogger<RecommenderModelProvider> logger)
    {
        _path = settings.ModelPath;
        _logger = logger;
    }

    public SalonRecommenderModel? Current
    {
        get
        {
            DateTime? writeTime = File.Exists(_path) ? File.GetLastWriteTimeUtc(_path) : null;
            lock (_lock)
            {
                if (writeTime != _loadedWriteTime)
                {
                    Reload(writeTime);
                }
                return _model;
            }
        }
    }

    private void Reload(DateTime? writeTime)
    {
        _loadedWriteTime = writeTime;
        if (writeTime is null)
        {
            _model = null;
            return;
        }

        try
        {
            _model = SalonRecommenderModel.Load(_path);
            _logger.LogInformation("Loaded the recommender model from {Path} (written {WriteTime:u}).", _path, writeTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "The recommender model at {Path} could not be read; keeping the previous model.", _path);
        }
    }
}
