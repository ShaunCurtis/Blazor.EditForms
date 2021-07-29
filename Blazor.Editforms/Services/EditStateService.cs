/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System;

namespace Blazor.EditForms.Services
{
    /// <summary>
    /// Service Class for managing Form Edit State
    /// </summary>
    public class EditStateService
    {
        public object RecordID { get; set; }

        public bool IsDirty { get; set; }

        public string Data { get; set; }

        public string EditFormUrl { get; set; }

        public bool ShowEditForm => (!String.IsNullOrWhiteSpace(EditFormUrl)) && IsDirty;

        public bool DoFormReload { get; set; }

        public void SetEditState(string data)
        {
            this.Data = data;
            this.IsDirty = true;
        }

        public void ClearEditState()
        {
            this.Data = null;
            this.IsDirty = false;
        }

        public void ResetEditState()
        {
            this.RecordID = null;
            this.Data = null;
            this.IsDirty = false;
            this.EditFormUrl = string.Empty;
        }
    }
}
