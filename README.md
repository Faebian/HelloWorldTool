# UnicornOne Hello World Tool Plugin

A minimal example of a third-party **UnicornOne Tool plugin** written in C# using Windows Forms.

This project demonstrates how an external DLL can provide a native WinForms tool that UnicornOne discovers at runtime and embeds directly into the **Tools** interface.

The example intentionally does very little: it displays a button and a label counting how many times the button has been clicked.

Its purpose is to provide a clean starting point for developing more useful UnicornOne tools.

## What It Demonstrates

The plugin demonstrates the basic UnicornOne Tool plugin lifecycle:

```text
UnicornOne starts
      ↓
Scans Plugins/Tools
      ↓
Loads plugin DLL
      ↓
Finds IToolPlugin implementation
      ↓
Initialize()
      ↓
CreateForm()
      ↓
Embeds returned Form in Tools
```

The plugin appears in the UnicornOne tool selector as:

```text
Hello World (plugin)
```

When opened, its `Form` behaves like a native UnicornOne Tool tab.

## Requirements

* UnicornOne with Tool plugin support
* .NET Framework 4.7.2
* `UnicornOne.Abstractions.dll`
* Windows Forms

The plugin does **not** reference the main UnicornOne application assemblies.

Instead, communication between the plugin and UnicornOne takes place through the interfaces exposed by:

```text
UnicornOne.Abstractions.dll
```

## Project Structure

```text
HelloWorldTool/
│
├── lib/
│   └── UnicornOne.Abstractions.dll
│
└── HelloWorldTool/
    ├── HelloWorldTool.csproj
    └── HelloWorldTool.cs
```

The supplied abstraction DLL is referenced by the plugin project but is not copied into the plugin build output.

## Tool Plugin Interface

A UnicornOne Tool plugin implements:

```csharp
IToolPlugin
```

The basic contract is:

```csharp
public interface IToolPlugin : IDisposable
{
    string Name { get; }
    string Version { get; }

    void Initialize(
        string jsonConfig,
        IToolHost host);

    Form CreateForm();
}
```

For example:

```csharp
public sealed class HelloWorldToolPlugin : IToolPlugin
{
    private IToolHost _host;

    public string Name => "Hello World";
    public string Version => "1.0.0";

    public void Initialize(
        string jsonConfig,
        IToolHost host)
    {
        _host = host;
    }

    public Form CreateForm()
    {
        return new HelloWorldForm(_host);
    }

    public void Dispose()
    {
    }
}
```

`Name` determines the name presented to the user by UnicornOne.

`Initialize()` provides access to services exposed by the UnicornOne host.

`CreateForm()` returns the Windows Form that UnicornOne embeds into its Tools interface.

## Tool Host

The plugin receives an `IToolHost` during initialization.

This provides a controlled interface between the external plugin and UnicornOne rather than requiring the plugin to reference UnicornOne's internal assemblies.

The host can expose services such as:

```csharp
host.AppendLog(...);

host.GetValue(...);

host.PublishValue(...);

host.ExecuteCommand(...);

host.Speak(...);

host.InvokeUI(...);

host.Theme
```

The Tool abstraction is intentionally small. Additional host functionality can be added as Tool plugins require more access to UnicornOne.

## Theme Support

Plugins can use the UnicornOne theme without referencing UnicornOne's internal theme implementation:

```csharp
host.Theme.ApplyBaseStyles(this);
```

The abstraction exposes the relevant theme information while UnicornOne provides the actual implementation.

This allows external Tools to visually integrate with the rest of the application while keeping the plugin API independent of UnicornOne internals.

## Hello World Example

The example form contains a button and label.

Initially:

```text
You clicked the button 0 times
```

Each button press increments the counter:

```text
You clicked the button 1 times
You clicked the button 2 times
You clicked the button 3 times
...
```

This deliberately simple example proves that an independently compiled DLL can create an interactive WinForms UI that runs as a native UnicornOne Tool.

## Building

From the project directory:

```bash
dotnet build
```

The project targets:

```text
.NET Framework 4.7.2
```

The compiled plugin can be found under:

```text
bin/Debug/net472/HelloWorldTool.dll
```

## Installation

Copy:

```text
HelloWorldTool.dll
```

to:

```text
C:\UnicornOne\Plugins\Tools\
```

Do not copy another version of `UnicornOne.Abstractions.dll` into the Tools plugin directory unless specifically required by your UnicornOne installation.

The running UnicornOne installation provides the abstraction assembly against which the plugin was built.

The resulting installation should therefore look like:

```text
C:\UnicornOne\
│
├── UnicornOne.Abstractions.dll
│
└── Plugins\
    └── Tools\
        └── HelloWorldTool.dll
```

## Running

Start UnicornOne and open **Tools**.

Click the `+` button to open the Tool selector.

The external tool should appear after the built-in tools as:

```text
Hello World (plugin)
```

Select it and UnicornOne will instantiate the plugin and embed its returned Windows Form into a Tool tab.

## Developing Your Own Tool

The Hello World example can be used as the starting point for another Tool.

At minimum:

1. Create a .NET Framework 4.7.2 Class Library.
2. Reference `UnicornOne.Abstractions.dll`.
3. Implement `IToolPlugin`.
4. Return a WinForms `Form` from `CreateForm()`.
5. Build the project.
6. Copy the resulting DLL into `C:\UnicornOne\Plugins\Tools\`.
7. Restart UnicornOne and select the new Tool.

The form itself can contain normal WinForms controls and application logic.

Where interaction with UnicornOne is required, functionality should be accessed through `IToolHost`.

## Tools vs Workbooks

Tools and Workbooks use similar plugin concepts but serve different purposes.

**Tools** are intended for relatively fixed-function utilities and interfaces.

Examples might include:

* calculators
* hardware utilities
* data viewers
* diagnostic interfaces
* specialist analysis tools
* custom operator interfaces

**Workbooks** are intended for more flexible workflows, including long-running operations, experimentation and AI/ML-driven functionality.

Workbooks therefore have additional execution, cancellation and lifetime management that a normal Tool does not require.

A Tool should remain a Tool unless it genuinely needs the additional Workbook lifecycle.

## Plugin Boundary

A Tool plugin is loaded as a .NET assembly and runs inside the UnicornOne process.

`IToolPlugin` and `IToolHost` provide an API boundary between UnicornOne and the plugin, but they are **not a security sandbox**.

Only trusted plugin DLLs should therefore be installed.

## Status

This repository is a minimal reference implementation for the UnicornOne external Tool plugin API.

The Tool abstraction is intentionally small and will be extended as additional plugin capabilities are required.
