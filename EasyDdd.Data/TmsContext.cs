using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyDdd.Kernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EasyDdd.Data
{
    public class TmsContext : DbContextWithDomainEvents
    {
		public TmsContext(DbContextOptions options, IMediator mediator) : base(options, mediator)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			
			base.OnModelCreating(modelBuilder);
		}
	}
}
