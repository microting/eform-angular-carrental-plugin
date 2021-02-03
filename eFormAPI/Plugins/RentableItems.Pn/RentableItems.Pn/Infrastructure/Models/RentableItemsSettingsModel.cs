﻿using System;

namespace RentableItems.Pn.Infrastructure.Models
{
    public class RentableItemsSettingsModel
    {
        public int Id { get; set; }
        public string WorkflowState { get; set; }
        public int Version { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedByUserID { get; set; }
        public int UpdatedByUserID { get; set; }
        public int? eFormId { get; set; }
        
    }
}
