using AdvertApi.models;
using AdvertApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AdvertApi.Services.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace AdvertApi.Services
{
    public class DynamoDBAdvertStorage : IAdvertStorageService
    {

        private readonly IMapper mapper;

        public DynamoDBAdvertStorage(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public async Task<string> Add(AdvertModel model)
        {

            var dbModel = mapper.Map<AdvertDBModel>(model);
            dbModel.ID = new Guid().ToString();
            dbModel.CreationDateTime = DateTime.Now;
            dbModel.Status = AdvertStatus.Pending;

            using (var client  = new AmazonDynamoDBClient())
            {
                using(var context = new DynamoDBContext(client))
                {
                    await context.SaveAsync(dbModel);
                }
            }

            return dbModel.ID;
        }

        public async Task<bool> Confirm(ConfirmAdvertModel model)
        {
            using(var client = new AmazonDynamoDBClient())
            {
                using(var context = new DynamoDBContext(client))
                {
                    var record = await context.LoadAsync<AdvertDBModel>(model.Id);
                    if(record == null)
                    {
                        throw new KeyNotFoundException($"A record with ID{model.Id} was not found");
                    }
                    if(model.Status == AdvertStatus.Active)
                    {
                        record.Status = AdvertStatus.Active;
                        await context.SaveAsync(record);
                    }
                    else
                    {
                        await context.DeleteAsync(record);
                    }
                }
            }
            throw new NotImplementedException();
        }
    }
}
