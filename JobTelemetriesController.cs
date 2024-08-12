using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_2_NWU_Tech_Trends.Models;

namespace Project_2_NWU_Tech_Trends.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobTelemetriesController : ControllerBase
    {
        private readonly NwutechtrendsDevContext _context;

        public JobTelemetriesController(NwutechtrendsDevContext context)
        {
            _context = context;
        }

        // GET: api/JobTelemetries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobTelemetry>>> GetJobTelemetries()
        {
            return await _context.JobTelemetries.ToListAsync();
        }

        // GET: api/JobTelemetries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobTelemetry>> GetJobTelemetry(int id)
        {
            var jobTelemetry = await _context.JobTelemetries.FindAsync(id);

            if (jobTelemetry == null)
            {
                return NotFound();
            }

            return jobTelemetry;
        }

        public class SavingsResult
        {
            public int TotalTimeSaved { get; set; }
            public decimal TotalCostSaved { get; set; }
        }
        private decimal CalculateCostSavings(int totalHumanTime)
        {
           
            const decimal hourlyRate = 50m; // Example rate
            return (totalHumanTime / 60m) * hourlyRate;
        }


        // GET: api/JobTelemetries/GetSavingsByProject
        [HttpGet("GetSavingsByProject")]
        public async Task<ActionResult<SavingsResult>> GetSavingsByProject(Guid projectId, DateTime startDate, DateTime endDate)
        {
            var projectTelemetry = await _context.JobTelemetries
                //.Where(jt => jt.ProjectId == projectId && jt.EntryDate >= startDate && jt.EntryDate <= endDate)
                .ToListAsync();

            if (!projectTelemetry.Any())
            {
                return NotFound("No telemetry data found for the specified project and date range.");
            }

            var totalHumanTime = projectTelemetry.Sum(jt => jt.HumanTime ?? 0);
            var totalCostSaved = CalculateCostSavings(totalHumanTime); // Define this method according to your cost-saving calculation logic

            var result = new SavingsResult
            {
                TotalTimeSaved = totalHumanTime,
                TotalCostSaved = totalCostSaved
            };

            return Ok(result);
        }

        // GET: api/JobTelemetries/GetSavingsByClient
        [HttpGet("GetSavingsByClient")]
        public async Task<ActionResult<SavingsResult>> GetSavingsByClient(Guid clientId, DateTime startDate, DateTime endDate)
        {
            var clientTelemetry = await _context.JobTelemetries
                //.Where(jt => jt.ClientId == clientId && jt.EntryDate >= startDate && jt.EntryDate <= endDate)
                .ToListAsync();

            if (!clientTelemetry.Any())
            {
                return NotFound("No telemetry data found for the specified client and date range.");
            }

            var totalHumanTime = clientTelemetry.Sum(jt => jt.HumanTime ?? 0);
            var totalCostSaved = CalculateCostSavings(totalHumanTime); // Define this method according to your cost-saving calculation logic

            var result = new SavingsResult
            {
                TotalTimeSaved = totalHumanTime,
                TotalCostSaved = totalCostSaved
            };

            return Ok(result);
        }


        // PUT: api/JobTelemetries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJobTelemetry(int id, JobTelemetry jobTelemetry)
        {
            if (id != jobTelemetry.Id)
            {
                return BadRequest();
            }

            _context.Entry(jobTelemetry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobTelemetryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/JobTelemetries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<JobTelemetry>> PostJobTelemetry(JobTelemetry jobTelemetry)
        {
            _context.JobTelemetries.Add(jobTelemetry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJobTelemetry", new { id = jobTelemetry.Id }, jobTelemetry);
        }

        // DELETE: api/JobTelemetries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobTelemetry(int id)
        {
            var jobTelemetry = await _context.JobTelemetries.FindAsync(id);
            if (jobTelemetry == null)
            {
                return NotFound();
            }

            _context.JobTelemetries.Remove(jobTelemetry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JobTelemetryExists(int id)
        {
            return _context.JobTelemetries.Any(e => e.Id == id);
        }
    }
}
