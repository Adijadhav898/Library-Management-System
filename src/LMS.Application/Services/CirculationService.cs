using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Enums;

namespace LMS.Application.Services;

public class CirculationService : ICirculationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoanRepository _loanRepository;
    private readonly IBookCopyRepository _bookCopyRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IFineRepository _fineRepository;
    private readonly ISettingRepository _settingRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IPaymentRepository _paymentRepository;

    public CirculationService(
        ILoanRepository loanRepository,
        IBookCopyRepository bookCopyRepository,
        IMemberRepository memberRepository,
        IReservationRepository reservationRepository,
        IFineRepository fineRepository,
        ISettingRepository settingRepository,
        IBookRepository bookRepository,
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _loanRepository = loanRepository;
        _bookCopyRepository = bookCopyRepository;
        _memberRepository = memberRepository;
        _reservationRepository = reservationRepository;
        _fineRepository = fineRepository;
        _settingRepository = settingRepository;
        _bookRepository = bookRepository;
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    // Loans
    public async Task<PaginatedResult<LoanDto>> GetLoansAsync(PaginationRequest request, Guid? memberId = null, Guid? bookCopyId = null, LoanStatus? status = null, bool? isOverdue = null, CancellationToken cancellationToken = default)
    {
        var (loans, totalCount) = await _loanRepository.GetPagedAsync(request, memberId, bookCopyId, status, null, cancellationToken);

        // Filter by isOverdue if specified
        if (isOverdue.HasValue && isOverdue.Value)
        {
            loans = loans.Where(l => l.DueDate < DateTime.UtcNow && l.Status == LoanStatus.Active).ToList();
        }

        var loanDtos = new List<LoanDto>();
        foreach (var loan in loans)
        {
            loanDtos.Add(new LoanDto
            {
                Id = loan.Id,
                MemberId = loan.MemberId,
                MemberName = loan.Member?.User != null ? $"{loan.Member.User.FirstName} {loan.Member.User.LastName}" : string.Empty,
                MembershipNumber = loan.Member?.MembershipNumber ?? string.Empty,
                BookCopyId = loan.BookCopyId,
                BookTitle = loan.BookCopy?.Book?.Title ?? string.Empty,
                Barcode = loan.BookCopy?.Barcode ?? string.Empty,
                IssueDate = loan.IssueDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                Status = loan.Status,
                RenewalCount = loan.RenewalCount,
                MaxRenewals = loan.MaxRenewals,
                IssuedByUserId = loan.IssuedById,
                IssuedByName = loan.IssuedBy != null ? $"{loan.IssuedBy.FirstName} {loan.IssuedBy.LastName}" : string.Empty,
                ReturnedByUserId = loan.ReturnedByUserId,
                ReturnedByName = loan.ReturnedByUser != null ? $"{loan.ReturnedByUser.FirstName} {loan.ReturnedByUser.LastName}" : string.Empty,
                Notes = loan.Notes,
                CreatedAt = loan.CreatedAt
            });
        }

        return new PaginatedResult<LoanDto>
        {
            Items = loanDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<LoanDto?> GetLoanByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var loan = await _loanRepository.GetByIdAsync(id, cancellationToken);
        if (loan == null)
            return null;

        return new LoanDto
        {
            Id = loan.Id,
            MemberId = loan.MemberId,
            MemberName = loan.Member?.User != null ? $"{loan.Member.User.FirstName} {loan.Member.User.LastName}" : string.Empty,
            MembershipNumber = loan.Member?.MembershipNumber ?? string.Empty,
            BookCopyId = loan.BookCopyId,
            BookTitle = loan.BookCopy?.Book?.Title ?? string.Empty,
            Barcode = loan.BookCopy?.Barcode ?? string.Empty,
            IssueDate = loan.IssueDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            Status = loan.Status,
            RenewalCount = loan.RenewalCount,
            MaxRenewals = loan.MaxRenewals,
            IssuedByUserId = loan.IssuedById,
            IssuedByName = loan.IssuedBy != null ? $"{loan.IssuedBy.FirstName} {loan.IssuedBy.LastName}" : string.Empty,
            ReturnedByUserId = loan.ReturnedByUserId,
            ReturnedByName = loan.ReturnedByUser != null ? $"{loan.ReturnedByUser.FirstName} {loan.ReturnedByUser.LastName}" : string.Empty,
            Notes = loan.Notes,
            CreatedAt = loan.CreatedAt
        };
    }

    public async Task<LoanDto> IssueBookAsync(CreateLoanRequest request, Guid issuedById, CancellationToken cancellationToken = default)
    {
        var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);
        if (member == null)
            throw new InvalidOperationException("Member not found");

        if (member.IsBlocked)
            throw new InvalidOperationException("Member is not active");

        if (member.ExpiryDate < DateTime.UtcNow)
            throw new InvalidOperationException("Member membership has expired");

        var bookCopy = await _bookCopyRepository.GetByIdAsync(request.BookCopyId, cancellationToken);
        if (bookCopy == null)
            throw new InvalidOperationException("Book copy not found");

        if (bookCopy.Status != CopyStatus.Available)
            throw new InvalidOperationException("Book copy is not available for loan");

        // Check if member has reached max loans limit
        var activeLoansCount = await _loanRepository.GetActiveLoansCountByMemberAsync(request.MemberId, cancellationToken);
        var maxLoansSetting = await _settingRepository.GetValueAsync("MaxLoansPerMember", cancellationToken);
        var maxLoans = int.TryParse(maxLoansSetting, out var ml) ? ml : 5;

        if (activeLoansCount >= maxLoans)
            throw new InvalidOperationException($"Member has reached maximum loan limit ({maxLoans})");

        // Check for unpaid fines
        var unpaidFines = await _fineRepository.GetTotalOutstandingByMemberAsync(request.MemberId, cancellationToken);
        if (unpaidFines > 0)
        {
            var blockOnFines = await _settingRepository.GetValueAsync("BlockLoansOnUnpaidFines", cancellationToken);
            if (bool.TryParse(blockOnFines, out var block) && block)
                throw new InvalidOperationException("Member has unpaid fines. Please clear fines before issuing new books.");
        }

        // Get loan period from settings
        var loanPeriodSetting = await _settingRepository.GetValueAsync("DefaultLoanPeriodDays", cancellationToken);
        var loanPeriodDays = int.TryParse(loanPeriodSetting, out var lpd) ? lpd : 14;

        var maxRenewalsSetting = await _settingRepository.GetValueAsync("MaxRenewalsAllowed", cancellationToken);
        var maxRenewals = int.TryParse(maxRenewalsSetting, out var mr) ? mr : 2;

        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            MemberId = request.MemberId,
            BookCopyId = request.BookCopyId,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(loanPeriodDays),
            Status = LoanStatus.Active,
            RenewalCount = 0,
            MaxRenewals = maxRenewals,
            IssuedById = issuedById,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        // Update book copy status
        bookCopy.Status = CopyStatus.Issued;
        bookCopy.UpdatedAt = DateTime.UtcNow;

        await _loanRepository.AddAsync(loan, cancellationToken);
        await _bookCopyRepository.UpdateAsync(bookCopy, cancellationToken);

        // Check if there's a reservation for this book copy
        var reservations = await _reservationRepository.GetActiveByBookAsync(bookCopy.BookId, cancellationToken);
        var reservation = reservations.FirstOrDefault();
        if (reservation != null)
        {
            reservation.Status = ReservationStatus.Fulfilled;
            reservation.FulfilledAt = DateTime.UtcNow;
            reservation.UpdatedAt = DateTime.UtcNow;
            await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        }

        // Persist first so the re-read below sees the saved row and its relations.

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetLoanByIdAsync(loan.Id, cancellationToken) ?? throw new InvalidOperationException("Failed to create loan");
    }

    public async Task<LoanDto?> RenewLoanAsync(RenewLoanRequest request, Guid renewedById, CancellationToken cancellationToken = default)
    {
        var loan = await _loanRepository.GetByIdAsync(request.LoanId, cancellationToken);
        if (loan == null)
            return null;

        if (loan.Status != LoanStatus.Active)
            throw new InvalidOperationException("Only active loans can be renewed");

        if (loan.RenewalCount >= loan.MaxRenewals)
            throw new InvalidOperationException("Maximum renewals reached");

        // Check if there's a reservation for this book
        var renewCopy = await _bookCopyRepository.GetByIdAsync(loan.BookCopyId, cancellationToken);
        if (renewCopy == null)
            throw new InvalidOperationException("Book copy not found");

        var reservations = await _reservationRepository.GetActiveByBookAsync(renewCopy.BookId, cancellationToken);
        if (reservations.Any())
            throw new InvalidOperationException("Book has active reservations, cannot renew");

        var loanPeriodSetting = await _settingRepository.GetValueAsync("DefaultLoanPeriodDays", cancellationToken);
        var loanPeriodDays = int.TryParse(loanPeriodSetting, out var lpd) ? lpd : 14;

        loan.DueDate = loan.DueDate.AddDays(loanPeriodDays);
        loan.RenewalCount++;
        loan.UpdatedAt = DateTime.UtcNow;

        await _loanRepository.UpdateAsync(loan, cancellationToken);

        // Persist first so the re-read below sees the saved row and its relations.

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetLoanByIdAsync(loan.Id, cancellationToken);
    }

    public async Task<LoanDto?> ReturnBookAsync(ReturnLoanRequest request, Guid returnedById, CancellationToken cancellationToken = default)
    {
        var loan = await _loanRepository.GetByIdAsync(request.LoanId, cancellationToken);
        if (loan == null)
            return null;

        if (loan.Status != LoanStatus.Active)
            throw new InvalidOperationException("Loan is not active");

        var bookCopy = await _bookCopyRepository.GetByIdAsync(loan.BookCopyId, cancellationToken);
        if (bookCopy == null)
            throw new InvalidOperationException("Book copy not found");

        loan.ReturnDate = DateTime.UtcNow;
        loan.Status = LoanStatus.Returned;
        loan.ReturnedByUserId = returnedById;
        loan.Notes = request.Notes;
        loan.UpdatedAt = DateTime.UtcNow;

        // Update book copy status based on condition
        bookCopy.Status = request.Condition switch
        {
            "Good" => CopyStatus.Available,
            "Damaged" => CopyStatus.Damaged,
            "Lost" => CopyStatus.Lost,
            _ => CopyStatus.Available
        };
        bookCopy.Condition = request.Condition ?? bookCopy.Condition;
        bookCopy.UpdatedAt = DateTime.UtcNow;

        await _loanRepository.UpdateAsync(loan, cancellationToken);
        await _bookCopyRepository.UpdateAsync(bookCopy, cancellationToken);

        // Check for overdue fine
        if (loan.DueDate < DateTime.UtcNow)
        {
            await CreateOverdueFineAsync(loan, null, cancellationToken);
        }

        // Persist first so the re-read below sees the saved row and its relations.

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetLoanByIdAsync(request.LoanId, cancellationToken);
    }

    public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        var loans = await _loanRepository.GetOverdueLoansAsync(cancellationToken);
        return loans.Select(loan => new LoanDto
        {
            Id = loan.Id,
            MemberId = loan.MemberId,
            MemberName = loan.Member?.User != null ? $"{loan.Member.User.FirstName} {loan.Member.User.LastName}" : string.Empty,
            MembershipNumber = loan.Member?.MembershipNumber ?? string.Empty,
            BookCopyId = loan.BookCopyId,
            BookTitle = loan.BookCopy?.Book?.Title ?? string.Empty,
            Barcode = loan.BookCopy?.Barcode ?? string.Empty,
            IssueDate = loan.IssueDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate,
            Status = loan.Status,
            RenewalCount = loan.RenewalCount,
            MaxRenewals = loan.MaxRenewals,
            IssuedByUserId = loan.IssuedById,
            IssuedByName = loan.IssuedBy != null ? $"{loan.IssuedBy.FirstName} {loan.IssuedBy.LastName}" : string.Empty,
            ReturnedByUserId = loan.ReturnedByUserId,
            ReturnedByName = loan.ReturnedByUser != null ? $"{loan.ReturnedByUser.FirstName} {loan.ReturnedByUser.LastName}" : string.Empty,
            Notes = loan.Notes,
            CreatedAt = loan.CreatedAt
        });
    }

    public async Task ProcessOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        var overdueLoans = await _loanRepository.GetOverdueLoansAsync(cancellationToken);
        var fineAmountSetting = await _settingRepository.GetValueAsync("OverdueFinePerDay", cancellationToken);
        var finePerDay = decimal.TryParse(fineAmountSetting, out var fpd) ? fpd : 1.00m;

        foreach (var loan in overdueLoans)
        {
            await CreateOverdueFineAsync(loan, finePerDay, cancellationToken);
        }
    }

    // Reservations
    public async Task<PaginatedResult<ReservationDto>> GetReservationsAsync(PaginationRequest request, Guid? memberId = null, Guid? bookId = null, ReservationStatus? status = null, CancellationToken cancellationToken = default)
    {
        var (reservations, totalCount) = await _reservationRepository.GetPagedAsync(request, memberId, bookId, status, cancellationToken);

        var reservationDtos = new List<ReservationDto>();
        foreach (var reservation in reservations)
        {
            reservationDtos.Add(new ReservationDto
            {
                Id = reservation.Id,
                MemberId = reservation.MemberId,
                MemberName = reservation.Member?.User != null ? $"{reservation.Member.User.FirstName} {reservation.Member.User.LastName}" : string.Empty,
                MembershipNumber = reservation.Member?.MembershipNumber ?? string.Empty,
                BookId = reservation.BookId,
                BookTitle = reservation.Book?.Title ?? string.Empty,
                BookCopyId = reservation.BookCopyId,
                Barcode = reservation.BookCopy?.Barcode ?? string.Empty,
                ReservationDate = reservation.ReservedAt,
                ExpiryDate = reservation.ExpiresAt,
                Status = reservation.Status,
                FulfilledAt = reservation.FulfilledAt,
                Notes = reservation.Notes,
                CreatedAt = reservation.CreatedAt
            });
        }

        return new PaginatedResult<ReservationDto>
        {
            Items = reservationDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<ReservationDto?> GetReservationByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, cancellationToken);
        if (reservation == null)
            return null;

        return new ReservationDto
        {
            Id = reservation.Id,
            MemberId = reservation.MemberId,
            MemberName = reservation.Member?.User != null ? $"{reservation.Member.User.FirstName} {reservation.Member.User.LastName}" : string.Empty,
            MembershipNumber = reservation.Member?.MembershipNumber ?? string.Empty,
            BookId = reservation.BookId,
            BookTitle = reservation.Book?.Title ?? string.Empty,
            BookCopyId = reservation.BookCopyId,
            Barcode = reservation.BookCopy?.Barcode ?? string.Empty,
            ReservationDate = reservation.ReservedAt,
            ExpiryDate = reservation.ExpiresAt,
            Status = reservation.Status,
            FulfilledAt = reservation.FulfilledAt,
            Notes = reservation.Notes,
            CreatedAt = reservation.CreatedAt
        };
    }

    public async Task<ReservationDto> CreateReservationAsync(CreateReservationRequest request, CancellationToken cancellationToken = default)
    {
        var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);
        if (member == null)
            throw new InvalidOperationException("Member not found");

        var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book == null)
            throw new InvalidOperationException("Book not found");

        // Check if member already has active reservation for this book
        var existingReservations = await _reservationRepository.GetActiveByMemberAsync(request.MemberId, cancellationToken);
        if (existingReservations.Any(r => r.BookId == request.BookId))
            throw new InvalidOperationException("Member already has an active reservation for this book");

        // Check if member has reached max reservations limit
        var maxReservationsSetting = await _settingRepository.GetValueAsync("MaxReservationsPerMember", cancellationToken);
        var maxReservations = int.TryParse(maxReservationsSetting, out var mr) ? mr : 3;

        if (existingReservations.Count() >= maxReservations)
            throw new InvalidOperationException($"Member has reached maximum reservation limit ({maxReservations})");

        // Check availability
        var availableCopies = await _bookCopyRepository.GetAvailableByBookIdAsync(request.BookId, cancellationToken);
        if (!availableCopies.Any())
        {
            // No copies available, create reservation
            var reservationExpirySetting = await _settingRepository.GetValueAsync("ReservationExpiryDays", cancellationToken);
            var expiryDays = int.TryParse(reservationExpirySetting, out var ed) ? ed : 3;

            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                MemberId = request.MemberId,
                BookId = request.BookId,
                ReservedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
                Status = ReservationStatus.Pending,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _reservationRepository.AddAsync(reservation, cancellationToken);
            // Persist first so the re-read below sees the saved row and its relations.
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return await GetReservationByIdAsync(reservation.Id, cancellationToken) ?? throw new InvalidOperationException("Failed to create reservation");
        }
        else
        {
            // Copy available, fulfill immediately
            var bookCopy = availableCopies.First();
            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                MemberId = request.MemberId,
                BookId = request.BookId,
                BookCopyId = bookCopy.Id,
                ReservedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(3),
                Status = ReservationStatus.Fulfilled,
                FulfilledAt = DateTime.UtcNow,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            bookCopy.Status = CopyStatus.Reserved;
            bookCopy.UpdatedAt = DateTime.UtcNow;

            await _reservationRepository.AddAsync(reservation, cancellationToken);
            await _bookCopyRepository.UpdateAsync(bookCopy, cancellationToken);

            // Persist first so the re-read below sees the saved row and its relations.

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await GetReservationByIdAsync(reservation.Id, cancellationToken) ?? throw new InvalidOperationException("Failed to create reservation");
        }
    }

    public async Task<bool> CancelReservationAsync(CancelReservationRequest request, CancellationToken cancellationToken = default)
    {
        var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
        if (reservation == null)
            return false;

        reservation.Status = ReservationStatus.Cancelled;
        reservation.UpdatedAt = DateTime.UtcNow;

        if (reservation.BookCopyId.HasValue)
        {
            var bookCopy = await _bookCopyRepository.GetByIdAsync(reservation.BookCopyId.Value, cancellationToken);
            if (bookCopy != null && bookCopy.Status == CopyStatus.Reserved)
            {
                bookCopy.Status = CopyStatus.Available;
                bookCopy.UpdatedAt = DateTime.UtcNow;
                await _bookCopyRepository.UpdateAsync(bookCopy, cancellationToken);
            }
        }

        await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        return true;
    }

    public async Task ProcessExpiredReservationsAsync(CancellationToken cancellationToken = default)
    {
        var expiredReservations = await _reservationRepository.GetExpiredReservationsAsync(cancellationToken);
        foreach (var reservation in expiredReservations)
        {
            reservation.Status = ReservationStatus.Expired;
            reservation.UpdatedAt = DateTime.UtcNow;

            if (reservation.BookCopyId.HasValue)
            {
                var bookCopy = await _bookCopyRepository.GetByIdAsync(reservation.BookCopyId.Value, cancellationToken);
                if (bookCopy != null && bookCopy.Status == CopyStatus.Reserved)
                {
                    bookCopy.Status = CopyStatus.Available;
                    bookCopy.UpdatedAt = DateTime.UtcNow;
                    await _bookCopyRepository.UpdateAsync(bookCopy, cancellationToken);
                }
            }

            await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        }
    }

    // Fines
    public async Task<PaginatedResult<FineDto>> GetFinesAsync(PaginationRequest request, Guid? memberId = null, FineStatus? status = null, CancellationToken cancellationToken = default)
    {
        var (fines, totalCount) = await _fineRepository.GetPagedAsync(request, memberId, status, cancellationToken);

        var fineDtos = fines.Select(f => new FineDto
        {
            Id = f.Id,
            MemberId = f.MemberId,
            MemberName = f.Member?.User != null ? $"{f.Member.User.FirstName} {f.Member.User.LastName}" : string.Empty,
            MembershipNumber = f.Member?.MembershipNumber ?? string.Empty,
            LoanId = f.LoanId,
            FineType = f.FineType,
            Amount = f.Amount,
            PaidAmount = f.PaidAmount,
            Status = f.Status,
            Description = f.Description,
            CreatedAt = f.CreatedAt,
            UpdatedAt = f.UpdatedAt
        }).ToList();

        return new PaginatedResult<FineDto>
        {
            Items = fineDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<FineDto?> GetFineByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var fine = await _fineRepository.GetByIdAsync(id, cancellationToken);
        if (fine == null)
            return null;

        return new FineDto
        {
            Id = fine.Id,
            MemberId = fine.MemberId,
            MemberName = fine.Member?.User != null ? $"{fine.Member.User.FirstName} {fine.Member.User.LastName}" : string.Empty,
            MembershipNumber = fine.Member?.MembershipNumber ?? string.Empty,
            LoanId = fine.LoanId,
            FineType = fine.FineType,
            Amount = fine.Amount,
            PaidAmount = fine.PaidAmount,
            Status = fine.Status,
            Description = fine.Description,
            CreatedAt = fine.CreatedAt,
            UpdatedAt = fine.UpdatedAt
        };
    }

    public async Task<FineDto> CreateFineAsync(CreateFineRequest request, CancellationToken cancellationToken = default)
    {
        var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);
        if (member == null)
            throw new InvalidOperationException("Member not found");

        var fine = new Fine
        {
            Id = Guid.NewGuid(),
            MemberId = request.MemberId,
            LoanId = request.LoanId,
            FineType = request.FineType,
            Amount = request.Amount,
            Status = FineStatus.Unpaid,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _fineRepository.AddAsync(fine, cancellationToken);

        return new FineDto
        {
            Id = fine.Id,
            MemberId = fine.MemberId,
            MemberName = member.User != null ? $"{member.User.FirstName} {member.User.LastName}" : string.Empty,
            MembershipNumber = member.MembershipNumber,
            LoanId = fine.LoanId,
            FineType = fine.FineType,
            Amount = fine.Amount,
            PaidAmount = fine.PaidAmount,
            Status = fine.Status,
            Description = fine.Description,
            CreatedAt = fine.CreatedAt,
            UpdatedAt = fine.UpdatedAt
        };
    }

    public async Task<FineDto?> UpdateFineStatusAsync(Guid fineId, FineStatus status, CancellationToken cancellationToken = default)
    {
        var fine = await _fineRepository.GetByIdAsync(fineId, cancellationToken);
        if (fine == null)
            return null;

        fine.Status = status;
        fine.UpdatedAt = DateTime.UtcNow;

        await _fineRepository.UpdateAsync(fine, cancellationToken);

        return new FineDto
        {
            Id = fine.Id,
            MemberId = fine.MemberId,
            MemberName = fine.Member?.User != null ? $"{fine.Member.User.FirstName} {fine.Member.User.LastName}" : string.Empty,
            MembershipNumber = fine.Member?.MembershipNumber ?? string.Empty,
            LoanId = fine.LoanId,
            FineType = fine.FineType,
            Amount = fine.Amount,
            PaidAmount = fine.PaidAmount,
            Status = fine.Status,
            Description = fine.Description,
            CreatedAt = fine.CreatedAt,
            UpdatedAt = fine.UpdatedAt
        };
    }

    // Payments
    public async Task<PaginatedResult<PaymentDto>> GetPaymentsAsync(PaginationRequest request, Guid? fineId = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        var (payments, totalCount) = await _paymentRepository.GetPagedAsync(request, fineId, fromDate, toDate, cancellationToken);

        var paymentDtos = payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            FineId = p.FineId,
            MemberId = p.Fine?.MemberId ?? Guid.Empty,
            MemberName = p.Fine?.Member?.User != null ? $"{p.Fine.Member.User.FirstName} {p.Fine.Member.User.LastName}" : string.Empty,
            Amount = p.Amount,
            PaymentMode = p.PaymentMode,
            TransactionId = p.TransactionRef ?? string.Empty,
            ReceivedByUserId = p.ReceivedById,
            ReceivedByName = p.ReceivedBy != null ? $"{p.ReceivedBy.FirstName} {p.ReceivedBy.LastName}" : string.Empty,
            PaymentDate = p.PaidAt,
            Notes = p.Notes,
            CreatedAt = p.CreatedAt
        }).ToList();

        return new PaginatedResult<PaymentDto>
        {
            Items = paymentDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PaymentDto> RecordPaymentAsync(CreatePaymentRequest request, Guid receivedById, CancellationToken cancellationToken = default)
    {
        var fine = await _fineRepository.GetByIdAsync(request.FineId, cancellationToken);
        if (fine == null)
            throw new InvalidOperationException("Fine not found");

        if (fine.Status == FineStatus.Paid)
            throw new InvalidOperationException("Fine is already paid");

        var remainingAmount = fine.Amount - fine.PaidAmount;
        if (request.Amount > remainingAmount)
            throw new InvalidOperationException($"Payment amount exceeds remaining fine amount ({remainingAmount})");

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            FineId = request.FineId,
            Amount = request.Amount,
            PaymentMode = request.PaymentMode,
            TransactionRef = request.TransactionId,
            ReceivedById = receivedById,
            PaidAt = DateTime.UtcNow,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _paymentRepository.AddAsync(payment, cancellationToken);

        // Update fine paid amount and status
        fine.PaidAmount += request.Amount;
        if (fine.PaidAmount >= fine.Amount)
        {
            fine.Status = FineStatus.Paid;
        }
        else if (fine.PaidAmount > 0)
        {
            fine.Status = FineStatus.PartiallyPaid;
        }
        fine.UpdatedAt = DateTime.UtcNow;

        await _fineRepository.UpdateAsync(fine, cancellationToken);

        return new PaymentDto
        {
            Id = payment.Id,
            FineId = payment.FineId,
            MemberId = fine.MemberId,
            MemberName = fine.Member?.User != null ? $"{fine.Member.User.FirstName} {fine.Member.User.LastName}" : string.Empty,
            Amount = payment.Amount,
            PaymentMode = payment.PaymentMode,
            TransactionId = payment.TransactionRef ?? string.Empty,
            ReceivedByUserId = payment.ReceivedById,
            ReceivedByName = payment.ReceivedBy != null ? $"{payment.ReceivedBy.FirstName} {payment.ReceivedBy.LastName}" : string.Empty,
            PaymentDate = payment.PaidAt,
            Notes = payment.Notes,
            CreatedAt = payment.CreatedAt
        };
    }

    private async Task CreateOverdueFineAsync(Loan loan, decimal? finePerDay = null, CancellationToken cancellationToken = default)
    {
        if (!finePerDay.HasValue)
        {
            var fineAmountSetting = await _settingRepository.GetValueAsync("OverdueFinePerDay", cancellationToken);
            finePerDay = decimal.TryParse(fineAmountSetting, out var fpd) ? fpd : 1.00m;
        }

        var daysOverdue = (DateTime.UtcNow - loan.DueDate).Days;
        if (daysOverdue <= 0)
            return;

        var existingFine = (await _fineRepository.GetByLoanAsync(loan.Id, cancellationToken))
            .FirstOrDefault(f => f.FineType == FineType.Overdue && f.Status != FineStatus.Paid);

        if (existingFine != null)
        {
            existingFine.Amount = daysOverdue * finePerDay.Value;
            existingFine.UpdatedAt = DateTime.UtcNow;
            await _fineRepository.UpdateAsync(existingFine, cancellationToken);
        }
        else
        {
            var fine = new Fine
            {
                Id = Guid.NewGuid(),
                MemberId = loan.MemberId,
                LoanId = loan.Id,
                FineType = FineType.Overdue,
                Amount = daysOverdue * finePerDay.Value,
                Status = FineStatus.Unpaid,
                Description = $"Overdue fine for loan {loan.Id} - {daysOverdue} days overdue",
                CreatedAt = DateTime.UtcNow
            };
            await _fineRepository.AddAsync(fine, cancellationToken);
        }
    }
}