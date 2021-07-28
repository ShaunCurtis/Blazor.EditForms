using Blazor.EditForms.Components;
using Blazor.EditForms.Services;
using Blazor.EditForms.Web.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Threading.Tasks;

namespace Blazor.EditForms.Web.Pages
{
    public partial class WeatherEditor : IDisposable
    {
        private EditContext _editContent;
        private EditFormState _editFormState;
        
        [Parameter]public Guid ID { get; set; }

        [Inject] private WeatherForecastViewService ViewService { get; set; }

        [Inject] private NavigationManager NavManager { get; set; }

        [Inject] private EditStateService EditStateService { get; set; }

        private bool IsDirty => this._editFormState?.IsDirty ?? false;

        public Guid FormId { get; } = new Guid("68eb8db6-65f4-40b4-b88a-be54d95ee866");

        protected async override Task OnInitializedAsync()
        {
            await ViewService.GetRecordAsync(ID);
            _editContent = new EditContext(this.ViewService.Record);
            this.EditStateService.EditFormId = this.FormId;
            this.EditStateService.EditForm = this.GetType();
            this.ViewService.RecordChanged += this.OnRecordChanged;
        }

        private void OnRecordChanged(object sender, EventArgs e)
        {
            this.InvokeAsync(StateHasChanged);
        }

        private void OnEditStateChanged(bool change)
        {
            this.InvokeAsync(StateHasChanged);
        }

        private async Task SaveRecord()
        {
            await this.ViewService.UpdateRecordAsync();
        }

        private void Exit()
        {
            NavManager.NavigateTo("/fetchdata");
        }

        public void Dispose()
        {
            this.ViewService.RecordChanged -= this.OnRecordChanged;
        }
    }
}
