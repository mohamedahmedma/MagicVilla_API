using Asp.Versioning;
using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace MagicVilla_VillaAPI.Controllers.V1
{
    //[Route("api/[Controller]")]
    [Route("api/v{version:apiVersion}/VillaAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class VillaAPIController : ControllerBase
    {
        private readonly ILogging _logger;
        private readonly IMapper _mapper;
        private readonly IVillaRepository _context;
        protected APIResponse _response;

        public VillaAPIController(ILogging logger, IVillaRepository context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _response = new();

        }
        [HttpGet]
        [ResponseCache(Duration = 30)]
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name ="FilterOccupancy")] int? occupancy,
            [FromQuery] string? search , int pageSize = 0 , int pageNumber = 1)
        {
            try
            {
                IEnumerable<Villa> villaList;
                if(occupancy > 0)
                {
                    villaList = await _context.GetAllAsync(u => u.Occupancy == occupancy , pageSize: pageSize , 
                        pageNumber: pageNumber);
                }
                else
                {
                    villaList = await _context.GetAllAsync();
                }
                if(!string.IsNullOrEmpty(search))
                {
                    villaList = villaList.Where(u => u.Amenity.ToLower().Contains(search.ToLower()) 
                    || u.Name.ToLower().Contains(search));
                }
                Pagination pagination = new() { PageNumber = pageNumber , PageSize = pageSize };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
                _logger.Log("Get all villas");
                _response.Result = _mapper.Map<List<VillaDto>>(villaList);
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

        [HttpGet("{id:int}", Name = "GetVilla")]
        [Authorize(Roles = "admin")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(200 , Type = typeof(VillaDto))]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(400)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.Log("Get Villa Error with Id = " + id, "error");
                    _response.IsSuccess = false;
                    return BadRequest();
                }
                var villa = await _context.GetAsync(u => u.Id == id);
                if (villa == null)
                {
                    _response.IsSuccess = false;
                    return NotFound();
                }
                _response.Result = _mapper.Map<VillaDto>(villa);
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
        [Authorize(Roles = "admin")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            try
            {
                if (await _context.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("CustomUniqe", "Villa already Exists!");
                    _response.IsSuccess = false;
                    return BadRequest(ModelState);
                }
                if (createDTO == null)
                {
                    _response.IsSuccess = false;
                    return BadRequest(createDTO);
                }

                Villa model = _mapper.Map<Villa>(createDTO);
                await _context.CreateAsync(model);
                _response.Result = _mapper.Map<VillaDto>(model);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVilla", new { id = model.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [Authorize(Roles = "CUSTOM")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    return BadRequest();
                }
                var villa = await _context.GetAsync(u => u.Id == id);
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



        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO UpdateDTO)
        {
            try
            {
                if (UpdateDTO == null || UpdateDTO.Id == 0 || id != UpdateDTO.Id)
                {
                    _response.IsSuccess = false;
                    return BadRequest();
                }
                Villa model = _mapper.Map<Villa>(UpdateDTO);
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

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patche)
        {
            if (patche == null || id == 0)
            {
                _response.IsSuccess = false;
                return BadRequest();
            }
            var villa = await _context.GetAsync(u => u.Id == id, tracked: false);
            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);
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
            Villa model = _mapper.Map<Villa>(villaDTO);
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
