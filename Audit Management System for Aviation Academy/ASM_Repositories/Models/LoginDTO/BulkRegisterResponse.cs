using System;
using System.Collections.Generic;

namespace ASM_Repositories.Models.LoginDTO
{
    public class BulkRegisterResponse
    {
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<BulkRegisterItem> SuccessItems { get; set; } = new List<BulkRegisterItem>();
        public List<BulkRegisterError> ErrorItems { get; set; } = new List<BulkRegisterError>();
    }

    public class BulkRegisterItem
    {
        public int RowNumber { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }

    public class BulkRegisterError
    {
        public int RowNumber { get; set; }
        public string Email { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}

