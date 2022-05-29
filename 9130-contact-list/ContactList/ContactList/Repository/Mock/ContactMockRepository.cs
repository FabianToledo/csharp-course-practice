using ContactList.Entities;
using ContactList.Repository.Interfaces;

namespace ContactList.Repository.Mock;

public class ContactMockRepository : IContactRepository
{
    private readonly List<Person> _people = new();

    static private int _newPersonId = 0;

    public async Task<Person> Add(Person person)
    {
        person.Id = ++_newPersonId;
        _people.Add(person);
        return person;
    }

    public async Task<bool> DeleteById(int personId)
    {
        var personToRemove = await GetById(personId);
        if (personToRemove == null) return false;
        
        return _people.Remove(personToRemove);
    }

    public async Task<IEnumerable<Person>> GetAll()
    {
        return _people;
    }

    public async Task<Person?> GetById(int personId)
    {
        return _people.FirstOrDefault(p => p.Id == personId);
    }

    public async Task<IEnumerable<Person>> GetByName(string name)
    {
        return _people.Where(p => p.FirstName?.Contains(name) ?? false);
    }
}
