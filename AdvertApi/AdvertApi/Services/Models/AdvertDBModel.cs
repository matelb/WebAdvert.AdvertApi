using AdvertApi.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace AdvertApi.Services.Models
{
    [DynamoDBTable("Adverts")]
    public class AdvertDBModel
    {
        [DynamoDBHashKey]
        public string ID { get; set; }

        [DynamoDBProperty]
        public string Title { get; set; }

        [DynamoDBProperty]
        public string Description { get; set; }

        [DynamoDBProperty]
        public double Price { get; set; }

        [DynamoDBProperty]
        public DateTime CreationDateTime { get; set; }

        [DynamoDBProperty]
        public AdvertStatus Status { get; set; }
    }
}
