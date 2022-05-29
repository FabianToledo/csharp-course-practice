using ContactList.Entities;

namespace ContactList.Repository.Interfaces;

public interface IContactRepository
{
    Task<IEnumerable<Person>> GetAll();
    Task<Person?> GetById(int personId);
    Task<IEnumerable<Person>> GetByName(string name);
    Task<Person> Add(Person person);
    Task<bool> DeleteById(int personId);
}
