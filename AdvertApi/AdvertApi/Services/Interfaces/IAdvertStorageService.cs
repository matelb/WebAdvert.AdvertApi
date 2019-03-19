using AdvertApi.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvertApi.Services.Interfaces
{
    public interface IAdvertStorageService
    {
        Task<string> Add(AdvertModel model);

        Task<bool> Confirm(ConfirmAdvertModel model);

        Task<bool> CheckHealthAsync();
    }
}
