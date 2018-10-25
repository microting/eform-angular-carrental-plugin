﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RentableItems.Pn.Infrastructure.Models
{
    class RentableItemSettingsModel
    {
        public int Id { get; set; }
        public string WorkflowState { get; set; }
        public int? Version { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedByUserID { get; set; }
        public int UpdatedByUserID { get; set; }
        public int EformId { get; set; }

    }
}