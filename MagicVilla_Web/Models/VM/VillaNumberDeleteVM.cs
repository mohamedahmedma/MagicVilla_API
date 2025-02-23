using MagicVilla_Web.Models.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_Web.Models.VM
{
	public class VillaNumberDeleteVM
	{

		public VillaNumberDTO Villanumber {  get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem> Villas { get; set; }
		public VillaNumberDeleteVM()
		{
			Villanumber = new VillaNumberDTO();
		}
	}
}