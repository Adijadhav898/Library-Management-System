# Plan: Library Management System (LMS) Implementation

**Source Document**: C:\Users\Aditya Jadhav\Downloads\LIBRARY_MANAGEMENT_SYSTEM.md  
**Complexity**: Large (8 phases, ~9 weeks, full-stack application)

---

## Summary
Build a complete Library Management System with Angular 17+ frontend and ASP.NET Core 8 backend using Clean Architecture. The system manages books, members, circulation (issue/return/renew), reservations, fines, notifications, reports, and admin functions with JWT-based authentication and role-based authorization (Admin, Librarian, Member).

---

## Requirements Restatement

### Core Features
1. **Authentication & Authorization**: JWT access/refresh tokens, role-based access (Admin/Librarian/Member), password reset
2. **Book/Catalog Management**: CRUD for books, authors, categories, publishers, book copies with barcode tracking, bulk import
3. **Member Management**: Member types with different limits, membership cards, borrowing history, block/unblock
4. **Circulation**: Issue/return/renew books with business rule validation, due dates, lost/damaged handling
5. **Reservations**: Queue-based holds, notifications, auto-expiry
6. **Fines & Payments**: Configurable fines, auto-generation, waivers, payment recording, PDF receipts
7. **Notifications**: Email (due reminders, overdue, reservation ready) + in-app bell
8. **Reports & Dashboard**: Statistics, charts, PDF/Excel exports
9. **Administration**: System settings, audit logs, user/role management

### Technical Stack
- **Backend**: ASP.NET Core 8 Web API, EF Core Code First, SQL Server, AutoMapper, FluentValidation, Serilog, xUnit
- **Frontend**: Angular 17+, Standalone Components, Angular Material, RxJS, ngx-charts/Chart.js, ngx-toastr
- **Auth**: JWT (15min access, 7-day refresh), BCrypt hashing, role-based guards
- **DevOps**: Docker, Docker Compose, CI/CD (Azure DevOps/GitHub Actions)

---

## Patterns to Mirror

| Category | Source | Pattern |
|---|---|---|
| Architecture | Design Doc §1, §5 | Clean Architecture: API → Application → Domain → Infrastructure → Tests |
| Naming (Backend) | Design Doc §5 | Controllers: `{Entity}Controller.cs`, Services: `{Entity}Service.cs`, DTOs: `{Entity}Dto.cs` |
| Naming (Frontend) | Design Doc §6 | Services: `{entity}.service.ts`, Models: `{entity}.model.ts`, Components: `kebab-case` folders |
| Error Handling | Design Doc §9, §10 | Global exception middleware → ProblemDetails, FluentValidation filters, consistent `ApiResponse<T>` |
| Data Access | Design Doc §5, §162 | GenericRepository + UnitOfWork, IEntityTypeConfiguration per entity |
| Auth | Design Doc §8, §9 | JWT in header, refresh token rotation (hashed in DB), `[Authorize(Roles)]` attributes |
| Testing | Design Doc §10 | xUnit + Moq for unit tests, in-memory/SQLite for integration |
| UI Patterns | Design Doc §6, §11 | Layout-based routing (auth/admin/member), lazy-loaded feature modules, shared components |

---

## Phase Breakdown

### Phase 1: Project Setup & Foundation (Week 1)
**Goal**: Scaffold solution, database, auth, Angular shell

#### Backend Tasks
- [ ] Create solution structure: `LMS.API`, `LMS.Application`, `LMS.Domain`, `LMS.Infrastructure`, `LMS.Tests`
- [ ] Configure `LibraryDbContext` with SQL Server connection
- [ ] Create all Domain entities (§4.1) with BaseEntity, enums
- [ ] Implement EF Core configurations (IEntityTypeConfiguration)
- [ ] Create initial migration and seed data (roles, admin, member types)
- [ ] Setup DI containers in each layer (DependencyInjection.cs)
- [ ] Implement JWT TokenService (access + refresh, BCrypt)
- [ ] Build AuthController: register, login, refresh, logout, forgot/reset password
- [ ] Add global exception middleware, validation filter, Serilog
- [ ] Configure Swagger with JWT auth
- [ ] Create Dockerfile for API

#### Frontend Tasks
- [ ] Create Angular 17+ workspace with Angular Material
- [ ] Setup environments (dev/prod), styles.scss, assets
- [ ] Build core: AuthService, JWT interceptor, refresh token interceptor, error interceptor
- [ ] Create guards: AuthGuard, RoleGuard
- [ ] Build layouts: AuthLayout, AdminLayout, MemberLayout
- [ ] Setup routing per §11 (lazy-loaded features)
- [ ] Create shared components: Navbar, Sidebar, NotificationBell, Loader, DataTable, Pagination
- [ ] Build Login/Register/ForgotPassword/ResetPassword pages
- [ ] Configure HTTP interceptors, toast service

#### Validation
```bash
# Backend
dotnet build
dotnet ef database update --project ../LMS.Infrastructure --startup-project .
dotnet run --project LMS.API
# Swagger at https://localhost:5001/swagger

# Frontend
ng serve
# App at http://localhost:4200
```

---

### Phase 2: Catalog Management (Weeks 1-2)
**Goal**: Books, Authors, Categories, Publishers, BookCopies CRUD + search

#### Backend Tasks
- [ ] Create DTOs: BookDto, CreateBookDto, UpdateBookDto, BookSearchDto, AuthorDto, CategoryDto, PublisherDto, BookCopyDto
- [ ] Implement IBookService, IAuthorService, ICategoryService, IPublisherService interfaces
- [ ] Build BookService with: search (title, author, ISBN, category, language, availability), pagination, sorting
- [ ] Build Author/Category/Publisher CRUD services
- [ ] Implement BookCopy management (barcode, condition, status)
- [ ] Add file upload for cover images (FileStorageService)
- [ ] Implement CSV/Excel bulk import (EPPlus or similar)
- [ ] Create Controllers: BooksController, BookCopiesController, AuthorsController, CategoriesController, PublishersController
- [ ] Add FluentValidation validators for all DTOs
- [ ] AutoMapper profiles for all entities
- [ ] Unit tests for BookService search/filter logic

#### Frontend Tasks
- [ ] Create Book models and services
- [ ] Build BookListComponent with search, filter, pagination, sorting (Angular Material Table)
- [ ] Build BookFormComponent (add/edit) with reactive forms, image upload
- [ ] Build BookDetailComponent with availability, copies, reserve button
- [ ] Build BookCopiesComponent for copy management
- [ ] Build BookImportComponent for CSV/Excel upload
- [ ] Create Author/Category/Publisher list and form components
- [ ] Add catalog masters to admin navigation

#### Validation
```bash
dotnet test LMS.UnitTests
ng test
# Manual: Create book → verify copies → search → import CSV
```

---

### Phase 3: Member Management (Week 2-3)
**Goal**: Members, MemberTypes, member portal basics

#### Backend Tasks
- [ ] Create MemberDto, CreateMemberDto, UpdateMemberDto, MemberTypeDto
- [ ] Implement IMemberService with: CRUD, block/unblock, membership validity, borrowing history
- [ ] MemberType configuration (MaxBooksAllowed, LoanDays, MaxRenewals, FinePerDay)
- [ ] MembersController with role-based endpoints
- [ ] Member self-service endpoints (/members/me)
- [ ] Validators and AutoMapper profiles
- [ ] Unit tests for member business rules

#### Frontend Tasks
- [ ] MemberListComponent with search, filter, pagination
- [ ] MemberFormComponent (add/edit) with member type selection
- [ ] MemberDetailComponent showing loans, fines, reservations, history
- [ ] Member portal: MyProfileComponent (update profile, change password)
- [ ] Block/unblock actions with confirmation dialog

#### Validation
```bash
dotnet test
ng test
# Manual: Create member types → create members → test block/unblock → verify portal
```

---

### Phase 4: Circulation - Issue/Return/Renew (Weeks 3-4)
**Goal**: Core lending logic with all business rules

#### Backend Tasks
- [ ] Create LoanDto, IssueBookDto, ReturnBookDto, RenewLoanDto
- [ ] Implement ILoanService with transactional operations:
  - **Issue**: Validate member limits, availability, fines, membership expiry → create Loan, update BookCopy status
  - **Return**: Calculate overdue days, auto-generate Fine, update Loan/BookCopy, check reservations
  - **Renew**: Check renewal count, overdue status, pending reservations
- [ ] Implement business rules (§8) in service layer
- [ ] LoansController with endpoints per §7
- [ ] Database transaction handling (UnitOfWork)
- [ ] AuditLog entries for all circulation actions
- [ ] Comprehensive unit tests for all business rule combinations

#### Frontend Tasks
- [ ] IssueBookComponent: Scan/enter member ID + barcode → validation feedback → confirm
- [ ] ReturnBookComponent: Scan barcode → show overdue/fine → confirm return
- [ ] RenewBookComponent: List renewable loans → renew action
- [ ] LoanListComponent: Filter by status, member, date range
- [ ] Member portal: MyLoansComponent with renew buttons
- [ ] Barcode scanner integration (optional: QuaggaJS or HTML5 Barcode API)

#### Validation
```bash
dotnet test LMS.UnitTests --filter "LoanService"
# Manual: Issue → Return (on time) → Issue → Return (late, verify fine) → Renew (allowed/blocked)
```

---

### Phase 5: Reservations + Fines & Payments (Week 5)
**Goal**: Hold queue, fine calculation, payment collection

#### Backend Tasks
- [ ] ReservationService: Create reservation (queue), notify when ready, auto-expire, cancel
- [ ] FineService: Auto-generate on return, configurable rates, waiver (admin only)
- [ ] PaymentService: Record payment (cash/UPI/card), generate PDF receipt (QuestPDF or similar)
- [ ] Background job: ReservationExpiryJob (daily, mark expired)
- [ ] Controllers: ReservationsController, FinesController, PaymentsController
- [ ] Validators, mappers, unit tests

#### Frontend Tasks
- [ ] ReservationListComponent (librarian): Queue view, notify, cancel
- [ ] MyReservationsComponent (member): Status, cancel
- [ ] FineListComponent: Filter by status, member
- [ ] CollectPaymentComponent: Modal with payment mode, amount, receipt download
- [ ] MyFinesComponent (member): View fines, payment history
- [ ] PDF receipt viewer/download

#### Validation
```bash
dotnet test --filter "ReservationService|FineService|PaymentService"
# Manual: Reserve book → return copy → verify notification → pick up → test fine payment → receipt
```

---

### Phase 6: Notifications & Background Jobs (Week 6)
**Goal**: Email + in-app notifications, scheduled jobs

#### Backend Tasks
- [ ] EmailService: SMTP/SendGrid, templates for due reminder, overdue, reservation ready, fine, password reset
- [ ] NotificationService: Create in-app notifications, mark read, unread count
- [ ] Background Jobs (Hosted Services):
  - DueDateReminderJob: Daily, 2 days before due date
  - OverdueFineJob: Daily, generate fines for overdue loans
  - ReservationExpiryJob: Daily, expire uncollected reservations
- [ ] Hangfire setup (optional, for reliability)
- [ ] NotificationsController: List, mark read
- [ ] Unit tests for job logic

#### Frontend Tasks
- [ ] NotificationBellComponent: Unread badge, dropdown list, mark read
- [ ] NotificationService: Polling or SignalR (future enhancement)
- [ ] Email template preview (admin)
- [ ] Settings for notification preferences

#### Validation
```bash
# Manual: Trigger jobs → verify emails sent → check in-app notifications → test markdown rendering
```

---

### Phase 7: Dashboard, Reports & Admin (Week 7)
**Goal**: Analytics, exports, system settings, audit logs

#### Backend Tasks
- [ ] DashboardService: Summary stats (books, members, issued today, overdue, fines, new members)
- [ ] DashboardService: Charts data (most borrowed, category distribution, monthly trends)
- [ ] ReportService: Issued, Overdue, Fines, Inventory, Member Activity, Lost/Damaged
- [ ] Export: PDF (QuestPDF) and Excel (EPPlus/ClosedXML) with format parameter
- [ ] SettingsService: Key-value system settings
- [ ] AuditLogService: Query with filters
- [ ] UserManagementService: Admin CRUD for users/roles
- [ ] Controllers: DashboardController, ReportsController, SettingsController, AuditLogsController, UsersController

#### Frontend Tasks
- [ ] AdminDashboardComponent: Stats cards + ngx-charts/Chart.js visualizations
- [ ] MemberDashboardComponent: Personal stats
- [ ] Report components: Filters, preview, export buttons (PDF/Excel)
- [ ] SettingsComponent: Key-value editor with validation
- [ ] UserManagementComponent: User list, role assignment, activate/deactivate
- [ ] AuditLogsComponent: Paginated, filterable table

#### Validation
```bash
dotnet test --filter "ReportService|DashboardService"
# Manual: Generate each report → verify PDF/Excel → check dashboard charts → test settings persistence
```

---

### Phase 8: Testing, Security, Docker, CI/CD (Week 8-9)
**Goal**: Production readiness

#### Testing
- [ ] Integration tests: Auth flow, full circulation cycle, reservation flow
- [ ] Frontend E2E: Cypress/Playwright for critical paths (login → issue → return → pay fine)
- [ ] Load test: Concurrent issues/returns, report generation
- [ ] Accessibility audit: Angular Material a11y, keyboard nav

#### Security Hardening
- [ ] Rate limiting on auth endpoints
- [ ] CORS policy locked to Angular origin
- [ ] Security headers (HSTS, CSP, X-Frame-Options)
- [ ] Secrets in Azure Key Vault / user-secrets (not Git)
- [ ] Input validation review (XSS, SQL injection)
- [ ] Dependency vulnerability scan (OWASP Dependency Check)

#### Docker & Deployment
- [ ] docker-compose.yml: sqlserver, lms-api, lms-ui (nginx)
- [ ] Multi-stage Dockerfiles for API and UI
- [ ] Health checks for all services
- [ ] GitHub Actions / Azure DevOps pipelines:
  - Build → Test → Docker Build → Push → Deploy
  - Separate pipelines for dev/staging/prod

#### Documentation
- [ ] README with setup instructions
- [ ] API documentation (Swagger)
- [ ] Architecture decision records (ADRs)
- [ ] Deployment guide

#### Validation
```bash
docker-compose up -d
# Verify all services healthy
curl https://localhost:5001/health
curl http://localhost:4200
# Run full test suite
dotnet test
ng test
ng e2e
```

---

## Dependencies Between Phases

```
Phase 1 (Foundation)
    │
    ├──→ Phase 2 (Catalog) ────┐
    ├──→ Phase 3 (Members) ────┤
    │                          │
    └──→ Phase 4 (Circulation) ←┘ (needs Books + Members)
            │
            ├──→ Phase 5 (Reservations + Fines) (needs Circulation)
            │
            ├──→ Phase 6 (Notifications + Jobs) (needs Fines, Reservations)
            │
            ├──→ Phase 7 (Dashboard + Reports) (needs all data)
            │
            └──→ Phase 8 (Testing + Deploy) (needs all features)
```

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| EF Core migration conflicts | Medium | High | Single developer on migrations, review before merge |
| Complex business rule interactions (circulation) | High | High | Comprehensive unit tests, BDD-style specs |
| Angular Material theming/customization | Medium | Medium | Use Material design tokens, plan theme early |
| Background job reliability | Medium | High | Use Hangfire with SQL storage, idempotent jobs |
| PDF/Excel export formatting | Medium | Medium | Use proven libraries (QuestPDF, ClosedXML), test templates |
| JWT refresh token race conditions | Low | High | Implement token rotation with concurrency handling |
| Docker SQL Server startup timing | Medium | Medium | Health checks, retry logic in API startup |
| CI/CD pipeline complexity | Medium | Medium | Start simple, add stages incrementally |

---

## Complexity Assessment

| Area | Complexity | Effort Estimate |
|---|---|---|
| Backend Architecture & Auth | Medium | 20-25 hrs |
| Catalog Management | Medium | 15-20 hrs |
| Member Management | Low | 10-15 hrs |
| Circulation (Core Business Logic) | High | 25-30 hrs |
| Reservations & Fines | Medium | 15-20 hrs |
| Notifications & Background Jobs | Medium | 15-20 hrs |
| Dashboard, Reports, Exports | Medium | 20-25 hrs |
| Frontend (All Phases) | High | 40-50 hrs |
| Testing (Unit + Integration + E2E) | Medium | 20-25 hrs |
| Docker, CI/CD, Security | Medium | 15-20 hrs |
| **Total** | **Large** | **~195-250 hrs** |

---

## Validation Checklist

- [ ] All 8 phases complete
- [ ] Backend: `dotnet build` + `dotnet test` pass
- [ ] Frontend: `ng build` + `ng test` + `ng e2e` pass
- [ ] Docker compose: All services healthy
- [ ] Manual smoke test: Register → Login → Create book → Create member → Issue → Return (late) → Pay fine → View report
- [ ] Security: No secrets in code, rate limiting works, CORS locked
- [ ] Documentation: README, API docs, deployment guide complete

---

**WAITING FOR CONFIRMATION**: Proceed with this plan? (yes/no/modify)

If modifying, specify which phases to adjust, reorder, or add detail to.