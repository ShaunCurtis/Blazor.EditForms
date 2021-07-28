// ==========================================================
//  Original code:
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// ============================================================

/// =================================
/// Mods Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


#nullable disable warnings

using Blazor.EditForms.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blazor.SPA.Components
{
    /// <summary>
    /// Displays the specified page component, rendering it inside its layout
    /// and any further nested layouts.
    /// Customized replacement version of RouteView Component
    /// Handles Dynamic Layout changes and changing RouteViews with no routing
    /// </summary>
    public class RouteViewManager : IComponent
    {
        private bool _RenderEventQueued;
        private RenderHandle _renderHandle;
        private bool _loadEditForm = false;

        [Parameter] public RouteData RouteData { get; set; }

        [Parameter] public Type DefaultLayout { get; set; }

        [Inject] private EditStateService EditStateService { get; set; }

        [Inject] private IJSRuntime _js { get; set; }

        /// <inheritdoc />
        public void Attach(RenderHandle renderHandle)
        {
            _renderHandle = renderHandle;
        }

        /// <inheritdoc />
        public async Task SetParametersAsync(ParameterView parameters)
        {
            // Sets the component parameters
            parameters.SetParameterProperties(this);

            // Check if we have or a EditForm
            if (RouteData is null || EditStateService.EditForm is null)
            {
                throw new InvalidOperationException($"The {nameof(RouteView)} component requires a non-null value for the parameter {nameof(RouteData)}.");
            }
            // Render the component
            await this.RenderAsync();
        }

        /// <summary>
        ///  RenderFragment Delegate run when rendering the component
        /// </summary>
        private RenderFragment _renderDelegate => builder =>
        {
            _RenderEventQueued = false;
            // Adds cascadingvalue for the ViewManager
            builder.OpenComponent<CascadingValue<RouteViewManager>>(0);
            builder.AddAttribute(1, "Value", this);
            // Get the layout render fragment
            builder.AddAttribute(2, "ChildContent", this._layoutViewFragment);
            builder.CloseComponent();
        };

        /// <summary>
        /// Render Fragment to build the layout with either the Routed component or the View Component
        /// </summary>
        private RenderFragment _layoutViewFragment => builder =>
        {
            Type _pageLayoutType = RouteData?.PageType.GetCustomAttribute<LayoutAttribute>()?.LayoutType
                ?? DefaultLayout;

            builder.OpenComponent<LayoutView>(0);
            builder.AddAttribute(1, nameof(LayoutView.Layout), _pageLayoutType);
            if (EditStateService.ConfirmDirtyExit && _loadEditForm is not true)
                builder.AddAttribute(2, nameof(LayoutView.ChildContent), _dirtyExitFragment);
            else
            {
                _loadEditForm = false;
                builder.AddAttribute(2, nameof(LayoutView.ChildContent), _renderComponentWithParameters);
            }
            builder.CloseComponent();
        };

        private RenderFragment _dirtyExitFragment => builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "dirty-exit");
            {
                builder.OpenElement(2, "div");
                builder.AddAttribute(3, "class", "dirty-exit-message");
                builder.AddContent(4, "You are existing a form with unsaved data");
                builder.CloseElement();
            }
            {
                builder.OpenElement(2, "div");
                builder.AddAttribute(3, "class", "dirty-exit-message");
                {
                    builder.OpenElement(2, "button");
                    builder.AddAttribute(3, "class", "dirty-exit-button");
                    builder.AddAttribute(5, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, this.DirtyExit));
                    builder.AddContent(6, "Exit and Clear Unsaved Data");
                    builder.CloseElement();
                }
                {
                    builder.OpenElement(2, "button");
                    builder.AddAttribute(3, "class", "load-dirty-form-button");
                    builder.AddAttribute(5, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, this.LoadDirtyForm));
                    builder.AddContent(6, "Reload Form");
                    builder.CloseElement();
                }
                builder.CloseElement();
            }
            builder.CloseElement();
        };

        /// <summary>
        /// Render Fragment to build the view or route component
        /// </summary>
        private RenderFragment _renderComponentWithParameters => builder =>
        {
            Type componentType = null;
            IReadOnlyDictionary<string, object> parameters = new Dictionary<string, object>();

            if (this.EditStateService.HasEditForm)
            {
                componentType = this.EditStateService.EditForm;
            }
            else 
            {
                componentType = RouteData.PageType;
                parameters = RouteData.RouteValues;
            }

            if (componentType != null)
            {
                builder.OpenComponent(0, componentType);
                foreach (var kvp in parameters)
                {
                    builder.AddAttribute(1, kvp.Key, kvp.Value);
                }
                builder.CloseComponent();
            }
            else
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, "No Route or View Configured to Display");
                builder.CloseElement();
            }
        };

        public async Task RenderAsync() => await InvokeAsync(() =>
        {
            if (!this._RenderEventQueued)
            {
                this._RenderEventQueued = true;
                _renderHandle.Render(_renderDelegate);
            }
        }
        );

        protected Task InvokeAsync(Action workItem) => _renderHandle.Dispatcher.InvokeAsync(workItem);

        protected Task InvokeAsync(Func<Task> workItem) => _renderHandle.Dispatcher.InvokeAsync(workItem);

        private Task DirtyExit(MouseEventArgs e)
        {
            this.EditStateService.ClearEditState();
            this.SetPageExitCheck(false);
            return RenderAsync();
        }

        private Task LoadDirtyForm(MouseEventArgs d)
        {
            _loadEditForm = true;
            return RenderAsync();
        }

        private void SetPageExitCheck(bool action)
            => _js.InvokeAsync<bool>("cecblazor_setEditorExitCheck", action);
    }
}
