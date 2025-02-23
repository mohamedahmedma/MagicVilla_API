﻿using MagicVilla_Web.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_Web.Services.IServices
{
	public interface IVillaNumberService : IBaseService
	{
		Task<T> GetAllAsync<T>(string token);
		Task<T> GetAsync<T>(int id , string token);
		Task<T> CreateAsync<T>(VillaNumberCreateDTO entity , string token);
		Task<T> UpdateAsync<T>(VillaNumberUpdateDTO entity , string token);
		Task<T> DeleteAsync<T>(int id , string token);
	}
}