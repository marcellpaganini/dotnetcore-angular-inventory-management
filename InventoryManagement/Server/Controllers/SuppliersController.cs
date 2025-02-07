﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Data.DTOs;
using Server.Data.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SuppliersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Suppliers
        [HttpGet]
        public async Task<ActionResult<ApiResult<SupplierDTO>>> GetSuppliers(int pageIndex = 0, int pageSize = 10, 
                                                                          string? sortColumn = null, string? sortOrder = null,
                                                                          string? filterColumn = null, string? filterQuery = null)
        {
          if (_context.Suppliers == null)
          {
              return NotFound();
          }
            return await ApiResult<SupplierDTO>.CreateAsync(
                _context.Suppliers.AsNoTracking()
                .Select(s => new SupplierDTO()
                {
                    SupplierId = s.SupplierId,
                    Name = s.Name,
                    Address = s.Address,
                    Province = s.Province,
                    Phone = s.Phone,
                    Email = s.Email,
                    TotalProducts = s.Products!.Count
                }),
                pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
        }

        // GET: api/Suppliers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplier(Guid id)
        {
          if (_context.Suppliers == null)
          {
              return NotFound();
          }
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            return supplier;
        }

        // PUT: api/Suppliers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> PutSupplier(Guid id, Supplier supplier)
        {
            if (id != supplier.SupplierId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(supplier).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(id))
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

        // POST: api/Suppliers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "RegisteredUser")]
        public async Task<ActionResult<Supplier>> PostSupplier(Supplier supplier)
        {
            if (_context.Suppliers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Suppliers'  is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSupplier", new { id = supplier.SupplierId }, supplier);
        }

        // DELETE: api/Suppliers/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteSupplier(Guid id)
        {
            if (_context.Suppliers == null)
            {
                return NotFound();
            }
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SupplierExists(Guid id)
        {
            return (_context.Suppliers?.Any(e => e.SupplierId == id)).GetValueOrDefault();
        }
    }
}
