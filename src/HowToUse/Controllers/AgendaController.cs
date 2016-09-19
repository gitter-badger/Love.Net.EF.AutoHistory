using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HowToUse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HowToUse.Controllers {
    [Route("api/[controller]")]
    public class AgendaController : ControllerBase {
        private readonly AgendaContext _context;

        public AgendaController(AgendaContext context) {
            _context = context;
        }

        [HttpPost]
        public async Task<int> Post([FromBody]Agenda agenda) {
            _context.Agenda.Add(agenda);

            // ensure auto history
            _context.EnsureAutoHistory();

            return await _context.SaveChangesAsync(true);
        }

        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody]Agenda agenda) {
            var item = _context.Agenda.Where(a => a.Id == id).FirstOrDefault();
            if (item == null) {
                return;
            }

            item.DayOfWeek = agenda.DayOfWeek;
            item.Items = agenda.Items;

            await _context.SaveChangesAsync(true);
        }
    }
}
