# NuGet-paket i N2.Everything.2026.sln

Inventering: 2026-11-03. Underlag: de 26 projektfilerna i `N2.Everything.2026.sln`, deras inkluderade `packages.config` och direkta `HintPath`-referenser till `packages/`.

**Summa:** 26 projekt, 69 unika paket-ID och 75 paket/version-kombinationer. Paket som bara nås via projektreferenser räknas i sitt eget projekt, inte om i beroende projekt. Installerade men oanvända paket i rotens `packages/` ingår inte.

## Paket och användande projekt

Märkningen `†` betyder att paket/version finns som DLL-referens i projektfilen men saknas i projektets inkluderade `packages.config`. Övriga rader kommer från `packages.config` (och kan också ha DLL-referenser).

| Paket | Version | Projekt |
| --- | --- | --- |
| `Antlr3.Runtime` | `3.5.1` | `N2`, `N2.Management.Tests`, `N2.Tests` |
| `AWSSDK.Core` | `4.0.102.16` | `N2.Persistence.MongoDB` |
| `AWSSDK.SecurityToken` | `4.0.101.7` | `N2.Persistence.MongoDB` |
| `Castle.Core` | `5.2.1` | `N2.IoC.Windsor3`, `N2.Raven.Tests`, `N2.Tests` |
| `Castle.Windsor` | `6.0.0` | `N2.IoC.Windsor3`, `N2.Raven.Tests`, `N2.Tests` |
| `DnsClient` | `1.8.0` | `N2.Persistence.MongoDB` |
| `EntityFramework` | `6.1.3` | `N2.Management.Tests` |
| `ICSharpCode.NRefactory` | `5.5.1` | `N2.Raven` |
| `Iesi.Collections` | `4.1.1` | `N2`, `N2.Management.Tests`, `N2.Tests` |
| `log4net` | `3.5.0` | `AddonCatalog`, `N2`, `N2.Management`, `N2.Templates`, `N2.Templates.Mvc`, `N2.Templates.UI.Tests`†, `N2.Tests`, `Wiki` |
| `Lucene.Net` | `3.0.3` | `N2.Extensions.Tests`, `N2.Raven`, `N2.Search.Lucene` |
| `Lucene.Net.Contrib` | `3.0.3` | `N2.Search.Lucene` |
| `Lucene.Net.Contrib.Spatial` | `3.0.3` | `N2.Raven` |
| `Microsoft.AspNet.Identity.Core` | `2.2.2` | `N2.Security.AspNet.Identity` |
| `Microsoft.AspNet.Mvc` | `5.3.0` | `Dinamico`, `N2`, `N2.Extensions`, `N2.Extensions.Tests`, `N2.Management`, `N2.Management.Targeting`, `N2.Management.Tests`, `N2.Razor`, `N2.ReusableParts`, `N2.Templates.Mvc`, `N2.Templates.Mvc.Tests`, `N2.Templates.Tests`, `N2.Tests` |
| `Microsoft.AspNet.Razor` | `3.2.4` | `N2.Extensions.Tests`, `N2.Razor` |
| `Microsoft.AspNet.Razor` | `3.3.0` | `Dinamico`, `N2`, `N2.Extensions`, `N2.Management`, `N2.Management.Targeting`, `N2.Management.Tests`, `N2.ReusableParts`, `N2.Templates.Mvc`, `N2.Templates.Mvc.Tests`, `N2.Templates.Tests`, `N2.Tests` |
| `Microsoft.AspNet.WebPages` | `3.2.4` | `N2.Extensions.Tests`, `N2.Razor` |
| `Microsoft.AspNet.WebPages` | `3.3.0` | `Dinamico`, `N2`, `N2.Extensions`, `N2.Management`, `N2.Management.Targeting`, `N2.Management.Tests`, `N2.ReusableParts`, `N2.Templates.Mvc`, `N2.Templates.Mvc.Tests`, `N2.Templates.Tests`, `N2.Tests` |
| `Microsoft.Bcl.AsyncInterfaces` | `10.0.14` | `N2.Persistence.MongoDB` |
| `Microsoft.CompilerServices.AsyncTargetingPack` | `1.0.0` | `N2.Raven` |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | `10.0.14` | `N2.Persistence.MongoDB` |
| `Microsoft.Extensions.Logging.Abstractions` | `10.0.14` | `N2.Persistence.MongoDB` |
| `Microsoft.Web.Infrastructure` | `1.0.0.0` | `N2.Razor` |
| `Microsoft.Web.Infrastructure` | `2.0.0` | `N2`, `N2.Extensions`, `N2.Management`, `N2.Management.Targeting`, `N2.Management.Tests`, `N2.ReusableParts`, `N2.Templates.Mvc`, `N2.Tests` |
| `Microsoft.Win32.Registry` | `5.0.0` | `N2.Persistence.MongoDB` |
| `mongocsharpdriver` | `2.30.0` | `N2.Persistence.MongoDB` |
| `MongoDB.Bson` | `2.30.0` | `N2.Persistence.MongoDB` |
| `MongoDB.Driver` | `2.30.0` | `N2.Persistence.MongoDB` |
| `MongoDB.Driver.Core` | `2.30.0` | `N2.Persistence.MongoDB` |
| `MongoDB.Libmongocrypt` | `1.12.0` | `N2.Persistence.MongoDB` |
| `Mono.Cecil` | `0.11.6` | `N2.Raven` |
| `Newtonsoft.Json` | `13.0.4` | `N2.Raven`, `N2.Raven.Tests` |
| `NHibernate` | `5.7.1` | `N2`, `N2.Management.Tests`, `N2.Tests` |
| `NLog` | `2.0.0.2000` | `N2.Raven.Tests` |
| `NLog` | `6.2.2` | `N2.Raven` |
| `NUnit` | `3.6.0` | `N2.Extensions.Tests`, `N2.Management.Tests`, `N2.Raven.Tests`, `N2.Templates.Mvc.Tests`, `N2.Templates.Tests`, `N2.Tests` |
| `NUnit3TestAdapter` | `3.16.1` | `N2.Extensions.Tests`, `N2.Management.Tests`, `N2.Raven.Tests`, `N2.Templates.Mvc.Tests`, `N2.Templates.Tests`, `N2.Tests` |
| `RavenDB` | `1.0.573` | `N2.Raven.Tests` |
| `RavenDB-Embedded` | `1.0.573` | `N2.Raven`†, `N2.Raven.Tests` |
| `Remotion.Linq` | `2.2.0` | `N2`, `N2.Management.Tests`, `N2.Tests` |
| `Remotion.Linq.EagerFetching` | `2.2.0` | `N2`, `N2.Management.Tests`, `N2.Tests` |
| `RhinoMocks` | `3.6` | `N2.Raven.Tests` |
| `SharpCompress` | `0.50.7` | `N2.Persistence.MongoDB` |
| `SharpZipLib` | `1.4.2` | `N2.Extensions.Tests`, `N2.Management`, `N2.Packaging`, `N2.Raven`, `N2.Search.Lucene` |
| `Shouldly` | `1.1.0.4` | `N2.Raven.Tests` |
| `Shouldly` | `3.0.2` | `N2.Extensions.Tests`, `N2.Management.Tests`, `N2.Tests` |
| `Snappier` | `1.3.1` | `N2.Persistence.MongoDB` |
| `Spatial4n.Core` | `0.4.1` | `N2.Raven` |
| `Stub.System.Data.SQLite.Core.NetFramework` | `1.0.119.0` | `AddonCatalog`, `N2.Templates`, `Wiki` |
| `System.Buffers` | `4.6.1` | `N2.Management`, `N2.Packaging`, `N2.Persistence.MongoDB`, `N2.Raven`, `N2.Search.Lucene` |
| `System.Data.SQLite.Core` | `1.0.112.0` | `Dinamico`, `N2.Extensions.Tests`, `N2.Management.Tests`, `N2.Templates`†, `N2.Templates.Mvc`, `N2.Templates.Mvc.Tests`, `N2.Tests` |
| `System.Data.SQLite.Core` | `1.0.119.0` | `AddonCatalog`, `N2.Templates`, `Wiki` |
| `System.Data.SQLite.Linq` | `1.0.112.0` | `N2.Management` |
| `System.Diagnostics.DiagnosticSource` | `10.0.14` | `N2.Persistence.MongoDB` |
| `System.IO` | `4.3.0` | `N2.Persistence.MongoDB` |
| `System.IO.Pipelines` | `10.0.14` | `N2.Persistence.MongoDB` |
| `System.Memory` | `4.6.3` | `N2.Management`, `N2.Packaging`, `N2.Persistence.MongoDB`, `N2.Raven`, `N2.Search.Lucene` |
| `System.Net.Http` | `4.3.4` | `N2.Persistence.MongoDB` |
| `System.Numerics.Vectors` | `4.6.1` | `N2.Management`, `N2.Packaging`, `N2.Persistence.MongoDB`, `N2.Raven`, `N2.Search.Lucene` |
| `System.Runtime` | `4.3.1` | `N2.Persistence.MongoDB` |
| `System.Runtime.CompilerServices.Unsafe` | `6.1.2` | `N2.Management`, `N2.Packaging`, `N2.Persistence.MongoDB`, `N2.Raven`, `N2.Search.Lucene` |
| `System.Runtime.InteropServices.RuntimeInformation` | `4.3.0` | `N2.Persistence.MongoDB` |
| `System.Security.AccessControl` | `6.0.1` | `N2.Persistence.MongoDB` |
| `System.Security.Cryptography.Algorithms` | `4.3.1` | `N2.Persistence.MongoDB` |
| `System.Security.Cryptography.Encoding` | `4.3.0` | `N2.Persistence.MongoDB` |
| `System.Security.Cryptography.Primitives` | `4.3.0` | `N2.Persistence.MongoDB` |
| `System.Security.Cryptography.X509Certificates` | `4.3.2` | `N2.Persistence.MongoDB` |
| `System.Security.Principal.Windows` | `5.0.0` | `N2.Persistence.MongoDB` |
| `System.Text.Encoding.CodePages` | `10.0.14` | `N2.Persistence.MongoDB` |
| `System.Text.Encodings.Web` | `10.0.14` | `N2.Persistence.MongoDB` |
| `System.Text.Json` | `10.0.14` | `N2.Persistence.MongoDB` |
| `System.Threading.Tasks.Extensions` | `4.6.3` | `N2.Management`, `N2.Packaging`, `N2.Persistence.MongoDB`, `N2.Raven`, `N2.Search.Lucene` |
| `System.ValueTuple` | `4.6.2` | `N2.Persistence.MongoDB` |
| `ZstdSharp.Port` | `0.8.8` | `N2.Persistence.MongoDB` |

## Projekt och paketlistor

| Projekt | Projektfil | `packages.config` | Deklarerade paket | Endast DLL-referens |
| --- | --- | --- | ---: | ---: |
| `N2` | `src/Framework/N2/N2.csproj` | `src/Framework/N2/packages.config` | 10 | 0 |
| `N2.Extensions` | `src/Framework/Extensions/N2.Extensions.csproj` | `src/Framework/Extensions/packages.config` | 4 | 0 |
| `N2.Extensions.Tests` | `src/Framework/Extensions.Tests/N2.Extensions.Tests.csproj` | `src/Framework/Extensions.Tests/packages.config` | 9 | 0 |
| `N2.Tests` | `src/Framework/Tests/N2.Tests.csproj` | `src/Framework/Tests/packages.config` | 16 | 0 |
| `N2.Templates` | `src/WebForms/WebFormsTemplates/N2.Templates.csproj` | `src/WebForms/WebFormsTemplates/packages.config` | 3 | 1 |
| `N2.Templates.Tests` | `src/WebForms/WebFormsTemplates.Tests/N2.Templates.Tests.csproj` | `src/WebForms/WebFormsTemplates.Tests/packages.config` | 5 | 0 |
| `AddonCatalog` | `src/WebForms/WebFormsTemplates/AddonCatalog.csproj` | `src/WebForms/WebFormsTemplates/packages.config` | 3 | 0 |
| `Wiki` | `src/WebForms/WebFormsTemplates/Wiki.csproj` | `src/WebForms/WebFormsTemplates/packages.config` | 3 | 0 |
| `N2.Templates.UI.Tests` | `src/WebForms/WebFormsTemplates/N2.Templates.UI.Tests.csproj` | — | 0 | 1 |
| `N2.Templates.Mvc` | `src/Mvc/MvcTemplates/N2.Templates.Mvc.csproj` | `src/Mvc/MvcTemplates/packages.config` | 6 | 0 |
| `N2.Templates.Mvc.Tests` | `src/Mvc/MvcTemplates.Tests/N2.Templates.Mvc.Tests.csproj` | `src/Mvc/MvcTemplates.Tests/packages.config` | 6 | 0 |
| `N2.Management.Tests` | `src/Framework/Management.Tests/N2.Management.Tests.csproj` | `src/Framework/Management.Tests/packages.config` | 14 | 0 |
| `Dinamico` | `src/Mvc/Dinamico/Dinamico.csproj` | `src/Mvc/Dinamico/packages.config` | 4 | 0 |
| `N2.Packaging` | `src/Framework/Packaging/N2.Packaging.csproj` | `src/Framework/Packaging/packages.config` | 6 | 0 |
| `N2.Razor` | `src/Framework/Razor/N2.Razor.csproj` | `src/Framework/Razor/packages.config` | 4 | 0 |
| `N2.Raven` | `src/Framework/Raven/N2.Raven.csproj` | `src/Framework/Raven/packages.config` | 14 | 1 |
| `N2.Raven.Tests` | `src/Framework/Raven.Tests/N2.Raven.Tests.csproj` | `src/Framework/Raven.Tests/packages.config` | 10 | 0 |
| `N2.Management` | `src/Mvc/MvcTemplates/N2/N2.Management.csproj` | `src/Mvc/MvcTemplates/N2/packages.config` | 12 | 0 |
| `N2.ReusableParts` | `src/Framework/ReusableParts/N2.ReusableParts.csproj` | `src/Framework/ReusableParts/packages.config` | 4 | 0 |
| `N2.Search.Lucene` | `src/Framework/Search.Lucene/N2.Search.Lucene.csproj` | `src/Framework/Search.Lucene/packages.config` | 8 | 0 |
| `N2.IoC.Windsor3` | `src/Framework/IoC.Windsor/N2.IoC.Windsor3.csproj` | `src/Framework/IoC.Windsor/packages.config` | 2 | 0 |
| `N2.Search.Remote.Client` | `src/Framework/Search.Remote.Client/N2.Search.Remote.Client.csproj` | — | 0 | 0 |
| `N2.Search.Remote.Server` | `src/Framework/Search.Remote.Server/N2.Search.Remote.Server.csproj` | — | 0 | 0 |
| `N2.Persistence.MongoDB` | `src/Framework/MongoDB/N2.Persistence.MongoDB.csproj` | `src/Framework/MongoDB/packages.config` | 36 | 0 |
| `N2.Security.AspNet.Identity` | `src/Framework/N2.Security.AspNet.Identity/N2.Security.AspNet.Identity.csproj` | `src/Framework/N2.Security.AspNet.Identity/packages.config` | 1 | 0 |
| `N2.Management.Targeting` | `src/Mvc/MvcTemplates/N2/Targeting/N2.Management.Targeting.csproj` | `src/Mvc/MvcTemplates/N2/Targeting/packages.config` | 4 | 0 |

## Avvikelser att känna till

- `src/WebForms/WebFormsTemplates/packages.config` är inkluderad i `N2.Templates`, `AddonCatalog` och `Wiki`. Därför visas samma tre deklarerade paket för alla tre, även om projektens DLL-referenser skiljer sig.
- `N2.Templates` refererar `System.Data.SQLite.Core` **1.0.112.0** i projektfilen och importerar dess `.targets`, medan den delade `packages.config` anger **1.0.119.0**. Båda versionerna visas i tabellen.
- `N2.Templates.UI.Tests` inkluderar ingen `packages.config`, men refererar `log4net` **3.5.0** direkt från `packages/`.
- `N2.Raven` refererar `RavenDB-Embedded` **1.0.573** direkt från `packages/`; paketet saknas i dess `packages.config`.
- `N2.Search.Remote.Client` och `N2.Search.Remote.Server` har varken inkluderad `packages.config` eller direkt `HintPath` till `packages/`.
- `src/Mvc/MvcTemplates/N2/packages.N2.Management.config` är inte inkluderad i `N2.Management.csproj` och räknas därför inte.

Paket i `packages.config` utan egen DLL-referens kan fortfarande behövas för byggmål, testadaptrar eller beroenden. Tabellen beskriver deklarationer och direkta referenser, inte en körningsanalys av transitiva assembly-beroenden.
