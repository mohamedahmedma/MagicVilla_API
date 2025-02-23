using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MagicVilla_Web.Services;
using Newtonsoft.Json;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Models;
using System.Net;
using MagicVilla_Web.Models.VM;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using MagicVilla_Utility;

namespace MagicVilla_Web.Controllers
{
	public class VillaNumberController : Controller
	{
		private readonly IVillaNumberService _villaNumberService;
		private readonly IVillaService _villaService;
		private readonly IMapper _mapper;

		public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper , IVillaService villaService)
		{
			_mapper = mapper;
			_villaNumberService = villaNumberService;
			_villaService = villaService;
		}


		public async Task<IActionResult> IndexVillaNumber()
		{
			List<VillaNumberDTO> list = new();
			var response = await _villaNumberService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
			}
			return View(list);
		}
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateVillaNumber()
		{
			VillaNumberCreateVM villaNumberVM = new();
			var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if(response != null && response.IsSuccess)
			{
				villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>
					(Convert.ToString(response.Result)).Select( i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
			}
			return View(villaNumberVM);
		}
		[HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateVillaNumber(/*[FromBody]*/VillaNumberCreateVM villa)
		{
			
			if (ModelState.IsValid)
			{
				var response = await _villaNumberService.CreateAsync<APIResponse>(villa.VillaNumber, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.IsSuccess)
                {
					return RedirectToAction(nameof(IndexVillaNumber));
				}
				else
				{
					if (response.ErrorMessages.Count > 0)
					{
						ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
					}

				}
			}


			var resp = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if(resp != null && resp.IsSuccess)
			{
				villa.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>
					(Convert.ToString(resp.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
				
			}
			return View(villa);
		}
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVillaNumber(int id)
		{
			VillaNumberUpdateVM number = new();
			var villa = await _villaNumberService.GetAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));


			if (villa.IsSuccess = true && villa != null)
			{
				VillaNumberDTO v = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(villa.Result));
				number.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(v);
			}
			else
			{
				return NotFound();
			}



			var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if(response != null && response.IsSuccess == true)
			{
				number.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>
					(Convert.ToString(response.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
			}
			return View(number);
		}

		[HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM villa)
		{
			if (ModelState.IsValid)
			{
				var response = await _villaNumberService.UpdateAsync<APIResponse>(villa.VillaNumber, HttpContext.Session.GetString(SD.SessionToken));
				if(response != null && response.IsSuccess == true)
				{
					return RedirectToAction("IndexVillaNumber");
				}
				else
				{
					if(response.ErrorMessages.Count > 0)
					{
						ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
					}
				}
			}
			var resp = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if (resp != null && resp.IsSuccess)
			{
				villa.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>
					(Convert.ToString(resp.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
			}
			return View(villa.VillaNumber);
		}
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVillaNumber(int id)
		{
			VillaNumberDeleteVM villa = new();
			var v  = await _villaNumberService.GetAsync<APIResponse>(id, HttpContext.Session.GetString(SD.SessionToken));

			if(v.IsSuccess == true && v != null)
			{
				VillaNumberDTO vi = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(v.Result));
				villa.Villanumber = _mapper.Map<VillaNumberDTO>(vi);
			}
			else
			{
				return NotFound();
			}

			var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.IsSuccess == true)
			{
				villa.Villas = JsonConvert.DeserializeObject<List<VillaDto>>
					(Convert.ToString(response.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
			}
			return View(villa);
		}

		[HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteVM villa)
		{
			var res = await _villaNumberService.DeleteAsync<APIResponse>(villa.Villanumber.VillaNo, HttpContext.Session.GetString(SD.SessionToken));
			if (res != null && res.IsSuccess == true)
			{
				return RedirectToAction(nameof(IndexVillaNumber));
			}
			else
			{
				if (res.ErrorMessages.Count() > 0)
				{
					ModelState.AddModelError("ErrorMessages", res.ErrorMessages.FirstOrDefault());
				}
			}
			return View(villa);
		}
	}
}