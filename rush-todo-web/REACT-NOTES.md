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

For now, change the text in `src/App.tsx`, save it, and watch the browser update. The next useful increment is the API facade; components should call that rather than scattering `fetch` requests.
