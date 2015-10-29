//-----------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Glimpse">
//     Copyright (c) Glimpse. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Glimpse.Core.Extensibility;

[assembly: ComVisible(false)]

[assembly: AssemblyTitle("Glimpse for ApplicationInsights")]
[assembly: AssemblyDescription("Main extensibility implementations for running Glimpse with ApplicationInsights.")]
[assembly: AssemblyProduct("Glimpse.ApplicationInsights")]

// Version is in major.minor.build format to support http://semver.org/
// Keep these three attributes in sync
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")] // Used to specify the NuGet version number at build time

[assembly: CLSCompliant(true)]
[assembly: NuGetPackage("Glimpse.ApplicationInsights")]
