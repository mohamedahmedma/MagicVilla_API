using Asp.Versioning;
using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers.V2
{
    //[Route("api/[Controller]")]
    [Route("api/v{version:apiVersion}/VillaNumberAPI")]
    [ApiController]
    [ApiVersion("2.0" , Deprecated = true)]
    public class VillaNumberV2APIController : ControllerBase
    {
        private readonly ILogging _logger;
        private readonly IMapper _mapper;
        private readonly IVillaNumberRepository _context;
        private readonly IVillaRepository _dbVilla;

        protected APIResponse _response;

        public VillaNumberV2APIController(ILogging logger, IVillaNumberRepository context, IMapper mapper, IVillaRepository villa)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _response = new();
            _dbVilla = villa;
        }

        [MapToApiVersion(2.0)]
        [HttpGet("GetString")]
        public IEnumerable<string> GetVillaNumbers()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
