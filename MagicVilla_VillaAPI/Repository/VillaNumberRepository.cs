﻿using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public class VillaNumberRepository : Repository<VillaNumber> ,  IVillaNumberRepository
    {
        private readonly AppDbContext _context;

        public VillaNumberRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
       
        public async Task<VillaNumber> UpdateAsync(VillaNumber villa)
        {
            villa.LastUpdatedDate = DateTime.Now;
             _context.VillaNumbers.Update(villa);
            await _context.SaveChangesAsync();
            return villa;
        }
    }
}
