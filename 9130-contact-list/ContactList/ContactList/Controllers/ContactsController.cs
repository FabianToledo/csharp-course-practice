using ContactList.Entities;
using ContactList.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContactList.Controllers;
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
public class ContactsController : ControllerBase
{
    private readonly IContactRepository _contactRepository;

    public ContactsController(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<Person>> GetAll()
    {
        return await _contactRepository.GetAll();
    }

    [HttpGet("{id}", Name = nameof(GetById))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Person>> GetById(uint id)
    {
        if (id == 0)
            return ValidationProblem();

        var person = await _contactRepository.GetById((int)id);
        if (person == null) 
            return NotFound();

        return person;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Person>> Add(Person person)
    {
        person = await _contactRepository.Add(person);
        return CreatedAtAction(nameof(GetById), new { id = person.Id }, person );
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteById(uint id)
    {
        if (id == 0)
            return ValidationProblem();

        var deleted = await _contactRepository.DeleteById((int)id);
        if(!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpGet("findByName")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IEnumerable<Person>> FindByName(string name)
    {
        return await _contactRepository.GetByName(name);
    }


}
