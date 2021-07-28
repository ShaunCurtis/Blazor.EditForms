/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazor.EditForms.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.EditForms.Services
{
    /// <summary>
    /// Service Class for managing Cusotm Routes and Runtime Layout Changes
    /// </summary>
    public class EditStateService
    {
        private double garbageCollectionMinutes = -15;

        public Guid EditFormId { get; set; }

        public Type EditForm { get; set; }

        public bool ConfirmDirtyExit => EditForm is not null;

        public bool HasEditForm => EditForm is not null;

        public List<EditStateData> EditStates { get; private set; } = new List<EditStateData>();

        public void AddEditState(EditStateData data)
        {
            ClearEditStateGarbage();
            if (this.EditStates.Any(item => item.FormId == data.FormId))
            {
                var rec = this.EditStates.FirstOrDefault(item => item.FormId == data.FormId);
                EditStates.Remove(rec);
            }
            EditStates.Add(data);
        }

        public EditStateData GetEditState()
        {
            ClearEditStateGarbage();
            return this.EditStates.FirstOrDefault(item => item.FormId == this.EditFormId);
        }

        public bool ClearEditState()
        {
            ClearEditStateGarbage();
            var rec = this.EditStates.FirstOrDefault(item => item.FormId == this.EditFormId);
            var isRecord = rec != null;
            if (isRecord)
                EditStates.Remove(rec);

            this.EditForm = null;
            this.EditFormId = Guid.Empty;
            return isRecord;
        }

        private void ClearEditStateGarbage()
        {
            var list = EditStates.Where(item => item.DateStamp < DateTimeOffset.Now.AddMinutes(garbageCollectionMinutes)).ToList();
            list?.ForEach(item => EditStates.Remove(item));
        }
    }
}
