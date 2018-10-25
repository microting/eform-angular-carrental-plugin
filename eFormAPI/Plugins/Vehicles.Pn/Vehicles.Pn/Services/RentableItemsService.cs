﻿using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microting.eFormApi.BasePn.Infrastructure.Extensions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using RentableItems.Pn.Abstractions;
using RentableItems.Pn.Infrastructure.Data;
using RentableItems.Pn.Infrastructure.Data.Entities;
using RentableItems.Pn.Infrastructure.Models;

namespace RentableItems.Pn.Services
{
    public class RentableItemsService : IRentableItemsService
    {
        private readonly ILogger<RentableItemsService> _logger;
        private readonly IRentableItemsLocalizationService _rentableItemsLocalizationService;
        private readonly RentableItemsPnDbMSSQL _dbContext;

        public RentableItemsService(RentableItemsPnDbMSSQL dbContext,
            ILogger<RentableItemsService> logger,
            IRentableItemsLocalizationService rentableItemLocalizationService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _rentableItemsLocalizationService = rentableItemLocalizationService;
        }

        public OperationDataResult<RentableItemsModel> GetAllRentableItems(RentableItemsRequestModel pnRequestModel)
        {
            try
            {
                var rentableItemsPnModel = new RentableItemsModel();
                var rentableItemsQuery = _dbContext.RentableItem.AsQueryable();
                if (!string.IsNullOrEmpty(pnRequestModel.SortColumnName))
                {
                    if (pnRequestModel.IsSortDsc)
                    {
                        rentableItemsQuery = rentableItemsQuery.CustomOrderByDescending(pnRequestModel.SortColumnName);
                    }
                    else
                    {
                        rentableItemsQuery = rentableItemsQuery.CustomOrderBy(pnRequestModel.SortColumnName);
                    }
                }

                rentableItemsPnModel.Total = rentableItemsQuery.Count();
                rentableItemsQuery = rentableItemsQuery.Skip(pnRequestModel.Offset).Take(pnRequestModel.PageSize);
                var rentableItems = rentableItemsQuery.ToList();
                rentableItems.ForEach(rentableItem =>
                {
                    rentableItemsPnModel.RentableItems.Add(new RentableItemModel()
                    {
                        VinNumber = rentableItem.VinNumber,
                        RegistrationDate = rentableItem.RegistrationDate,
                        Brand = rentableItem.Brand,
                        PlateNumber = rentableItem.PlateNumber,
                        ModelName = rentableItem.ModelName,
                        SerialNumber = rentableItem.SerialNumber,
                        Id = rentableItem.Id,
                    });
                });
                return new OperationDataResult<RentableItemsModel>(true, rentableItemsPnModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<RentableItemsModel>(true,
                    _rentableItemsLocalizationService.GetString("ErrorObtainingRentableItemsInfo"));
            }
        }

        public OperationResult CreateRentableItem(RentableItemModel rentableItemPnCreateModel)
        {
            try
            {
                //RentableItemModel rentableItemModel = new RentableItemModel(rentableItemPnCreateModel.Id, rentableItemPnCreateModel.Brand,
                //    rentableItemPnCreateModel.ModelName, rentableItemPnCreateModel.RegistrationDate, rentableItemPnCreateModel.VinNumber,
                //    rentableItemPnCreateModel.SerialNumber, rentableItemPnCreateModel.PlateNumber);

                //rentableItemPn.VinNumber = rentableItemPnCreateModel.VinNumber;
                //rentableItemPn.Brand = rentableItemPnCreateModel.Brand;
                //rentableItemPn.SerialNumber = rentableItemPnCreateModel.SerialNumber;
                //rentableItemPn.PlateNumber = rentableItemPnCreateModel.PlateNumber;
                //rentableItemPn.ModelName = rentableItemPnCreateModel.ModelName;
                //rentableItemPn.RegistrationDate = rentableItemPnCreateModel.RegistrationDate;

                rentableItemPnCreateModel.Save(_dbContext);

                //_dbContext.RentableItem.Add(rentableItemPn);
                //_dbContext.SaveChanges();
                return new OperationResult(true,
                    _rentableItemsLocalizationService.GetString("RentableItemCreated", rentableItemPnCreateModel.Brand,
                        rentableItemPnCreateModel.ModelName));
            }
            catch (Exception ex)
            {
                //Trace.TraceError(e.Message);
                //_logger.LogError(e.Message);
                return new OperationResult(true, _rentableItemsLocalizationService.GetString("ErrorWhileCreatingRentableItem"));
            }
        }

        public OperationResult UpdateRentableItem(RentableItemModel rentableItemPnUpdateModel)
        {
            try
            {
                //    var rentableItem = _dbContext.RentableItem.FirstOrDefault(x => x.Id == rentableItemPnUpdateModel.Id);
                //    if (rentableItem == null)
                //    {
                //        return new OperationResult(true, "Rentable Item not found");
                //    }

                //    rentableItem.VinNumber = rentableItemPnUpdateModel.VinNumber;
                //    rentableItem.Brand = rentableItemPnUpdateModel.Brand;
                //    rentableItem.PlateNumber = rentableItemPnUpdateModel.PlateNumber;
                //    rentableItem.SerialNumber = rentableItemPnUpdateModel.SerialNumber;
                //    rentableItem.ModelName = rentableItemPnUpdateModel.ModelName;
                //    rentableItem.RegistrationDate = rentableItemPnUpdateModel.RegistrationDate;

                rentableItemPnUpdateModel.Update(_dbContext);
                return new OperationDataResult<RentableItemsModel>(true);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<RentableItemsModel>(true,
                    _rentableItemsLocalizationService.GetString("ErrorWhileUpdatingRentableItemInfo"));
            }
        }
    }
}