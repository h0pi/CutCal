using System.IO.Compression;
using System.Text.Json;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

namespace CutCal.Recommender;

/// <summary>One positive signal: the user booked, favourited or viewed the salon.</summary>
public readonly record struct Interaction(int UserId, int SalonId);

/// <summary>
/// Collaborative-filtering model: one-class matrix factorization (ML.NET / LIBMF) trained on implicit feedback.
/// Every user and every salon gets a vector of latent factors; a high dot product means the user is likely to
/// like the salon. Only users and salons seen during training can be scored.
/// </summary>
public sealed class SalonRecommenderModel
{
    private const int Seed = 1;
    // Few latent factors on purpose: with about as many factors as salons the model memorises the matrix instead of generalising.
    private const int ApproximationRank = 4;
    private const int Iterations = 20;
    private const double LearningRate = 0.1;
    private const double Lambda = 0.025;
    private const double Alpha = 1;
    private const double NegativeEntryValue = 0.0001;
    private const float PositiveLabel = 1;

    private const string ModelEntryName = "model.zip";
    private const string IdsEntryName = "ids.json";

    private readonly MLContext _ml;
    private readonly ITransformer _model;
    private readonly DataViewSchema _inputSchema;
    private readonly List<int> _userIds;
    private readonly List<int> _salonIds;
    private readonly Dictionary<int, uint> _userIndex;
    private readonly Dictionary<int, uint> _salonIndex;

    private SalonRecommenderModel(MLContext ml, ITransformer model, DataViewSchema inputSchema, List<int> userIds, List<int> salonIds)
    {
        _ml = ml;
        _model = model;
        _inputSchema = inputSchema;
        _userIds = userIds;
        _salonIds = salonIds;
        _userIndex = ToIndex(userIds);
        _salonIndex = ToIndex(salonIds);
    }

    private sealed class MatrixEntry
    {
        public uint Row { get; set; }
        public uint Column { get; set; }
        public float Label { get; set; }
    }

    private sealed class MatrixScore
    {
        public float Score { get; set; }
    }

    private sealed record KnownIds(List<int> Users, List<int> Salons);

    /// <summary>Same data and same code give the same model, so a retrain never changes recommendations by chance.</summary>
    public static SalonRecommenderModel Train(IEnumerable<Interaction> interactions)
    {
        var distinct = interactions.Distinct().ToList();
        if (distinct.Count == 0)
        {
            throw new ArgumentException("At least one interaction is needed to train the model.", nameof(interactions));
        }

        var userIds = distinct.Select(x => x.UserId).Distinct().Order().ToList();
        var salonIds = distinct.Select(x => x.SalonId).Distinct().Order().ToList();
        var userIndex = ToIndex(userIds);
        var salonIndex = ToIndex(salonIds);

        var ml = new MLContext(Seed);
        var entries = distinct.Select(x => new MatrixEntry { Row = userIndex[x.UserId], Column = salonIndex[x.SalonId], Label = PositiveLabel });
        var data = ml.Data.LoadFromEnumerable(entries, SchemaFor(userIds.Count, salonIds.Count));

        var trainer = ml.Recommendation().Trainers.MatrixFactorization(new MatrixFactorizationTrainer.Options
        {
            MatrixRowIndexColumnName = nameof(MatrixEntry.Row),
            MatrixColumnIndexColumnName = nameof(MatrixEntry.Column),
            LabelColumnName = nameof(MatrixEntry.Label),
            LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass,
            ApproximationRank = ApproximationRank,
            NumberOfIterations = Iterations,
            LearningRate = LearningRate,
            Lambda = Lambda,
            Alpha = Alpha,
            C = NegativeEntryValue,
            NumberOfThreads = 1,
            Quiet = true
        });

        return new SalonRecommenderModel(ml, trainer.Fit(data), data.Schema, userIds, salonIds);
    }

    /// <summary>Scores the given salons for the user; salons the model has never seen are left out, and an unknown user gets no scores.</summary>
    public IReadOnlyDictionary<int, float> Score(int userId, IEnumerable<int> salonIds)
    {
        var known = salonIds.Distinct().Where(_salonIndex.ContainsKey).ToList();
        if (known.Count == 0 || !_userIndex.TryGetValue(userId, out var row))
        {
            return new Dictionary<int, float>();
        }

        var entries = known.Select(id => new MatrixEntry { Row = row, Column = _salonIndex[id] });
        var scored = _model.Transform(_ml.Data.LoadFromEnumerable(entries, SchemaFor(_userIds.Count, _salonIds.Count)));
        var scores = _ml.Data.CreateEnumerable<MatrixScore>(scored, reuseRowObject: false).Select(x => x.Score);
        return known.Zip(scores).ToDictionary(x => x.First, x => x.Second);
    }

    /// <summary>Writes one file (model plus the ids it knows). The file appears atomically, so a reader never sees a half-written model.</summary>
    public void Save(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        var temporaryPath = $"{path}.{Guid.NewGuid():N}.tmp";

        using (var archive = ZipFile.Open(temporaryPath, ZipArchiveMode.Create))
        {
            using var modelBytes = new MemoryStream();
            _ml.Model.Save(_model, _inputSchema, modelBytes);
            using (var entry = archive.CreateEntry(ModelEntryName).Open())
            {
                modelBytes.WriteTo(entry);
            }
            using (var entry = archive.CreateEntry(IdsEntryName).Open())
            {
                JsonSerializer.Serialize(entry, new KnownIds(_userIds, _salonIds));
            }
        }

        File.Move(temporaryPath, path, overwrite: true);
    }

    public static SalonRecommenderModel Load(string path)
    {
        using var archive = ZipFile.OpenRead(path);

        using var modelBytes = new MemoryStream();
        using (var entry = archive.GetEntry(ModelEntryName)!.Open())
        {
            entry.CopyTo(modelBytes);
        }
        modelBytes.Position = 0;

        var ml = new MLContext(Seed);
        var model = ml.Model.Load(modelBytes, out var inputSchema);
        using var idsEntry = archive.GetEntry(IdsEntryName)!.Open();
        var ids = JsonSerializer.Deserialize<KnownIds>(idsEntry)!;
        return new SalonRecommenderModel(ml, model, inputSchema, ids.Users, ids.Salons);
    }

    /// <summary>The matrix factorization trainer reads key columns as zero-based matrix positions; an index equal to the count is out of range and scores NaN.</summary>
    private static Dictionary<int, uint> ToIndex(List<int> ids) =>
        ids.Select((id, position) => (id, index: (uint)position)).ToDictionary(x => x.id, x => x.index);

    private static SchemaDefinition SchemaFor(int userCount, int salonCount)
    {
        var schema = SchemaDefinition.Create(typeof(MatrixEntry));
        schema[nameof(MatrixEntry.Row)].ColumnType = new KeyDataViewType(typeof(uint), userCount);
        schema[nameof(MatrixEntry.Column)].ColumnType = new KeyDataViewType(typeof(uint), salonCount);
        return schema;
    }
}
