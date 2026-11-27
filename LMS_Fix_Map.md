# LMS Application layer: Fix Map (281 errors, 118 distinct galat naam)

Ye tumhare `build-errors` output (281 errors, sirf `LMS.Application`) aur repo snapshot `0cccd1a` ko match karke bana hai.
Pehle ye samjho: 281 errors me sirf **118 alag naam** galat hain. Ek naam theek karne se 2 se 8 errors jaate hain. Agar tumne local me kuch fix kiya hai to count alag ho sakta hai; ye map snapshot ke hisaab se hai.

Rule: **Domain source of truth hai.** Pehle Domain me jo cheez sach me chahiye (decisions section) wo add karo, phir baaki sab naam Domain ke hisaab se badlo.

Errors ki 4 qismein:

| Qism | Matlab | Kitne naam (approx.) |
| --- | --- | --- |
| A. Rename | Domain me hai, Application ne galat naam likha | ~45 |
| B. DTO me property add/rename | Domain me hai, DTO me nahi (ya alag naam) | ~30 |
| C. Domain me add karna | Domain me nahi hai, lekin feature ko chahiye | ~8 (Decisions) |
| D. Hatao / replace | Domain me nahi hai aur chahiye bhi nahi | ~25 |
| E. Repo method missing | Interface/Infra me method nahi | ~9 |

---

## 1. Decisions (pehle ye lo, phir baaki)

Har decision ke saath meri recommendation hai. Agar kuch alag chahiye to wahi Domain me badlo, baaki map us hisaab se chalega.

| # | Sawal | Recommendation | Errors jo isse jaate hain |
| --- | --- | --- | --- |
| D1 | `Fine` me `PaidAmount` aur `Description` nahi hain (Application 14 jagah use karta hai) | `Fine` me add karo: `decimal PaidAmount`, `string? Description`. `OutstandingAmount` DTO me computed rakho (`Amount - PaidAmount`). | `Fine.PaidAmount` 8, `Fine.Description` 6, `Fine.AmountPaid` 1, `Fine.OutstandingAmount` 1 |
| D2 | `Notes` aur `FulfilledAt` Domain me nahi | `Loan`, `Reservation`, `Payment` me `string? Notes`; `Reservation` me `DateTime? FulfilledAt` | `Loan.Notes` 4, `Reservation.Notes` 4, `Payment.Notes` 2, `Reservation.FulfilledAt` 3 |
| D3 | `Reservation.BookCopyId` / `BookCopy` | **Add karo** (nullable): `Guid? BookCopyId`, `BookCopy? BookCopy`. Design rule 6 ("reservation ready hone par copy hold karo") ke liye zaroori hai, aur service hold/expire me use karti hai. EF config me FK `DeleteBehavior.Restrict` rakho (SQL Server multiple cascade path error se bachne ke liye). | `Reservation.BookCopyId` 7, `Reservation.BookCopy` 3 |
| D4 | `MemberType.ReservationExpiryHours`, `Description` | `ReservationExpiryHours` add karo (`int`, default 48). `Description` DTOs se hata do. `CanReserve`, `MaxReservationsAllowed`, `MembershipFee`, `MembershipDurationDays` **hata do** (max reservations setting me hai, baaki design doc me nahi). | `MemberType.CanReserve` 6, `.Description` 6, `.MaxReservationsAllowed` 6 + DTO requests |
| D5 | `CreateMemberRequest.UserId` | Abhi `UserId` add karo (member ek existing user ke liye banta hai). `FirstName/LastName/Email` ko `CreateMemberRequest` me rehne do ya hata do, jo bhi `MemberService.CreateAsync` use na kare. Baad me decide karna ki member create karte waqt user bhi ban jaye (Librarian ke liye zaroori, kyunki `UserController` AdminOnly hai). | `CreateMemberRequest.UserId` 3 |
| D6 | `Gender`, `DateOfBirth` (Member) | Hata do, Domain me `Member` ke paas ye nahi. | `Gender` 4, `DateOfBirth` 1 |

Domain me kul badlav: `Fine` (+2), `Loan` (+1), `Reservation` (+4: Notes, FulfilledAt, BookCopyId, BookCopy), `Payment` (+1), `MemberType` (+1). Inke EF configs bhi update karo (`Notes` HasMaxLength(500), `Description` HasMaxLength(500), `PaidAmount` precision (18,2)).

---

## 2. Qism A: Rename (Application me Domain ke naam likho)

### Entity properties

| Galat | Sahi (Domain) | Count | Kahan |
| --- | --- | --- | --- |
| `Loan.IssuedByUser` | `Loan.IssuedBy` | 6 | CirculationService/Profile |
| `Loan.IssuedByUserId` | `Loan.IssuedById` | 3 | CirculationService/Profile |
| `Loan.ReturnedBy` | `Loan.ReturnedByUser` | 3 | CirculationService/Profile |
| `Member.IsActive` | `!member.IsBlocked` (condition ulta) | 6 | MemberService, CirculationService, MemberProfile |
| `Member.FirstName` / `LastName` | `member.User.FirstName` / `.LastName` (User include hona chahiye) | 3 | MemberService |
| `Member.MemberNumber` | `Member.MembershipNumber` | 3 | MemberService |
| `MemberType.LoanDurationDays` | `LoanDays` | 6 | MemberService, MemberProfile |
| `MemberType.FineRatePerDay` | `FinePerDay` | 6 | MemberService, MemberProfile |
| `MemberType.MaxRenewalsAllowed` | `MaxRenewals` | (pehle ke steps) | MemberProfile |
| `Reservation.ExpiryDate` | `Reservation.ExpiresAt` | 5 | CirculationService/Profile |
| `Reservation.ReservationDate` | `Reservation.ReservedAt` | 4 | CirculationService/Profile |
| `Payment.ReceivedByUser` | `Payment.ReceivedBy` | 3 | CirculationService/Profile |
| `Payment.ReceivedByUserId` | `Payment.ReceivedById` | 2 | CirculationService |
| `Payment.PaymentDate` | `Payment.PaidAt` | 2 | CirculationService |
| `Payment.TransactionId` | `Payment.TransactionRef` | 2 | CirculationService |
| `Payment.Mode` | `Payment.PaymentMode` | 1 | Profile |
| `Fine.Type` / `Fine.Reason` | `Fine.FineType` (aur `Description`) | 2 | Profile/Service |
| `Book.Copies` | `Book.BookCopies` | 2 | CatalogService |

Dhyan: **DTO ki property ka naam badalna aur Domain ka naam badalna alag cheez hai.** Upar sab "Domain wali side" ke rename hain: yaani `loan.IssuedByUser` (entity pe) → `loan.IssuedBy`. DTO ki apni property (jaise `LoanDto.IssuedByUserId`) waise hi rehti hai, jab tak neeche Qism B me na likha ho.

### Enums

| Galat | Sahi | Count |
| --- | --- | --- |
| `FineStatus.Pending` | `FineStatus.Unpaid` | 2 (+1 error output me chhoota) |
| `FineStatus.Partial` | `FineStatus.PartiallyPaid` | 1 |
| `CopyStatus.OnLoan` | `CopyStatus.Issued` | 1 |
| `LoanStatus.Renewed` | hata do, renew me status `Active` rehta hai | 1 |

### Repository method naam

| Galat | Sahi | Count |
| --- | --- | --- |
| `IFineRepository.GetTotalUnpaidByMemberAsync` | `GetTotalOutstandingByMemberAsync` | 4 |
| `IReservationRepository.GetActiveByBookCopyAsync(copyId)` | `GetActiveByBookAsync(bookCopy.BookId)` | 2 |
| `IMemberRepository.GetByMembershipNumberAsync` | `GetByMemberNumberAsync` | 1 |
| `IMemberRepository.IsMembershipNumberUniqueAsync` | `IsMemberNumberUniqueAsync` | 1 |

---

## 3. Qism B: DTO me property add/rename (Domain me hai, DTO me nahi)

| DTO | Karo | Errors |
| --- | --- | --- |
| `BookCopyDto`, `CreateBookCopyRequest`, `UpdateBookCopyRequest` | `AcquiredDate` ko `AcquisitionDate` karo, `Price` ko `Cost` karo, `Condition` (string) add karo; Create me `Status` add; Update me `CopyNumber`, `Cost`, `Condition` add | 4 + 4 + 4 + 2 + 2 + 1 + 1 + 1 + 1 |
| `BookDto`, `CreateBookRequest`, `UpdateBookRequest` | `Edition` (string?) add karo; `PageCount` ko `Pages` (int?) karo; `PublicationYear` ko `int?` karo; `NumberOfPages` aur `Summary` wali lines service me hata do ya `Pages`/`Description` se badlo | 3 + 3 + 3 + 3 + 2 + 2 + 3 |
| `Update*Request.Id` (Category, Author, Book, BookCopy, Member, MemberType, Publisher) | DTO me `Id` nahi hai. Service methods ke `Guid id` parameter ka use karo: `request.Id` ko `id` se badlo | 2 + 1 + 1 + 1 + 1 + 1 + 1 |
| `CreatePublisherRequest` / `UpdatePublisherRequest` | `PhoneNumber` ko `Phone` | 1 + 1 |
| `CreateMemberRequest` / `UpdateMemberRequest` | `MembershipStartDate` ko `JoinDate`, `MembershipEndDate` ko `ExpiryDate`, `MemberNumber`/`MembershipNumber` ko `MembershipNumber`; `UserId` add (D5) | ~10 |
| `ReturnLoanRequest`, `RenewLoanRequest` | `ConditionNotes` ko `Notes` karo; `ReturnedById` / `RenewedById` DTO me nahi hain, method ke `returnedById`/`renewedById` parameter use karo | 2 + 1 + 1 |
| `CreateFineRequest` | `IncurredDate`, `Reason`, `Type` hatao. `FineType` aur `Description` use karo. `LoanId` required karo (Guid, nullable nahi) | 3 + 1 |
| `BaseDto.UpdatedAt` | `DateTime` ko `DateTime?` karo (ek change se `CirculationService` ke 4 errors jaate hain: lines 502, 533, 570, 598) | 4 |
| `MemberDto.JoinDate` aur `UnpaidFines` | Ye abhi read-only alias hain (`JoinDate => MembershipDate`). Normal `{ get; set; }` properties bana do aur check karo ki `MembershipDate`/`TotalOutstandingFines` kahin aur use ho rahe hain ya nahi | 5 |
| `MemberDto.MemberNumber` ko `MembershipNumber` | Dono hain; ek hi rakho (`MembershipNumber`) | (cleanup) |

---

## 4. Qism D: Hatao ya replace karo (Domain me nahi, chahiye bhi nahi)

Ye lines (jinme sirf mapping/DTO property hai, logic nahi) hata do:

| Cheez | Kahan | Karo |
| --- | --- | --- |
| `MemberType.CanReserve`, `MaxReservationsAllowed`, `MembershipFee`, `MembershipDurationDays`, `Description` | MemberDto (MemberTypeDto + Create/Update requests), MemberService, MemberProfile, MemberValidators | Properties aur unki mapping/validation lines hatao |
| `Gender`, `DateOfBirth` | CreateMemberRequest, UpdateMemberRequest, MemberService, MemberValidators | Hatao |
| `FineDto.StatusName`, `TypeName`, `PaymentDto.ModeName`, `LoanDto.StatusName`, `ReservationDto.StatusName` | CirculationProfile | Mapping lines hatao. Status ko text me bhejna ho to `JsonStringEnumConverter` lagao |
| `LoanDto.CopyNumber`, `FineDto.BookTitle` | CirculationProfile/Service | `loan.BookCopy.CopyNumber`, `loan.Loan?.BookCopy?.Book?.Title` jaise include ke saath map karo, ya hata do |
| `MemberNumber` (LoanDto, ReservationDto, FineDto) | Profile | `MembershipNumber` se map karo |
| `DateTime.HasValue`, `int.HasValue` | Service (1 jagah har ek) | Property non-nullable hai, to `.HasValue` hata do (ya property nullable karo) |
| `Book.MemberType` | CirculationService (1 jagah) | Typo/galat navigation. Us line me asli intent (member ka type) `member.MemberType` hona chahiye |

---

## 5. Qism E: Repository methods jo interface me nahi

| Method | Kahan use hota hai | Karo |
| --- | --- | --- |
| `IFineRepository.GetByLoanAsync(loanId)` | CirculationService | `IFineRepository` me add + `FineRepository` me implement |
| `IFineRepository.AddPaymentAsync(payment)` | RecordPaymentAsync | Better: `IPaymentRepository.AddAsync` use karo (generic repo me hai) aur `CirculationService` me `IPaymentRepository` inject karo |
| `IFineRepository.GetPaymentsPagedAsync(...)` | GetPaymentsAsync | `IPaymentRepository.GetPagedAsync(request, fineId, from, to, ct)` pehle se hai, usi ko call karo |
| `IMemberRepository.GetByUserIdAsync(userId)` | MemberService | Interface me add + `MemberRepository` me implement |
| `INotificationRepository.UpdateRangeAsync` | NotificationService | Generic repo ka `Update(entity)` loop me chalao, ya `UpdateRangeAsync` interface me add karo |

Jab bhi interface me method add karo, **Infrastructure ki repository class me bhi implement karna hai**, warna Infrastructure build me naya error aayega.

---

## 6. 30 "baaki" errors, exact fix (pura compile-level)

| File:line | Error | Fix |
| --- | --- | --- |
| CirculationService:39 | `var (loans, totalCount) = await _loanRepository.GetPagedAsync(request, memberId, bookCopyId, status, cancellationToken)` | Repo `PaginatedResult<Loan>` deta hai, tuple nahi. `var page = await _loanRepository.GetPagedAsync(request, memberId, bookCopyId, status, null, cancellationToken);` phir `page.Items`, `page.TotalCount`. (5th argument `bool? isOverdue` hai, isliye `null` daalna hai.) |
| CirculationService:605 | `var (payments, totalCount) = await _fineRepository.GetPaymentsPagedAsync(...)` | `var page = await _paymentRepository.GetPagedAsync(request, fineId, fromDate, toDate, cancellationToken);` |
| CirculationService:258 | `CreateOverdueFineAsync(loan, cancellationToken)` | `CreateOverdueFineAsync(loan, null, cancellationToken)` (dusra parameter `decimal? finePerDay` hai) |
| CirculationService:502, 533, 570, 598 | `UpdatedAt` `DateTime?` se `DateTime` | `BaseDto.UpdatedAt` ko `DateTime?` karo |
| CirculationService:547 | `LoanId = request.LoanId` (`Guid?` se `Guid`) | `CreateFineRequest.LoanId` ko required `Guid` karo |
| MemberService:32 | `GetPagedAsync(request, search, memberTypeId, isActive, ct)`, aur tuple deconstruction | Signature `(request, bool? isActive, Guid? memberTypeId, string? search, ct)`. `var page = await _memberRepository.GetPagedAsync(request, isActive, memberTypeId, search, cancellationToken);` |
| MemberService:47, 95, 134 | `MemberDto.JoinDate` read-only | Section 3 (`MemberDto` ko settable karo) |
| MemberService:66, 114, 153 | `MemberDto.UnpaidFines` read-only | Section 3 |
| MemberService:182 | `ExpiryDate = request.ExpiryDate` (`DateTime?` se `DateTime`) | `ExpiryDate = request.ExpiryDate ?? member.ExpiryDate` |
| MemberService:327 | `memberType.MaxBooksAllowed = request.MaxBooksAllowed` (`int?` se `int`) | `= request.MaxBooksAllowed ?? memberType.MaxBooksAllowed` |
| CatalogService:321, 371, 412 | `PublicationYear = book.PublicationYear` (`int?` se `int`) | `BookDto.PublicationYear` ko `int?` karo |
| CatalogService:440 | `IsISBNUniqueAsync(request.ISBN, cancellationToken)` | `IsISBNUniqueAsync(request.ISBN, null, cancellationToken)` |
| CatalogService:610 | `IsBarcodeUniqueAsync(request.Barcode, cancellationToken)` | `IsBarcodeUniqueAsync(request.Barcode, null, cancellationToken)` |
| ReportService:88 | `memberActivities.Select(...)` | Repo `PaginatedResult<MemberActivityInfo>` deta hai: `memberActivities.Items.Select(...)` |
| UserService:101 | `user.IsActive = request.IsActive` (`bool?` se `bool`) | `user.IsActive = request.IsActive ?? user.IsActive;` |

---

## 7. Kis order me karo

1. **Domain me decisions apply karo** (section 1) aur EF configs update karo. `dotnet build src/LMS.Domain`.
2. **Qism B:** DTO changes (section 3). Ek-ek DTO file khol ke karo, kyunki ye sabse zyada errors hataate hain (BaseDto.UpdatedAt akela 4 hata deta hai).
3. **Qism A:** renames (section 2). VS Code me `Ctrl+Shift+H`, `src/LMS.Application` folder, **Match Case + Whole Word on**, ek-ek naam. `Member.IsActive` aur `.FirstName` wale blind replace mat karo, un jagahon ko khud dekho.
4. **Qism E:** repository methods (section 5) interface + Infrastructure dono me.
5. **Section 6** ke 15 compile-level fixes.
6. **Qism D:** bachi hui lines hatao.
7. Har step ke baad build chalao. Count girta hona chahiye. Jo errors bachein, wo yahan paste karo.

Ek baat: jab Application green ho jaye, Infrastructure me abhi tak compile nahi hua code hai. Wahan naye errors aana normal hai (GenericRepository me `UpdateAsync` ka implement na hona, DbSeeder me `RoleId`, Hangfire job method names, DI me `IPasswordHasher`).
