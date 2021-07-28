/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System;

namespace Blazor.EditForms.Data
{
    public record EditStateData
    {
        public Guid FormId { get; init; }

        public DateTimeOffset DateStamp { get; init; } = DateTimeOffset.Now;

        public string Data { get; init; }
    }
}
