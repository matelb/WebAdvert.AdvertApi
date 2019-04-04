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
using Amazon;
using System.IO;

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
            dbModel.Id = Guid.NewGuid().ToString();
            dbModel.CreationDateTime = DateTime.Now;
            dbModel.Status = AdvertStatus.Pending;
            dbModel.FilePath = $"{dbModel.Id}/{model.FilePath}";

            using (var client  = new AmazonDynamoDBClient(RegionEndpoint.USEast2))
            {
                using (var context = new DynamoDBContext(client))
                {
                    await context.SaveAsync(dbModel);
                }
            }
            return dbModel.Id;
        }

        public async Task<bool> Confirm(ConfirmAdvertModel model)
        {
            using(var client = new AmazonDynamoDBClient(RegionEndpoint.USEast2))
            {                

                using (var context = new DynamoDBContext(client))
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
                //I don't know if it's ok --> return true¿?
                return true;
            }
            throw new NotImplementedException();
        }

        public async Task<AdvertModel> Get(string Id)
        {
            using (var client = new AmazonDynamoDBClient(RegionEndpoint.USEast2))
            {

                using (var context = new DynamoDBContext(client))
                {
                    var record = await context.LoadAsync<AdvertDBModel>(Id);
                    if (record == null)
                    {
                        throw new KeyNotFoundException($"A record with ID{Id} was not found");
                    }
                    var advertModel = mapper.Map<AdvertDBModel, AdvertModel>(record);
                    return advertModel;
                }
            }
            throw new NotImplementedException();
        }


        public async Task<bool> CheckHealthAsync()
        {
            try { 
                using (var client = new AmazonDynamoDBClient(RegionEndpoint.USEast2))
                {
                    //File.WriteAllText(@"C:\Data\entra.txt", "entra");
                    var tableData = await client.DescribeTableAsync("Adverts");
                    //File.WriteAllText(@"C:\Data\good.txt", tableData.Table.TableStatus.ToString().ToLower());
                    return tableData.Table.TableStatus.ToString().ToLower() == "active";
                }
            }catch(Exception ex)
            {
                File.WriteAllText(@"C:\Data\error.txt", ex.Message);
                return false;
            }
        }
    }
}
