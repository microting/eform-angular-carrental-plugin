﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using eFormCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.eForm.Dto;
using Microting.eForm.Infrastructure;
using Microting.eForm.Infrastructure.Constants;
using Microting.eForm.Infrastructure.Data.Entities;
using Microting.eForm.Infrastructure.Models;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Extensions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eFormBaseCustomerBase.Infrastructure.Data;
using Microting.eFormBaseCustomerBase.Infrastructure.Data.Entities;
using Microting.eFormRentableItemBase.Infrastructure.Data;
using Microting.eFormRentableItemBase.Infrastructure.Data.Entities;
using RentableItems.Pn.Abstractions;
using RentableItems.Pn.Infrastructure.Models;
using RentableItems.Pn.Infrastructure.Models.Customer;

namespace RentableItems.Pn.Services
{
    public class ContractsInspectionService : IContractsInspectionService
    {
        private readonly ILogger<ContractsInspectionService> _logger;
        private readonly IRentableItemsLocalizationService _rentableItemsLocalizationService;
        private readonly eFormRentableItemPnDbContext _dbContext;
        private readonly IUserService _userService;
        private readonly CustomersPnDbAnySql _customersPnDbContext;
        private readonly IEFormCoreService _coreHelper;
        private readonly CustomersPnDbAnySql _customerDbContext;

        public ContractsInspectionService(eFormRentableItemPnDbContext dbContext,
            CustomersPnDbAnySql customerDbContext,
            IUserService userService,
            CustomersPnDbAnySql customersPnDbContext,
            ILogger<ContractsInspectionService> logger,
            IEFormCoreService coreHelper,
            IRentableItemsLocalizationService rentableItemLocalizationService
            )
        {
            _dbContext = dbContext;
            _customerDbContext = customerDbContext;
            _userService = userService;
            _customersPnDbContext = customersPnDbContext;
            _logger = logger;
            _coreHelper = coreHelper;
            _rentableItemsLocalizationService = rentableItemLocalizationService;
        }
        public async Task<OperationDataResult<ContractInspectionsModel>> Index(ContractInspectionsRequestModel contractInspectionsPnRequestModel)
        {
            try
            {
                ContractInspectionsModel contractInspectionsModel = new ContractInspectionsModel();
                Core _core = await _coreHelper.GetCore();

                IQueryable<ContractInspection> contractInspectionsQuery = _dbContext.ContractInspection.
                    Where(x => x.WorkflowState != Constants.WorkflowStates.Removed).AsQueryable();
                if (!string.IsNullOrEmpty(contractInspectionsPnRequestModel.SortColumnName)
                    && contractInspectionsPnRequestModel.SortColumnName != "")
                {
                    if (contractInspectionsPnRequestModel.IsSortDsc)
                    {
                        contractInspectionsQuery = contractInspectionsQuery.CustomOrderByDescending(contractInspectionsPnRequestModel.SortColumnName);
                    }
                    else
                    {
                        contractInspectionsQuery = contractInspectionsQuery.CustomOrderBy(contractInspectionsPnRequestModel.SortColumnName);

                    }
                }
                contractInspectionsModel.Total = await contractInspectionsQuery.CountAsync(x => x.WorkflowState != Constants.WorkflowStates.Removed);
                contractInspectionsQuery
                    = contractInspectionsQuery
                        .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed)
                        .Skip(contractInspectionsPnRequestModel.Offset)
                        .Take(contractInspectionsPnRequestModel.PageSize);
                List<ContractInspection> contractInspections = await contractInspectionsQuery.ToListAsync();
                contractInspections.ForEach(contractInspection =>
                {
                    ContractInspectionItem contractInspectionItem =
                        _dbContext.ContractInspectionItem.FirstOrDefault(x =>
                            x.ContractInspectionId == contractInspection.Id);

                    Contract contract = _dbContext.Contract.Single(x => x.Id == contractInspection.ContractId);

                    Customer customer =
                        _customerDbContext.Customers.Single(x => x.Id == contract.CustomerId);
                    RentableItemCustomerModel rentableItemCustomerModel = new RentableItemCustomerModel
                    {
                        Id = customer.Id,
                        CustomerNo = customer.CustomerNo,
                        CompanyName = customer.CompanyName,
                        ContactPerson = customer.ContactPerson,
                        CompanyAddress = customer.CompanyAddress,
                        CompanyAddress2 = customer.CompanyAddress2,
                        CityName = customer.CityName,
                        ZipCode = customer.ZipCode,
                        CountryCode = customer.CountryCode,
                        EanCode = customer.EanCode,
                        VatNumber = customer.VatNumber,
                        Email = customer.Email,
                        Phone = customer.Phone,
                        Description = customer.Description
                    };
                    List<RentableItemModel> rentableItemModels = new List<RentableItemModel>();
                    foreach (ContractRentableItem contractRentableItem in _dbContext.ContractRentableItem.Where(x =>
                        x.ContractId == contract.Id && x.WorkflowState == Constants.WorkflowStates.Created).ToList())
                    {
                        RentableItem rentableItem = _dbContext.RentableItem.Single(x => x.Id == contractRentableItem.RentableItemId);
                        RentableItemModel rentableItemModel = new RentableItemModel
                        {
                            Id = rentableItem.Id,
                            Brand = rentableItem.Brand,
                            ModelName = rentableItem.ModelName,
                            PlateNumber = rentableItem.PlateNumber,
                            VinNumber = rentableItem.VinNumber,
                            SerialNumber = rentableItem.SerialNumber,
                            RegistrationDate = rentableItem.RegistrationDate,
                            EFormId = rentableItem.eFormId
                        };
                        rentableItemModels.Add(rentableItemModel);
                    }

                    using (var dbContext = _core.DbContextHelper.GetDbContext())
                    {
                        if (contractInspectionItem != null)
                            contractInspectionsModel.ContractInspections.Add(new ContractInspectionModel
                            {
                                SdkCaseApiId = contractInspectionItem.SDKCaseId,
                                SdkCaseId = dbContext.Cases
                                    .Single(x => x.MicrotingUid == contractInspectionItem.SDKCaseId).Id,
                                eFormId = rentableItemModels.First().EFormId,
                                ContractId = contractInspection.ContractId,
                                ContractStart = contract.ContractStart,
                                ContractEnd = contract.ContractEnd,
                                DoneAt = contractInspection.DoneAt,
                                Id = contractInspection.Id,
                                Status = contractInspectionItem.Status,
                                RentableItemCustomer = rentableItemCustomerModel,
                                RentableItems = rentableItemModels
                            });
                    }


                });
                return new OperationDataResult<ContractInspectionsModel>(true, contractInspectionsModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<ContractInspectionsModel>(false,
                    _rentableItemsLocalizationService.GetString("ErrorObtainingContractInspectionInfo"));
            }
        }

        public async Task<OperationResult> Create(ContractInspectionModel contractInspectionCreateModel)
        {
            try
            {
                Core core = await _coreHelper.GetCore();
                await using MicrotingDbContext context = core.DbContextHelper.GetDbContext();

                // finde eform fra settings
                List<ContractRentableItem> contractRentableItem =
                    await _dbContext.ContractRentableItem.Where(x =>
                        x.ContractId == contractInspectionCreateModel.ContractId && x.WorkflowState != Constants.WorkflowStates.Removed).ToListAsync();
                foreach (var item in contractRentableItem)
                {
                    int rentableItemId = item.RentableItemId;

                    RentableItem rentableItem =
                        await _dbContext.RentableItem.FirstOrDefaultAsync(x => x.Id == rentableItemId);
                    int eFormId = rentableItem.eFormId;

                    Contract dbContract =
                        await _dbContext.Contract.FirstOrDefaultAsync(x =>
                            x.Id == contractInspectionCreateModel.ContractId);
                    Customer dbCustomer =
                        await _customersPnDbContext.Customers.SingleOrDefaultAsync(x => x.Id == dbContract.CustomerId);

                    Site site = await context.Sites.SingleAsync(x => x.Id == contractInspectionCreateModel.SiteId);
                    Language language = await context.Languages.SingleAsync(x => x.Id == site.LanguageId);

                    MainElement mainElement = await core.ReadeForm(eFormId, language);
                    mainElement.Repeated = 1;
                    mainElement.EndDate = DateTime.Now.AddDays(14).ToUniversalTime();
                    mainElement.StartDate = DateTime.Now.ToUniversalTime();
                    mainElement.Label = "";
                    mainElement.Label += string.IsNullOrEmpty(rentableItem.SerialNumber)
                        ? ""
                        : $"{rentableItem.SerialNumber}";
                    mainElement.Label += string.IsNullOrEmpty(rentableItem.VinNumber)
                        ? ""
                        : $"{rentableItem.VinNumber}";
                    mainElement.Label += string.IsNullOrEmpty(rentableItem.Brand)
                        ? ""
                        : $"<br>{rentableItem.Brand}";
                    mainElement.Label += string.IsNullOrEmpty(rentableItem.ModelName)
                        ? ""
                        : $"<br>{rentableItem.ModelName}";
                    mainElement.Label += string.IsNullOrEmpty(dbCustomer.ContactPerson)
                        ? ""
                        : $"<br>{dbCustomer.ContactPerson}";

                    CDataValue cDataValue = new CDataValue();
                    cDataValue.InderValue = $"<b>Kontrakt Nr:<b>{dbContract.ContractNr.ToString()}<br>";
                    cDataValue.InderValue += $"<b>Kunde Nr:<b>{dbContract.CustomerId.ToString()}";

                    List<SiteDto> sites = new List<SiteDto>();

                    int? sdkCaseId = await core.CaseCreate(mainElement, "", (int)site.MicrotingUid, null);

                    if (sdkCaseId != null)
                    {
                        ContractInspection contractInspection = new ContractInspection
                        {
                            ContractId = contractInspectionCreateModel.ContractId
                        };
                        await contractInspection.Create(_dbContext);
                        ContractInspectionItem contractInspectionItem = new ContractInspectionItem
                        {
                            ContractInspectionId = contractInspection.Id,
                            RentableItemId = rentableItemId,
                            SiteId = site.Id,
                            SDKCaseId = (int) sdkCaseId,
                            Status = 33
                        };
                        await contractInspectionItem.Create(_dbContext);
                    }
                }

                return new OperationResult(true, "Inspection Created Successfully");
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false, _rentableItemsLocalizationService.GetString("ErrorWhileCreatingContractInspection"));
            }
        }

        public async Task<OperationResult> Update(ContractInspectionModel contractInspectionUpdateModel)
        {
            try
            {
                ContractInspection contractInspection =
                    await _dbContext.ContractInspection.SingleOrDefaultAsync(x =>
                        x.Id == contractInspectionUpdateModel.Id);
                if (contractInspection != null)
                {
                    contractInspection.ContractId = contractInspectionUpdateModel.ContractId;
                    contractInspection.DoneAt = contractInspectionUpdateModel.DoneAt;
                    await contractInspection.Update(_dbContext);
                }
                return new OperationResult(true);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(true, _rentableItemsLocalizationService.GetString("ErrorWhileUpdatingContractInspection"));
            }
        }
        public async Task<OperationResult> Delete(int id)
        {
            Core _core = await _coreHelper.GetCore();

            ContractInspection dbContractInspection =
                await _dbContext.ContractInspection.SingleOrDefaultAsync(x => x.Id == id);

            try
            {
                if (dbContractInspection != null)
                {
                    await dbContractInspection.Delete(_dbContext);
                    ContractInspectionItem contractInspectionItem =
                        await _dbContext.ContractInspectionItem.SingleOrDefaultAsync(x =>
                            x.ContractInspectionId == dbContractInspection.Id);
                    CaseDto caseDto = await _core.CaseLookupMUId(contractInspectionItem.SDKCaseId);
                    if (caseDto.MicrotingUId != null)
                    {
                        await _core.CaseDelete((int) caseDto.MicrotingUId);
                    }
                }
                return new OperationResult(true);
            }
            catch ( Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false, _rentableItemsLocalizationService.GetString("ErrorWhileDeletingContractInspection"));
            }
        }

        public async Task<OperationDataResult<ContractInspectionModel>> Get(int id)
        {
            try
            {
                return new OperationDataResult<ContractInspectionModel>(true);
            }
            catch (Exception ex)
            {
                return new OperationDataResult<ContractInspectionModel>(false, _rentableItemsLocalizationService.GetString("ErrorGettingContractInspection"));
            }
        }

        public async Task<string> DownloadEFormPdf(int id, string token, string fileType)
        {
            Core core = await _coreHelper.GetCore();
            var locale = await _userService.GetCurrentUserLocale();
            var language = core.DbContextHelper.GetDbContext().Languages.Single(x => x.LanguageCode.ToLower() == locale.ToLower());
            string microtingUId;
            string microtingCheckUId;
            int caseId = 0;
            int eFormId = 0;

            ContractInspection contractInspection =
                await _dbContext.ContractInspection.SingleOrDefaultAsync(x => x.Id == id);

            Contract contract = _dbContext.Contract.Single(x => x.Id == contractInspection.ContractId);

            int i = 0;
            XElement xmlContent = new XElement("ContractInspection");
            foreach (ContractRentableItem contractRentableItem in _dbContext.ContractRentableItem
                .Where(x => x.ContractId == contract.Id
                            && x.WorkflowState == Constants.WorkflowStates.Created)
                .ToList())
            {
                RentableItem rentableItem = _dbContext.RentableItem.Single(x => x.Id == contractRentableItem.RentableItemId);
                xmlContent.Add(new XElement($"Brand_{i}", rentableItem.Brand));
                xmlContent.Add(new XElement($"ModelName_{i}", rentableItem.ModelName));
                xmlContent.Add(new XElement($"Serial_{i}", rentableItem.SerialNumber));
                xmlContent.Add(new XElement($"RegistrationDate_{i}", rentableItem.RegistrationDate));
                xmlContent.Add(new XElement($"vinNumber_{i}", rentableItem.VinNumber));
                xmlContent.Add(new XElement($"PlateNumber_{i}", rentableItem.PlateNumber));
                i += 1;
            }

            Customer customer =
                _customerDbContext.Customers.Single(x => x.Id == contract.CustomerId);
            xmlContent.Add(new XElement("CustomerCustomerNo", customer.CustomerNo));
            xmlContent.Add(new XElement("CustomerCompanyName", customer.CompanyName));
            xmlContent.Add(new XElement("CustomerContactPerson", customer.ContactPerson));
            xmlContent.Add(new XElement("CustomerCompanyAddress", customer.CompanyAddress));
            xmlContent.Add(new XElement("CustomerCompanyAddress2", customer.CompanyAddress2));
            xmlContent.Add(new XElement("CustomerCityName", customer.CityName));
            xmlContent.Add(new XElement("CustomerZipCode", customer.ZipCode));
            xmlContent.Add(new XElement("CustomerCountryCode", customer.CountryCode));
            xmlContent.Add(new XElement("CustomerEanCode", customer.EanCode));
            xmlContent.Add(new XElement("CustomerVatNumber", customer.VatNumber));
            xmlContent.Add(new XElement("CustomerEmail", customer.Email));
            xmlContent.Add(new XElement("CustomerPhone", customer.Phone));
            xmlContent.Add(new XElement("CustomerDescription", customer.Description));

            _coreHelper.LogEvent($"DownloadEFormPdf: xmlContent is {xmlContent}");
            ContractInspectionItem contractInspectionItem =
                _dbContext.ContractInspectionItem.FirstOrDefault(x =>
                    x.ContractInspectionId == contractInspection.Id);

            CaseDto caseDto = await core.CaseLookupMUId(contractInspectionItem.SDKCaseId);
            caseId = (int)caseDto.CaseId;
            eFormId = caseDto.CheckListId;

            if (caseId != 0 && eFormId != 0)
            {
                _coreHelper.LogEvent($"DownloadEFormPdf: caseId is {caseId}, eFormId is {eFormId}");
                var filePath = await core.CaseToPdf(caseId, eFormId.ToString(),
                    DateTime.Now.ToString("yyyyMMddHHmmssffff"),
                    $"{await core.GetSdkSetting(Settings.httpServerAddress)}/" + "api/template-files/get-image/", xmlContent.ToString(), language);
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException();
                }

                return filePath;
            }

            throw new Exception("could not find case of eform!");
        }

        private void LogEvent(string appendText)
        {
            try
            {
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("[DBG] " + appendText);
                Console.ForegroundColor = oldColor;
            }
            catch
            {
            }
        }
    }
}
