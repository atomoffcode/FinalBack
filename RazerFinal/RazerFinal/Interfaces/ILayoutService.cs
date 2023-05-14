﻿using RazerFinal.Models;
using RazerFinal.ViewModels.BasketViewModels;

namespace RazerFinal.Interfaces
{
    public interface ILayoutService
    {
        Task<List<BasketVM>> GetBasket();
        Task<AppUser> GetUser();
    }
}
