# CustomWallpaperUWP

CustomWallpaperUWP is a Universal Windows Platform (UWP) application designed to manage, display, and interact with images as custom wallpapers. It allows users to navigate image collections, view image details, and manage wallpaper history efficiently.

## Objective

The goal of this project is to provide a performant, modular, and extensible image management system for wallpapers on UWP, with a clean UI, logging, state management, and Prism-based MVVM structure.

## Architecture

The project follows the MVVM (Model-View-ViewModel) pattern using Prism for navigation, dependency injection, and view composition. Key features:

- **Prism + Unity**: For MVVM infrastructure, DI and navigation.
- **Services Layer**: Includes logging (`LoggerService`), wallpaper management (`WallpaperService`), and database access (`WallpaperDatabase`).
- **Views and ViewModels**: Pages are organized under `Views` and their logic under `ViewModels`, with bindings enabled via `ViewModelLocationProvider`.
- **State Management**: Uses `PageStateService` to persist and restore UI state.
- **UWP Controls**: NavigationView, GridView, and DataGrid for rich user interaction.

## Project Structure

- `Views/` - Contains UI pages like `AppShell`, `PicturesGridPage`, `WallpaperHistoryPage`, etc.
- `ViewModels/` - ViewModel counterparts for each page.
- `Services/` - Application services for wallpapers, logging, and state handling.
- `Models/` - Definitions of entities such as `WallpaperEntry` and others.
- `Infrastructure/` - Dependency injection and bootstrap logic.
- `Assets/` - Icons and images.
- `Constants/` - String and folder constants used globally.

## Prerequisites

- Visual Studio 2022 or later with UWP development workload
- NuGet packages restored (handled by Visual Studio)
- SQLitePCLRaw.bundle_green

## Building and Running

1. Clone the repository:

```bash
git clone https://github.com/sandromendes/CustomWallpaperUWP.git
```

2. Open the `.sln` file in Visual Studio.

3. Set `CustomWallpaperUWP` as the startup project.

4. Select a target (e.g., `x64`, `Debug`) and a local machine.

5. Press `F5` to build and run.

## 🧭 Navigation Flow

- App starts with `AppShell`, the main shell containing a `NavigationView`.
- Initial navigation is handled in `OnLaunchApplicationAsync` via the Prism NavigationService.
- Example pages: `PicturesGridPage`, `WallpaperHistoryPage`, `SavedImagesListPage`, `ImageViewerPage`.

## Features

- Wallpaper history and viewer
- Persistent saved images
- Logging system (FileLogger)
- Background task registration via DI
- Exception-safe initialization

## Known Issues

- Initial navigation from shell may require manual trigger if Prism navigation doesn't hook correctly.
- Background tasks might not register if app is not launched in debug mode.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

© Sandro Mendes - CustomWallpaperUWP Project