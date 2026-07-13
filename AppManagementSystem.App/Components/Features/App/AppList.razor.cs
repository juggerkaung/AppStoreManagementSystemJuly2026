using AppStoreManagementSystem.Domain;
using AppStoreManagementSystem.Domain.Features.Api.Features.App.Models;
using AppStoreManagementSystem.Domain.Features.App.Models;

namespace AppManagementSystem.App.Components.Features.App
{
    public partial class AppList
    {
        private AppListRequestModel request = new();
        private AppCreateRequestModel uploadRequest = new();
        private Result<AppListResponseModel> response = new();
        private List<AppCategoryListModel> categories = new();
        private int rowNo = 0;
        private bool showUploadForm;
        private bool isUploading;
        private string uploadMessage = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            request = new AppListRequestModel
            {
                PageNo = 1,
                PageSize = 10
            };

            await LoadApps();
            await LoadCategories();
        }

        private async Task LoadApps()
        {
            response = await ApiService.GetApps(request);
        }

        private async Task LoadCategories()
        {
            var result = await ApiService.GetCategories();
            categories = result.Data ?? new List<AppCategoryListModel>();
        }

        private void ShowUploadForm()
        {
            uploadMessage = string.Empty;
            uploadRequest = new AppCreateRequestModel();
            showUploadForm = true;
        }

        private void HideUploadForm()
        {
            uploadMessage = string.Empty;
            showUploadForm = false;
        }

        private async Task UploadApp()
        {
            if (uploadRequest.CategoryId == 0)
            {
                uploadMessage = "Please select a category.";
                return;
            }

            isUploading = true;
            uploadMessage = string.Empty;

            try
            {
                var result = await ApiService.CreateApp(uploadRequest);

                if (result.IsSuccess)
                {
                    uploadMessage = result.Message;
                    uploadRequest = new AppCreateRequestModel();
                    showUploadForm = false;
                    await LoadApps();
                }
                else
                {
                    uploadMessage = result.Message;
                }
            }
            catch (Exception ex)
            {
                uploadMessage = ex.Message;
            }
            finally
            {
                isUploading = false;
            }
        }
    }
}
