# Embedding Existing JavaScript Components Into Uno-WASM - Part 1

## Leveraging the Uno.WASM Bootstrapper

At the heart of Uno-WASM, there's a package called [`Uno.Wasm.Bootstrap`](https://www.nuget.org/packages/Uno.Wasm.Bootstrap) (Uno Bootstrapper) project. It contains the tooling required to build, package, deploy, run and debug a *.NET* project in a web browser using WebAssembly. It's automatically included in the WASM head of  an Uno app.

It is possible to [add JavaScript files](https://github.com/unoplatform/Uno.Wasm.Bootstrap#support-for-additional-js-files) or [CSS  files](https://github.com/unoplatform/Uno.Wasm.Bootstrap#support-for-additional-css-files) into any Uno-WASM applications: they will be loaded automatically by the runtime. It is also possible to add any kind of downloadable assets as content.

## HTML5 is a Rich and Powerful Platform.

Uno fully embraces HTML5 as its display backend when targeting WebAssembly (WASM). As a result, it is possible to integrate with almost any existing JavaScript library to extend the behavior of an app.

## Embedding assets

In the HTML world, everything running in the browser is assets that must be downloaded from a server. To integrate existing JavaScript frameworks, they can be either download those from another location on the Internet (usually from a CDN service) or embed and deployed with the app.

The Uno Bootstrapper can automatically embed any asset and deploy them with the app. Some of them (CSS & JavaScript) can also be loaded with the app. Here's how to declare them in a _Uno Wasm_ project:

1. **JavaScript files** should be in the `WasmScripts`  folder: they will be copied to the output folder and loaded automatically by the bootstrapper when the page loads. **They must be marked with the `EmbeddedResources` build action**:

   ``` xml
   <!-- .csproj file -->
   <ItemGroup>
     <EmbeddedResource Include="WasmScripts\javascriptfile.js" />
     <EmbeddedResource Include="WasmScripts\**\*.js" /> <!-- globing works too -->
   </ItemGroup>
   ```

2. **CSS Style files** should be in the `WasmCSS` folder: they will be copied to the output folder and referenced in the _HTML head_ of the application. **They must be marked with the `EmbeddedResources` build action**.

   ``` xml
   <!-- .csproj file -->
   <ItemGroup>
     <EmbeddedResource Include="WasmCSS\stylefile.css" />
     <EmbeddedResource Include="WasmCSS\**\*.css" /> <!-- globing works too -->
   </ItemGroup>
   ```

3. **Asset files** should be marked with the `Content` build action in the app. The file will be copied to the output folder and will preserve the same relative path.

   ``` xml
   <!-- .csproj file -->
   <ItemGroup>
     <Content Include="Assets\image.png" />
   </ItemGroup>
   ```

4. Alternatively, **any kind of asset file** can be placed directly in the `wwwroot` folder as with any standard ASP.NET Core project. They will be deployed with the app, but the application code is responsible for fetching and using them.

   > **Is it an ASP.NET Core "web" project?**
   > No, but it shares a common structure. Some of the deployment features, like the `wwwroot` folder, and the Visual Studio integration for running/debugging are reused in a similar way to an ASP.NET Core project. The C# code put in the project will run in the browser, using the .NET runtime. There is no need for a server side component in Uno-Wasm projects.

## Uno-Wasm Controls Are Actual HTML5 Elements

The [philosophy of Uno](https://platform.uno/docs/articles/concepts/overview/philosophy-of-uno.html) is to rely on native platforms where it makes sense. In the context of a browser, that's the HTML5 DOM. This means that each time is created, a class deriving from `UIElement` is creating a corresponding HTML element.

That also means that it is possible to control how this element is created.  By default it is a `<div>`, but it can be changed in the constructor by providing the `htmlTag` parameter to the one required. For example:

``` csharp
public partial sealed class MyDivControl : FrameworkElement
{
    public MyDivControl() // will create a <div> HTML element (by default)
    {
    }
}

[HtmlElement("input")] 
public partial sealed class MyInputControl : FrameworkElement
{
    public MyInputControl() // Will create an <input> HTML element
    {
    }
}

[HtmlElement("span")]
public partial sealed class MyInputControl : FrameworkElement
{
    public MyInputControl() // Will create a <span> HTML element
    {
    }
}
```

Once created, it is possible to interact directly with this element by calling helper methods available in Uno. Note that those methods are only available when targeting the _Wasm_ platform. It is possible to use [conditional code](https://platform.uno/docs/articles/platform-specific-csharp.html) to use these methods in a multi-platform project.

Here is a list of helper methods used to facilitate the integration with the HTML DOM:

* The extension method `element.SetCssStyle()` can be used to set a CSS Style on the HTML element. Example:

  ``` csharp
  // Setting only one CSS style
  this.SetCssStyle("text-shadow", "2px 2px red");
  
  // Setting many CSS styles at once using C# tuples
  this.SetCssStyle(("text-shadow", "2px 2px blue"), ("color", "var(--app-fg-color1)"));
  ```

* The `element.ClearCssStyle()` extension method can be used to set CSS styles to their default values. Example:

  ``` csharp
  // Reset text-shadow style to its default value
  this.ClearCssStyle("text-shadow");

  // Reset both text-shadow and color to their default values
  this.ClearCssStyle("text-shadow", "color");
  ```

* The `element.SetHtmlAttribute()` and `element.ClearHtmlAttribute()` extension methods can be used to set HTML attributes on the element:

  ``` csharp
  // Set the "href" attribute of an <a> element
  this.SetHtmlAttribute("href", "#section2");
  
  // Set many attributes at once (less interop)
  this.SetHtmlAttribute(("target", "_blank"), ("referrerpolicy", "no-referrer"));
  
  // Remove attribute from DOM element
  this.ClearHtmlAttribute("href");
  
  // Get the value of an attribute of a DOM element
  var href = this.GetHtmlAttribute("href");
  ```

* The `element.SetCssClass()` and `element.UnsetCssClass()` extension methods can be used to add or remove CSS classes to the HTML Element:

  ``` csharp
  // Add the class to element
  this.SetCssClass("warning");
  
  // Add many classes at once (less interop)
  this.SetCssClass("warning", "level2");
  
  // Remove class from element
  this.UnsetCssClass("paused");
  
  // You can also set one class from a list of possible values.
  // Like a radio-button, like non-selected values will be unset
  var allClasses = new [] { "Small", "Medium", "Large"};
  this.SetCssClass(allClasses, 2); // set to "Large"
  ```

* The `element.SetHtmlContent()` extension method can be used to set arbitrary HTML content as child of the control.

  ``` csharp
  this.SetHtmlContent("<h2>Welcome to Uno Platform!</h2>");
  ```

  > **IMPORTANT**: This method should not be used when children "managed" controls are present: doing so can result in inconsistent runtime errors because of desynchronized visual tree.

* Finally, it is possible to invoke an arbitrary JavaScript code by using the static method `WebAssembleRuntime.InvokeJS()`. The script is directly executed in the context of the browser, giving the ability to perform anything that JavaScript can do. See next section for more details.

## Invoke JavaScript code From C#

Whenever there's a need to invoke a JavaScript code in the browser, the `WebAssembly.WebAssemblyRuntime` static class should be used. There is also helpers you can call as _extension methods_ on the elements.

``` csharp
// Invoke javascript synchronously
WebAssemblyRuntime.InvokeJS("alert(\"It works!\");");

// Use "string" return value of .InvokeJS()
var html = WebAssemblyRuntime.InvokeJS("document.getElementById('banner').innerHTML");

// Invoke javascript asynchronously and await completition
await WebAssemblyRuntime.InvokeAsync(
    "fetch('https://api.domain.tld/documents/1301', {method: 'DELETE'});");

// Invoke javascript asynchronously and await returned string
var str = await WebAssemblyRuntime.InvokeAsync(
	"(async () => "It works asynchronously!")();");

// Escape javascript data to prevent javascript script injection
var escapedUserId = WebAssemblyRuntime.EscapeJS(userId);
WebAssemblyRuntime.InvokeJS($"MyApp.setUserId(\"{escapedUserId}\");");

// Call javascript in the context os a specific UIElement.
// In this case, it will be available as "element" in the execution scope.
MyControl.ExecuteJavascript("element.toggleAttribute(\"readonly\");"); // sync
await MyControl.ExecuteJavascriptAsync("element.requestFullScreen();"); // async
```

* `InvokeAsync` should return a [`Promise`](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Promise) or, be an `async` method.
* Any Promise _rejected_ result will get translated into an `ApplicationException` exception.
* Remember to always use `InvariantCulture` when generating JavaScript for numbers. There's also a helper in [`Uno.Core`](https://www.nuget.org/packages/Uno.Core) called `.ToStringInvariant()`: this dependency is already present in any Uno projects in the namespace `Uno.Extensions`.
* Calling the javascript `document.getElementById()` with the element's `HtmlId` will only work when the element is actually loaded in the DOM. So it's better to call the extensions `<element>.ExecuteJavascript()` or `<element>.ExecuteJavascriptAsync()`: they will work all the time.

## Invoke C# code From JavaScipt

There's 2 ways to _callback_ to managed C# code from JavaScript:

1. Use Mono to wrap a dotnet static method into a JavaScript function like this:JavaScript_:

   ``` javascript
   // Register the method wrapper (should be cached)
   const sumMethod = Module.mono_bind_static_method(
       "[Assembly.Name] Fully.Qualified.ClassType:SumMethod");
   // Call the method
   const result = sumMethod(1, 2); // should return 3
   ```

   _C#_:

   ``` csharp
   // In assembly "Assembly.Name"
   namespace Fully.Qualified
   {
       public static class ClassType
       {
           public static int SumMethod(int i1, int i2) => i1 + i2;
       }
   }
   ```

2. Use HTML's `Event` or `CustomEvent` and dispatch them to managed code like this:

   _JavaScript_:

   ``` javascript
   // Generate a custom generic event from Javascript/Typescript
   htmlElement.dispatchEvent(new Event("simpleEvent"));
   ```

   _C#_:

   ``` csharp
   this.RegisterHtmlEventHandler("simpleEvent", (sender, evt)=> { [...] });
   ```

   More details [on this page](https://platform.uno/docs/articles/wasm-custom-events.html).
   
   > Note: current there's no easy way to asynchronously call managed (dotnet) code from JavaScript in the current version of Uno.

## 🔬 Going further

* [Continue with Part 2](wasm-javascript-2.md) - an integration of a syntax highlighter component.
* [Continue with Part 3](wasm-javascript-3.md) - an integration of a more complex library with callbacks to application.
* Read the [Uno Wasm Bootstrapper](https://github.com/unoplatform/Uno.Wasm.Bootstrap) documentation.
