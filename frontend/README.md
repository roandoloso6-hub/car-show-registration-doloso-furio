# Car Show Registration Frontend

React + Vite + Tailwind CSS frontend for the Car Show 2027 registration system.

## Run

```powershell
cd frontend
npm install
npm run dev
```

The frontend runs at `http://localhost:5173` by default. The development API URL is defined in `.env.development`:

```text
VITE_API_BASE_URL=http://localhost:5205
```

Start the C# backend from the `backend` folder, then open the frontend.

## Switch data sources

The page is currently connected to `src/api/registrationsApi.js` in `src/pages/RegistrationsPage.jsx`.

To use local sample data, change:

```js
import * as registrationsApi from '../api/registrationsApi.js'
const dataSource = registrationsApi
```

to:

```js
import * as sampleData from '../api/sample-data.js'
const dataSource = sampleData
```

The sample and real API modules expose the same `getRegistrations(...)` and `getSummary()` functions.

# React + Vite

This template provides a minimal setup to get React working in Vite with HMR and some Oxlint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Oxc](https://oxc.rs)
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/)

## React Compiler

The React Compiler is not enabled on this template because of its impact on dev & build performances. To add it, see [this documentation](https://react.dev/learn/react-compiler/installation).

## Expanding the Oxlint configuration

If you are developing a production application, we recommend using TypeScript with type-aware lint rules enabled. Check out the [TS template](https://github.com/vitejs/vite/tree/main/packages/create-vite/template-react-ts) for information on how to integrate TypeScript and Oxlint's TypeScript related rules in your project.
