using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.models;
using AdvertApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdvertApi.Controllers
{
    [ApiController]
    [Route("adverts/v1")]
    public class Advert : Controller
    {

        private readonly IAdvertStorageService advertStorageService;

        public Advert(IAdvertStorageService advertStorageService)
        {
            this.advertStorageService = advertStorageService;
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(CreateAdvertResponse))]
        public async Task<IActionResult> Create(AdvertModel model)
        {
            string recordId;
            try
            {
                 recordId = await advertStorageService.Add(model);
            }
            catch(KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            return StatusCode(201,new CreateAdvertResponse { Id = recordId});
        }

        [HttpPut]
        [Route("Confirm")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm (ConfirmAdvertModel model)
        {
            try
            {
                await advertStorageService.Confirm(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return new OkResult();
        }
    }
}
