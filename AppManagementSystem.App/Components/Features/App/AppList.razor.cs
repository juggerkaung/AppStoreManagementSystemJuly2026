using AppStoreManagementSystem.Domain;
using AppStoreManagementSystem.Domain.Features.Api.Features.App.Models;

namespace AppManagementSystem.App.Components.Features.App
{
    public partial class AppList
    {

        private AppListRequestModel request = new();
        private Result<AppListResponseModel> response = new();
        private int rowNo = 0;

        protected override async Task OnInitializedAsync()
        {
            request = new AppListRequestModel
            {
                PageNo = 1,
                PageSize = 10
            };
            response = await ApiService.GetApps(request);

        }
    }
}
