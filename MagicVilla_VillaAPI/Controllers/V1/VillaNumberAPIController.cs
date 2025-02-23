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

namespace MagicVilla_VillaAPI.Controllers.V1
{
    //[Route("api/[Controller]")]
    [Route("api/v{version:apiVersion}/VillaNumberAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class VillaNumberAPIController : ControllerBase
    {
        private readonly ILogging _logger;
        private readonly IMapper _mapper;
        private readonly IVillaNumberRepository _context;
        private readonly IVillaRepository _dbVilla;

        protected APIResponse _response;

        public VillaNumberAPIController(ILogging logger, IVillaNumberRepository context, IMapper mapper, IVillaRepository villa)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _response = new();
            _dbVilla = villa;
        }
        [HttpGet]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> villaList = await _context.GetAllAsync(includePro: "villa");
                _logger.Log("Get all villas Number");
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        //[MapToApiVersion(2.0)]
        //[HttpGet]
        //public IEnumerable<string> GetNumbers()
        //{
        //    return new string[] {"value1" , "value2"};
        //}

        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        //[ProducesResponseType(200 , Type = typeof(VillaDto))]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(400)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.Log("Get Villa Error with Id = " + id, "error");
                    _response.IsSuccess = false;
                    return BadRequest();
                }
                var villa = await _context.GetAsync(u => u.VillaNo == id, includes: "villa");
                if (villa == null)
                {
                    _response.IsSuccess = false;
                    return NotFound();
                }
                _response.Result = _mapper.Map<VillaNumberDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
        {
            try
            {
                if (await _context.GetAsync(u => u.VillaNo == createDTO.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa already Exists!");
                    _response.IsSuccess = false;
                    return BadRequest(ModelState);
                }
                if (await _dbVilla.GetAsync(u => u.Id == createDTO.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa ID is Invalid!");
                    _response.IsSuccess = false;
                    return BadRequest(ModelState);
                }
                if (createDTO == null)
                {
                    _response.IsSuccess = false;
                    return BadRequest(createDTO);
                }

                VillaNumber model = _mapper.Map<VillaNumber>(createDTO);

                await _context.CreateAsync(model);
                _response.Result = model;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVilla", new { id = model.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    return BadRequest();
                }
                var villa = await _context.GetAsync(u => u.VillaNo == id);
                if (villa == null)
                {
                    _response.IsSuccess = false;
                    return NotFound();
                }
                await _context.RemoveAsync(villa);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }



        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO UpdateDTO)
        {
            try
            {
                if (UpdateDTO == null || UpdateDTO.VillaNo == 0 || id != UpdateDTO.VillaNo)
                {
                    _response.IsSuccess = false;
                    return BadRequest();
                }
                if (await _dbVilla.GetAsync(u => u.Id == UpdateDTO.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa ID is Invalid!");
                    _response.IsSuccess = false;
                    return BadRequest(ModelState);
                }
                VillaNumber model = _mapper.Map<VillaNumber>(UpdateDTO);
                //Villa model = new()
                //{
                //    Amenity = villaDto.Amenity,
                //    Details = villaDto.Details,
                //    Id = villaDto.Id,
                //    ImageUrl = villaDto.ImageUrl,
                //    Name = villaDto.Name,
                //    Occupancy = villaDto.Occupancy,
                //    Rate = villaDto.Rate,
                //    Sqft = villaDto.Sqft
                //};
                await _context.UpdateAsync(model);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdatePartialVillanumber(int id, JsonPatchDocument<VillaNumberUpdateDTO> patche)
        {
            if (patche == null || id == 0)
            {
                _response.IsSuccess = false;
                return BadRequest();
            }
            var villa = await _context.GetAsync(u => u.VillaNo == id, tracked: false);
            VillaNumberUpdateDTO villaDTO = _mapper.Map<VillaNumberUpdateDTO>(villa);
            //VillaUpdateDTO modelDto = new()
            //{
            //    Amenity = villa.Amenity,
            //    Details = villa.Details,
            //    Id = villa.Id,
            //    ImageUrl = villa.ImageUrl,
            //    Name = villa.Name,
            //    Occupancy = villa.Occupancy,
            //    Rate = villa.Rate,
            //    Sqft = villa.Sqft
            //};
            if (villa == null)
            {
                _response.IsSuccess = false;
                return BadRequest();
            }
            patche.ApplyTo(villaDTO, ModelState);
            VillaNumber model = _mapper.Map<VillaNumber>(villaDTO);
            //Villa model = new()
            //{
            //    Amenity = modelDto.Amenity,
            //    Details = modelDto.Details,
            //    Id = modelDto.Id,
            //    ImageUrl = modelDto.ImageUrl,
            //    Name = modelDto.Name,
            //    Occupancy = modelDto.Occupancy,
            //    Rate = modelDto.Rate,
            //    Sqft = modelDto.Sqft
            //};
            if (!ModelState.IsValid)
            {
                _response.IsSuccess = false;
                return BadRequest();
            }
            await _context.UpdateAsync(model);
            return NoContent();
        }

    }
}
