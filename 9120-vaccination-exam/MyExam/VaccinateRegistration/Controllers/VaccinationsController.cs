using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VaccinateRegistration.Data;

namespace VaccinateRegistration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccinationsController : ControllerBase
    {
        private readonly VaccinateDbContext _context;
        public VaccinationsController(VaccinateDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<Vaccination> StoreVaccination([FromBody] StoreVaccination vaccination)
        {
            return await _context.StoreVaccination(vaccination);
        }
    }
}
