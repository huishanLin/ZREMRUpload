using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ZREMRUpload.Models
{
    public class HopeCareDbContext : DbContext
    {
        public HopeCareDbContext(DbContextOptions<HopeCareDbContext> options)
            : base(options)
        {
        }

        // 定義資料表集
        public DbSet<HdReportFile> HdReportFiles { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<HcmHdList> HcmHdLists { get; set; }
        public DbSet<HcBscOrders> HcBscOrders { get; set; }

        public DbSet<HcdOrder> HcdOrders { get; set; }

        public DbSet<Worker> Workers { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("dbo");
        }
    }
}
