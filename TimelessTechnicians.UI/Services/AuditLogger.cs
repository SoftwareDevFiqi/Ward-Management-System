using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimelessTechnicians.UI.Data;
using TimelessTechnicians.UI.Models;

namespace TimelessTechnicians.UI.Services
{
    public class AuditLogger : IAuditLogger
    {
        private readonly ApplicationDbContext _context;

        public AuditLogger(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string action, string firstName, string lastName, string details)
        {
            var log = new AuditLog
            {
                Action = action,
                FirstName = firstName,
                LastName = lastName,
                Details = details,
                ActionDate = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
