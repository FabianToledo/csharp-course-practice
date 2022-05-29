using EventManagementApi.Entities;
using EventManagementApi.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly IEventsRepository _repository;

    public EventsController(IEventsRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Event>))]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repository.GetAll());
    }

    [HttpGet("{id}", Name = nameof(GetById))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Event))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<IActionResult> GetById(int id)
    {
        var retrievedEvent = await _repository.GetById(id);

        if (retrievedEvent == null) return NotFound();

        return Ok(retrievedEvent);

    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Event))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> Add([FromBody]Event newEvent)
    {
        if(newEvent.Id != 0) return BadRequest("Invalid Id");

        var addedEvent = await _repository.Add(newEvent);
        return CreatedAtAction(nameof(GetById), new { id = addedEvent.Id }, addedEvent);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _repository.Delete(id);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        return NoContent();
    }


}
