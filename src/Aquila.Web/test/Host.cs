using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

[assembly: ProvideApplicationPartFactory("Microsoft.AspNetCore.Mvc.ApplicationParts.ConsolidatedAssemblyApplicationPartFactory, Microsoft.AspNetCore.Mvc.Razor")]
//[assembly: RazorCompiledItem(typeof(Pages_Error), "mvc.1.0.razor-page", "/Pages/Error.cshtml")]
[assembly: RazorCompiledItem(typeof(Pages__Host), "mvc.1.0.razor-page", "/Pages/_Host.cshtml")]
[assembly: RazorCompiledItem(typeof(Pages__Layout), "mvc.1.0.view", "/Pages/_Layout.cshtml")]

[RazorCompiledItemMetadata("RouteTemplate", "/")]
[RazorCompiledItemMetadata("Identifier", "/Pages/_Host.cshtml")]
[CreateNewOnMetadataUpdate]
internal sealed class Pages__Host : Page
{
	private TagHelperExecutionContext __tagHelperExecutionContext;

	private TagHelperRunner __tagHelperRunner = new TagHelperRunner();

	private string __tagHelperStringValueBuffer;

	private TagHelperScopeManager __backed__tagHelperScopeManager = null;

	private ComponentTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_ComponentTagHelper;

	private TagHelperScopeManager __tagHelperScopeManager
	{
		get
		{
			if (__backed__tagHelperScopeManager == null)
			{
				__backed__tagHelperScopeManager = new TagHelperScopeManager(base.StartTagHelperWritingScope, base.EndTagHelperWritingScope);
			}
			return __backed__tagHelperScopeManager;
		}
	}

	[RazorInject]
	public IModelExpressionProvider ModelExpressionProvider { get; private set; } = null;


	[RazorInject]
	public IUrlHelper Url { get; private set; } = null;


	[RazorInject]
	public IViewComponentHelper Component { get; private set; } = null;


	[RazorInject]
	public IJsonHelper Json { get; private set; } = null;


	[RazorInject]
	public IHtmlHelper<Pages__Host> Html { get; private set; } = null;


	public ViewDataDictionary<Pages__Host> ViewData => (ViewDataDictionary<Pages__Host>)(base.PageContext?.ViewData);

	public Pages__Host Model => ViewData.Model;

	public override async Task ExecuteAsync()
	{
		base.Layout = "_Layout";
		WriteLiteral("\r\n");
		__tagHelperExecutionContext = __tagHelperScopeManager.Begin("component", TagMode.SelfClosing, "ca944311909f6648bf95208305fa7df1ce6b7f86a19713b6ccf7b61947b999ce2893", async delegate
		{
		});
		__Microsoft_AspNetCore_Mvc_TagHelpers_ComponentTagHelper = CreateTagHelper<ComponentTagHelper>();
		__tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_ComponentTagHelper);
		__Microsoft_AspNetCore_Mvc_TagHelpers_ComponentTagHelper.ComponentType = typeof(App);
		__tagHelperExecutionContext.AddTagHelperAttribute("type", __Microsoft_AspNetCore_Mvc_TagHelpers_ComponentTagHelper.ComponentType, HtmlAttributeValueStyle.DoubleQuotes);
		__Microsoft_AspNetCore_Mvc_TagHelpers_ComponentTagHelper.RenderMode = RenderMode.ServerPrerendered;
		__tagHelperExecutionContext.AddTagHelperAttribute("render-mode", __Microsoft_AspNetCore_Mvc_TagHelpers_ComponentTagHelper.RenderMode, HtmlAttributeValueStyle.DoubleQuotes);
		await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
		if (!__tagHelperExecutionContext.Output.IsContentModified)
		{
			await __tagHelperExecutionContext.SetOutputContentAsync();
		}
		Write(__tagHelperExecutionContext.Output);
		__tagHelperExecutionContext = __tagHelperScopeManager.End();
		WriteLiteral("\r\n");
	}
}
