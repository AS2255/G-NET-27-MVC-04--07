using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.BookingViewModels;
using GymManagement.DAL.Repositories.Interfaces;


namespace GymManagement.BLL.Services.Classes
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null) return Result.NotFound($"Failed To Cancel Booking, Session With Id {sessionId} Not Found!");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail($"Failed To Cancel Booking, Session With Id {sessionId} Has Already Started!");

            var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.SessionId == sessionId && b.MemberId == memberId, tracking: true, ct: ct);
            if (booking is null) return Result.NotFound("Failed To Cancel Booking, Booking Not Found!");

            _unitOfWork.BookingRepository.DeleteAsync(booking);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Cancel Booking!");
        }

        public async Task<Result> MarkAttendedAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.MemberId == memberId && b.SessionId == sessionId, tracking: true, ct: ct);
            if (booking is null) return Result.NotFound("Failed To Mark Attendence, Booking Not Found!");

            booking.IsAttended = true;
            booking.UpdatedAt = DateTime.Now;
            _unitOfWork.BookingRepository.UpdateAsync(booking);

            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Mark Attendence!");
        }

        public async Task<Result> CreateNewBookingAsync(CreateBookingViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(model.SessionId, ct);
            if (session is null) return Result.NotFound($"Failed To Create Booking, Session With Id {model.SessionId} Not Found!");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail($"Failed To Book Session, Session With Id {model.SessionId} Has Already Started!");

            var hasActiveMembership = await _unitOfWork.MembershipRepository.AnyAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now, ct);
            if (!hasActiveMembership)
                return Result.Fail("Failed To Book Session, Member Does Not Have An Active Membership!");

            var doubleBooking = await _unitOfWork.BookingRepository
                .AnyAsync(b => b.SessionId == model.SessionId && b.MemberId == model.MemberId, ct);
            if (doubleBooking)
                return Result.Fail($"Failed To Book Session, Member With Id {model.MemberId} Is Already Booked For Session With Id {model.SessionId}!");

            var booked = await _unitOfWork.SessionRepository.GetBookedSessionsCountAsync(model.SessionId, ct);
            if (booked >= session.Capacity)
                return Result.Fail($"Failed To Book Session, Session With Id {model.SessionId} Is Full!");

            _unitOfWork.BookingRepository.AddAsync(new MemberSessions
            {
                MemberId = model.MemberId,
                SessionId = model.SessionId,
                IsAttended = false,
                CreatedAt = DateTime.Now,
            });

            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Book Session!");
        }
        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.SessionRepository.GetSessionsWithIncludeAsync(x => x.EndDate >= DateTime.Now);
            if (!bookings.Any()) return null!;
            var MappedSession = _mapper.Map<IEnumerable<SessionViewModel>>(bookings);
            foreach (var item in MappedSession)
            {
                item.AvailableSlots = item.Capacity - await _unitOfWork.SessionRepository.GetBookedSessionsCountAsync(item.Id, ct);
            }
            return MappedSession;
        }
        public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForUpcomingBySessionIdAsync(int sessionId, CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBookingsBySessionIdAsync(sessionId, ct);
            return bookings.Select(b => new MemberForSessionViewModel
            {
                MemberId = b.MemberId,
                SessionId = sessionId,
                MemberName = b.Member.Name,
                BookingDate = b.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
            }).ToList();
        }
        public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForOngoingBySessionIdAsync(int sessionId, CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBookingsBySessionIdAsync(sessionId, ct);
            return bookings.Select(b => new MemberForSessionViewModel
            {
                MemberId = b.MemberId,
                SessionId = sessionId,
                MemberName = b.Member.Name,
                BookingDate = b.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                IsAttended = b.IsAttended,
            }).ToList();
        }
        public async Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropDownAsync(int sessionId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.BookingRepository.GetAllAsync(predicate: x => x.SessionId == sessionId, ct: ct);

            var bookedMemberIds = booking.Select(x => x.MemberId);

            var availableMembers = await _unitOfWork.GetRepository<Member>().GetAllAsync(predicate: x => !bookedMemberIds.Contains(x.Id), ct: ct);

            return _mapper.Map<IEnumerable<MemberSelectListViewModel>>(availableMembers);
        }
    }
}
