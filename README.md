# Title Archive

Title Archive (titles.gbx.tools) is an enhanced browser and archive for ManiaPlanet title packs.

## Use cases

* Downloading a title pack without starting the game
* Inspecting a title pack before installing it
* Keeping a local archive of title packs and their artwork

## Features

* Search title packs by name or UID
* Pin frequently used title packs in the browser
* Browse title-pack metadata, included packs, files, and multiplayer scripts
* View author details, game-mode capabilities, and player-count history
* Download `Title.Pack.Gbx` files directly
* Archive title packs, cards, backgrounds, and logos locally with conditional refreshes

## Build

Make sure to have the .NET 10 SDK installed to build the web application.

You also need a MariaDB or MySQL database. The default development connection is in `Src/BigBang1112.GbxTools.Titles.BlazorWebApp/appsettings.Development.json`; the database is created automatically when the web application runs in Development.

Start the web application with:

```powershell
dotnet run --project Src/BigBang1112.GbxTools.Titles.BlazorWebApp
```

For local containers, the included Docker Compose setup starts the web app, MariaDB, phpMyAdmin, and the Aspire dashboard. Visual Studio 2022 can launch the `Docker Compose` profile directly.

For production, provide a secure `ConnectionStrings__DefaultConnection` value and apply the Entity Framework migrations before deploying.

### Local archive

The local archive is disabled by default. Enable it and choose its location through configuration or environment variables:

```json
{
  "TitleArchive": {
    "Enabled": true,
    "Path": "title-archive",
    "RefreshInterval": "1.00:00:00"
  }
}
```

The archive keeps packs under `titles/<title-uid>/`, saves related images alongside them, and records HTTP validators in `archive-manifest.json` so unchanged resources are not downloaded again.

## License

AGPL-3.0. See [LICENSE.txt](LICENSE.txt).
