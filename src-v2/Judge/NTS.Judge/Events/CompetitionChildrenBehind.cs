﻿using Not.Application.Ports.CRUD;
using Not.Blazor.Ports.Behinds;
using Not.Exceptions;
using NTS.Domain.Setup.Entities;

namespace NTS.Judge.Events;
public class CompetitionChildrenBehind : INotBehindParent<Contestant>, INotBehindParent<Loop>, INotBehindWithChildren<Competition>
{
    private readonly IRead<Competition> _competitionReader;
    private readonly IRepository<Competition> _competitionRepository;
    private Competition? _competition;

    public CompetitionChildrenBehind(IRead<Competition> competitionReader, IRepository<Competition> competitionRepository) 
    {
        _competitionReader = competitionReader;
        _competitionRepository = competitionRepository;
    }

    IEnumerable<Contestant> INotBehindParent<Contestant>.Children => _competition?.Contestants ?? Enumerable.Empty<Contestant>();
    IEnumerable<Loop> INotBehindParent<Loop>.Children => _competition?.Loops ?? Enumerable.Empty<Loop>();

    public async Task<Contestant> Create(Contestant entity)
    {
        GuardHelper.ThrowIfDefault(_competition);

        _competition.Add(entity);
        await _competitionRepository.Update(_competition);
        return entity;
    }

    public async Task<Contestant> Update(Contestant entity)
    {
        GuardHelper.ThrowIfDefault(_competition);

        _competition.Update(entity);
        await _competitionRepository.Update(_competition);
        return entity;
    }

    public async Task<Contestant> Delete(Contestant entity)
    {
        GuardHelper.ThrowIfDefault(_competition);

        _competition.Remove(entity);
        await _competitionRepository.Update(_competition);
        return entity;
    }

    public async Task<Loop> Create(Loop entity)
    {
        GuardHelper.ThrowIfDefault(_competition);

        _competition.Add(entity);
        await _competitionRepository.Update(_competition);
        return entity;
    }

    public async Task<Loop> Update(Loop entity)
    {
        GuardHelper.ThrowIfDefault(_competition);

        _competition.Update(entity);
        await _competitionRepository.Update(_competition);
        return entity;
    }

    public async Task<Loop> Delete(Loop entity)
    {
        GuardHelper.ThrowIfDefault(_competition);

        _competition.Remove(entity);
        await _competitionRepository.Update(_competition);
        return entity;
    }

    public async Task Initialize(int id)
    {
        _competition = await _competitionReader.Read(id);
    }
}
