using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VaccinateRegistration.Data;

namespace VaccinateRegistration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationsController : ControllerBase
    {
        private readonly VaccinateDbContext _context;
        public RegistrationsController(VaccinateDbContext context) 
        {
            _context = context;
        }

        [HttpGet]
        public async Task<GetRegistrationResult?> GetRegistration([FromQuery] long ssn, [FromQuery] int pin)
        {
           return await _context.GetRegistration(ssn, pin);
        }

        [HttpGet("all")]
        public async Task<IEnumerable<GetRegistrationResult>> Get()
        {
            return await _context.GetRegistrations();
        }

        [HttpGet]
        [Route("timeSlots")]
        public async Task<IEnumerable<DateTime>> GetTimeslots([FromQuery] DateTime date)
        {
            return await _context.GetTimeslots(date);
        }
    }
}
