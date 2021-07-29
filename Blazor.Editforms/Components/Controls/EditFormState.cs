/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazor.EditForms.Data;
using Blazor.EditForms.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazor.EditForms.Components
{
    /// <summary>
    /// Component Class that adds Edit State and Validation State to a Blazor EditForm Control
    /// Should be placed within thr EditForm Control
    /// </summary>
    public class EditFormState : ComponentBase, IDisposable
    {
        private bool disposedValue;
        private EditFieldCollection EditFields = new EditFieldCollection();

        [CascadingParameter] public EditContext EditContext { get; set; }

        [Parameter] public EventCallback<bool> EditStateChanged { get; set; }

        [Inject] private EditStateService EditStateService { get; set; }

        [Inject] private IJSRuntime _js { get; set; }

        public bool IsDirty => EditFields?.IsDirty ?? false;

        protected override Task OnInitializedAsync()
        {
            Debug.Assert(this.EditContext != null);

            if (this.EditContext != null)
            {
                // Populates the EditField Collection
                this.LoadEditState();
                // Wires up to the EditContext OnFieldChanged event
                this.EditContext.OnFieldChanged += FieldChanged;
            }
            return Task.CompletedTask;
        }

        private void LoadEditState()
        {
            object data = null;
            var recordtype = this.EditContext.Model.GetType();
            if (EditStateService.IsDirty && !string.IsNullOrWhiteSpace(EditStateService.Data))
                data = JsonSerializer.Deserialize(EditStateService.Data, recordtype);

            this.GetEditFields(this.EditContext.Model, data);
            this.SetModelToEditState(this.EditContext.Model);
            if (EditFields.IsDirty)
                this.EditStateChanged.InvokeAsync(true);
        }

        private void GetEditFields(object model, object editedsource = null)
        {
            // Gets the fields from the model
            this.EditFields.Clear();
            if (model is not null)
            {
                var props = model.GetType().GetProperties();
                foreach (var prop in props)
                {
                    if (prop.CanWrite)
                    {
                        var value = prop.GetValue(model);
                        EditFields.AddField(model, prop.Name, value);
                    }
                }
            }
            // Update the fields with the values from the source
            if (editedsource is not null)
            {
                var props = editedsource.GetType().GetProperties();
                foreach (var property in props)
                {
                    var value = property.GetValue(editedsource);
                    var prop = model.GetType().GetProperty(property.Name);
                    if (prop != null)
                    {
                        // Sets the edit value in the EditField
                        EditFields.SetField(property.Name, value);
                    }
                }
            }
        }

        private void SetModelToEditState(object model)
        {
            var props = model.GetType().GetProperties();
            foreach (var property in props)
            {
                var value = EditFields.GetEditValue(property.Name);
                if (value is not null && property.CanWrite)
                    property.SetValue(model, value);
            }
        }

        private async void FieldChanged(object sender, FieldChangedEventArgs e)
        {
            var isDirty = EditFields?.IsDirty ?? false;
            // Get the PropertyInfo object for the model property
            // Uses reflection to get property and value
            var prop = e.FieldIdentifier.Model.GetType().GetProperty(e.FieldIdentifier.FieldName);
            if (prop != null)
            {
                // Get the value for the property
                var value = prop.GetValue(e.FieldIdentifier.Model);
                // Sets the edit value in the EditField
                EditFields.SetField(e.FieldIdentifier.FieldName, value);
                // Invokes EditStateChanged if changed
                var stateChange = (EditFields?.IsDirty ?? false) != isDirty;
                isDirty = EditFields?.IsDirty ?? false;
                if (stateChange)
                    await this.EditStateChanged.InvokeAsync(isDirty);
                if (isDirty)
                    this.SaveEditState();
                else
                    this.ClearEditState();
            }
        }

        private void SaveEditState()
        {
            this.SetPageExitCheck(true);
            var jsonData = JsonSerializer.Serialize(this.EditContext.Model);
            EditStateService.SetEditState(jsonData);
        }

        private void ClearEditState()
        {
            this.SetPageExitCheck(false);
            EditStateService.ClearEditState();
        }

        private void SetPageExitCheck(bool action)
            => _js.InvokeAsync<bool>("cecblazor_setEditorExitCheck", action);


        // IDisposable Implementation
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this.EditContext != null)
                        this.EditContext.OnFieldChanged -= this.FieldChanged;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
