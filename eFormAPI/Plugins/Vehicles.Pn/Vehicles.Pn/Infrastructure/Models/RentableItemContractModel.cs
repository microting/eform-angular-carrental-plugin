﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RentableItems.Pn.Infrastructure.Data;
using RentableItems.Pn.Infrastructure.Data.Entities;

namespace RentableItems.Pn.Infrastructure.Models
{
    public class RentableItemContractModel : IModel
    {
        public int Id { get; set; }
        public string WorkflowState { get; set; }
        public int? Version { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedByUserID { get; set; }
        public int UpdatedByUserID { get; set; }
        public int RentableItemId { get; set; }
        public int ContractId { get; set; }

        public void Save(RentableItemsPnDbMSSQL _dbContext)
        {
            RentableItemContract rentableItemContract = new RentableItemContract();

            rentableItemContract.ContractId = ContractId;
            rentableItemContract.RentableItemId = RentableItemId;
            rentableItemContract.Workflow_state = eFormShared.Constants.WorkflowStates.Created;
            rentableItemContract.Created_at = DateTime.Now;
            rentableItemContract.Updated_at = DateTime.Now;
            rentableItemContract.Created_By_User_Id = CreatedByUserID;
            rentableItemContract.Updated_By_User_Id = UpdatedByUserID;

            _dbContext.RentableItemContract.Add(rentableItemContract);
            _dbContext.SaveChanges();

        }
        public void Update(RentableItemsPnDbMSSQL _dbContext)
        {
            RentableItemContract rentableItemContract = _dbContext.RentableItemContract.FirstOrDefault(x => x.Id == Id);

            if (rentableItemContract == null)
            {
                throw new NullReferenceException($"Could not find RentableItem Contract with id {Id}");
            }

            rentableItemContract.ContractId = ContractId;
            rentableItemContract.RentableItemId = RentableItemId;
            rentableItemContract.Workflow_state = WorkflowState;
            
            if (_dbContext.ChangeTracker.HasChanges())
            {
                rentableItemContract.Updated_at = DateTime.Now;
                rentableItemContract.Updated_By_User_Id = UpdatedByUserID;
                rentableItemContract.Version += 1;
                _dbContext.SaveChanges();
                MapRentableItemContractVersions(_dbContext, rentableItemContract);
            }
        }
        public void Delete(RentableItemsPnDbMSSQL _dbContext)
        {
            WorkflowState = eFormShared.Constants.WorkflowStates.Removed;
            Update(_dbContext);
        }

        public void MapRentableItemContractVersions(RentableItemsPnDbMSSQL _dbContext, RentableItemContract rentableItemContract)
        {
            RentableItemsContractVersions rentableItemscontractVer = new RentableItemsContractVersions();

            rentableItemscontractVer.ContractId = rentableItemContract.ContractId;
            rentableItemscontractVer.Created_at = rentableItemContract.Created_at;
            rentableItemscontractVer.Created_By_User_Id = rentableItemContract.Created_By_User_Id;
            rentableItemscontractVer.RentableItemId = rentableItemContract.RentableItemId;
            rentableItemscontractVer.Updated_at = rentableItemContract.Updated_at;
            rentableItemscontractVer.Updated_By_User_Id = rentableItemContract.Updated_By_User_Id;
            rentableItemscontractVer.Version = rentableItemContract.Version;
            rentableItemscontractVer.Workflow_state = rentableItemContract.Workflow_state;

            rentableItemscontractVer.RentableItemContractId = rentableItemContract.Id;

        }
    }
}