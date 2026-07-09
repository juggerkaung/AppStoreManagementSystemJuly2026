using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppStoreManagementSystem.Domain.Features.App.Models
{
    public class AppUpdateRequestModel
    {
        public int AppId { get; set; }

        public string AppName { get; set; } = null!;

        public string? Description { get; set; }


        public string Version { get; set; } = null!;

        public string Status { get; set; } = null!;
    }
}
