using AppStoreManagementSystem.Domain;
using AppStoreManagementSystem.Domain.Features.Api.Features.App.Models;
using AppStoreManagementSystem.Domain.Features.App.Models;

namespace AppManagementSystem.App.Components.Features.App
{
    public partial class AppList
    {
        private const string ListMode = "list";
        private const string UploadMode = "upload";
        private const string EditAppMode = "edit-app";
        private const string AddCategoryMode = "add-category";
        private const string EditCategoryMode = "edit-category";

        private AppListRequestModel request = new();
        private AppCreateRequestModel uploadRequest = new();
        private AppUpdateRequestModel editAppRequest = new();
        private AppCategoryCreateRequestModel categoryCreateRequest = new();
        private AppCategoryUpdateRequestModel categoryEditRequest = new();
        private Result<AppListResponseModel> response = new();
        private List<AppCategoryListModel> categories = new();
        private List<AppCategoryListModel> editCategories = new();
        private int rowNo = 0;
        private string currentMode = ListMode;
        private string uploadMessage = string.Empty;
        private string appEditMessage = string.Empty;
        private string categoryMessage = string.Empty;
        private string originalVersion = string.Empty;
        private bool isBusy;
        private bool isEditingApp;
        private bool isEditingCategory;
        private bool showConfirmModal;
        private string confirmMessage = string.Empty;
        private Func<Task>? confirmAction;

        protected override async Task OnInitializedAsync()
        {
            request = new AppListRequestModel
            {
                PageNo = 1,
                PageSize = 10
            };

            await LoadApps(false);
            await LoadCategories(false);
        }

        private async Task LoadApps(bool includeInactive)
        {
            request.IncludeInactive = includeInactive;
            response = await ApiService.GetApps(request);
        }

        private async Task LoadCategories(bool includeInactive)
        {
            var result = await ApiService.GetCategories(includeInactive);

            if (includeInactive)
            {
                editCategories = result.Data ?? new List<AppCategoryListModel>();
            }
            else
            {
                categories = result.Data ?? new List<AppCategoryListModel>();
            }
        }

        private async Task ShowList()
        {
            currentMode = ListMode;
            ClearMessages();
            isEditingApp = false;
            isEditingCategory = false;
            await LoadApps(false);
            await LoadCategories(false);
        }

        private void ShowUploadForm()
        {
            currentMode = UploadMode;
            ClearMessages();
            uploadRequest = new AppCreateRequestModel();
        }

        private async Task ShowEditApps()
        {
            currentMode = EditAppMode;
            ClearMessages();
            isEditingApp = false;
            await LoadApps(true);
            await LoadCategories(false);
        }

        private void ShowAddCategory()
        {
            currentMode = AddCategoryMode;
            ClearMessages();
            categoryCreateRequest = new AppCategoryCreateRequestModel();
        }

        private async Task ShowEditCategories()
        {
            currentMode = EditCategoryMode;
            ClearMessages();
            isEditingCategory = false;
            await LoadCategories(true);
        }

        private async Task UploadApp()
        {
            await ExecuteSave(async () =>
            {
                var result = await ApiService.CreateApp(uploadRequest);
                uploadMessage = result.Message;

                if (result.IsSuccess)
                {
                    uploadRequest = new AppCreateRequestModel();
                    await ShowList();
                }
            }, message => uploadMessage = message);
        }

        private void StartEditApp(AppModel app)
        {
            originalVersion = app.Version;
            editAppRequest = new AppUpdateRequestModel
            {
                AppId = app.AppId,
                AppName = app.AppName,
                Description = app.Description,
                Version = app.Version,
                FileSize = app.FileSize,
                Status = app.Status,
                CategoryId = app.CategoryId,
                IsDelete = app.IsDelete
            };
            appEditMessage = string.Empty;
            isEditingApp = true;
        }

        private void CancelEditApp()
        {
            isEditingApp = false;
            appEditMessage = string.Empty;
            editAppRequest = new AppUpdateRequestModel();
        }

        private void SetAppStatus(string status)
        {
            editAppRequest.Status = status;
        }

        private void SetAppIsDelete(bool isDelete)
        {
            editAppRequest.IsDelete = isDelete;
        }

        private void ConfirmEditApp()
        {
            if (!IsVersionGreater(editAppRequest.Version, originalVersion))
            {
                appEditMessage = $"Version must be greater than current version ({originalVersion}).";
                return;
            }

            ShowConfirm("Are you sure you want to apply these changes to this app?", ApplyAppUpdate);
        }

        private async Task ApplyAppUpdate()
        {
            await ExecuteSave(async () =>
            {
                var result = await ApiService.UpdateApp(editAppRequest);
                appEditMessage = result.Message;

                if (result.IsSuccess)
                {
                    isEditingApp = false;
                    await LoadApps(true);
                    await LoadCategories(false);
                }
            }, message => appEditMessage = message);
        }

        private void ConfirmAddCategory()
        {
            ShowConfirm("Are you sure you want to add this category?", AddCategory);
        }

        private async Task AddCategory()
        {
            await ExecuteSave(async () =>
            {
                var result = await ApiService.CreateCategory(categoryCreateRequest);
                categoryMessage = result.Message;

                if (result.IsSuccess)
                {
                    categoryCreateRequest = new AppCategoryCreateRequestModel();
                    await LoadCategories(false);
                }
            }, message => categoryMessage = message);
        }

        private void StartEditCategory(AppCategoryListModel category)
        {
            categoryEditRequest = new AppCategoryUpdateRequestModel
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                IsActive = category.IsActive,
                IsDelete = category.IsDelete
            };
            categoryMessage = string.Empty;
            isEditingCategory = true;
        }

        private void CancelEditCategory()
        {
            isEditingCategory = false;
            categoryMessage = string.Empty;
            categoryEditRequest = new AppCategoryUpdateRequestModel();
        }

        private void SetCategoryIsActive(bool isActive)
        {
            categoryEditRequest.IsActive = isActive;
        }

        private void SetCategoryIsDelete(bool isDelete)
        {
            categoryEditRequest.IsDelete = isDelete;
        }

        private void ConfirmEditCategory()
        {
            ShowConfirm("Are you sure you want to save these category changes?", ApplyCategoryUpdate);
        }

        private async Task ApplyCategoryUpdate()
        {
            await ExecuteSave(async () =>
            {
                var result = await ApiService.UpdateCategory(categoryEditRequest);
                categoryMessage = result.Message;

                if (result.IsSuccess)
                {
                    isEditingCategory = false;
                    await LoadCategories(true);
                    await LoadCategories(false);
                }
            }, message => categoryMessage = message);
        }

        private void ShowConfirm(string message, Func<Task> action)
        {
            confirmMessage = message;
            confirmAction = action;
            showConfirmModal = true;
        }

        private async Task ConfirmModalYes()
        {
            showConfirmModal = false;

            if (confirmAction is not null)
            {
                await confirmAction();
            }

            confirmAction = null;
            confirmMessage = string.Empty;
        }

        private void ConfirmModalNo()
        {
            showConfirmModal = false;
            confirmAction = null;
            confirmMessage = string.Empty;
        }

        private async Task ExecuteSave(Func<Task> action, Action<string> setError)
        {
            isBusy = true;

            try
            {
                await action();
            }
            catch (Exception ex)
            {
                setError(ex.Message);
            }
            finally
            {
                isBusy = false;
            }
        }

        private void ClearMessages()
        {
            uploadMessage = string.Empty;
            appEditMessage = string.Empty;
            categoryMessage = string.Empty;
        }

        private static bool IsVersionGreater(string newVersion, string currentVersion)
        {
            var newParts = ParseVersion(newVersion);
            var currentParts = ParseVersion(currentVersion);
            int maxLength = Math.Max(newParts.Count, currentParts.Count);

            for (int i = 0; i < maxLength; i++)
            {
                int newPart = i < newParts.Count ? newParts[i] : 0;
                int currentPart = i < currentParts.Count ? currentParts[i] : 0;

                if (newPart > currentPart) return true;
                if (newPart < currentPart) return false;
            }

            return false;
        }

        private static List<int> ParseVersion(string version)
        {
            return version
                .Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => int.TryParse(x, out int value) ? value : -1)
                .ToList();
        }
    }
}
