# N2CMS 2026: Release-bygge och lokala NuGet-paket för net48

## Läge

`N2.Everything.2026.sln` innehåller 26 projekt. Projekten riktar sig mot **.NET Framework 4.8** (`v4.8`), vilket ger `lib/net48` i paketen. Det är ännu inte en omriktning till .NET Framework 4.8.1. Visual Studios Release-bygge har rapporterats gå igenom utan fel eller varningar. Användaren rapporterade den 4 november 2026 att `vp_build.bat`, `vp_publish_to_nuget.bat` och `vp_remove_nuget.bat` fungerar vid test i Windows. Exakta serverresultat och vilken version som användes vid testet har inte dokumenterats.

Den 3 november 2026 kördes `build.bat 4.0.0-local.1` på Windows. Åtta lokala paket skapades under `output/vp-n2cms/4.0.0-local.1/`. Denna provversion har senare tagits bort. Ny arbetsversion är `2.0.0-beta.1`; N2CMS-paketens version behöver inte vara samma som VPBase3:s planerade 4.x-version.

## Bygg paketen på Windows

Kör från en Visual Studio Developer Command Prompt i n2cms-repot:

```bat
cd /d D:\Code\Github\n2cms\build
vp_build.bat 2.0.0-beta.1
```

[`build/vp_build.bat`](build/vp_build.bat) tar versionen som första argument eller frågar efter den om argumentet saknas, och anropar [`build/pack-vp-n2cms.ps1`](build/pack-vp-n2cms.ps1). Skriptet hittar aktuell MSBuild, bygger [`N2.Everything.2026.sln`](N2.Everything.2026.sln) i Release, kontrollerar nödvändiga DLL:er och skapar åtta `.nupkg`. Det publicerar eller taggar ingenting. Utdata ligger i `output/vp-n2cms/<version>/`; en befintlig versionskatalog skrivs inte över.

Kör `vp_build.bat` utan argument för att mata in versionen vid en prompt. Skriptet visar sin mapp, reporoten, vald version, utdatamapp och om Release ska byggas. Det kontrollerar att lösningen finns och att versionsmappen inte redan finns. Först efter frågan `Starta med dessa installningar? [j/N]` och svaret J startar bygget eller paketeringen; Enter avbryter. Efter en interaktiv körning väntar fönstret på en tangenttryckning så att resultatet kan läsas.

Om exakt samma källor redan är byggda i Release kan bygget hoppas över:

```bat
vp_build.bat 2.0.0-beta.1 --no-build
```

Använd ett nytt versionsnummer vid varje omkörning. `N2.Management` lägger sin DLL i `src/Mvc/MvcTemplates/bin/` även i Release, så `--no-build` förutsätter att just den filen kommer från rätt källversion.

För paketering krävs `NuGet.exe` version 5 eller senare på `PATH`, eller .NET SDK 10 eller senare. Medföljande `build/lib/NuGet.exe` är version 2.8 och används inte. En separat NuGet.exe kan anges direkt:

```powershell
.\pack-vp-n2cms.ps1 -Version 2.0.0-beta.1 -NuGetExe 'D:\Tools\NuGet.exe'
```

Det gamla `build/n2.proj` med `Build-NuGet` och `Push-NuGet` finns kvar som äldre kedja. Det bygger `N2.Sources.sln` med andra konfigurationer och gamla `net45`-recept. För de här åtta lokala paketen används `vp_build.bat` enligt ovan. `build.bat` finns kvar och anropar `vp_build.bat`, eftersom äldre byggfiler fortfarande använder namnet.

## Vad paketsteget gör

1. Bygger den aktuella 26-projektslösningen i Release, om `--no-build` inte angavs.
2. Läser externa beroendeversioner från berörda projekts `packages.config` och skriver dem som exakta beroenden i paketmanifesten. Interna `VP_N2CMS`-beroenden får samma angivna provversion.
3. Kopierar Release-DLL:er och `N2.XML` till `lib/net48`. Managementinnehållet hämtas från `<Content Include>` i `N2.Management.csproj` och packas som `content/N2/N2.zip` med `build/lib/BuildSupport.exe`.
4. Packar `.nuspec` och innehåll till åtta `.nupkg` under `output/vp-n2cms/<version>/`. Saknade filer, byggfel eller paketfel avbryter körningen.

| Paket | Innehåll och direkta interna beroenden |
| --- | --- |
| `VP_N2CMS.Config` | `web.config`-omvandlingar. |
| `VP_N2CMS.Config.SQLite` | SQLite-konfigurationsomvandling; `System.Data.SQLite.Core`. |
| `VP_N2CMS.Library` | `N2.dll` och `N2.XML` i `lib/net48`; externa beroenden från `src/Framework/N2/packages.config`. |
| `VP_N2CMS.Management` | `N2.Management.dll`, `N2.Extensions.dll` och `N2.Packaging.dll` i `lib/net48`; management-ZIP och konfiguration. Beror på Library och Config. |
| `VP_N2CMS.Razor` | `N2.Razor.dll` i `lib/net48`. Beror på Library. |
| `VP_N2CMS.Mvc` | `content/App_Start/CmsConfig.cs.pp`. Beror på Library och Razor. |
| `VP_N2CMS.Search.Lucene` | `N2.Search.Lucene.dll` i `lib/net48`. Beror på Library. |
| `VP_N2CMS` | Samlingspaket med dokument och beroenden till Config, Library, Management och Search.Lucene. |

De åtta paketen täcker de tidigare `VP_N2CMS`-paket-ID som har identifierats för VPBase3. De paketerar inte alla 26 projekt i lösningen. Se även [NuGet-paketöversikten](NuGet-paketoversikt.md) för projektens samlade paketinventering.

## Verifierat för `4.0.0-local.1`

- Åtta `.nupkg` fanns vid verifieringen och samtliga öppnades utan ZIP-fel. Managementpaketets inre `N2.zip` öppnades också utan ZIP-fel. Denna provversion har senare tagits bort.
- Alla paketmanifest har rätt paket-ID och version `4.0.0-local.1`. Samtliga tio direkta interna paketberoenden pekar på ett av de åtta skapade paketen med samma version.
- De sex paketerade DLL:erna och `N2.XML` är byte för byte identiska med motsvarande Release-resultat på disk. Alla bibliotek ligger under `lib/net48`.
- Management-ZIP innehåller alla **440** filer som projektfilen listar som `Content`; varje fil matchar källfilen byte för byte. ZIP-filen har 517 poster inklusive kataloger och innehåller 56 `.aspx`-sidor.
- Konfigurationsomvandlingarna, `Web.config` och MVC-startmallen i paketen matchar sina källfiler. Alla 26 direkta externa beroenden i manifesten har motsvarande paketversioner i den lokala `packages/`-katalogen.

Detta verifierar paketinnehåll och lokala beroenden. Installation, konfigurationsomvandlingar i en konsumentapp och N2CMS-funktioner vid körning är **ännu inte verifierade** av den kontrollen.

## Nästa kontroll i VPBase3

Lägg paketkatalogen som lokal NuGet-källa i en separat VPBase3-testapp. Prova nyinstallation av samlingspaketet tillsammans med de paket som appen faktiskt använder, exempelvis `VP_N2CMS.Config.SQLite` och `VP_N2CMS.Mvc`. Kontrollera att NuGet installerar beroendena, att `web.config`-omvandlingar fungerar och att appen startar. Testa därefter inloggning, behörighet, redigering/publicering, filer, MVC/Razor, SQLite och Lucene. Gör också ett uppgraderingstest från de tidigare `VP_N2CMS`-paketen.

`VP_N2CMS.Config.SQLite` läser `System.Data.SQLite.Core` **1.0.112.0** från `src/Mvc/MvcTemplates/packages.config`; NuGet normaliserar detta till `1.0.112` i manifestet. Vissa WebForms-projekt använder **1.0.119.0**, vilket behöver jämföras med VPBase3-appens faktiska SQLite-användning innan en skarp paketversion beslutas.

Välj stabilt versionsnummer efter dessa tester. Arbetsversionen är `2.0.0-beta.1`. Publicering görs separat och ingår inte i `vp_build.bat`.

## Publicering till VP:s NuGet-server

Använd samma version vid bygge och publicering, till exempel `vp_build.bat 2.0.0-beta.1` och sedan `vp_publish_to_nuget.bat 2.0.0-beta.1`. Bygget skapar `output\vp-n2cms\2.0.0-beta.1\` och placerar paketen där.

[`build/vp_publish_to_nuget.bat`](build/vp_publish_to_nuget.bat) tar paketversionen som första argument eller frågar efter den, och hittar alla `.nupkg` i `output\vp-n2cms\<version>`. Byggsteget skapar åtta paketfiler per version. Om serveradress inte finns i miljön använder skriptet `https://nuget2.voidpointer.se/v3/index.json`, enligt `OS/inbox/nuget-publish.txt`. Miljövariablerna `nugetSeverFullPath` eller `nugetSeverBrowsePath` kan åsidosätta adressen; `nugetServerFullPath` och `nugetServerBrowsePath` accepteras också. Vid publicering läses `NugetServerPassword` från systemets miljövariabel. Ingen nyckel behöver anges som argument eller sparas i repot.

Kör skriptet utan argument för att bli tillfrågad om version och därefter om paketen ska publiceras:

```bat
cd /d D:\Code\Github\n2cms\build
vp_publish_to_nuget.bat
```

Skriptet visar sin egen mapp, den uträknade reporoten, paketmappen `output\vp-n2cms\<version>` och alla `.nupkg` där. Frågan `Publicera dessa paket nu? [j/N]` accepterar ja eller nej; Enter väljer nej. Skriptet kontrollerar API-nyckeln först om publicering väljs. När versionen har matats in via prompt väntar fönstret på tangenttryckning vid slutet, så att resultat och fel kan läsas.

Versionen kan även skickas som argument. `--dry-run` visar urvalet utan publiceringsfråga eller nätverksanrop; `--publish` publicerar utan frågan:

```bat
vp_publish_to_nuget.bat 2.0.0-beta.1 --dry-run
vp_publish_to_nuget.bat 2.0.0-beta.1 --publish
```

`dotnet nuget push` körs för varje `.nupkg` i den valda mappen och skriptet stoppar vid första fel. Om ett senare paket misslyckas kan tidigare paket redan finnas på servern; kontrollera serverns faktiska läge innan samma version försöks igen. Skriptet hoppar inte tyst över dubbletter. Inga paket har publicerats inom denna verifiering.

## Ta bort en publicerad paketversion

[`build/vp_remove_nuget.bat`](build/vp_remove_nuget.bat) tar en version som argument eller frågar efter den. Skriptet visar server, version och de åtta fasta paket-ID:n som byggkedjan skapar. Efter frågan `Ta bort denna version for alla paketen nu? [j/N]` skickar det ett `dotnet nuget delete`-anrop per paket om du svarar J; Enter eller N avbryter. Samma serveradress och systemvariabel `NugetServerPassword` som vid publicering används. Lokal versionsmapp visas som referens men behöver inte finnas för att begära borttagning från servern.

```bat
cd /d D:\Code\Github\n2cms\build
vp_remove_nuget.bat 2.0.0-beta.1 --dry-run
vp_remove_nuget.bat 2.0.0-beta.1
```

`--dry-run` visar urvalet utan serveranrop. Skriptet avbryter vid första fel; tidigare paket kan då redan vara borttagna eller avlistade. `dotnet nuget delete` kan antingen ta bort eller avlista versionen beroende på servern, så kontrollera resultatet i paketkällan efteråt.
