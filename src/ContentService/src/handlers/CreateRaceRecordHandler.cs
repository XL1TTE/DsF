using Common.DataAccess;
using Common.Utilities;
using ContentService.Commands;
using Contracts.Responses;
using Exceptions;
using Persistence.Documents;
using Wolverine.Attributes;

namespace ContentService.Handlers;

[WolverineHandler]
public class CreateRaceRecordHandler
{
    public async Task<RaceDto> Consume(CreateRaceRecord command, IUnitOfWork unit)
    {
        var repository = unit.Repository<RaceDocument>();
        var race = repository.Add(new RaceDocument
        {
            Name = command.Name,
            History = command.History,
            Health = command.Health,
            Slug = command.Name.Slugify(),
            PreviewUrl = command.PreviewUrl
        });

        try
        {
            await unit.SaveAsync();
            return new RaceDto(race.Name, race.History, race.Health, previewImagePath: race.PreviewUrl);
        }
        catch
        {
            throw new FailedToAddRaceException();
        }
    }
}
