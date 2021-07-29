using Blazor.EditForms.Components;
using Blazor.EditForms.Services;
using Blazor.EditForms.Web.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Blazor.EditForms.Web.Pages
{
    public partial class WeatherEditor : IDisposable
    {
        private EditContext _editContent;
        
        [Parameter]public Guid ID { get; set; }

        [Inject] private WeatherForecastViewService ViewService { get; set; }

        [Inject] private NavigationManager NavManager { get; set; }

        [Inject] private EditStateService EditStateService { get; set; }

        [Inject] private IJSRuntime _js { get; set; }

        private bool IsDirty => this.EditStateService.IsDirty;

        protected ComponentState LoadState { get; set; } = ComponentState.New;

        protected async override Task OnInitializedAsync()
        {
            this.LoadState = ComponentState.Loading;
            var id = Guid.Empty;
            if (this.EditStateService.IsDirty)
                id = (Guid)this.EditStateService.RecordID;
            id = id != Guid.Empty ? id : this.ID;
            await ViewService.GetRecordAsync(id);
            _editContent = new EditContext(this.ViewService.Record);
            this.EditStateService.EditFormUrl = NavManager.Uri;
            this.EditStateService.RecordID = id;
            this.ViewService.RecordChanged += this.OnRecordChanged;
            this.LoadState = ComponentState.Loaded;
        }

        private void OnRecordChanged(object sender, EventArgs e)
            =>  this.InvokeAsync(StateHasChanged);

        private void OnEditStateChanged(bool change)
            =>  this.InvokeAsync(StateHasChanged);

        private async Task SaveRecord()
        {
            await this.ViewService.UpdateRecordAsync();
            this.EditStateService.NotifyRecordSaved();
        }

        private void Exit()
        {
            this.EditStateService.ResetEditState();
            this.SetPageExitCheck(false);
            NavManager.NavigateTo("/fetchdata");
        }

        private void SetPageExitCheck(bool action)
            => _js.InvokeAsync<bool>("cecblazor_setEditorExitCheck", action);

        public void Dispose()
            => this.ViewService.RecordChanged -= this.OnRecordChanged;
    }
}
