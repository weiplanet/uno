# Using the IL Linker for WebAssembly

The [linker step](https://github.com/mono/linker/tree/master/docs) (also known as tree shaking, or IL Trimming) is responsible for the detection and removal of code that may not be used at runtime. This step is particularly important when targeting WebAssembly or native code in general, to reduce significantly the final package size.

The linker is generally efficient at removing code but in some cases it may remove too much code, making some assemblies fail to work properly. In particular, assemblies or features that rely on reflection don't work by default with the linker enabled. `JSON.NET` is a good example of this, as it relies on reflection very heavily.

To handle cases where the default linker behaviour removes too much code, the linker can be manually configured using the `LinkerConfig.xml` file in the Uno CrossPlatform project template.

When parts of an application fail to work, you can:
- Add full assembly names to the `LinkerConfig.xml` file:
```xml
<assembly fullname="MyAssembly" />
```
- Add namespaces for the linker to ignore:
```xml
  <assembly fullname="MyAssembly">
	<!-- This is required by JSon.NET and any expression.Compile caller -->
	<type fullname="MyNamespace*" />
  </assembly>
```

As a troubleshooting step to determine if the linker is causing your code to break, you can also disable the linker completely by adding the following to your `csproj` file:
```xml
<PropertyGroup>
  <WasmShellILLinkerEnabled>true</WasmShellILLinkerEnabled>
</PropertyGroup>
```
Note that disabling the linker is not recommended as it can force the compiler to generate very large WebAssembly modules AOT process, and go over the browser's size limits.

You can find additional information about the linker step in Uno Platform [here](https://github.com/unoplatform/Uno.Wasm.Bootstrap#linker-configuration).
