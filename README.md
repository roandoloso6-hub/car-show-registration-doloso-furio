# Car Show Registration System

**Car Show 2027** is a registration system for **AutoVista Events** in Sorsogon City Coliseum. It manages registrations for three participant categories: **Exhibitors**, **Competitors**, and **VIPs**.

The project provides a C#/.NET 10 Vertical Slice API backed by EF Core and SQLite, together with a React JS Vite frontend.

## Repository Structure

```text
car-show-registration-doloso-furio/
├── backend/                 # .NET 10 Vertical Slice API
├── frontend/                # React JS Vite application
├── docs/
│   └── screenshots/         # Project documentation screenshots
└── README.md
```

## Team & Roles

| Role | Responsibilities | Team Member |
|---|---|---|
| Backend Developer | C#/.NET 10, EF Core, SQLite, FluentValidation, and Scalar | `<Backend Developer Name>` |
| Frontend Developer | React JS Vite, Tailwind CSS/Bootstrap | `<Frontend Developer Name>` |

## Tech Stack

### Backend

- C# and .NET 10
- Vertical Slice Architecture
- Entity Framework Core
- SQLite
- FluentValidation
- Scalar OpenAPI UI

### Frontend

- React JS
- Vite
- Tailwind CSS or Bootstrap

## Running the Backend API

Open a terminal in the repository root.

1. Navigate to the API project:

   ```powershell
   cd backend
   dotnet restore
   ```

2. Install the EF Core CLI, if it is not already available:

   ```powershell
   dotnet tool install --global dotnet-ef
   ```

3. Navigate to the API project if the previous step has not returned you there:

   ```powershell
   cd Api
   ```

4. Create the initial EF Core SQLite migration:

   ```powershell
   dotnet ef migrations add InitialCreate --project ../Infrastructure --startup-project .
   ```

5. Create or update the SQLite database:

   ```powershell
   dotnet ef database update --project ../Infrastructure --startup-project .
   ```

6. Run the API:

   ```powershell
   dotnet run
   ```

7. Open Scalar in a browser:

   ```text
   http://localhost:5205/scalar/v1
   ```

If the API port differs from `5205`, use the URL printed by `dotnet run` and append `/scalar/v1` to that address.

## Running the Frontend

Open another terminal in the repository root.

1. Navigate to the frontend folder:

   ```powershell
   cd frontend
   ```

2. Install the dependencies:

   ```powershell
   npm install
   ```

3. Start the Vite development server:

   ```powershell
   npm run dev
   ```

4. Open the frontend in a browser:

   ```text
   http://localhost:5173
   ```

Use the URL displayed by Vite if its port changes.

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| `GET` / `POST` | `/api/registrations` | List registrations or create a new registration. |
| `GET` / `PUT` / `DELETE` | `/api/registrations/{id}` | Retrieve, update, or delete a registration by its ID. |
| `GET` | `/api/registrations/summary` | Retrieve total and category-based registration summaries. |

## Business Rules

- Registration fees:
  - Exhibitor: **₱1,500**
  - Competitor: **₱2,500**
  - VIP: **₱5,000**
- The registration number is generated automatically using the format `CS27-0001`, with sequential numbering.
- Registration dates are stored in UTC.
- New registrations default to the `Pending` status.
- Payment status is computed from the amount paid:
  - `Unpaid` — no payment
  - `Partial` — payment is less than the required fee
  - `Paid` — payment equals or exceeds the required fee
- A registration with a duplicate plate number is rejected with **`409 Conflict`**.
