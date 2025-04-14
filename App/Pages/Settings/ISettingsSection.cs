using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace Zine.App.Pages.Settings;

public interface ISettingsSection
{
	[Parameter]
	[SuppressMessage("Usage", "BL0007:Component parameters should be auto properties")]
	public Domain.Settings.Settings SettingsValues { get; set; }

	[Parameter]
	[SuppressMessage("Usage", "BL0007:Component parameters should be auto properties")]
	public EventCallback<Domain.Settings.Settings> SettingsValuesChanged { get; set; }

}
