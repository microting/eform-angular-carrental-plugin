﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RentableItems.Pn.Infrastructure.Data;
using RentableItems.Pn.Infrastructure.Data.Entities;

namespace RentableItems.Pn.Infrastructure.Models
{
    public class ContractInspectionModel : IModel
    {
        public int Id { get; set; }
        public string WorkflowState { get; set; }
        public int Version { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedByUserID { get; set; }
        public int UpdatedByUserID { get; set; }
        public int ContractId { get; set; }
        public int SdkCaseId { get; set; }
        public DateTime? DoneAt { get; set; }


        public void Save(RentableItemsPnDbMSSQL _dbContext)
        {
            ContractInspection contractInspection = new ContractInspection();

            contractInspection.WorkflowState = eFormShared.Constants.WorkflowStates.Created;
            contractInspection.Version = Version;
            contractInspection.CreatedAt = DateTime.Now;
            contractInspection.UpdatedAt = DateTime.Now;
            contractInspection.Created_By_User_Id = CreatedByUserID;
            contractInspection.Updated_By_User_Id = UpdatedByUserID;
            contractInspection.ContractId = ContractId;

            _dbContext.ContractInspection.Add(contractInspection);
            _dbContext.SaveChanges();
        }

        public void Update(RentableItemsPnDbMSSQL _dbContext)
        {
            ContractInspection contractInspection = _dbContext.ContractInspection.FirstOrDefault(x => x.Id == Id);

            if (contractInspection == null)
            {
                throw new NullReferenceException($"Could not find Contract Inspection with id {Id}");
            }

            contractInspection.WorkflowState = WorkflowState;
            contractInspection.ContractId = ContractId;
            contractInspection.SDK_Case_Id = SdkCaseId;
            
            if(_dbContext.ChangeTracker.HasChanges())
            {
                contractInspection.UpdatedAt = DateTime.Now;
                contractInspection.Updated_By_User_Id = UpdatedByUserID;
                contractInspection.Version += 1;
                _dbContext.SaveChanges();
                MapContractInspection(_dbContext, contractInspection);
            }

        }

        public void Delete(RentableItemsPnDbMSSQL _dbContext)
        {
            WorkflowState = eFormShared.Constants.WorkflowStates.Removed;
            Update(_dbContext);
        }

        public void MapContractInspection(RentableItemsPnDbMSSQL _dbContext, ContractInspection contractInspection)
        {
            ContractInspectionVersion contractInspectionVer = new ContractInspectionVersion();

            contractInspectionVer.ContractId = contractInspection.ContractId;
            contractInspectionVer.Created_at = contractInspection.CreatedAt;
            contractInspectionVer.Created_By_User_Id = contractInspection.Created_By_User_Id;
            contractInspectionVer.DoneAt = contractInspection.DoneAt;
            contractInspectionVer.SDK_Case_Id = contractInspection.SDK_Case_Id;
            contractInspectionVer.Status = contractInspection.Status;
            contractInspectionVer.Updated_at = contractInspection.UpdatedAt;
            contractInspectionVer.Updated_By_User_Id = contractInspection.Updated_By_User_Id;
            contractInspectionVer.Version = contractInspection.Version;
            contractInspectionVer.Workflow_state = contractInspection.WorkflowState;

            contractInspectionVer.ContractInspectionId = contractInspection.Id;

        }
    }
}