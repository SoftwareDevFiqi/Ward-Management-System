using System.Threading.Tasks;

namespace TimelessTechnicians.UI.Services
{
    public interface IAuditLogger
    {
        Task LogAsync(string action, string firstName, string lastName, string details);
    }
}
