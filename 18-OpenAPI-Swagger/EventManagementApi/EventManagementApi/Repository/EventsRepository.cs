using EventManagementApi.Entities;

namespace EventManagementApi.Repository;

public class EventsRepository : IEventsRepository
{
    private int _idCount = 0;
    private List<Event> Events { get; } = new();
    public async Task<Event> Add(Event newEvent)
    {
        var insertedEvent = newEvent with { Id = ++_idCount };
        Events.Add(insertedEvent);
        await Task.Delay(1);
        return insertedEvent;
    }

    public async Task<IEnumerable<Event>> GetAll() 
    {
        await Task.Delay(1);
        return Events; 
    }

    public async Task<Event?> GetById(int id)
    {
        await Task.Delay(1);
        return Events.FirstOrDefault(e => e.Id == id);
    }

    public async Task Delete(int id)
    {
        var eventToDelete = await GetById(id);
        
        if (eventToDelete == null)
            throw new ArgumentException("No event exists with the given id", nameof(id));

        Events.Remove(eventToDelete);
    }
}
