using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.models;
using AdvertApi.models.Messages;
using AdvertApi.Services.Interfaces;
using Amazon;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdvertApi.Controllers
{
    [ApiController]
    [Route("adverts/v1")]
    public class Advert : Controller
    {

        private readonly IAdvertStorageService advertStorageService;

        private readonly IConfiguration configuration;

        public Advert(IAdvertStorageService advertStorageService, IConfiguration configuration)
        {
            this.advertStorageService = advertStorageService;
            this.configuration = configuration;
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
                await RaiseAdvertConfirmMessage(model);
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

        private Task RaiseAdvertConfirmMessage(ConfirmAdvertModel model)
        {

            var topicArn = configuration.GetValue<string>("TopicARN");
            var adverModel = await advertStorageService.g
            using(var client = new AmazonSimpleNotificationServiceClient(RegionEndpoint.USEast2))
            {
                var message = new AdvertConfirmMessage
                {
                    ID= model.Id,
                    Title = model.
                }
                client.PublishAsync(topicArn: topicArn, message: "");
            }
        }
    }
}
