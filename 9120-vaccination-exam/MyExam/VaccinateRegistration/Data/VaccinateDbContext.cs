using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.

namespace VaccinateRegistration.Data
{
    public record GetRegistrationResult(int Id, long Ssn, string FirstName, string LastName);

    public record StoreVaccination(int RegistrationId, DateTime Datetime);

    public class VaccinateDbContext : DbContext
    {
        public VaccinateDbContext(DbContextOptions<VaccinateDbContext> options) : base(options) { }

        public DbSet<Vaccination> Vaccinations => Set<Vaccination>();
        public DbSet<Registration> Registrations => Set<Registration>();

        /// <summary>
        /// Import registrations from JSON file
        /// </summary>
        /// <param name="registrationsFileName">Name of the file to import</param>
        /// <returns>
        /// Collection of all imported registrations
        /// </returns>
        public async Task<IEnumerable<Registration>> ImportRegistrations(string registrationsFileName)
        {
            if(!File.Exists(registrationsFileName))
                return Enumerable.Empty<Registration>();

            using var reader = File.OpenRead(registrationsFileName);
            var registrationsToImport = await JsonSerializer.DeserializeAsync<IEnumerable<Registration>>(reader);

            if (registrationsToImport == null)
                return Enumerable.Empty<Registration>();
            if (!registrationsToImport.Any())
                return Enumerable.Empty<Registration>();

            try
            {
                await Database.BeginTransactionAsync();
                
                await Registrations.AddRangeAsync(registrationsToImport);
                await SaveChangesAsync();

                await Database.CommitTransactionAsync();
                return registrationsToImport;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Database.RollbackTransactionAsync();
                throw;
            }
        }

        /// <summary>
        /// Delete everything (registrations, vaccinations)
        /// </summary>
        public async Task DeleteEverything()
        {
            try
            {
                await Database.BeginTransactionAsync();
                await Database.ExecuteSqlRawAsync("DELETE FROM registrations");
                await Database.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Database.RollbackTransactionAsync();
                throw;
            }
        }

        /// <summary>
        /// Get registration by social security number (SSN) and PIN
        /// </summary>
        /// <param name="ssn">Social Security Number</param>
        /// <param name="pin">PIN code</param>
        /// <returns>
        /// Registration result or null if no registration with given SSN and PIN was found.
        /// </returns>
        public async Task<GetRegistrationResult?> GetRegistration(long ssn, int pin)
        {
            var reg = await Registrations.FirstOrDefaultAsync(r => r.SocialSecurityNumber == ssn && r.PinCode == pin);
            if (reg == null) return null;

            return new GetRegistrationResult(reg.Id, reg.SocialSecurityNumber, reg.FirstName, reg.LastName);
        }

        public async Task<IEnumerable<GetRegistrationResult>> GetRegistrations()
        {
            return await Registrations.Select(reg => new GetRegistrationResult(reg.Id, reg.SocialSecurityNumber, reg.FirstName, reg.LastName)).ToListAsync();
        }

        /// <summary>
        /// Get available time slots on the given date
        /// </summary>
        /// <param name="date">Date (without time, i.e. time is 00:00:00)</param>
        /// <returns>
        /// Collection of all available time slots
        /// </returns>
        public async Task<IEnumerable<DateTime>> GetTimeslots(DateTime date)
        {
            var dateSlots = SlotsBetweenDates(from: DateOnly.FromDateTime(date), to: DateOnly.FromDateTime(date));

            await foreach(var vaccination in Vaccinations)
            {
                dateSlots = dateSlots.Where(s => s != vaccination.VaccinationDate);
            }
            return dateSlots;
        }

        private IEnumerable<DateTime> SlotsBetweenDates(DateOnly from, DateOnly to)
        {
            for(DateOnly date = from ; date <= to; date = date.AddDays(1))
            {
                foreach (var time in SlotsPerDay(from: new TimeOnly(8, 0), to: new TimeOnly(11, 0)))
                {
                    yield return date.ToDateTime(time);
                }
            }
        }

        private IEnumerable<TimeOnly> SlotsPerDay(TimeOnly from, TimeOnly to, int minutesPerVac = 15)
        {
            for (TimeOnly time = from; time < to; time = time.AddMinutes(minutesPerVac))
            {
                yield return time;
            }
        }

        /// <summary>
        /// Store a vaccination
        /// </summary>
        /// <param name="vaccination">Vaccination to store</param>
        /// <returns>
        /// Stored vaccination after it has been written to the database.
        /// </returns>
        /// <remarks>
        /// If a vaccination with the given vaccination.RegistrationID already exists,
        /// overwrite it. Otherwise, insert a new vaccination.
        /// </remarks>
        public async Task<Vaccination> StoreVaccination(StoreVaccination vaccination)
        {
            var register = await Vaccinations.FirstOrDefaultAsync(v => v.RegistrationId == vaccination.RegistrationId);
 
            if (register == null)
            {
                register = new Vaccination()
                {
                    RegistrationId = vaccination.RegistrationId,
                    VaccinationDate = vaccination.Datetime
                };
                Vaccinations.Add(register);
            }
            else
            {
                register.VaccinationDate = vaccination.Datetime;
                register.RegistrationId = vaccination.RegistrationId;
                Vaccinations.Update(register);
            }
            await SaveChangesAsync();
            return register;
        }
    }
}
