# RushTodo React Notes

## Running the UI

Open `C:\GitHub\RushToDo\rush-todo-web` as a folder in Visual Studio Code, then use its integrated terminal:

```powershell
npm.cmd install
npm.cmd run dev
```

Opening the UI folder keeps Explorer, search results and suggestions focused on React. Open the repository root only when a task genuinely spans both the API and UI.

Open the local URL printed by Vite. Keep the command running while developing; saving a source file updates the browser almost immediately through hot-module replacement. Press `Ctrl+C` in the terminal to stop it.

After the first `npm.cmd install`, normal development usually needs only:

```powershell
npm.cmd run dev
```

Use these before handing over a change:

```powershell
npm.cmd run lint
npm.cmd run build
```

`npm.cmd` avoids the local PowerShell execution policy blocking `npm.ps1`; it is otherwise ordinary npm.

## Mental Map

| React/Vite | Rough .NET or Blazor analogue |
|---|---|
| `package.json` | Project dependencies and runnable commands; part `.csproj`, part launch tooling |
| `src/main.tsx` | Startup/bootstrap code that mounts the application, loosely like `Program.cs` |
| `src/App.tsx` | The root component, roughly `App.razor` |
| `src/App.css` | Styles owned by the root component |
| `src/index.css` | Application-wide styles and CSS variables |
| `vite.config.ts` | Development/build-server configuration |
| `index.html` | The single HTML host containing the React `root` element |

Files ending in `.tsx` are TypeScript files that may contain JSX markup. A React component is normally a function that returns that markup. `main.tsx` renders `<App />` into `<div id="root">`; React then owns everything inside that element.

`StrictMode` in `main.tsx` enables additional development checks. Some component lifecycle work may intentionally run twice during development; this does not happen in a production build.

The current component hierarchy is:

```text
App
└── WorkItemEditPage
    └── PageFrame
        └── Panel
            ├── Button (Save)
            ├── TextField (Title)
            ├── TextField (Address)
            ├── GardenerField
            │   └── AutocompleteField
            ├── TextAreaField (Description)
            └── DateField (Scheduled)
```

`WorkItemEditPage` owns the draft WorkItem in `useState`. Each `TextField` is controlled: it receives its current value and reports changes through `onChange`. The page creates a new state object for each change rather than mutating the existing object; React then renders the updated values.

Saving uses `Api.saveWorkItem`, which selects POST or PUT from `workItemId`. The server-returned model becomes both the current edit model and the persisted baseline, retaining its generated ID and concurrency timestamp while disabling Save until another field changes.

Disabled buttons use the shared `Tooltip` host so their explanation remains available by tap, mouse hover or keyboard focus even though the native button itself cannot receive those interactions.

Shared field presentation belongs in `FieldWrapper`; input-specific behaviour belongs in controls such as `TextField`. Base styling is mobile-first: fields use one column and expand to two columns only when enough width is available.

`WorkItemValidator` supplies field labels, limits, visibility and validation. Invalid controls have a red border; their validation tooltip appears on focus/tap or mouse hover. `StatusId` is not edited directly: setting or clearing `ScheduledDate` selects `Scheduled` or `New`, while later completion and cancellation actions own their terminal dates and statuses.

`GardenerField` loads real lookup data through `Api.getGardeners()` and passes the resulting `LookupItem[]` to the generic `AutocompleteField`. During development, Vite proxies `/gardener` to the API running at `https://localhost:63240`, so the browser sees the request as same-origin and needs no CORS configuration.

Run the API from Visual Studio and the UI from Visual Studio Code. If the API is not running, `GardenerField` displays the API error inline using the same `FieldWrapper` validation area.

The TypeScript `WorkItemModel` mirrors the API model. Nullable .NET `DateOnly` values are represented as nullable `yyyy-MM-dd` strings, which is also the native value format of `<input type="date">`.
