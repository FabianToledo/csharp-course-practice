using EventManagementApi.Entities;

namespace EventManagementApi.Repository;

public interface IEventsRepository
{
    Task<Event> Add(Event newEvent);

    Task<IEnumerable<Event>> GetAll();

    Task<Event?> GetById(int id);

    Task Delete(int id);
}
