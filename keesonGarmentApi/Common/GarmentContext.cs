using keesonGarmentApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace keesonGarmentApi.Common;

public class GarmentContext : DbContext
{
    public GarmentContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Garment> Garments { get; set; }
    public DbSet<GarmentAssignedLog> GarmentsAssignedLogs { get; set; }
    public DbSet<GarmentIssuingRule> GarmentsIssuingRules { get; set; }
    public DbSet<GarmentSize> GarmentsSizes { get; set; }
    public DbSet<GarmentIssuing> GarmentsIssuings { get; set; }
    public DbSet<GEmployee> GEmployees { get; set; }
}
