﻿using EMS.Judge.Application.Common.Services;
using EMS.Witness.Rpc;

namespace EMS.Witness.Services;

public class PersistenceService : IPersistenceService
{
    private readonly WitnessState state;
    private readonly IJsonSerializationService jsonSerializer;
	private readonly IWitnessLogger _witnessLogger;
	private readonly string path;

    public PersistenceService(WitnessState state, IJsonSerializationService jsonSerializer, IWitnessLogger witnessLogger)
    {
        this.state = state;
        this.jsonSerializer = jsonSerializer;
		_witnessLogger = witnessLogger;
		this.path = Path.Combine(FileSystem.Current.CacheDirectory, "e.witness");
    }

    public async Task RestoreIfAny()
    {
        try
        {
			if (!File.Exists(path))
			{
				return;
			}
			var contents = await File.ReadAllTextAsync(path);
			var state = this.jsonSerializer.Deserialize<WitnessState>(contents);
			this.state.Set(state);
		}
        catch (Exception ex)
        {
            _witnessLogger.Log(ex);
        }
    }

    public async Task Store()
    {
        try
        {
			var serialized = this.jsonSerializer.Serialize(this.state);
			await File.WriteAllTextAsync(this.path, serialized);
		}
        catch (Exception ex)
        {
            _witnessLogger.Log(ex);
        }
    }
}

public interface IPersistenceService
{
    public Task Store();
    public Task RestoreIfAny();
}
